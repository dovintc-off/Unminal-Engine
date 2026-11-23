// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Command;

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

[SupportedOSPlatform("windows")]
public static class CommandManager {
    private static readonly Dictionary<string, Action<string[]>> _commands = new();
    private static List<string> NameOfCommands = new List<string>();

    static CommandManager() {
        LoadMethods();

        foreach (string commandName in _commands.Keys) {
            NameOfCommands.Add(commandName);
        }
    }

    // In C# use [IsCommand(...)] this mrthod for Lua Unminal API
    public static bool Register(string name, Action<string[]> action) {
        if (string.IsNullOrWhiteSpace(name) || action == null) return false;
        name = name.Trim().ToLowerInvariant();
        if (_commands.ContainsKey(name)) return false;
        _commands.Add(name, action);
        NameOfCommands.Add(name);
        return true;
    }

    public static bool Unregister(string name, Action<string[]> owner) {
        if (string.IsNullOrWhiteSpace(name)) return false;
        name = name.Trim().ToLowerInvariant();
        if (!_commands.TryGetValue(name, out var current)) return false;
        // Удаляем только ту команду, которую зарегистрировал этот владелец.
        if (current != owner) return false;
        NameOfCommands.Remove(name);
        return _commands.Remove(name);
    }

    public static void LoadMethods() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        foreach (Type type in assembly.GetTypes()) {
            // BindingFlags.Instance для поиска обычных (нестатических) методов
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            // Сюда сохраняем экземпляр класса
            object? classInstance = null;

            foreach (var method in methods) {
                var attr = method.GetCustomAttribute<IsCommand>();
                if (attr != null) {
                    try {
                        string cmdName = attr.Name.ToLower();
                        if (_commands.ContainsKey(cmdName)) {
                            throw new Exception($"Command '{cmdName}' already exists!");
                        }

                        Action<string[]> action;

                        if (method.IsStatic) {
                            // Если метод статический - создаем делегат как раньше
                            action = method.CreateDelegate<Action<string[]>>();
                        } else {
                            // Если метод обычный - создаем экземпляр класса (один раз на класс)
                            classInstance ??= Activator.CreateInstance(type);
                            
                            // Привязываем метод к созданному объекту classInstance
                            action = method.CreateDelegate<Action<string[]>>(classInstance);
                        }

                        _commands[cmdName] = action;
                    } catch (Exception e) {
                        Console.WriteLine($"Cant create Method '{method.Name}' in '{type.Name}'! {e.Message}");
                    }
                }
            }
        }
    }

    public static void Execute(string command) {
        if (string.IsNullOrWhiteSpace(command)) return;

        (string name, string[] args) = SplitArgs(command);

        if (_commands.TryGetValue(name, out var action)) {
            action(args);
        } else {
            Console.WriteLine($"Команда '{name}' не найдена.");
        }
    }

    // имя команды и аргументы (если есть ковычки акуратно вытаскиваем)
    private static (string, string[]) SplitArgs(string command){

        var args = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in command) {
            if (c == '"') {
                inQuotes = !inQuotes; // Переключаем режим кавычек
            } else if (c == ' ' && !inQuotes) {
                if (current.Length > 0) {
                    args.Add(current.ToString());
                    current.Clear();
                }
            } else {
                current.Append(c);
            }
        }

        if (current.Length > 0) args.Add(current.ToString());

        return (
                command.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]
                    .TrimStart('\\', '/').ToLower(), 
                args.ToArray()[1..]
            );
    }

    public static bool CanCreate(string name) {
        foreach (string commandName in _commands.Keys) {
            if (commandName == name) {
                return false;
            }
        }
        return true;
    }

    [IsCommand("debug")]
    public static void debug(string[] args) {
        if (args == null || args.Length == 0) {
            Console.WriteLine("[#cyan]available commands: write, ...");
            return;
        }

        switch (args[0].ToLower()) {
            case "write":
                if (args.Length >= 2) {
                    Console.WriteLine(args[1]);
                } else {
                    Console.WriteLine("[#red]Need text");
                }
                break;

            // debug (-1) camera (0) set_fov (1)
            case "camera":
                if (args.Length >= 2) {
                    // need set_fov and number (2 args)
                    if (args[1].ToLower() == "set_fov") {
                        if (args.Length < 3) {Console.WriteLine("Need numder"); return;}

                        try {
                            CalledMethods.Fov_set(Convert.ToInt32(args[2]));
                        } catch (Exception e) {
                            Console.WriteLine($"Error: {e}");
                        }
                    }

                    // need get_fov
                    if (args[1].ToLower() == "get_fov") {
                        try {
                            CalledMethods.Fov_get();
                        } catch (Exception e) {
                            Log.Create(Log.LogType.ERROR, $"{e}");
                        }
                    }
                } else {
                    Console.WriteLine("[#white]Available commands from [#cyan]camera: [#white]set_fov, get_fov");
                }
                break;
    
            case "light":
                if (args.Length >= 2 && args[1].ToLower() == "display") {
                    try {
                        CalledMethods.ToggleLightDisplay();
                    } catch (Exception e) {
                        Log.Create(Log.LogType.ERROR, $"{e}");
                    }
                } else {
                    Console.WriteLine("[#white]Available commands from [#cyan]light: [#white]display");
                }
                break;

            default:
                Console.WriteLine($"[#cyan]available commands: write, camera, lights, {string.Join(", ", NameOfCommands)}");
                break;
        }
    }    
}


[AttributeUsage(AttributeTargets.Method)]
[SupportedOSPlatform("windows")]
public class IsCommand : Attribute {

    public string Name {get;}

    public IsCommand(string name) {
       Name = name;
    }
}

[SupportedOSPlatform("windows")]
public static class CalledMethods {
    public static bool Fov_set(int fov) {
        if (Engine.Player.CameraObj == null) throw new Exception("[#red]Something went wrong, camera is null: see file ExecutedMethods.cs (line ~10)");
        
        float min = Engine.Player.CameraObj.limitationFOV[0];
        float max = Engine.Player.CameraObj.limitationFOV[1];

        if (fov < min) {
            Console.WriteLine($"[#red] must be more then {min}");
            return false;
        }
        if (fov > max) { 
            Console.WriteLine($"[#red] must be smalest then {max}");
            return false;
        }

        if (Engine.Player.CameraObj != null) {
            Engine.Player.CameraObj.FOV = MathHelper.DegreesToRadians(fov);
        }

        return true;
    }

    public static bool Fov_get() {
        if (Engine.Player.CameraObj == null) throw new Exception("[#red]Something went wrong, camera is null: see file ExecutedMethods.cs (line ~30)");
        
        Console.WriteLine($"Player camera fov: {MathHelper.RadiansToDegrees(Engine.Player.CameraObj.FOV)}");
        return true;
    }

    public static bool ToggleLightDisplay() {
        Engine.ShowLight = !Engine.ShowLight;
        Console.WriteLine($"Light display: {(Engine.ShowLight ? "[#green]ON" : "[#red]OFF")}");
        return true;
    }


    // Strange code...
    // public static bool ReloadScripts(Dictionary<string, object> args) {
    //     if (Engine.GameInstance is Main.Main main) {
    //         main.ReloadScript();
    //         return true;
    //     } else {
    //         Log.Create(Log.LogType.ERROR, "Cannot access Main instance for reloading.");
    //         return false;
    //     }
    //     return true;
    // }
} 