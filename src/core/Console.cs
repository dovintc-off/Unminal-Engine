namespace Unminal.Core.EngineConsole;
using System.Runtime.CompilerServices;

[SupportedOSPlatform("windows")]
public class Console {
    public bool IsOpen {get; private set;} = false;
    public static List<string> History {get; private set;} = new List<string>();
    public static Console? Instance { get; private set; }
    private Text? _textRenderer;
    private RichTextSegment? _richTextRenderer;
    public string InputedCommand {get; private set;} = "";
    private bool _wasToggleKeyPressed = false;
    private readonly string _pathToFileHistory = GetPath.GetCorrectPath(Engine.Paths.Config.ConsoleHistory);
    private KeyboardState? _prevInput;
    private static string _prevlog = "";
    public enum LogType {
        ERROR, INFO, WARNING 
    }

    Square _background = new Square(
        new Vector2(0, 0),
        new Vector2(200, 150),
        new Vector4(0.0f, 1.0f, 0.0f, 0.5f),
        0
    );

    public Console(bool isOpen = false) {
        Instance = this;
        History = ReadHistory();
        IsOpen = isOpen;
        _richTextRenderer = new RichTextSegment(new Vector4(1, 1, 1, 1));
        _textRenderer = new Text(
            GetPath.GetCorrectPath(Engine.Paths.Fonts.Arial),
            32,
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textV),
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textF)
        );
    }

    public void ProcessInput(KeyboardState input) {   

        bool isToggleKeyDown = input.IsKeyDown(Keys.GraveAccent);

        if (isToggleKeyDown && !_wasToggleKeyPressed)
        {
            IsOpen = !IsOpen;
            if (IsOpen) InputedCommand = "";
        }
        _wasToggleKeyPressed = isToggleKeyDown;

        if (!IsOpen) {
            _prevInput = input; 
            return;
        }

        if (input.IsKeyReleased(Keys.Backspace)) {
            if (InputedCommand.Length > 0) {
                InputedCommand = InputedCommand[..^1];
            }
            return;
        }

        if (input.IsKeyReleased(Keys.Enter)) {
            if (string.IsNullOrWhiteSpace(InputedCommand)) return;
            CommandExecutor.Execute(InputedCommand);
            WriteHistory(InputedCommand);
            InputedCommand = "";
            return;
        }
    }

    public void AppendToCommand(string text) {InputedCommand += text;}

    private List<string> ReadHistory()
    {   

        if (!File.Exists(_pathToFileHistory))
        {
            Console.CreateLog(Console.LogType.WARNING, "Cant read history file");
            return new List<string>();
        } try {       
            return new List<string>(File.ReadAllLines(_pathToFileHistory));
        } catch (Exception e) {
            Console.CreateLog(Console.LogType.WARNING, $"Cant read history file {e}");
            return new List<string>();
        }
    }
     
    private void WriteHistory(string command) {
        try {
            File.AppendAllText(_pathToFileHistory, command + Environment.NewLine);
        } catch (Exception e) {
            Console.CreateLog(Console.LogType.ERROR, $"Error write command history: {e}");
            
        }
    }   

    public static void WriteLine(object text){
        string safeText = text?.ToString() ?? string.Empty;
        List<RichTextSegment.TextPart> Parts;
        Parts = RichTextSegment.ParseColor(safeText, new Vector4(Colors.White, 1));
        foreach (RichTextSegment.TextPart part in Parts) { 
            Vector3 vec3Color = Colors.VEC3toRGB(new Vector3(part.TextColor));
            System.Console.Write($"\x1b[38;2;{vec3Color[0]};{vec3Color[1]};{vec3Color[2]}m{part.Text}\x1b[0m");
        }
        System.Console.Write("\x1b[0m\n");
    }

    public static void Write(object text){
        string safeText = text?.ToString() ?? string.Empty;
        List<RichTextSegment.TextPart> Parts;
        Parts = RichTextSegment.ParseColor(safeText, new Vector4(Colors.White, 1));
        foreach (RichTextSegment.TextPart part in Parts) { 
            Vector3 vec3Color = Colors.VEC3toRGB(new Vector3(part.TextColor));
            System.Console.Write($"\x1b[38;2;{vec3Color[0]};{vec3Color[1]};{vec3Color[2]}m{part.Text}\x1b[0m");
        }
         System.Console.Write("\x1b[0m");
    }

    public static void CreateLog(LogType Level, string LogText, bool CrashGame = false, string CrashError = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0){
        LogToSystemConsole(Level, LogText, CrashGame, CrashError: CrashError, file: file, line: line);
        LogToGameConsole(Level, LogText, CrashGame);
    }

    private static void LogToSystemConsole(LogType Level, string LogText, bool CrashGame = false, string CrashError = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0){
        if (_prevlog == LogText) return;
        _prevlog = LogText;

        var (prefix, textcolor, resetcolor) = Level switch {
            LogType.ERROR => ("[#red][ERROR] ", "[#crimson]", " [#darkgrey]"),
            LogType.INFO =>  ("[#cornflowerblue][INFO] ", "[#white]", " [#darkgrey]"),
            LogType.WARNING => ("[#yellow][WARNING] ", "[#gold]", " [#darkgrey]"),
            _ => ("[#white][LOG]", "[#white]", "[#white]")
        };

        string calledFileName = Path.GetFileName(file);
        string FinalLogText = $"{prefix}{textcolor}{LogText}{resetcolor}Called in {calledFileName}:{line}";

        WriteLine(FinalLogText);
        History.Add(FinalLogText);

        if (CrashGame) {
            string message = string.IsNullOrWhiteSpace(CrashError) ? "Game Crashed!" : $"Game Crashed: {CrashError}";
            throw new Crash(message);
        }
    }

    private static void LogToGameConsole(LogType Level, string LogText, bool CrashGame = false){
        
    }

    public void DrawConsole(int width, int height) {
        if (!IsOpen) return;
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);

        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _background.Position = new Vector2(0, 0);
        _background.Scale = new Vector2(width, height);

        _background.Color = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);

        _background.Draw();

        int index = 0;
        foreach (var line in History) {
            _richTextRenderer?.Draw(_textRenderer!, $"{line}", 10, 50 * index, 0.8f);
            index++;
        }

        _textRenderer?.DrawString(InputedCommand, 10, Engine.WindowSize.Y - 30, 0.5f, new Vector4(Colors.White, 1f), 1f);
        GL.Enable(EnableCap.DepthTest);
        
    }

}