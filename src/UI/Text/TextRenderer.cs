// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.UI.TextRender.TextRenderer;

[SupportedOSPlatform("windows")]
public class Text : IDisposable {
    private readonly Atlas? _fontAtlas;

    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _shaderProgram;
    private readonly int _locProjection;
    private readonly int _locTexture;
    private float FontSize;
    private readonly List<float> _vertexBuffer = new List<float>();
    private const int VERTEX_SIZE = 9;

    public Text(string fontPath, int fontSize) 
        : this(fontPath, fontSize, GetPath.GetCorrectPath(Engine.Paths.Shaders.textV), GetPath.GetCorrectPath(Engine.Paths.Shaders.textF)) 
    {}

    public Text(string fontPath, int fontSize, string shaderVertex, string shaderFragment) {
        FontSize = fontSize;
        _fontAtlas = new Atlas(GetPath.GetCorrectPath(fontPath), fontSize);

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

        int stride = VERTEX_SIZE * sizeof(float);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);

        _shaderProgram = LoadShaderProgram(shaderVertex, shaderFragment);
        GL.UseProgram(_shaderProgram);
        _locProjection = GL.GetUniformLocation(_shaderProgram, "projection");
        _locTexture = GL.GetUniformLocation(_shaderProgram, "uTexture");
        GL.UseProgram(0);
    }

    private int LoadShaderProgram(string vertPath, string fragPath) {
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
        if (success == 0) {
            string log = GL.GetShaderInfoLog(shader);
            throw new Exception($"{type} Shader Compilation Error:\n{log}");
        }
    }
    private void AddVertex(float x, float y, float z, float u, float v, Vector4 color) {
        _vertexBuffer.Add(x); _vertexBuffer.Add(y); _vertexBuffer.Add(z);
        _vertexBuffer.Add(u); _vertexBuffer.Add(v);
        _vertexBuffer.Add(color.X); _vertexBuffer.Add(color.Y); _vertexBuffer.Add(color.Z); _vertexBuffer.Add(color.W);
    }

    private void AddCharToBuffer(char c, float x, float y, float scale, Vector4 color) {
        if (!_fontAtlas!.TryGetGlyph(c, out var glyph)) return;

        float w = glyph.Width * _fontAtlas.Width * scale;
        float h = glyph.Height * _fontAtlas.Height * scale;

        float x1 = x, y1 = y;
        float x2 = x + w, y2 = y + h;

        float u1 = glyph.U, v1 = glyph.V;
        float u2 = glyph.U + glyph.Width, v2 = glyph.V + glyph.Height;

        AddVertex(x1, y1, 0.0f, u1, v1, color);
        AddVertex(x2, y1, 0.0f, u2, v1, color);
        AddVertex(x2, y2, 0.0f, u2, v2, color);

        AddVertex(x1, y1, 0.0f, u1, v1, color);
        AddVertex(x2, y2, 0.0f, u2, v2, color);
        AddVertex(x1, y2, 0.0f, u1, v2, color);
    }
    
    public void DrawString(string text, float x, float y, float scale, Vector4 defaultColor, float spacing = 1.0f) {
        Matrix4 projection = Engine.Ortho;
        if (string.IsNullOrEmpty(text)) return;

        _vertexBuffer.Clear();
        float currentX = x;
        Vector4 curColor = defaultColor;

        int i = 0;
        while (i < text.Length) {
            if (i + 1 < text.Length && text[i] == '[' && text[i + 1] == '#') {
                int closeBracket = text.IndexOf(']', i + 2);
                if (closeBracket != -1) {
                    string content = text.Substring(i + 2, closeBracket - i - 2);
                    if (Colors.TryGetNamedColor(content, out Vector4 nColor)) {
                        curColor = nColor;
                    } else if (Colors.IsValidHEX(content)) {
                        curColor = Colors.HEXtoVEC4(content);
                    }
                    i = closeBracket + 1;
                    continue;
                }
            }
            char c = text[i];
            if (_fontAtlas!.TryGetGlyph(c, out var glyph)) {
                AddCharToBuffer(c, currentX, y, scale / FontSize, curColor);
                currentX += (glyph.Advance + spacing) * (scale / FontSize);
            }
            i++;
        }

        float[] vertices = _vertexBuffer.ToArray();
        if (vertices.Length == 0) return;

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.DepthTest);
        GL.DepthMask(false); 

        GL.UseProgram(_shaderProgram);
        GL.UniformMatrix4(_locProjection, false, ref projection);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _fontAtlas!.TextureID);
        GL.Uniform1(_locTexture, 0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / VERTEX_SIZE);
        
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DepthMask(true);
        GL.Enable(EnableCap.DepthTest); 
        GL.Disable(EnableCap.Blend);
    }

    public static List<TextPart> ParseColor(string text, Vector4 DefaultColor)  {
        var Parts = new List<TextPart>{};
        string parsedText = text;
        bool IsValidColor = Colors.IsValidHEX(DefaultColor); 
        if (!IsValidColor) DefaultColor = new Vector4(1, 1, 1, 1);
        
        while(!string.IsNullOrWhiteSpace(parsedText)) {
            int entry_color = parsedText.IndexOf("[#");
            int close_color = parsedText.IndexOf("]");
            if (entry_color == -1) return new List<TextPart> { new TextPart { Text = parsedText, TextColor = DefaultColor } };
            if (entry_color == -1 || close_color == -1 || close_color <= entry_color) break;
            if (entry_color > 0) {
                Parts.Add(new TextPart {Text = parsedText[..entry_color], TextColor = DefaultColor});
                parsedText = parsedText[entry_color..];
            } else {
                string color = parsedText[(entry_color+2)..close_color];
                parsedText = parsedText[(close_color+1)..];
                if (parsedText.IndexOf("[#") is int next_close_color && next_close_color != -1) {
                    string PartText = parsedText[..next_close_color]; 
                    parsedText = parsedText[next_close_color..]; 
                    Parts.Add(new TextPart {Text = PartText, TextColor = Colors.HEXtoVEC4(color)});
                } else {
                    Parts.Add(new TextPart {Text = parsedText, TextColor = Colors.HEXtoVEC4(color)});
                    break;
                }
            }
        }
        return Parts;
    }

    public float MeasureWidth(string text, float scale, float spacing = 1.0f) {
        if (string.IsNullOrEmpty(text)) return 0f;
        float width = 0f;
        
        int i = 0;
        while (i < text.Length) {
            if (i + 1 < text.Length && text[i] == '[' && text[i + 1] == '#') {
                int closeBracket = text.IndexOf(']', i + 2);
                if (closeBracket != -1) {
                    i = closeBracket + 1;
                    continue;
                }
            }
            char c = text[i];
            if (_fontAtlas!.TryGetGlyph(c, out var glyph))
                width += (glyph.Advance + spacing) * scale;
            i++;
        }
        return width;
    }

    public struct TextPart {
        public string Text;
        public Vector4 TextColor;
    } 

    public void Dispose() {
        _fontAtlas!.Dispose();
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GL.DeleteProgram(_shaderProgram);
    }
}
