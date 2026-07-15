namespace Unminal.Core.EngineValues;

public static class EngineValues {
    public static Vector2i WindowSize {get; set;}
    public static float DeltaTime {get; set;}
    public static double TotalTime {get; set;}
    public static KeyboardState? CurrentKeyboard {get; set;}
    public static MouseState? CurrentMouse {get; set;}
    public static bool IsPaused {get; set;}
    public static bool IsConsoleOpen {get; set;}
    public static bool IsDebugOpen {get; set;}
    public static bool CanF3 {get; set;}
}