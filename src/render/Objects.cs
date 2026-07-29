namespace Unminal.Render.Objects;

[SupportedOSPlatform("windows")]
public class GameObject : IDisposable
{
    public Mesh? Mesh { get; set; }
    public Shader? Shader { get; set; }
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Quaternion Orientation { get; set; } = Quaternion.Identity;
    public Vector3 Scale { get; set; } = Vector3.One;
    public Vector3 Color { get; set; } = Vector3.One;
    private bool _disposed = false;
    private static Shader? _defaultShader;

    public GameObject(Mesh mesh, Shader shader) {
        Mesh = mesh;
        Shader = shader;
    }

    public GameObject(string objFilePath) 
        : this(LoadMeshFromObj(objFilePath), GetOrCreateDefaultShader()) 
    {
    }

    private static Mesh LoadMeshFromObj(string path)
    {
        var modelData = ObjLoader.Load(path);
        return new Mesh(modelData.Vertices, modelData.Indices);
    }

    private static Shader GetOrCreateDefaultShader()
    {
        if (_defaultShader == null)
        {
            _defaultShader = new Shader(
                GetPath.GetCorrectPath(Engine.Paths.Shaders.mainV),
                GetPath.GetCorrectPath(Engine.Paths.Shaders.mainF)
            );
        }
        return _defaultShader;
    }

    public void Draw() {
        if (Engine.Player.CameraObj == null) {
            Console.CreateLog(Console.LogType.WARNING, "Camera is null");
            return;
        }
        if (Mesh == null || Shader == null) return;
        Shader.Use();

        Engine.LightingPipeline?.ApplyLighting(Shader);

        Shader.SetVector3("objectColor", Color);
        Shader.SetVector3("viewPos", Engine.Player.CameraObj.Position);
        
        Matrix4 model = Matrix4.CreateScale(Scale) 
                    * Matrix4.CreateFromQuaternion(Orientation) 
                    * Matrix4.CreateTranslation(Position);
        
        Shader.SetMatrix4("model", model);
        Shader.SetMatrix4("view", Engine.View);
        Shader.SetMatrix4("projection", Engine.Projection);
        Mesh.Draw();
    }

    public void Rotate(float angle, string axis) 
    {
        float radians = MathHelper.DegreesToRadians(angle * Engine.DeltaTime);
        
        Vector3 rotationAxis = axis.ToLower() switch 
        {
            "x" => Vector3.UnitX,
            "z" => Vector3.UnitZ,
            "y" => Vector3.UnitY,
            _   => Vector3.Zero 
        };

        if (rotationAxis == Vector3.Zero) return;

        Orientation = Quaternion.FromAxisAngle(rotationAxis, radians) * Orientation;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (disposing) {
                Mesh?.Dispose();
            }
            Mesh = null;
            Shader = null;
            
            _disposed = true;
        }
    }

    ~GameObject() {
        Dispose(false);
    }
}