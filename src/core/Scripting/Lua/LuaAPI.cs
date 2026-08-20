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
        lua.NewTable("unminal");

        lua["unminal.log"] = new Action<string>(msg => 
            Log.Create(Log.LogType.INFO, $"[Lua] {msg}"));

        lua["unminal.get_path"] = new Func<string, string>(path => GetPath.GetLuaPath(path));

        lua["unminal.create_object"] = new Func<string, GameObject?>((path) => {
            try { return new GameObject(path); } 
            catch (Exception ex) { Log.Create(Log.LogType.ERROR, $"[Lua] Obj Error: {ex.Message}"); return null; }
        });

        lua["unminal.set_position"] = new Action<GameObject, float, float, float>((obj, x, y, z) => {
            obj.Position = new Vector3(x, y, z);
        });

        lua["unminal.set_scale"] = new Action<GameObject, float, float, float>((obj, x, y, z) => {
            obj.Scale = new Vector3(x, y, z);
        });

        lua["unminal.set_color"] = new Action<GameObject, float, float, float, float>((obj, r, g, b, a) => {
            obj.Color = new Vector3(r, g, b);
        });

        lua.NewTable("unminal.ui");
        lua["unminal.ui.create_button"] = new Func<float,float,float,float,float,float,float,float,float,float, Button?>(
            (x,y,w,h,r,g,b,a,rot,dim) => UIFactory.CreateButton(
                new Vector2(x,y), 
                new Vector2(w,h), 
                new Vector4(r,g,b,a), 
                rot, 
                dim
            )
        );

        lua.NewTable("unminal.light");
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

        lua["unminal.Engine"] = new WraperEngineStates();
        lua.NewTable("unminal.keys");
        // KEYBOARD MOUSE
        lua["unminal.keys.Unknown"] = (int)Keys.Unknown;
        lua["unminal.keys.Space"] = (int)Keys.Space;
        lua["unminal.keys.Apostrophe"] = (int)Keys.Apostrophe;
        lua["unminal.keys.Comma"] = (int)Keys.Comma;
        lua["unminal.keys.Minus"] = (int)Keys.Minus;
        lua["unminal.keys.Period"] = (int)Keys.Period;
        lua["unminal.keys.Slash"] = (int)Keys.Slash;
        lua["unminal.keys.D0"] = (int)Keys.D0;
        lua["unminal.keys.D1"] = (int)Keys.D1;
        lua["unminal.keys.D2"] = (int)Keys.D2;
        lua["unminal.keys.D3"] = (int)Keys.D3;
        lua["unminal.keys.D4"] = (int)Keys.D4;
        lua["unminal.keys.D5"] = (int)Keys.D5;
        lua["unminal.keys.D6"] = (int)Keys.D6;
        lua["unminal.keys.D7"] = (int)Keys.D7;
        lua["unminal.keys.D8"] = (int)Keys.D8;
        lua["unminal.keys.D9"] = (int)Keys.D9;
        lua["unminal.keys.Semicolon"] = (int)Keys.Semicolon;
        lua["unminal.keys.Equal"] = (int)Keys.Equal;
        lua["unminal.keys.A"] = (int)Keys.A;
        lua["unminal.keys.B"] = (int)Keys.B;
        lua["unminal.keys.C"] = (int)Keys.C;
        lua["unminal.keys.D"] = (int)Keys.D;
        lua["unminal.keys.E"] = (int)Keys.E;
        lua["unminal.keys.F"] = (int)Keys.F;
        lua["unminal.keys.G"] = (int)Keys.G;
        lua["unminal.keys.H"] = (int)Keys.H;
        lua["unminal.keys.I"] = (int)Keys.I;
        lua["unminal.keys.J"] = (int)Keys.J;
        lua["unminal.keys.K"] = (int)Keys.K;
        lua["unminal.keys.L"] = (int)Keys.L;
        lua["unminal.keys.M"] = (int)Keys.M;
        lua["unminal.keys.N"] = (int)Keys.N;
        lua["unminal.keys.O"] = (int)Keys.O;
        lua["unminal.keys.P"] = (int)Keys.P;
        lua["unminal.keys.Q"] = (int)Keys.Q;
        lua["unminal.keys.R"] = (int)Keys.R;
        lua["unminal.keys.S"] = (int)Keys.S;
        lua["unminal.keys.T"] = (int)Keys.T;
        lua["unminal.keys.U"] = (int)Keys.U;
        lua["unminal.keys.V"] = (int)Keys.V;
        lua["unminal.keys.W"] = (int)Keys.W;
        lua["unminal.keys.X"] = (int)Keys.X;
        lua["unminal.keys.Y"] = (int)Keys.Y;
        lua["unminal.keys.Z"] = (int)Keys.Z;
        lua["unminal.keys.LeftBracket"] = (int)Keys.LeftBracket;
        lua["unminal.keys.Backslash"] = (int)Keys.Backslash;
        lua["unminal.keys.RightBracket"] = (int)Keys.RightBracket;
        lua["unminal.keys.GraveAccent"] = (int)Keys.GraveAccent;
        lua["unminal.keys.Escape"] = (int)Keys.Escape;
        lua["unminal.keys.Enter"] = (int)Keys.Enter;
        lua["unminal.keys.Tab"] = (int)Keys.Tab;
        lua["unminal.keys.Backspace"] = (int)Keys.Backspace;
        lua["unminal.keys.Insert"] = (int)Keys.Insert;
        lua["unminal.keys.Delete"] = (int)Keys.Delete;
        lua["unminal.keys.Right"] = (int)Keys.Right;
        lua["unminal.keys.Left"] = (int)Keys.Left;
        lua["unminal.keys.Down"] = (int)Keys.Down;
        lua["unminal.keys.Up"] = (int)Keys.Up;
        lua["unminal.keys.PageUp"] = (int)Keys.PageUp;
        lua["unminal.keys.PageDown"] = (int)Keys.PageDown;
        lua["unminal.keys.Home"] = (int)Keys.Home;
        lua["unminal.keys.End"] = (int)Keys.End;
        lua["unminal.keys.CapsLock"] = (int)Keys.CapsLock;
        lua["unminal.keys.ScrollLock"] = (int)Keys.ScrollLock;
        lua["unminal.keys.NumLock"] = (int)Keys.NumLock;
        lua["unminal.keys.PrintScreen"] = (int)Keys.PrintScreen;
        lua["unminal.keys.Pause"] = (int)Keys.Pause;
        lua["unminal.keys.F1"] = (int)Keys.F1;
        lua["unminal.keys.F2"] = (int)Keys.F2;
        lua["unminal.keys.F3"] = (int)Keys.F3;
        lua["unminal.keys.F4"] = (int)Keys.F4;
        lua["unminal.keys.F5"] = (int)Keys.F5;
        lua["unminal.keys.F6"] = (int)Keys.F6;
        lua["unminal.keys.F7"] = (int)Keys.F7;
        lua["unminal.keys.F8"] = (int)Keys.F8;
        lua["unminal.keys.F9"] = (int)Keys.F9;
        lua["unminal.keys.F10"] = (int)Keys.F10;
        lua["unminal.keys.F11"] = (int)Keys.F11;
        lua["unminal.keys.F12"] = (int)Keys.F12;
        lua["unminal.keys.F13"] = (int)Keys.F13;
        lua["unminal.keys.F14"] = (int)Keys.F14;
        lua["unminal.keys.F15"] = (int)Keys.F15;
        lua["unminal.keys.F16"] = (int)Keys.F16;
        lua["unminal.keys.F17"] = (int)Keys.F17;
        // MOUSE BUTTONS
        lua["unminal.keys.MouseButton1"] = (int)MouseButton.Button1;
        lua["unminal.keys.MouseButton2"] = (int)MouseButton.Button2;
        lua["unminal.keys.MouseButton3"] = (int)MouseButton.Button3;
        lua["unminal.keys.MouseButton4"] = (int)MouseButton.Button4;
        lua["unminal.keys.MouseButton5"] = (int)MouseButton.Button5;
        lua["unminal.keys.MouseButton6"] = (int)MouseButton.Button6;
        lua["unminal.keys.MouseButton7"] = (int)MouseButton.Button7;
        lua["unminal.keys.MouseButton8"] = (int)MouseButton.Button8;
        lua["unminal.keys.MouseLeft"] = (int)MouseButton.Left;
        lua["unminal.keys.MouseRight"] = (int)MouseButton.Right;
        lua["unminal.keys.MouseMiddle"] = (int)MouseButton.Middle;
        lua["unminal.keys.MouseLast"] = (int)MouseButton.Last;

        return camera;
    }   
}