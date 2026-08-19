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

[SupportedOSPlatform("Windows")]
public static class LuaAPI {
    public static Camera RegisterApi(Lua lua, Camera camera) {
        lua.NewTable("unminal");
        lua["unminal.Engine"] = new WraperEngineStates();
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

        lua["unminal.SetCursorNormal"] = new Action(() => Engine.LightManager?.ClearLights());


        return camera;
    }   
}