// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
public enum ScriptType {
    Lua, CSharp
}

public class Exception : System.Exception {
    public Exception() : base() {}
    public Exception(string message) : base($"\x1b[31m{message}\x1b[0m") {}
    public Exception(string message, System.Exception exception) : base($"\x1b[31m{message}\x1b[0m", exception) {}
}

public class ShaderError: Exception {
    public ShaderError(): base(){}
    public ShaderError(string message): base(message){}
    public ShaderError(string message, Exception exception): base(message, exception){}
}

public class Crash: Exception {
    public Crash(): base(){}
    public Crash(string message): base(message){}
    public Crash(string message, Exception exception): base(message, exception){}
}

public class RenderCrash: Exception {
    public RenderCrash(): base(){}
    public RenderCrash(string message): base(message){}
    public RenderCrash(string message, Exception exception): base(message, exception){}
}

public class ScriptNotFound : Exception {
    public ScriptNotFound() : base() {}
    public ScriptNotFound(string message) : base(message) {}
    public ScriptNotFound(string message, System.Exception exception) : base(message, exception) {}
    public ScriptNotFound(ScriptType scriptType) : this("", scriptType) {}
    public ScriptNotFound(string message, ScriptType scriptType) : base(scriptType switch {
        ScriptType.Lua => $"{message} (Lua скрипт не найден)",
        ScriptType.CSharp => $"{message} (C# скрипт не найден)",
        _ => message
    }) {}
}