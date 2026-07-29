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