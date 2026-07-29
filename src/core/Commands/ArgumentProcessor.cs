namespace Unminal.Core.Commands.Processor;

[SupportedOSPlatform("windows")]
public static class ArgumentProcessor {
    public static Dictionary<string, object> Process(Command command, List<string> userTokens) {
        var finalArgs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var expectedParams = command.ArgsExecuteMethod
            .Select(kvp => new { ParamName = kvp.Key, Token = kvp.Value.Split('?')[0].Trim() })
            .ToList();

        if (userTokens.Count > expectedParams.Count) {
            Console.CreateLog(Console.LogType.WARNING, $"Too many arguments. Expected {expectedParams.Count}, got {userTokens.Count}.");
            return finalArgs;
        }

        for (int i = 0; i < expectedParams.Count; i++) {
            string argName = expectedParams[i].ParamName;
            string rawValue = command.ArgsExecuteMethod[argName];
            var parts = rawValue.Split('?', 2);
            
            if (parts.Length < 2) {
                Console.CreateLog(Console.LogType.WARNING, $"[#red]Config Error: Missing '?' in '{argName}'.");
                continue;
            }

            string token = parts[0].Trim();
            string logic = parts[1];
            object? finalValue = null;
            string? userValue = i < userTokens.Count ? userTokens[i] : null;

            if (userValue != null) {
                if (command.ConfigInput.TryGetValue(token, out string? rules)) {
                    if (!ValidateType(rules, userValue, argName)) return finalArgs;
                    if (!ValidateRange(rules, userValue, argName)) return finalArgs;
                }
                
                if (rules != null && rules.Contains("type:bool", StringComparison.OrdinalIgnoreCase)) {
                    finalValue = userValue.ToLowerInvariant() is "true" or "1";
                } else {
                    finalValue = userValue;
                }
            } else {
                finalValue = HandleDefaultLogic(logic, argName);
                if (finalValue == null && logic.StartsWith("error(")) return finalArgs; 
            }

            if (finalValue is string strVal && int.TryParse(strVal, out int intVal))
                finalArgs[argName] = intVal;
            else
                finalArgs[argName] = finalValue ?? "";
        }
        return finalArgs;
    }

    private static bool ValidateType(string rules, string value, string argName) {
        foreach (var rule in rules.Split('|')) {
            string r = rule.Trim();
            if (r.StartsWith("type:int") && !int.TryParse(value, out _)) {
                Console.CreateLog(Console.LogType.WARNING, $"Type Error: '{argName}' must be integer."); return false;
            } else if (r.StartsWith("type:bool")) {
                string lowerVal = value.ToLowerInvariant();
                if (lowerVal != "true" && lowerVal != "false" && lowerVal != "1" && lowerVal != "0") {
                    Console.CreateLog(Console.LogType.WARNING, $"Type Error: '{argName}' must be boolean."); return false;
                }
            }
        }
        return true;
    }

    private static bool ValidateRange(string rules, string value, string argName) {
        foreach (var rule in rules.Split('|')) {
            string r = rule.Trim();
            if (r.StartsWith("lim:range(")) {
                int sIdx = r.IndexOf('(') + 1;
                int eIdx = r.LastIndexOf(')');
                if (sIdx > 0 && eIdx > sIdx) {
                    var limits = r[sIdx..eIdx].Split(',');
                    if (limits.Length == 2 && int.TryParse(limits[0], out int min) && int.TryParse(limits[1], out int max)) {
                        if (!Command.range(min, max, value)) {
                            Console.CreateLog(Console.LogType.WARNING, $"Range Error: '{argName}' is out of bounds [{min}-{max}].");
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private static object? HandleDefaultLogic(string logic, string argName) {
        if (logic.StartsWith("error(")) {
            int s = logic.IndexOf('(') + 1;
            int e = logic.LastIndexOf(')');
            Console.CreateLog(Console.LogType.WARNING, $"{(s > 0 && e > s ? logic[s..e] : "Missing argument")}");
            return null;
        } else if (logic.StartsWith("get(")) {
            int s = logic.IndexOf('(') + 1;
            int e = logic.LastIndexOf(')');
            if (s > 0 && e > s) {
                string path = logic[s..e];
                var val = Command.get(typeof(Engine), path);
                if (val == null) Console.CreateLog(Console.LogType.WARNING, $"Path '{path}' returned null.");
                return val;
            }
        }
        return logic;
    }
}