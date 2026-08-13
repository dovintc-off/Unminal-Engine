namespace Unminal.Utils.Loging;
using System.Runtime.CompilerServices;

[SupportedOSPlatform("windows")]
public static class Log {
    public enum LogType {
        ERROR, INFO, WARNING, SUCCESS
    }

    private static (string, int, string) prevlog;
    private static List<string> Logs = new List<string>();

    public static void Create(LogType Level, string LogText, bool CrashGame = false, string CrashError = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0){
        (string prefix, string textcolor, string resetcolor, bool repeated) = BaseLog(Level, LogText, file, line);
        if (repeated) return;
        LogToSystemConsole(LogText, prefix, textcolor, resetcolor, CrashGame, CrashError: CrashError, file: file, line: line);
        LogToGameConsole(LogText, prefix, textcolor, resetcolor, CrashGame, CrashError: CrashError, file: file, line: line);
        LogToFile(LogText, prefix, textcolor, resetcolor, CrashGame, CrashError: CrashError, file: file, line: line);
    }

    private static void LogToSystemConsole(string LogText, string prefix, string textcolor, string resetcolor, bool CrashGame = false, string CrashError = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0){
        string calledFileName = Path.GetFileName(file);
        string FinalLogText = $"{prefix}{textcolor}{LogText}{resetcolor}Called in {calledFileName}:{line}";
        Logs.Add(FinalLogText);

        Console.WriteLine(FinalLogText);

        if (CrashGame) {
            string message = string.IsNullOrWhiteSpace(CrashError) ? "Game Crashed!" : $"Game Crashed: {CrashError}";
            SaveLog();
            throw new Crash(message);
        }
    }

    private static void LogToGameConsole(string LogText, string prefix, string textcolor, string resetcolor, bool CrashGame = false, string CrashError = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        // Drawing Code Here
        Logs.Add(LogText);

        if (CrashGame) {
            string message = string.IsNullOrWhiteSpace(CrashError) ? "Game Crashed!" : $"Game Crashed: {CrashError}";
            SaveLog();
            throw new Crash(message);
        }
    }

    public static void LogToFile(string LogText, string prefix, string textcolor, string resetcolor, bool CrashGame = false, string CrashError = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        string calledFileName = Path.GetFileName(file);
        string FinalLogText = $"{prefix}{textcolor}{LogText}{resetcolor}Called in {calledFileName}:{line}";
        Logs.Add(FinalLogText);
        if (CrashGame) {
            string message = string.IsNullOrWhiteSpace(CrashError) ? "Game Crashed!" : $"Game Crashed: {CrashError}";
            SaveLog();
            throw new Crash(message);
        }
    }

    private static (string, string, string, bool) BaseLog(LogType Level, string LogText, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0){
        if (prevlog == (LogText, line, file)) {
            return ("", "", "", true);
        }
        prevlog = (LogText, line, file);
        var (prefix, textcolor, resetcolor) = Level switch {
            LogType.ERROR => ("[#red][ERROR] ", "[#crimson]", " [#darkgrey]"),
            LogType.INFO =>  ("[#cornflowerblue][INFO] ", "[#white]", " [#darkgrey]"),
            LogType.WARNING => ("[#yellow][WARNING] ", "[#gold]", " [#darkgrey]"),
            LogType.SUCCESS => ("[#green][SUCCESS]", "[#white]", "[#gray]"),
            _ => ("[#white][LOG]", "[#white]", "[#white]")
        };

        return (prefix, textcolor, resetcolor, false);
    }

    public static void SaveLog(string pathToFile = "data:/Log.txt"){File.AppendAllLines(GetPath.GetPath.GetCorrectPath(pathToFile), Logs);}
    public static void ClearFileLog(string pathToFile = "data:/Log.txt"){File.WriteAllText(GetPath.GetPath.GetCorrectPath(pathToFile), "");}
    public static void ClearLog(){Logs.Clear();}
}