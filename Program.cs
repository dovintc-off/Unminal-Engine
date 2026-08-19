// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal;

using Unminal.Core.Scripting.Utils;
using Unminal.Core.Commands.Manager;

[SupportedOSPlatform("windows")]
class Program {
    static void Main(string[] args) {
        Engine.Paths.BaseFolder = AppContext.BaseDirectory;
        ScriptingUtils.LoadScriptingSystem(args);
        Engine.LoadEngineStates(args);
        CommandManager.LoadCommands();
    }
}