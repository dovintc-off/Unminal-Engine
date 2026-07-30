namespace Unminal.Script;

using Unminal.Core.Commands.Manager;
using static Unminal.Script.Objects;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame {
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? skybox;
    private Text? _textRenderer;

    public override void Load(Matrix4 initialProjection) {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);
        _objects = LoadObjects(_objects);

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(new Vector3(0, 0, 0), Colors.White, 30f));

        _textRenderer = new Text("font:/Arial/arialmt.ttf", 32);

        skybox = new Skybox(Engine.Paths.BaseSkyBoxAssets);

        Command myCommand = new Command(){
            Name = "SayHello",
            Layer = null!,
            ExecuteMethod = "SayHello",
            ExecutedLayer = true
        };

        CommandManager.AddCommand("debug", myCommand);
    }

    public override void Update() {
        base.Update();
        if (Engine.Player.CameraObj == null) return;

        Scene.teapot1?.Rotate(90f, "x");
        Scene.cube1?.Rotate(90f, "y");
        Scene.cube2?.Rotate(90f, "z");

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(Engine.Player.CameraObj.Position, Colors.White, 30f));
    }

    public override void Draw() {
        skybox!.Draw();

        foreach (var obj in _objects) obj.Draw();

        new Billboard()
            .Position(new Vector3(15, 8, -40)).Scale(new Vector2(8.0f, 5.0f))
            .Texture("texture:/cat.png").Draw();
    }

    public override void Unload() {
        base.Unload();

        foreach (var obj in _objects) obj.Dispose();

        Billboard.Dispose();
        _textRenderer?.Dispose();
        skybox?.Dispose();
    }
}