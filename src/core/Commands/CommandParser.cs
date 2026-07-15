namespace Unminal.Core.Commands.CommandParser;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;

public class ParserCommands 
{
    public string Path { get; set; }
    public List<Command> Commands { get; private set; } = new();
    private JsonNode? _json;

    public ParserCommands(string path) {
        Path = path;
        
        if (File.Exists(Path)) {
            try {
                string jsonText = File.ReadAllText(Path);
                _json = JsonNode.Parse(jsonText);
            } catch (Exception ex) {
                Console.WriteLine($"Ошибка чтения JSON: {ex.Message}");
            }
        } else { Console.WriteLine($"Файл не найден: {Path}");}
    }

    public List<Command> Parse() {
        if (Commands.Count > 0) return Commands;

        if (_json is JsonObject rootObject) {
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
            var command = new Command { Name = nodeName };
            
            foreach (var property in jsonObject) {
                string key = property.Key;
                JsonNode? value = property.Value;

                if (value == null) continue;

                if (key.Equals("ExecuteMetod", StringComparison.OrdinalIgnoreCase))
                    command.ExecuteMethod = value.ToString();
                else if (key.Equals("ExecutedLayer", StringComparison.OrdinalIgnoreCase))
                    command.ExecutedLayer = value.GetValue<bool>();
                else if (key.Equals("ArgsExecuteMetod", StringComparison.OrdinalIgnoreCase))
                    command.ArgsExecuteMethod = ParseSimpleDictionary(value.AsObject());
                else if (key.Equals("AdditionalArgs", StringComparison.OrdinalIgnoreCase))
                    command.AdditionalArgs = ParseNestedDictionary(value.AsObject());
                else if (key.Equals("castomArgs", StringComparison.OrdinalIgnoreCase)){}
                else if (value is JsonObject childObject) {
                    var subCommand = childObject.ToCommand(key);
                    if (subCommand != null) command.Layer.Add(subCommand);
                }
            }
            return command;
        }
        return null;
    }

    private static Dictionary<string, string> ParseSimpleDictionary(JsonObject obj) {
        var dict = new Dictionary<string, string>();
        foreach (var prop in obj) dict[prop.Key] = prop.Value?.ToString() ?? "";
        return dict;
    }

    private static Dictionary<string, Dictionary<string, string>> ParseNestedDictionary(JsonObject obj) {
        var result = new Dictionary<string, Dictionary<string, string>>();
        foreach (var outerProp in obj) {
            if (outerProp.Value is JsonObject innerObj) {
                var innerDict = new Dictionary<string, string>();
                foreach (var innerProp in innerObj) {
                    innerDict[innerProp.Key] = innerProp.Value is JsonArray 
                        ? innerProp.Value.ToJsonString() 
                        : innerProp.Value?.ToString() ?? "";
                }
                result[outerProp.Key] = innerDict;
            }
        }
        return result;
    }
}
