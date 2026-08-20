// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Scripting.Lua.API;

using Unminal.Core.PlayerCamera;
using Unminal.Render.Light;
using Unminal.Render.Objects;
using Unminal.UI.ButtonObject;
using Unminal.UI.Factory;
using Unminal.Core.Scripting.Lua.WraperStates;
using NLua;
using OpenTK.Windowing.GraphicsLibraryFramework;

[SupportedOSPlatform("Windows")]
public static class LuaAPI {
    public static Camera RegisterApi(Lua lua, Camera camera) {
        // Lua Tables 
        lua.NewTable("unminal");
        lua.NewTable("unminal.ui");
        lua.NewTable("unminal.light");
        lua.NewTable("input");
        lua.NewTable("key");
        lua.NewTable("mouse");
        lua.NewTable("mouse.visual");

        // api's
        lua["log"] = new Action<string>(msg => 
            Log.Create(Log.LogType.INFO, $"[Lua] {msg}"));

        lua["get_path"] = new Func<string, string>(path => GetPath.GetLuaPath(path));

        lua["unminal.create_object"] = new Func<string, GameObject?>((path) => {
            try { return new GameObject(path); } 
            catch (Exception ex) { Log.Create(Log.LogType.ERROR, $"[Lua] Obj Error: {ex.Message}"); return null; }
        });

        lua["get_config"] = new Action(() => {});

        lua["unminal.set_position"] = new Action<GameObject, float, float, float>((obj, x, y, z) => {
            obj.Position = new Vector3(x, y, z);
        });

        lua["unminal.set_scale"] = new Action<GameObject, float, float, float>((obj, x, y, z) => {
            obj.Scale = new Vector3(x, y, z);
        });

        lua["unminal.set_color"] = new Action<GameObject, float, float, float, float>((obj, r, g, b, a) => {
            obj.Color = new Vector3(r, g, b);
        });

        lua["unminal.ui.create_button"] = new Func<float,float,float,float,float,float,float,float,float,float, Button?>(
            (x,y,w,h,r,g,b,a,rot,dim) => UIFactory.CreateButton(
                new Vector2(x,y), 
                new Vector2(w,h), 
                new Vector4(r,g,b,a), 
                rot, 
                dim
            )
        );

        lua["unminal.light.clear"] = new Action(() => Engine.LightManager?.ClearLights());
        lua["unminal.light.add"] = new Action<float,float,float,float,float,float,float>(
            (x,y,z,r,g,b,intensity) => Engine.LightManager?.AddLight(
                new LightData(new Vector3(x,y,z), new Vector3(r,g,b), intensity)
            )
        );

        lua.NewTable("unminal.camera");
        lua["unminal.camera.set_position"] = new Action<float,float,float>((x,y,z) => {
            if (camera != null) {
                camera.Position = new Vector3(x,y,z);
            }
        });

        lua["input.MouseWheelDelta"] = new Func<float>(() => 
            Engine.CurrentWindow?.GetMouseWhellDelta() ?? 0);

        lua["input.IsKeyDown"] = new Func<int, bool>(keyId => 
            Engine.CurrentKeyboard != null && Engine.CurrentKeyboard.IsKeyDown((Keys)keyId));
            
        lua["input.IsKeyPressed"] = new Func<int, bool>(keyId => 
            Engine.CurrentKeyboard != null && Engine.CurrentKeyboard.IsKeyPressed((Keys)keyId));
            
        lua["input.IsKeyReleased"] = new Func<int, bool>(keyId => 
            Engine.CurrentKeyboard != null && Engine.CurrentKeyboard.IsKeyReleased((Keys)keyId));

        lua["input.IsMouseButtonDown"] = new Func<int, bool>(buttonID => 
            Engine.CurrentMouse != null && Engine.CurrentMouse.IsButtonDown((MouseButton)buttonID));

        lua["input.IsMouseButtonPressed"] = new Func<int, bool>(buttonID => 
            Engine.CurrentMouse != null && Engine.CurrentMouse.IsButtonPressed((MouseButton)buttonID));

        lua["input.IsMouseButtonReleased"] = new Func<int, bool>(buttonID => 
            Engine.CurrentMouse != null && Engine.CurrentMouse.IsButtonReleased((MouseButton)buttonID));

        lua["unminal.state"] = new WraperEngineStates();

        bool CanChangeCursor() => 
            Engine.CurrentCursorState == CursorState.Normal || 
            Engine.CurrentCursorState == CursorState.Confined;

        lua["mouse.visual.SetArrow"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetArrow();
        });

