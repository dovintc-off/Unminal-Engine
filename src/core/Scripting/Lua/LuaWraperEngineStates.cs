// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Scripting.Lua.WraperStates;
using OpenTK.Windowing.GraphicsLibraryFramework;

[SupportedOSPlatform("windows")]
public class WraperEngineStates {
    public string[] args => Engine.args;
    public float dt => Engine.DeltaTime;
    public float tt => (float)Engine.TotalTime;
    public bool IsDebug => Engine.IsDebug;
    public bool InConsole => Engine.GlobalWindowState.InConsole;
    public bool IsFullScreen => Engine.GlobalWindowState.IsFullScreen;
    public bool InDebugMenu => Engine.GlobalWindowState.InDebugMenu;
    public bool InPause => Engine.GlobalWindowState.InPause;
    public string BaseFolder => Engine.Paths.BaseFolder;
    public int WindowSizeW = Engine.WindowSize.X; 
    public int WindowSizeH = Engine.WindowSize.Y; 
    public bool CanF3 {
        get => Engine.CanF3;
        set => Engine.CanF3 = value;
    }
    public bool ShowLight {
        get => Engine.ShowLight;
        set => Engine.ShowLight = value;
    }
}