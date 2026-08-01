namespace Unminal.UI.ButtonRender;

[SupportedOSPlatform("windows")]
public class RenderButton {
    private readonly Vector4 _normalColor;
    private readonly Vector4 _hoveredColor;

    public RenderButton(Vector4 normalColor, float dimmingMultiplier = 0.8f) {
        if (dimmingMultiplier > 1f || dimmingMultiplier < 0f) dimmingMultiplier = 0.8f;
        _normalColor = normalColor;
        _hoveredColor = normalColor * dimmingMultiplier;
        _hoveredColor.W = 1f;
    }

    public RenderButton(Vector4 normalColor, Vector4 hoveredColor) {
        _normalColor = normalColor;
        _hoveredColor = hoveredColor;
    }

    public RenderButton(Vector3 normalColor, Vector3 hoveredColor) 
        : this(new Vector4(normalColor, 1), new Vector4(hoveredColor, 1))
    {}

    public RenderButton(Vector3 normalColor, float dimmingMultiplier = 0.8f) 
        : this(new Vector4(normalColor, 1), dimmingMultiplier) 
    {}

    public void Draw(EngineButton button) {
        if (button.Bounds == null) return;

        MouseState? mouseState = Engine.CurrentMouse;
        bool isHovered = mouseState != null && button.Bounds.Contains(mouseState.Position);
        button.Bounds.Color = isHovered ? _hoveredColor : _normalColor;
        button.Bounds.Draw();
    }
}