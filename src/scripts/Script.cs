namespace Unminal.Script;

using Unminal.Core.Commands.Manager;
using static Unminal.Script.Objects;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame {
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? skybox;
    private Text? _textRenderer;

    private EngineButton? _uiButtonLogic1;
    private RenderButton? _uiButtonVisual1;
    private EngineButton? _uiButtonLogic2;
    private RenderButton? _uiButtonVisual2;

    public override void Load(Matrix4 initialProjection) {

        EventBusUi.Subscribe<ButtonPressedEvent>(OnBtnPress);
        EventBusUi.Subscribe<ButtonHeldEvent>(OnBtnHeld);
        EventBusUi.Subscribe<ButtonRelesedEvent>(OnBtnReleased);

        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);
        _objects = LoadObjects(_objects);

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(new Vector3(0, 0, 0), Colors.White, 30f));

        _textRenderer = new Text("font:/Arial/arialmt.ttf", 32);

        skybox = new Skybox(Engine.Paths.BaseSkyBoxAssets);

        (_uiButtonLogic1, _uiButtonVisual1) = UIFactory.CreateButton(
            pos:new Vector2(50f, 50f), 
            scale:new Vector2(20f, 20f), 
            rotation:0f,
            color:Colors.Blue,
            hoveredColor:Colors.Green
        );

        (_uiButtonLogic2, _uiButtonVisual2) = UIFactory.CreateButton(
            pos:new Vector2(100f, 100f), 
            scale:new Vector2(20f, 20f), 
            rotation:0f,
            color:Colors.Blue,  
            hoveredColor:Colors.Green
        );

        Command myCommand = new Command(){
            Name = "SayHello",
            Layer = null!,
            ExecuteMethod = "SayHello",
            ExecutedLayer = true
        };

        CommandManager.AddCommand("debug", myCommand);
    }

    public void OnBtnPress(ButtonPressedEvent e) =>
        Console.WriteLine($"\n[#blue]Button Pressed on {e.ButtonId}");

    public void OnBtnHeld(ButtonHeldEvent e) =>
        Console.Write($"\rDuration Held: {e.Duration:F3} on {e.ButtonId}");

    public void OnBtnReleased(ButtonRelesedEvent e) => 
        Console.WriteLine($"\n[#blue]Button Released on {e.ButtonId}");

    public override void Update() {
        base.Update();
        
        if (Engine.Player.CameraObj == null) return;

        _uiButtonLogic1?.Update();
        _uiButtonLogic2?.Update();

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

        if (_uiButtonLogic1 != null) {
            _uiButtonVisual1?.Draw(_uiButtonLogic1);
        }
        if (_uiButtonLogic2 != null) {
            _uiButtonVisual2?.Draw(_uiButtonLogic2);
        }
    }

    public override void Unload() {
        base.Unload();

        foreach (var obj in _objects) obj.Dispose();
        EventBusUi.Unsubscribe<ButtonPressedEvent>(OnBtnPress);
        EventBusUi.Unsubscribe<ButtonHeldEvent>(OnBtnHeld);
        EventBusUi.Unsubscribe<ButtonRelesedEvent>(OnBtnReleased);
        Billboard.Dispose();
        _textRenderer?.Dispose();
        skybox?.Dispose();
    }
}