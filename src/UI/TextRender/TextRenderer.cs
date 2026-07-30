namespace Unminal.UI.TextRender.TextRenderer;

[SupportedOSPlatform("windows")]
public class Text : IDisposable
{
    private readonly Atlas? _fontAtlas;

    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _shaderProgram;
    private readonly int _locProjection;
    private readonly int _locTexture;
    private readonly int _locColor;

    private readonly List<float> _vertexBuffer = new List<float>();
    public Text(string fontPath, int fontSize) {
        new Text(fontPath, fontSize, GetPath.GetCorrectPath(Engine.Paths.Shaders.textV), GetPath.GetCorrectPath(Engine.Paths.Shaders.textF));
    }
    public Text(string fontPath, int fontSize, string shaderVertex, string shaderFragment) {
        _fontAtlas = new Atlas(GetPath.GetCorrectPath(fontPath), fontSize);

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

    private void CheckShaderCompile(int shader, string type) {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string log = GL.GetShaderInfoLog(shader);
            throw new Exception($"{type} Shader Compilation Error:\n{log}");
        }
    }
    private void AddVertex(float x, float y, float z, float u, float v) {
        _vertexBuffer.Add(x);
        _vertexBuffer.Add(y);
        _vertexBuffer.Add(z);
        _vertexBuffer.Add(u);
        _vertexBuffer.Add(v);
    }

    private void AddCharToBuffer(char c, float x, float y, float scale) {
        if (!_fontAtlas!.TryGetGlyph(c, out var glyph)) return;

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

    public void DrawString(string text, float x, float y, float scale, Vector4 color, float spacing = 1.0f) {
        // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        Matrix4 projection = Engine.Ortho;
        if (string.IsNullOrEmpty(text)) return;

        _vertexBuffer.Clear();
        float currentX = x;

        foreach (char c in text)
        {
            if (_fontAtlas!.TryGetGlyph(c, out var glyph))
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
        GL.BindTexture(TextureTarget.Texture2D, _fontAtlas!.TextureID);
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
            if (_fontAtlas!.TryGetGlyph(c, out var glyph))
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
        _fontAtlas!.Dispose();
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GL.DeleteProgram(_shaderProgram);
    }
}