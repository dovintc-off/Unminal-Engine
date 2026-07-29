namespace Unminal.Core.Commands.Manager;
using System.Text.Json.Nodes;
[SupportedOSPlatform("windows")]
public static class CommandManager {
    public static List<Command>? Commands = new List<Command>();
    public static JsonNode? json;

    public static void LoadCommands() {
        json = ParserCommands.InitParser(GetPath.GetCorrectPath(Engine.Paths.Config.CommandConfig));
        Commands = ParserCommands.Parse(json, Commands!);
    }
}