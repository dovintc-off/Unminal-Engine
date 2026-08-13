// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
using StbImageSharp;

namespace Unminal.Render.Texture;

[SupportedOSPlatform("windows")]
public class Texture2D : IDisposable {
    public int Handle { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    private static readonly Dictionary<Vector4, Texture2D> _colorCache = new();
    private static readonly Dictionary<string, Texture2D> _fileCache = new();

    private static int _uiShaderHandle = -1;
    private static int _dummyVao = -1;
    private static bool _isUiInitialized = false;

    private Texture2D(int handle, int width, int height) {
        Handle = handle;
        Width = width;
        Height = height;
    }

    private static void InitializeUiRenderer() {
        if (_isUiInitialized) return;

        string vertPath = GetPath.GetCorrectPath("shader:/ui2d.vert");
        string fragPath = GetPath.GetCorrectPath("shader:/ui2d.frag");

        if (!File.Exists(vertPath) || !File.Exists(fragPath)) {
            throw new FileNotFoundException("[Unminal UI] Файлы шейдеров ui2d.vert или ui2d.frag не найдены!");
        }

        string VertShaderCode = File.ReadAllText(vertPath).Replace("\uFEFF", "") + "\n";
        string FragShaderCode = File.ReadAllText(fragPath).Replace("\uFEFF", "") + "\n";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, VertShaderCode);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertStatus);
        if (vertStatus == 0) {
            throw new Exception($"Vertex Compile Error: {GL.GetShaderInfoLog(vertexShader)}");
        }

        // 2. Фрагментный шейдер
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, FragShaderCode);
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragStatus);
        if (fragStatus == 0) {
            throw new Exception($"Fragment Compile Error: {GL.GetShaderInfoLog(fragmentShader)}");
        }

        _uiShaderHandle = GL.CreateProgram();
        GL.AttachShader(_uiShaderHandle, vertexShader);
        GL.AttachShader(_uiShaderHandle, fragmentShader);
        GL.LinkProgram(_uiShaderHandle);
        GL.GetProgram(_uiShaderHandle, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus == 0) {
            throw new Exception($"Link Error: {GL.GetProgramInfoLog(_uiShaderHandle)}");
        }

        GL.DetachShader(_uiShaderHandle, vertexShader);
        GL.DetachShader(_uiShaderHandle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        _dummyVao = GL.GenVertexArray();

        _isUiInitialized = true;
    }


    public void Draw2D(float pixelX, float pixelY, float screenWidth, float screenHeight) {
        if (!_isUiInitialized) {
            InitializeUiRenderer();
        }

        if (_uiShaderHandle == -1) return;

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.DepthTest); 

        GL.UseProgram(_uiShaderHandle);

        int locPos = GL.GetUniformLocation(_uiShaderHandle, "u_Position");
        if (locPos != -1) GL.Uniform2(locPos, pixelX, pixelY);

        int locTexSize = GL.GetUniformLocation(_uiShaderHandle, "u_TextureSize");
        if (locTexSize != -1) GL.Uniform2(locTexSize, (float)Width, (float)Height);

        int locScreen = GL.GetUniformLocation(_uiShaderHandle, "u_ScreenSize");
        if (locScreen != -1) GL.Uniform2(locScreen, screenWidth, screenHeight);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
        int locTex = GL.GetUniformLocation(_uiShaderHandle, "u_Texture");
        if (locTex != -1) GL.Uniform1(locTex, 0);

        GL.BindVertexArray(_dummyVao);
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

        GL.BindVertexArray(0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.UseProgram(0);
        GL.Enable(EnableCap.DepthTest); 
    }


    private static Texture2D LoadFromFile(string path) {
        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        StbImage.stbi_set_flip_vertically_on_load(1);
        int width, height;
        using (Stream stream = File.OpenRead(path)) {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            width = image.Width;
            height = image.Height;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                          width, height, 0, 
                          PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.BindTexture(TextureTarget.Texture2D, 0);
        return new Texture2D(handle, width, height);
    }

    public static Texture2D GetOrCreateFileTexture(string path) {
        if (_fileCache.TryGetValue(path, out var cachedTexture))
            return cachedTexture;

        Texture2D newTexture = LoadFromFile(path);
        _fileCache.Add(path, newTexture);
        return newTexture;
    }

    public static Texture2D GetOrCreateColorTexture(Vector4 color) {
        if (_colorCache.TryGetValue(color, out var cachedTexture))
            return cachedTexture;

        byte r = (byte)(Math.Clamp(color.X, 0f, 1f) * 255);
        byte g = (byte)(Math.Clamp(color.Y, 0f, 1f) * 255);
        byte b = (byte)(Math.Clamp(color.Z, 0f, 1f) * 255);
        byte a = (byte)(Math.Clamp(color.W, 0f, 1f) * 255);
        byte[] pixelData = { r, g, b, a };

        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                      1, 1, 0, 
                      PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        Texture2D newTexture = new Texture2D(handle, 1, 1);
        _colorCache.Add(color, newTexture);
        return newTexture;
    }

    public void Dispose() {
        GL.DeleteTexture(Handle);
    }

    public static void ClearAllCaches() {
        foreach (var tex in _colorCache.Values) tex.Dispose();
        foreach (var tex in _fileCache.Values)  tex.Dispose();
        _colorCache.Clear();
        _fileCache.Clear();

        if (_isUiInitialized) {
            if (_uiShaderHandle != -1)
                GL.DeleteProgram(_uiShaderHandle);
            
            if (_dummyVao != -1)
                GL.DeleteVertexArray(_dummyVao);

            _uiShaderHandle = -1;
            _dummyVao = -1;
            _isUiInitialized = false;
        }
    }
}
