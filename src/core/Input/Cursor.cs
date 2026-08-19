// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.

namespace Unminal.Core.Input.Mouse;
using Unminal.Core.Input.Mouse.VisualState;
using System.Runtime.InteropServices;

[SupportedOSPlatform("windows")]
public static class Cursor {
    private static readonly Dictionary<CursorVisualState, int> CursorMap = new() {
        [CursorVisualState.Normal] = 32512, // IDC_ARROW
        [CursorVisualState.Input] = 32513, // IDC_IBEAM
        [CursorVisualState.Hand] = 32649, // IDC_HAND
        [CursorVisualState.ResizeALL] = 32646, // IDC_SIZEALL
        [CursorVisualState.Crosshair] = 32515, // IDC_CROSS
        [CursorVisualState.Wait] = 32514, // IDC_WAIT
        [CursorVisualState.Forbidden] = 32648, // IDC_NO
        [CursorVisualState.ResizeWE] = 32644, // IDC_SIZEWE
        [CursorVisualState.ResizeNS] = 32645,  // IDC_SIZENS
        [CursorVisualState.ResizeNWSE] = 32642, // IDC_SIZENWSE
        [CursorVisualState.ResizeNESW] = 32643, // IDC_SIZENESW
        [CursorVisualState.AppStarting] = 32650, // IDC_APPSTARTING
        [CursorVisualState.Help] = 32651, // IDC_HELP
        [CursorVisualState.UpArrow] = 32516, // IDC_UPARROW
        [CursorVisualState.Icon] = 32641  // IDC_ICON
    };
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetCursor(IntPtr hCursor);

    public static CursorVisualState VisualState {
        get => Engine.GlobalWindowState.InputState.CurrentCursorVisual;
        set {
            Engine.GlobalWindowState.InputState.CurrentCursorVisual = value;

            if (CursorMap.TryGetValue(value, out int cursorId)) {
                var cursor = LoadCursor(IntPtr.Zero, cursorId);
                if (cursor != IntPtr.Zero) {
                    SetCursor(cursor);
                } else {
                    Log.Create(Log.LogType.ERROR, 
                        $"[Cursor] Failed to load cursor for {value} (ID: {cursorId})");
                }
            } else {
                Log.Create(Log.LogType.ERROR, $"[Cursor] Unknown visual state: {value}");
            }
        }
    }

    public static void CursorSetArrow() => VisualState = CursorVisualState.Normal;
    public static void CursorSetInput() => VisualState = CursorVisualState.Input;
    public static void CursorSetHand() => VisualState = CursorVisualState.Hand;
    public static void CursorSetCrosshair() => VisualState = CursorVisualState.Crosshair;
    public static void CursorSetWait() => VisualState = CursorVisualState.Wait;
    public static void CursorSetForbidden() => VisualState = CursorVisualState.Forbidden;
    public static void CursorSetHelp() => VisualState = CursorVisualState.Help;
    public static void CursorSetUpArrow() => VisualState = CursorVisualState.UpArrow;
    public static void CursorSetIcon() => VisualState = CursorVisualState.Icon;
    public static void CursorSetAppStarting() => VisualState = CursorVisualState.AppStarting;
    public static void CursorSetResize(string Direction) {
        switch (Direction) {
            case "WE": VisualState = CursorVisualState.ResizeWE; break;
            case "NS": VisualState = CursorVisualState.ResizeNS; break;
            case "NWSE": VisualState = CursorVisualState.ResizeNWSE; break;
            case "NESW": VisualState = CursorVisualState.ResizeNESW; break;
            case "ALL":  VisualState = CursorVisualState.ResizeALL; break;
        }
    }
}