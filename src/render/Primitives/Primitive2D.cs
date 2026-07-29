namespace Unminal.Render.Primitive._2D;

[SupportedOSPlatform("windows")]
public abstract class Primitive2D : IDisposable {
    protected int VBO;
    protected int VAO;
    protected int ShaderProgram = -1;
    protected int VertexCount;
    
    public Vector2 Position {get; set;} = Vector2.Zero;
    public Vector4 Color {get; set;} = Vector4.One;
    public Vector2 Scale {get; set;} = Vector2.One;
    public float Rotation {get; set;} = 0.0f;
    public Vector2 Pivot {get; set;} = Vector2.Zero;
    
    private readonly string pathVertex = GetPath.GetCorrectPath(Engine.Paths.Shaders.baseV);
    private readonly string pathFragment = GetPath.GetCorrectPath(Engine.Paths.Shaders.baseF);

    public Primitive2D()
    {
        InitializeGeometry();
        InitializeShader();
    }

    protected void InitializeShader()
    {
        if (ShaderProgram != -1) return;
        if (!File.Exists(pathVertex))
        {
            Console.WriteLine($"[Primitive2D] Cant load file '{pathVertex}' - cant exist");
            return;
        }
        if (!File.Exists(pathFragment))
        {
            Console.WriteLine($"[Primitive2D] Cant load file '{pathFragment}' - cant exist");
            return; 
        }
        string vertexSource = File.ReadAllText(pathVertex);
        string fragmentSource = File.ReadAllText(pathFragment);
        int vertShader = GL.CreateShader(ShaderType.VertexShader);
        int fragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(vertShader, vertexSource);
        GL.ShaderSource(fragShader, fragmentSource);
        GL.CompileShader(vertShader);
        GL.CompileShader(fragShader);
        CheckShaderCompilation(vertShader);
        CheckShaderCompilation(fragShader);
        ShaderProgram = GL.CreateProgram();
        GL.AttachShader(ShaderProgram, vertShader);
        GL.AttachShader(ShaderProgram, fragShader);
        GL.LinkProgram(ShaderProgram);
        CheckShaderLink();
        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);
    }

    protected void InitializeGeometry()
    {   
        float[] vertices = GetVertices();
        VertexCount = vertices.Length / 2;
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void CheckShaderCompilation(int shaderId)
    {   
        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
            throw new Exception($"[Primitive2D] Shader Error: {GL.GetShaderInfoLog(shaderId)}");
    }

    public void CheckShaderLink()
    {
        GL.GetProgram(ShaderProgram, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
            throw new Exception($"[Primitive2D] Link Error: {GL.GetProgramInfoLog(ShaderProgram)}");
    }

    protected abstract float[] GetVertices();

    public void Draw() {
        Matrix4 projection = Engine.Ortho;
        GL.UseProgram(ShaderProgram);
        
        int locProj = GL.GetUniformLocation(ShaderProgram, "uProjection");
        int locPos = GL.GetUniformLocation(ShaderProgram, "uPosition");
        int locRot = GL.GetUniformLocation(ShaderProgram, "uRotation");
        int locScale = GL.GetUniformLocation(ShaderProgram, "uScale");
        int locPivot = GL.GetUniformLocation(ShaderProgram, "uPivot");
        int locColor = GL.GetUniformLocation(ShaderProgram, "uColor");
        
        GL.UniformMatrix4(locProj, false, ref projection);
        GL.Uniform2(locPos, Position);
        GL.Uniform1(locRot, Rotation);
        GL.Uniform2(locScale, Scale);
        GL.Uniform2(locPivot, Pivot);
        GL.Uniform4(locColor, Color);
        
        GL.BindVertexArray(VAO);
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, VertexCount);
        GL.BindVertexArray(0);
    }

    public virtual void Dispose()
    {
        GL.DeleteVertexArray(VAO);
        GL.DeleteBuffer(VBO);
        if (ShaderProgram != -1){
            GL.DeleteProgram(ShaderProgram);
            ShaderProgram = -1;
        }
    }
}