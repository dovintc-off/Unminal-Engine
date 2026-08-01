// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Commands.CommandParser;

using System.Text.Json.Nodes;

[SupportedOSPlatform("windows")]
public static class ParserCommands {
    public static JsonNode? InitParser(string path) {
        if (File.Exists(path)) {
            try {
                string jsonText = File.ReadAllText(path);
                return JsonNode.Parse(jsonText);
            } catch (Exception ex) {
                Console.CreateLog(Console.LogType.ERROR, $"JSON write error: {ex.Message}");
                throw new Exception();
            }
        } else { 
            Console.CreateLog(Console.LogType.ERROR, $"File not found: {path}");
            throw new Exception();
        }
    }

    public static List<Command> Parse(JsonNode? json, List<Command> Commands) {
        if (Commands.Count > 0) return Commands;

        if (json is JsonObject rootObject) {
            foreach (var property in rootObject) {
                string rootKey = property.Key;
                JsonNode? rootValue = property.Value;

                if (rootValue is JsonObject commandObject) {
                    Command? command = commandObject.ToCommand(rootKey);
                    if (command != null) Commands.Add(command);
                }
            }
        }
        return Commands;
    }
}

internal static class JsonNodeCommandExtensions {
    public static Command? ToCommand(this JsonNode? node, string nodeName = "") {
        if (node is JsonObject jsonObject) {
            var command = new Command { 
                Name = nodeName,
                ArgsExecuteMethod = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                ConfigInput = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
            
            foreach (var property in jsonObject) {
                string key = property.Key;
                JsonNode? value = property.Value;
                if (value == null) continue;

                switch (key.ToLowerInvariant()) {
                    case "executemethod":
                        command.ExecuteMethod = value.ToString();
                        break;
                        
                    case "executedlayer":
                        command.ExecutedLayer = value.GetValue<bool>();
                        break;
                        
                    case "argsexecutemethod":
                        command.ArgsExecuteMethod = ParseSimpleDictionary(value.AsObject());
                        break;
                        
                    case "configinput":
                        command.ConfigInput = ParseSimpleDictionary(value.AsObject());
                        break;
                        
                    case "castomargs":
                        break;

                    default:
                        if (value is JsonObject childObject) 
                        {
                            var subCommand = childObject.ToCommand(key);
                            if (subCommand != null) command.Layer.Add(subCommand);
                        }
                        break;
                }
            }
            return command;
        }
        return null;
    }

    private static Dictionary<string, string> ParseSimpleDictionary(JsonObject obj) {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in obj) {
            string val = prop.Value is JsonArray arr 
                ? string.Join("|", arr.Select(x => x?.ToString())) 
                : prop.Value?.ToString() ?? "";
            dict[prop.Key] = val;
        }
        return dict;
    }
}

