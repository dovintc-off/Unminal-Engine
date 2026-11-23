namespace Unminal.Core.Scripting.Lua;

using NLua;
using Unminal.Core.Command;

[SupportedOSPlatform("windows")]
public sealed class LuaCommandRegistry : IDisposable {
    private readonly Dictionary<string, Action<string[]>> _commands = new();

    public bool Register(string name, LuaFunction callback) {
        name = name.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(name))
            return false;

        Action<string[]> handler = args => {
            try {
                // Каждый аргумент передаётся Lua как отдельный параметр.
                callback.Call(args.Cast<object>().ToArray());
            }
            catch (Exception ex) {
                Log.Create(Log.LogType.ERROR,
                    $"[Lua Command: {name}] {ex.Message}");
            }
        };

        if (!CommandManager.Register(name, handler))
            return false;

        _commands.Add(name, handler);
        return true;
    }

    public void Dispose() {
        foreach (var command in _commands) {
            CommandManager.Unregister(command.Key, command.Value);
        }

        _commands.Clear();
    }
}