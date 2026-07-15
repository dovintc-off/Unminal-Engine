// TextRender/TextRenderer.cs
namespace Unminal.UI.TextRender.TextRenderer;

/// <summary>
/// Renders text using dynamic vertex buffers and a pre-generated font texture atlas in OpenGL.
/// </summary>
[SupportedOSPlatform("windows")]
public class Text : IDisposable
{
    private readonly Atlas _fontAtlas;

    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _shaderProgram;
    private readonly int _locProjection;
    private readonly int _locTexture;
    private readonly int _locColor;

    private readonly List<float> _vertexBuffer = new List<float>();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderer"/> class, generates OpenGL buffers, and compiles text shaders.
    /// </summary>
    /// <param name="fontPath">The file path to the TrueType (.ttf) font file.</param>
    /// <param name="fontSize">The point size of the font to generate in the atlas.</param>
    /// <param name="shaderVertex">The file path to the vertex shader source code.</param>
    /// <param name="shaderFragment">The file path to the fragment shader source code.</param>
    public Text(string fontPath, int fontSize, string shaderVertex, string shaderFragment)
    {
        _fontAtlas = new Atlas(fontPath, fontSize);

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);

        _shaderProgram = LoadShaderProgram(shaderVertex, shaderFragment);

        GL.UseProgram(_shaderProgram);
        _locProjection = GL.GetUniformLocation(_shaderProgram, "projection");
        _locTexture = GL.GetUniformLocation(_shaderProgram, "uTexture");
        _locColor = GL.GetUniformLocation(_shaderProgram, "textColor");
        GL.UseProgram(0);
    }

    /// <summary>
    /// Loads, compiles, and links the vertex and fragment shaders into a complete shader program.
    /// </summary>
    /// <param name="vertPath">The file path to the vertex shader source code.</param>
    /// <param name="fragPath">The file path to the fragment shader source code.</param>
    /// <returns>The OpenGL identifier of the linked shader program.</returns>
    private int LoadShaderProgram(string vertPath, string fragPath)
    {
        string vertCode = File.ReadAllText(vertPath);
        string fragCode = File.ReadAllText(fragPath);

        int vertShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShader, vertCode);
        GL.CompileShader(vertShader);
        CheckShaderCompile(vertShader, "Vertex");

        int fragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShader, fragCode);
        GL.CompileShader(fragShader);
        CheckShaderCompile(fragShader, "Fragment");

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertShader);
        GL.AttachShader(program, fragShader);
        GL.LinkProgram(program);

        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);

        return program;
    }

    /// <summary>
    /// Validates the compilation status of a shader and throws an exception detailing compilation errors if it failed.
    /// </summary>
    /// <param name="shader">The OpenGL shader identifier to inspect.</param>
    /// <param name="type">A label describing the shader stage (e.g., "Vertex" or "Fragment") for logging purposes.</param>
    /// <exception cref="Exception">Thrown if the shader contains syntactic or semantic compilation errors.</exception>
    private void CheckShaderCompile(int shader, string type)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string log = GL.GetShaderInfoLog(shader);
            throw new Exception($"{type} Shader Compilation Error:\n{log}");
        }
    }

    /// <summary>
    /// Appends the standard layout components (position and UV texture coordinates) for a single vertex to the internal vertex buffer.
    /// </summary>
    /// <param name="x">The spatial X-coordinate of the vertex.</param>
    /// <param name="y">The spatial Y-coordinate of the vertex.</param>
    /// <param name="z">The spatial Z-coordinate of the vertex (typically 0.0f for UI).</param>
    /// <param name="u">The horizontal U texture coordinate mapping to the font atlas.</param>
    /// <param name="v">The vertical V texture coordinate mapping to the font atlas.</param>
    private void AddVertex(float x, float y, float z, float u, float v)
    {
        _vertexBuffer.Add(x);
        _vertexBuffer.Add(y);
        _vertexBuffer.Add(z);
        _vertexBuffer.Add(u);
        _vertexBuffer.Add(v);
    }

    /// <summary>
    /// Generates two triangles (six vertices total) to form a text quad representing a specific character glyph.
    /// </summary>
    /// <param name="c">The character to process and map to the glyph atlas.</param>
    /// <param name="x">The horizontal anchor coordinate where the glyph quad begins.</param>
    /// <param name="y">The vertical anchor coordinate where the glyph quad begins.</param>
    /// <param name="scale">The uniform multiplier applied to adjust text size scaling dynamically.</param>
    private void AddCharToBuffer(char c, float x, float y, float scale)
    {
        if (!_fontAtlas.TryGetGlyph(c, out var glyph)) return;

        float w = glyph.Width * _fontAtlas.Width * scale;
        float h = glyph.Height * _fontAtlas.Height * scale;

        float x1 = x;
        float y1 = y;
        float x2 = x + w;
        float y2 = y + h;

        float u1 = glyph.U;
        float v1 = glyph.V;
        float u2 = glyph.U + glyph.Width;
        float v2 = glyph.V + glyph.Height;

        AddVertex(x1, y1, 0.0f, u1, v1);
        AddVertex(x2, y1, 0.0f, u2, v1);
        AddVertex(x2, y2, 0.0f, u2, v2);
        AddVertex(x1, y1, 0.0f, u1, v1);
        AddVertex(x2, y2, 0.0f, u2, v2);
        AddVertex(x1, y2, 0.0f, u1, v2);
    }

    /// <summary>
    /// Constructs vertex quads for the text string, switches OpenGL blend/depth states, and renders the text to the screen.
    /// </summary>
    /// <param name="text">The string content to be displayed.</param>
    /// <param name="x">The screen-space starting X position for the text string layout.</param>
    /// <param name="y">The screen-space starting Y position for the text string layout.</param>
    /// <param name="scale">The multiplier used to scale the textual font rendering scale up or down.</param>
    /// <param name="projection">The orthographic or perspective matrix mapping text coordinates to the screen layout.</param>
    /// <param name="color">The color vector applied to shade the text glyphs.</param>
    /// <param name="spacing">The pixel gap multiplier applied uniformly between individual characters. Default is 1.0f.</param>
    public void DrawString(string text, float x, float y, float scale, Matrix4 projection, Vector4 color, float spacing = 1.0f)
    {
        if (string.IsNullOrEmpty(text)) return;

        _vertexBuffer.Clear();
        float currentX = x;

        foreach (char c in text)
        {
            if (_fontAtlas.TryGetGlyph(c, out var glyph))
            {
                AddCharToBuffer(c, currentX, y, scale);
                currentX += (glyph.Advance + spacing) * scale; 
            }
        }

        float[] vertices = _vertexBuffer.ToArray();
        if (vertices.Length == 0) return;

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        GL.Disable(EnableCap.DepthTest);
        GL.DepthMask(false); 

        GL.UseProgram(_shaderProgram);
        GL.UniformMatrix4(_locProjection, false, ref projection);
        
        GL.Uniform4(_locColor, color);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _fontAtlas.TextureID);
        GL.Uniform1(_locTexture, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);
        
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        
        GL.DepthMask(true);
        GL.Enable(EnableCap.DepthTest); 

        GL.Disable(EnableCap.Blend);
    }

    public float MeasureWidth(string text, float scale, float spacing = 1.0f)
    {
        if (string.IsNullOrEmpty(text)) return 0f;

        float width = 0f;
        foreach (char c in text)
        {
            if (_fontAtlas.TryGetGlyph(c, out var glyph))
            {
                width += (glyph.Advance + spacing) * scale;
            }
        }
        
        return width;
    }

    /// <summary>
    /// Releases the inner font atlas resources and deletes unmanaged OpenGL objects (VAO, VBO, and shader program).
    /// </summary>
    public void Dispose()
    {
        _fontAtlas.Dispose();
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GL.DeleteProgram(_shaderProgram);
    }
}