namespace Unminal.Script;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame
{
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? _skybox;
    private Text? _textRenderer;
    private RichTextSegment? _richTextRenderer;

    public override void Load(Matrix4 initialProjection)
    {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);

        _richTextRenderer = new RichTextSegment(new Vector4(1, 1, 1, 1));
        _textRenderer = new Text(
            "./Assets/fonts/PFAgoraSlabPro-Bold.ttf",
            32,
            "./Assets/shaders/text/shader.vert",
            "./Assets/shaders/text/shader.frag"
        );

        var modelData = ObjLoader.Load("./Assets/3D_objects/teapol.obj");
        var mesh = new Mesh(modelData.Vertices, modelData.Indices);
        var shader = new Shader(
            "./Assets/shaders/main/shader.vert", 
            "./Assets/shaders/main/shader.frag"
        );

        var teapot1 = new GameObject(mesh, shader)
        {
            Position = new Vector3(-20, 0, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.8f, 0.2f, 0.2f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(teapot1);

        modelData = ObjLoader.Load("./Assets/3D_objects/cube.obj");
        mesh = new Mesh(modelData.Vertices, modelData.Indices);

        var cube1 = new GameObject(mesh, shader)
        {
            Position = new Vector3(20, 12, -50), 
            Scale = new Vector3(6f),
            Color = new Vector3(0.2f, 0.8f, 0.2f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(cube1);
        var cube2 = new GameObject(mesh, shader)
        {
            Position = new Vector3(20, 0, -50), 
            Scale = new Vector3(6f),
            Color = new Vector3(0.2f, 0.2f, 0.8f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(cube2);

        string[] skyboxFaces = 
        {
            "./Assets/SkyBox/right.png",
            "./Assets/SkyBox/left.png",
            "./Assets/SkyBox/top.png",
            "./Assets/SkyBox/bottom.png",
            "./Assets/SkyBox/front.png",
            "./Assets/SkyBox/back.png"
        };
        _skybox = new Skybox(skyboxFaces);
    }

    public override void Update(FrameUpdateVars FUV)
    {
        base.Update(FUV);
    }

    public override void Draw(Matrix4 projection) {
        if (ActiveCamera == null) return;
        Matrix4 view = ActiveCamera.GetViewMatrix();

        _skybox!.Draw(view, projection);

        foreach (var obj in _objects)
        {
            obj.Draw(view, projection);
        }
        if (!EngineValues.IsConsoleOpen && EngineValues.IsPaused && _textRenderer != null){
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, EngineValues.WindowSize.Y, EngineValues.WindowSize.X, 0, -1, 1);
            _richTextRenderer?.Draw(_textRenderer, "In Pause", 10, 550, 0.5f, ortho);
        }
    }
}