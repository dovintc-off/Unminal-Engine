// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Scripting.Utils;

using System.Reflection;
using Unminal.Core.Scripting.Lua;

[SupportedOSPlatform("Windows")]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ScriptAttribute : Attribute { }

[SupportedOSPlatform("Windows")]
public class ScriptingUtils {
    private static Script.Script? FindGameClass() {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
            if (type.IsSubclassOf(typeof(Script.Script)) && 
                type.GetCustomAttribute<ScriptAttribute>() != null && !type.IsAbstract) {
                try {
                    Log.Create(Log.LogType.INFO, $"Found script: {type.FullName}");
                    return (Script.Script?)Activator.CreateInstance(type);
                } catch (Exception ex) {
                    Log.Create(Log.LogType.ERROR, $"Failed to init script: {ex.Message}");
                    return null;
                }
            }
        }
        return null;
    }

    public static void LoadScriptingSystem(string[] args) {
        Script.Script script;
        if (args.Length > 0 && args[0].EndsWith(".lua")) {
            script = new LuaScriptAdapter(Path.GetFullPath(args[0]));
            Engine.ScriptingSystem = "lua";
            Log.Create(Log.LogType.INFO, "Loading Scripting System: Lua");
        } else {
            Engine.ScriptingSystem = "csharp";
            Log.Create(Log.LogType.INFO, "Loading Scripting System: CSharp");
            script = FindGameClass() ?? throw new ScriptNotFound(
                "\n[FATAL] User script not found!\n" + "Rules:\n" + "1. Class name must be exactly \"Game\".\n" +
                "2. Class must inherit from \"Unminal.Scripting.Core.Script\".\n" + "3. Class must be public.");
        }

        using var engine = new Main.Main(script);
        engine.Run();
    }
}