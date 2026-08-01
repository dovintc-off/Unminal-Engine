namespace Unminal.UI.Factory;

[SupportedOSPlatform("windows")]
public static class UIFactory {
    private static uint _curID;
    
    // with exact hovered color
    public static Button CreateButton(Vector2 pos, Vector2 scale, Vector3 color, float rotation, Vector3 hoveredColor) =>
        CreateButton(pos, scale, new Vector4(color, 1f), rotation, new Vector4(hoveredColor, 1f));

    public static Button CreateButton(Vector2 pos, Vector2 scale, Vector4 color, float rotation, Vector4 hoveredColor) {
        uint id = _curID++;
        Square bounds = new Square(pos, scale, color, rotation);
        EngineButton logic = new EngineButton(id, bounds);
        RenderButton visual = new RenderButton(color, hoveredColor);
        return new Button(logic, visual);
    }

    // with dimming multiplier
    public static Button CreateButton(Vector2 pos, Vector2 scale, Vector4 color, float rotation, float dimmingMultiplier) {
        uint id = _curID++;
        Square bounds = new Square(pos, scale, color, rotation);
        EngineButton logic = new EngineButton(id, bounds);
        RenderButton visual = new RenderButton(color, dimmingMultiplier);
        return new Button(logic, visual);
    }

    public static Button CreateButton(Vector2 pos, Vector2 scale, Vector3 color, float rotation, float dimmingMultiplier)
        => UIFactory.CreateButton(pos, scale, new Vector4(color, 1f), rotation, dimmingMultiplier);

    // automate hovered color
    public static Button CreateButton(Vector2 pos, Vector2 scale, Vector4 color, float rotation) 
        => UIFactory.CreateButton(pos, scale, color, rotation, 0.8f);

    public static Button CreateButton(Vector2 pos, Vector2 scale, Vector3 color, float rotation)
        =>  UIFactory.CreateButton(pos, scale, new Vector4(color, 1f), rotation);
}