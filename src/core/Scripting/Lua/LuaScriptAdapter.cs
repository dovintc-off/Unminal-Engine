// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Scripting.Lua;

using NLua;
using OpenTK.Mathematics;
using Unminal.Core.Scripting.Lua.API;
using Unminal.Core.Scripting.Script;

[SupportedOSPlatform("windows")]
public class LuaScriptAdapter : Script {
    private readonly string _scriptPath;
    private Lua? _lua;

    private LuaFunction? _fnLoad;
    private LuaFunction? _fnUpdate;
    private LuaFunction? _fnDraw;
    private LuaFunction? _fnUnload;

    public LuaScriptAdapter(string scriptPath) {
        _scriptPath = scriptPath;
    }

    public override void Load(Matrix4 initialProjection) {
        base.Load(initialProjection);

        try {
            _lua = new Lua();
            _lua.State.Encoding = System.Text.Encoding.UTF8;

            ActiveCamera = LuaAPI.RegisterApi(_lua, ActiveCamera!);

            Log.Create(Log.LogType.INFO, $"[Lua] Loading: {_scriptPath}");
            _lua.DoFile(_scriptPath);

            _fnLoad = _lua.GetFunction("unminal.load");
            _fnUpdate = _lua.GetFunction("unminal.update");
            _fnDraw = _lua.GetFunction("unminal.draw");
            _fnUnload = _lua.GetFunction("unminal.unload");

            _fnLoad?.Call();
        } catch (Exception ex) {
            Log.Create(Log.LogType.ERROR, $"[Lua] FATAL LOAD ERROR: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public override void Update() {
        base.Update();
        if (_lua == null || _fnUpdate == null) return;

        try {
            _fnUpdate.Call(Engine.DeltaTime);
        } catch (Exception ex) {
            Log.Create(Log.LogType.ERROR, $"[Lua] Update Error: {ex.Message}");
            _fnUpdate = null;
        }
    }

    public override void Draw() {
        if (_lua == null || _fnDraw == null) return;
        try {
            _fnDraw.Call();
        } catch (Exception ex) {
            Log.Create(Log.LogType.ERROR, $"[Lua] Draw Error: {ex.Message}");
            _fnDraw = null;
        }
    }

    public override void Unload() {
        if (_lua != null) {
            try { _fnUnload?.Call(); } catch {}
            _lua.Dispose();
            _lua = null;
        }
        base.Unload();
    }
}