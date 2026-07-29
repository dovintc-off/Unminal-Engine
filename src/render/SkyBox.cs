namespace Unminal.Render.SkyBox;

[SupportedOSPlatform("windows")]
public class Skybox : IDisposable
{
    private int _vao;
    private int _vbo;
    private int _textureId;
    private int _shaderProgram;
    
    private int _projLoc;
    private int _viewLoc;
    private int _skyboxLoc;

    private static readonly float[] _vertices = new float[] {
        -1.0f,  1.0f, -1.0f,  -1.0f, -1.0f, -1.0f,   1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,   1.0f,  1.0f, -1.0f,  -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,  -1.0f, -1.0f, -1.0f,  -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,  -1.0f,  1.0f,  1.0f,  -1.0f, -1.0f,  1.0f,

        1.0f, -1.0f, -1.0f,   1.0f, -1.0f,  1.0f,   1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,   1.0f,  1.0f, -1.0f,   1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,  -1.0f,  1.0f,  1.0f,   1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,   1.0f, -1.0f,  1.0f,  -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,   1.0f,  1.0f, -1.0f,   1.0f,  1.0f,  1.0f,
        1.0f,  1.0f,  1.0f,  -1.0f,  1.0f,  1.0f,  -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,  -1.0f, -1.0f,  1.0f,   1.0f, -1.0f, -1.0f,
        1.0f, -1.0f, -1.0f,  -1.0f, -1.0f,  1.0f,   1.0f, -1.0f,  1.0f
    };

    /// <summary>
    /// Initializes a new instance of the Skybox class by loading shaders, setting up geometry, and loading cubemap textures.
    /// </summary>
    /// <param name="faces">An array of file paths to the six texture images representing the skybox faces.</param>
    public Skybox(string[] faces)
    {
        LoadShaders();
        SetupGeometry();
        LoadTexture(faces);
    }

    private void LoadShaders()
    {
        string vertCode = File.ReadAllText(GetPath.GetCorrectPath(Engine.Paths.Shaders.skyboxV));

        string fragCode = File.ReadAllText(GetPath.GetCorrectPath(Engine.Paths.Shaders.skyboxF));

        int vertShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShader, vertCode);
        GL.CompileShader(vertShader);
        
        GL.GetShader(vertShader, ShaderParameter.CompileStatus, out int vSuccess);
        if (vSuccess == 0) {
            Console.CreateLog(Console.LogType.ERROR, "Vertex Shader Error: " + GL.GetShaderInfoLog(vertShader));
        }

        int fragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShader, fragCode);
        GL.CompileShader(fragShader);

        GL.GetShader(fragShader, ShaderParameter.CompileStatus, out int fSuccess);
        if (fSuccess == 0) {
            Console.CreateLog(Console.LogType.ERROR, "Fragment Shader Error: " + GL.GetShaderInfoLog(fragShader));
        }

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertShader);
        GL.AttachShader(_shaderProgram, fragShader);
        GL.LinkProgram(_shaderProgram);

        GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out int lSuccess);
        if (lSuccess == 0)
        {
            Console.CreateLog(Console.LogType.ERROR, "Link Error: " + GL.GetProgramInfoLog(_shaderProgram));
        }

        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);

        _projLoc = GL.GetUniformLocation(_shaderProgram, "projection");
        _viewLoc = GL.GetUniformLocation(_shaderProgram, "view");
        _skyboxLoc = GL.GetUniformLocation(_shaderProgram, "skybox");
    }

    private void SetupGeometry()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), IntPtr.Zero);
        
        GL.BindVertexArray(0);
    }

    /// <summary>
    /// Loads and configures the six cubemap textures, applying necessary image rotations and alignment adjustments for each face.
    /// </summary>
    /// <param name="faces">An array of file paths to the six texture images representing the skybox faces.</param>
    private void LoadTexture(string[] faces)
    {
        _textureId = GL.GenTexture();
        GL.BindTexture(TextureTarget.TextureCubeMap, _textureId);

        for (int i = 0; i < faces.Length; i++)
        {
            try 
            {
                if (!File.Exists(faces[i]))
                {
                    Console.CreateLog(Console.LogType.ERROR, $"File not found: {faces[i]}");
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, 1, 1, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                    continue;
                }

                using (var bitmap = new Bitmap(faces[i]))
                {
                    if (i == 0 || i == 1 || i == 4 || i == 5)
                        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    else if (i == 2)
                    {                            
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone); 
                        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (i == 3)
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);

                    var data = bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    TextureTarget target = TextureTarget.TextureCubeMapPositiveX + i;

                    GL.TexImage2D(
                        target, 
                        0, 
                        PixelInternalFormat.Rgb,
                        bitmap.Width, 
                        bitmap.Height, 
                        0,
                        PixelFormat.Rgb, 
                        PixelType.UnsignedByte,
                        data.Scan0);

                    bitmap.UnlockBits(data);
                }
            }
            catch (Exception ex)
            {
                Console.CreateLog(Console.LogType.ERROR, $"Cant Loading face {i} ({faces[i]}): {ex.Message}");
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, 1, 1, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            }
        }

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
    }

    public void Draw() {
        Matrix4 projection = Engine.Projection;
        bool depthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
        bool cullFaceEnabled = GL.IsEnabled(EnableCap.CullFace);

        GL.Disable(EnableCap.DepthTest);
        GL.DepthMask(false);
        GL.Disable(EnableCap.CullFace);

        GL.UseProgram(_shaderProgram);
        
        Matrix4 skyboxView = Engine.View;
        skyboxView.Row3.X = 0;
        skyboxView.Row3.Y = 0;
        skyboxView.Row3.Z = 0;

        GL.UniformMatrix4(_viewLoc, false, ref skyboxView);
        GL.UniformMatrix4(_projLoc, false, ref projection);
        
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, _textureId);
        GL.Uniform1(_skyboxLoc, 0);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        
        if (depthTestEnabled) GL.Enable(EnableCap.DepthTest);
        else GL.Disable(EnableCap.DepthTest);
        
        GL.DepthMask(true);
      
        if (cullFaceEnabled) GL.Enable(EnableCap.CullFace);
        else GL.Disable(EnableCap.CullFace);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        if (_vao != 0) GL.DeleteVertexArray(_vao);
        if (_vbo != 0) GL.DeleteBuffer(_vbo);
        if (_textureId != 0) GL.DeleteTexture(_textureId);
        if (_shaderProgram != 0) GL.DeleteProgram(_shaderProgram);
        
        _vao = 0;
        _vbo = 0;
        _textureId = 0;
        _shaderProgram = 0;
    }
}