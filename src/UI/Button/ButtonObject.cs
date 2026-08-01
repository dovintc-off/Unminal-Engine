namespace Unminal.UI.ButtonObject;

[SupportedOSPlatform("windows")]
public class Button : IDisposable {
    private readonly EngineButton logic;
    private readonly RenderButton visual;

    public uint ID => logic.ID;
    public string? name;
    public Square Bounds => logic.Bounds!;
    
    public Button(EngineButton Logic, RenderButton Visual) {
        logic = Logic;
        visual = Visual;
    }

    public void Update() {
        logic.Update();
    }

    public void Draw() {
        visual.Draw(logic);
    }

    public void Dispose() {
        logic.Dispose();
    }
}