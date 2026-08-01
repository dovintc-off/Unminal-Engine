// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal;

using System.Runtime.Versioning;
using Unminal.Script;

[SupportedOSPlatform("windows")]
class Program {
    static void Main() {
        Engine.Paths.BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        Engine.ConfigManager = new();
        Core.Commands.Manager.CommandManager.LoadCommands();
        var userGame = new MyGame();
        using var engine = new Main.Main(userGame);
        engine.Run();
    }
}