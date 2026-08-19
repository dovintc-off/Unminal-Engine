// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Commands.Executor;
using System.Reflection;

using Unminal.Core.Commands.Structure;
using Unminal.Core.Commands.ExecutedMethods;
using Unminal.Core.Commands.Processor;
using Unminal.Core.Commands.Tokenizer;

[SupportedOSPlatform("windows")]
public static class CommandExecutor {
    public static void Execute(string Excommand) {
        if (string.IsNullOrWhiteSpace(Excommand)) return;
        
        string trimmed = Excommand.TrimStart('/');
        var tokens = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        Command? current = Manager.CommandManager.Commands?.Find(c => 
            string.Equals(c.Name, tokens[0], StringComparison.OrdinalIgnoreCase));

        if (current == null) { 
            Log.Create(Log.LogType.WARNING, $"Unknown root command: {tokens[0]}"); 
            return; 
        }

        int i = 1;
        bool commandFound = true;

        while (i < tokens.Length && !current.ExecutedLayer) {
            var next = current.Layer.Find(c => 
                string.Equals(c.Name, tokens[i], StringComparison.OrdinalIgnoreCase));
                
            if (next == null) {
                commandFound = false;
                break;
            }
            
            current = next; 
            i++;
        }

        if (!commandFound) {
            Log.Create(Log.LogType.WARNING, $"Unknown subcommand '{tokens[i]}' for '{current.Name}'. Available: {string.Join(", ", current.Layer.Select(c => c.Name))}");
            return;
        }

        if (!current.ExecutedLayer) {
            var subs = current.Layer.Select(c => c.Name);
            Log.Create(Log.LogType.INFO, $"'{current.Name}' requires action. Available: {string.Join(", ", subs)}");
            return;
        }

        int pos = 0;
        int tokenIndex = 0;
        bool inQuotes = false;
        while (pos < trimmed.Length && tokenIndex < i) {
            char c = trimmed[pos];
            if (c == '"') inQuotes = !inQuotes;
            if (c == ' ' && !inQuotes) {
                tokenIndex++;
                while (pos < trimmed.Length && trimmed[pos] == ' ') pos++;
                continue;
            }
            pos++;
        }
        string argsString = pos < trimmed.Length ? trimmed.Substring(pos).Trim() : string.Empty;
        
        var argTokens = CommandTokenizer.GetArgs(argsString);
        var finalArgs = ArgumentProcessor.Process(current, argTokens);

        if (finalArgs.Count == 0 && current.ArgsExecuteMethod.Count > 0) return;

        Type type = typeof(CalledMethods);
        MethodInfo? method = type.GetMethod(current.ExecuteMethod!, BindingFlags.Public | BindingFlags.Static);
        if (method != null) {
            bool result = (method.Invoke(null, new object[] { finalArgs }) as bool?) ?? false;
            if (!result) {
                Log.Create(Log.LogType.ERROR, $"Something went wrong with executing method: \"{current.Name}\"");
            } else {
                Log.Create(Log.LogType.INFO, "Command Executed");
            }
        } else {
            Log.Create(Log.LogType.ERROR, $"Method \"{current.ExecuteMethod}\" not found");
        }
    }   
}