        lua["mouse.visual.SetInput"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetInput();
        });

        lua["mouse.visual.SetHand"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetHand();
        });

        lua["mouse.visual.SetCrosshair"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetCrosshair();
        });

        lua["mouse.visual.SetWait"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetWait();
        });

        lua["mouse.visual.SetForbidden"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetForbidden();
        });

        lua["mouse.visual.SetHelp"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetHelp();
        });

        lua["mouse.visual.SetUpArrow"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetUpArrow();
        });

        lua["mouse.visual.SetIcon"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetIcon();
        });

        lua["mouse.visual.SetAppStarting"] = new Action(() => {
            if (!CanChangeCursor()) return;
            Input.Mouse.Cursor.CursorSetAppStarting();
        });

        lua["mouse.visual.CursorSetResize"] = new Action<string>(dir => {Input.Mouse.Cursor.CursorSetResize(dir);});

        lua["mouse.SetNormal"] = new Action(() => Engine.CurrentCursorState = CursorState.Normal);
        lua["mouse.SetGrabbed"] = new Action(() => Engine.CurrentCursorState = CursorState.Grabbed);
        lua["mouse.SetHidden"] = new Action(() => Engine.CurrentCursorState = CursorState.Hidden);
        lua["mouse.SetConfined"] = new Action(() => Engine.CurrentCursorState = CursorState.Confined);

        // KEYBOARD MOUSE
        lua["key.Unknown"] = (int)Keys.Unknown;
        lua["key.Space"] = (int)Keys.Space;
        lua["key.Apostrophe"] = (int)Keys.Apostrophe;
        lua["key.Comma"] = (int)Keys.Comma;
        lua["key.Minus"] = (int)Keys.Minus;
        lua["key.Period"] = (int)Keys.Period;
        lua["key.Slash"] = (int)Keys.Slash;
        lua["key.D0"] = (int)Keys.D0;
        lua["key.D1"] = (int)Keys.D1;
        lua["key.D2"] = (int)Keys.D2;
        lua["key.D3"] = (int)Keys.D3;
        lua["key.D4"] = (int)Keys.D4;
        lua["key.D5"] = (int)Keys.D5;
        lua["key.D6"] = (int)Keys.D6;
        lua["key.D7"] = (int)Keys.D7;
        lua["key.D8"] = (int)Keys.D8;
        lua["key.D9"] = (int)Keys.D9;
        lua["key.Semicolon"] = (int)Keys.Semicolon;
        lua["key.Equal"] = (int)Keys.Equal;
        lua["key.A"] = (int)Keys.A;
        lua["key.B"] = (int)Keys.B;
        lua["key.C"] = (int)Keys.C;
        lua["key.D"] = (int)Keys.D;
        lua["key.E"] = (int)Keys.E;
        lua["key.F"] = (int)Keys.F;
        lua["key.G"] = (int)Keys.G;
        lua["key.H"] = (int)Keys.H;
        lua["key.I"] = (int)Keys.I;
        lua["key.J"] = (int)Keys.J;
        lua["key.K"] = (int)Keys.K;
        lua["key.L"] = (int)Keys.L;
        lua["key.M"] = (int)Keys.M;
        lua["key.N"] = (int)Keys.N;
        lua["key.O"] = (int)Keys.O;
        lua["key.P"] = (int)Keys.P;
        lua["key.Q"] = (int)Keys.Q;
        lua["key.R"] = (int)Keys.R;
        lua["key.S"] = (int)Keys.S;
        lua["key.T"] = (int)Keys.T;
        lua["key.U"] = (int)Keys.U;
        lua["key.V"] = (int)Keys.V;
        lua["key.W"] = (int)Keys.W;
        lua["key.X"] = (int)Keys.X;
        lua["key.Y"] = (int)Keys.Y;
        lua["key.Z"] = (int)Keys.Z;
        lua["key.LeftBracket"] = (int)Keys.LeftBracket;
        lua["key.Backslash"] = (int)Keys.Backslash;
        lua["key.RightBracket"] = (int)Keys.RightBracket;
        lua["key.GraveAccent"] = (int)Keys.GraveAccent;
        lua["key.Escape"] = (int)Keys.Escape;
        lua["key.Enter"] = (int)Keys.Enter;
        lua["key.Tab"] = (int)Keys.Tab;
        lua["key.Backspace"] = (int)Keys.Backspace;
        lua["key.Insert"] = (int)Keys.Insert;
        lua["key.Delete"] = (int)Keys.Delete;
        lua["key.Right"] = (int)Keys.Right;
        lua["key.Left"] = (int)Keys.Left;
        lua["key.Down"] = (int)Keys.Down;
        lua["key.Up"] = (int)Keys.Up;
        lua["key.PageUp"] = (int)Keys.PageUp;
        lua["key.PageDown"] = (int)Keys.PageDown;
        lua["key.Home"] = (int)Keys.Home;
        lua["key.End"] = (int)Keys.End;
        lua["key.CapsLock"] = (int)Keys.CapsLock;
        lua["key.ScrollLock"] = (int)Keys.ScrollLock;
        lua["key.NumLock"] = (int)Keys.NumLock;
        lua["key.PrintScreen"] = (int)Keys.PrintScreen;
        lua["key.Pause"] = (int)Keys.Pause;
        lua["key.F1"] = (int)Keys.F1;
        lua["key.F2"] = (int)Keys.F2;
        lua["key.F3"] = (int)Keys.F3;
        lua["key.F4"] = (int)Keys.F4;
        lua["key.F5"] = (int)Keys.F5;
        lua["key.F6"] = (int)Keys.F6;
        lua["key.F7"] = (int)Keys.F7;
        lua["key.F8"] = (int)Keys.F8;
        lua["key.F9"] = (int)Keys.F9;
        lua["key.F10"] = (int)Keys.F10;
        lua["key.F11"] = (int)Keys.F11;
        lua["key.F12"] = (int)Keys.F12;
        lua["key.F13"] = (int)Keys.F13;
        lua["key.F14"] = (int)Keys.F14;
        lua["key.F15"] = (int)Keys.F15;
        lua["key.F16"] = (int)Keys.F16;
        lua["key.F17"] = (int)Keys.F17;
        // MOUSE BUTTONS
        lua["mouse.Button1"] = (int)MouseButton.Button1;
        lua["mouse.Button2"] = (int)MouseButton.Button2;
        lua["mouse.Button3"] = (int)MouseButton.Button3;
        lua["mouse.Button4"] = (int)MouseButton.Button4;
        lua["mouse.Button5"] = (int)MouseButton.Button5;
        lua["mouse.Button6"] = (int)MouseButton.Button6;
        lua["mouse.Button7"] = (int)MouseButton.Button7;
        lua["mouse.Button8"] = (int)MouseButton.Button8;
        lua["mouse.Left"] = (int)MouseButton.Left;
        lua["mouse.Right"] = (int)MouseButton.Right;
        lua["mouse.Middle"] = (int)MouseButton.Middle;
        lua["mouse.Last"] = (int)MouseButton.Last;

        return camera;
    }   
}