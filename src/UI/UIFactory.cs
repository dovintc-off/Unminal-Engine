namespace Unminal.UI.Factory;

[SupportedOSPlatform("windows")]
public static class UIFactory {
    private static uint _curID = 0;
    // with exact hovered color
    public static (EngineButton Logic, RenderButton Visual) CreateButton(Vector2 pos, Vector2 scale, Vector3 color, float rotation, Vector3 hoveredColor) {
        return CreateButton(pos, scale, new Vector4(color, 1), rotation, new Vector4(hoveredColor, 1));
    }
    
    public static (EngineButton Logic, RenderButton Visual) CreateButton(Vector2 pos, Vector2 scale, Vector4 color, float rotation, Vector4 hoveredColor) {
        uint id = _curID++;
        Square bounds = new Square(pos, scale, color, rotation);
        EngineButton logic = new EngineButton(id, bounds);
        RenderButton visual = new RenderButton(color, hoveredColor);
        return (logic, visual);
    }

    // with dimming multiplier
    public static (EngineButton Logic, RenderButton Visual) CreateButton(Vector2 pos, Vector2 scale, Vector4 color, float rotation, float dimmingMultiplier) {
        uint id = _curID++;
        Square bounds = new Square(pos, scale, color, rotation);
        EngineButton logic = new EngineButton(id, bounds);
        RenderButton visual = new RenderButton(color, dimmingMultiplier);
        return (logic, visual);
    }

    public static (EngineButton Logic, RenderButton Visual) CreateButton(Vector2 pos, Vector2 scale, Vector3 color, float rotation, float dimmingMultiplier) {
        return CreateButton(pos, scale, new Vector4(color, 1), rotation, dimmingMultiplier);
    }
    
    // automate hovered color
    public static (EngineButton Logic, RenderButton Visual) CreateButton(Vector2 pos, Vector2 scale, Vector4 color, float rotation) {
        return CreateButton(pos, scale, color, rotation, 0.8f);
    }

    public static (EngineButton Logic, RenderButton Visual) CreateButton(Vector2 pos, Vector2 scale, Vector3 color, float rotation) {
        return CreateButton(pos, scale, new Vector4(color, 1f), rotation);
    }

}       