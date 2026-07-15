namespace Unminal.Render.PrimitiveFigures._2D.Primitive2D_AbstractClass;

public abstract class Primitive2D : IDisposable
{
    private int VBO;
    private int VAO;
    private int ShaderProgram = -1;

    public Vector2 Position {get; set;} = Vector2.Zero;
    public Vector3 Color {get; set;} = Vector3.One;
    public float Alpha {get; set;} = 1.0f;
    public Vector2 Scale {get; set;} = Vector2.One;
    public float Rotation {get; set;} = 0.0f; 

    protected int VertexCount {get; private set;}

    private const string pathVertex = "./Assets/shaders/base.vert";
    private const string pathFragment = "./Assets/shaders/base.frag";

    /// <summary>
    /// Initializes a new instance of the class with default values, automatically loading geometry and compiling shaders.
    /// </summary>
    public Primitive2D()
    {
        InitializeGeometry();
        InitializeShader();
    }

    /// <summary>
    /// Initializes a new instance of the class with specified transformation, color, alpha, and rotation parameters.
    /// </summary>
    /// <param name="position">The initial position on the 2D plane.</param>
    /// <param name="scale">The initial scale factor of the primitive.</param>
    /// <param name="color">The RGB color vector of the primitive.</param>
    /// <param name="alpha">The alpha opacity value.</param>
    /// <param name="rotation">The rotation angle in radians.</param>
    public Primitive2D(Vector2 position, Vector2 scale, Vector3 color, float alpha, float rotation) : this()
    {
        Position = position;
        Scale = scale;
        Color = color;
        Alpha = alpha;
        Rotation = rotation;
    }

    /// <summary>
    /// Loads the vertex and fragment shader source code from files, compiles them, and links them into a single shader program.
    /// </summary>
    private void InitializeShader()
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

    /// <summary>
    /// Generates and configures OpenGL buffers (VAO and VBO) and uploads the vertex coordinate array to the GPU memory.
    /// </summary>
    private void InitializeGeometry()
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

    /// <summary>
    /// Checks the compilation status of the specified shader and throws an exception if it failed.
    /// </summary>
    /// <param name="shaderId">The OpenGL identifier of the shader to check.</param>
    /// <exception cref="Exception">Thrown if the shader compilation status is unsuccessful.</exception>
    public void CheckShaderCompilation(int shaderId)
    {   
        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
            throw new Exception($"[Primitive2D] Shader Error: {GL.GetShaderInfoLog(shaderId)}");
    }

    /// <summary>
    /// Checks the linking status of the shader program and throws an exception if it failed.
    /// </summary>
    /// <exception cref="Exception">Thrown if the shader program linking status is unsuccessful.</exception>
    public void CheckShaderLink()
    {
        GL.GetProgram(ShaderProgram, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
            throw new Exception($"[Primitive2D] Link Error: {GL.GetProgramInfoLog(ShaderProgram)}");
    }

    /// <summary>
    /// When overridden in a derived class, returns a flat array of vertex coordinates (X, Y) for the primitive's geometry.
    /// </summary>
    /// <returns>An array of floats representing the vertex positions of the primitive.</returns>
    protected abstract float[] GetVertices();

    /// <summary>
    /// Renders the primitive on the screen by calculating the model matrix (scale, rotation, translation) and passing all uniform parameters to the shader.
    /// </summary>
    /// <param name="projection">The projection matrix used to transform coordinates into screen space.</param>
    public void Draw(Matrix4 projection)
    {
        GL.UseProgram(ShaderProgram);

        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateScale(Scale.X, Scale.Y, 1.0f);
        model *= Matrix4.CreateRotationZ(Rotation);
        model *= Matrix4.CreateTranslation(Position.X, Position.Y, 0.0f);

        int locProj = GL.GetUniformLocation(ShaderProgram, "uProjection");
        int locModel = GL.GetUniformLocation(ShaderProgram, "uModel");
        int locColor = GL.GetUniformLocation(ShaderProgram, "uColor");

        GL.UniformMatrix4(locProj, false, ref projection);
        GL.UniformMatrix4(locModel, false, ref model);
        
        Vector4 finalColor = new Vector4(Color.X, Color.Y, Color.Z, Alpha);
        
        GL.Uniform4(locColor, finalColor);

        GL.BindVertexArray(VAO);
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, VertexCount);
        GL.BindVertexArray(0);
    }

    /// <summary>
    /// Frees the unmanaged OpenGL resources (VAO and VBO) allocated by this primitive.
    /// </summary>
    public virtual void Dispose()
    {
        GL.DeleteVertexArray(VAO);
        GL.DeleteBuffer(VBO);

        if (ShaderProgram != -1){
            GL.DeleteShader(ShaderProgram);
            ShaderProgram = -1;
        }
    }
}