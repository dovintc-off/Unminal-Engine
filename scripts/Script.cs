namespace Dov1ntc.MyGameIn3d;

using static Dov1ntc.MyGameIn3d.Scene;
using Unminal.Core.Scripting.Script;
using Unminal.Core.Scripting.Utils;
using Unminal.Core.Commands.Structure;
using Unminal.Core.State;
using Unminal.Core.PlayerCamera;
using Unminal.Core.Commands.Manager;
using Unminal.Render.Light;
using Unminal.Render.Objects;
using Unminal.Render.SkyBox;
using Unminal.Render.Billboards;
using Unminal.UI.TextRender.TextRenderer;
using Unminal.UI.EventBus;
using Unminal.Utils.Colors;

[SupportedOSPlatform("windows")]
[Script]
public class Game : Script {
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? skybox;
    private Text? _textRenderer;

    public override void Load(Matrix4 initialProjection) {
        EventBusUi.Subscribe<ButtonPressedEvent>(OnBtnPress);
        EventBusUi.Subscribe<ButtonHeldEvent>(OnBtnHeld);
        EventBusUi.Subscribe<ButtonRelesedEvent>(OnBtnReleased);

        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);
        _objects = LoadScene(_objects);

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(new Vector3(0, 0, 0), Colors.White, 30f));

        _textRenderer = new Text("Assets/fonts/PFAgoraSlabPro-Bold.ttf", 256);

        skybox = new Skybox(Engine.Paths.BaseSkyBoxAssets);

        CommandManager.AddCommand("debug", new Command(){Name = "SayHello", Layer = null!, ExecuteMethod = "SayHello", ExecutedLayer = true});
        CommandManager.AddCommand("debug", new Command(){Name = "Server", Layer = null!, ExecutedLayer = false});
        CommandManager.AddCommand("Server", new Command(){Name = "Open", Layer = null!, ExecuteMethod = "ServerOpen", ExecutedLayer = true});
        CommandManager.AddCommand("Server", new Command(){Name = "Conect", Layer = null!, ExecuteMethod = "ServerConect", ExecutedLayer = true});
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

        btn1?.Update();

        teapot1?.Rotate(90f, "x");
        cube1?.Rotate(90f, "y");
        cube2?.Rotate(90f, "z");

        Engine.LightManager?.ClearLights();
        Engine.LightManager?.AddLight(new LightData(Engine.Player.CameraObj.Position, Colors.White, 30f));
    }

    public override void Draw() {
        skybox!.Draw();

        foreach (var obj in _objects) obj.Draw();

        new Billboard()
            .Position(new Vector3(15, 8, -40)).Scale(new Vector2(8.0f, 5.0f))
            .Texture("Assets/textures/cat.png").Draw();

        if (Console.Instance!.IsOpen) {
            Scene.btn1?.Draw();
            _textRenderer!.DrawString("Hello!", 200, 200, 20, new Vector4(1, 1, 1, 1));
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
        Scene.btn1?.Dispose();
    }
}