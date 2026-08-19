// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.EngineConsole;

using Unminal.Core.Commands.Executor;
using Unminal.Render.Primitive._2D;
using Unminal.UI.TextRender.TextRenderer;
using Unminal.Utils.Colors;

[SupportedOSPlatform("windows")]
public class Console {
    public bool IsOpen {get; private set;} = false;
    public static List<string> History {get; private set;} = new List<string>();
    public static Console? Instance { get; private set; }
    private Text? _textRenderer;
    public string InputedCommand {get; private set;} = "";
    private bool _wasToggleKeyPressed = false;
    private readonly string _pathToFileHistory = GetPath.GetCorrectPath(Engine.Paths.Config.ConsoleHistory, true);
    private KeyboardState? _prevInput;

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
        _textRenderer = new Text(
            AppContext.BaseDirectory + "Assets/fonts/PFAgoraSlabPro-Bold.ttf",
            256,
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textV, true),
            GetPath.GetCorrectPath(Engine.Paths.Shaders.textF, true)
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
            Log.Create(Log.LogType.WARNING, "Cant read history file");
            return new List<string>();
        } try {       
            return new List<string>(File.ReadAllLines(_pathToFileHistory));
        } catch (Exception e) {
            Log.Create(Log.LogType.WARNING, $"Cant read history file {e}");
            return new List<string>();
        }
    }
     
    private void WriteHistory(string command) {
        try {
            File.AppendAllText(_pathToFileHistory, command + Environment.NewLine);
        } catch (Exception e) {
            Log.Create(Log.LogType.ERROR, $"Error write command history: {e}");
            
        }
    }   

    public static void WriteLine(object text){
        string safeText = text?.ToString() ?? string.Empty;
        List<Text.TextPart> Parts;
        Parts = Text.ParseColor(safeText, new Vector4(Colors.White, 1));
        foreach (Text.TextPart part in Parts) { 
            Vector3 vec3Color = Colors.VEC3toRGB(new Vector3(part.TextColor));
            System.Console.Write($"\x1b[38;2;{vec3Color[0]};{vec3Color[1]};{vec3Color[2]}m{part.Text}\x1b[0m");
        }
        System.Console.Write("\x1b[0m\n");
    }

    public static void Write(object text){
        string safeText = text?.ToString() ?? string.Empty;
        List<Text.TextPart> Parts;
        Parts = Text.ParseColor(safeText, new Vector4(Colors.White, 1));
        foreach (Text.TextPart part in Parts) { 
            Vector3 vec3Color = Colors.VEC3toRGB(new Vector3(part.TextColor));
            System.Console.Write($"\x1b[38;2;{vec3Color[0]};{vec3Color[1]};{vec3Color[2]}m{part.Text}\x1b[0m");
        }
        System.Console.Write("\x1b[0m");
    }

    public static void Write() => Write("");
    public static void WriteLine() => WriteLine("");

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
            _textRenderer?.DrawString($"{line}", 10, 20 * index, 15f, new Vector4(Colors.White, 1));
            index++;
        }

        _textRenderer?.DrawString(InputedCommand, 10, Engine.WindowSize.Y - 30, 15f, new Vector4(Colors.White, 1f), 1f);
        GL.Enable(EnableCap.DepthTest);
        
    }

}
