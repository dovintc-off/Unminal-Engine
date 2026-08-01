// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
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

    public static bool AddCommand(string parentName, Command command){
        Command? parent = FindDeep(Commands!, parentName);
        if (parent != null) {
            if (parent.Layer == null) {parent.Layer = new List<Command>{};}
            parent.Layer.Add(command);
            return true;
        }
        return false;
    }

    private static Command? FindDeep(List<Command> commandList, string Name){
        foreach (var cmd in commandList) {
            if (cmd.Name != null && cmd.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)) {
                return cmd;
            }

            if (cmd.Layer != null && cmd.Layer.Count > 0) {
                var found = FindDeep(cmd.Layer, Name);
                if (found != null) return found;
            }
        }
        return null;
    }
}
