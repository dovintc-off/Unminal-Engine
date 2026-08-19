// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Commands.Structure;

// Im cant delete this, so that in new system can use new class CommandWithLink
public class Command {
    public string? Name {get; set;}
    public List<Command> Layer {get; set;} = new List<Command>(); 
    public string? ExecuteMethod {get; set;}
    public bool ExecutedLayer {get; set;}
    public Dictionary<string, string> ArgsExecuteMethod {get; set;} = new Dictionary<string, string>();
    public Dictionary<string, string> ConfigInput {get; set;} = new Dictionary<string, string>();
    public Command? this[string subCommandName] { get {
            foreach (var cmd in Layer) if (cmd.Name != null && cmd.Name.Equals(subCommandName, System.StringComparison.OrdinalIgnoreCase)) return cmd;
            return null;
        }
    }
}

public class CommandWithLink : Command {
    [Obsolete("Use Handler property instead.", true)]
    public new string? ExecuteMethod { 
        get => throw new NotSupportedException("Use Handler property instead."); 
        set => throw new NotSupportedException("Use Handler property instead."); 
    }
    [Obsolete("Args are passed via CommandContext. Do not use this dictionary.", true)]
    public new Dictionary<string, string> ArgsExecuteMethod { 
        get => throw new NotSupportedException("Args are passed via CommandContext."); 
        set => throw new NotSupportedException("Args are passed via CommandContext."); 
    }
    [Obsolete("Config input is not supported in CommandWithLink.", true)]
    public new Dictionary<string, string> ConfigInput { 
        get => throw new NotSupportedException("Config input is not supported."); 
        set => throw new NotSupportedException("Config input is not supported."); 
    }
    public Action<CommandContext>? Handler { get; set; }
}

public class CommandContext {
    public string[] Args { get; init; } = Array.Empty<string>();
}