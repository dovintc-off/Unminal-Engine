// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Commands.Structure;
using System.Reflection;
public class Command {
    public string? Name {get; set;}
    public List<Command> Layer {get; set;} = new List<Command>(); 
    public string? ExecuteMethod {get; set;}
    public bool ExecutedLayer {get; set;}
    public Dictionary<string, string> ArgsExecuteMethod {get; set;} = new Dictionary<string, string>();
    public Dictionary<string, string> ConfigInput {get; set;} = new Dictionary<string, string>();
    public ExtensionArgs? castomArgs {get; set;}
    public Command? this[string subCommandName] { get {
            foreach (var cmd in Layer) if (cmd.Name != null && cmd.Name.Equals(subCommandName, System.StringComparison.OrdinalIgnoreCase)) return cmd;
            return null;
        }
    }
    public static bool range(int s, int e, object inputed){
        if (inputed.GetType() == typeof(string)) {
            string? inputedS = inputed?.ToString();
            if (inputedS == null) return false;
            if (inputedS.Length >= s && inputedS.Length <= e) return true;
            else return false;
        } else if (inputed.GetType() == typeof(int)) {
            if ((int)inputed >= s && (int)inputed <= e) return true;
            else return true;
        } else return false;
    }
    public static object? get(object root, string key) {
        if (root == null || string.IsNullOrWhiteSpace(key)) return null;
        var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
        Type currentType = root.GetType();
        object? current = root; 
        foreach (var part in parts) {
            if (current == null) return null;
            var member = currentType.GetMember(part, 
                BindingFlags.Public | BindingFlags.NonPublic | 
                BindingFlags.Static | BindingFlags.IgnoreCase).FirstOrDefault();
            if (member == null) return null;
            current = member switch {
                PropertyInfo prop => prop.GetValue(null),
                FieldInfo field => field.GetValue(null),
                _ => null
            };
            if (current != null && parts.Last() != part) {
                currentType = current.GetType();
            }
        }
        return current;
    }
}
public class ExtensionArgs { 
    // here may be a engine "extensions" data (maybe)
}
