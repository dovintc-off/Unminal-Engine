namespace Unminal.Main;

[SupportedOSPlatform("windows")]
public class Main : GameWindow
{
    public static bool debug_mod = false;
    private readonly BaseGame _userGame;
    Matrix4 _model, _view, _projection;
    bool debug_menu = false;
    bool _GamePaused = false;
    bool _GameFullscreen = false;
    float _initialFov = MathHelper.PiOver4;
    private Text? _textRenderer;
    private Camera? _activeCameraRef; 
    private GameConsole? gameConsole;

    public Main(BaseGame userGame) 
        : base(
            new GameWindowSettings() 
            {
                UpdateFrequency = Config.Get<double>("UpdateFrequency", 60.0) 
            }, 
            new NativeWindowSettings()
            {
                ClientSize = new Vector2i(
                    Config.Get<int>("WindowWidth", 1200), 
                    Config.Get<int>("WindowHeight", 900)
                ),
                Title = Config.Get<string>("Title", "Unminal Engine"),
                
                APIVersion = new Version(
                    Config.Get<int>("ApiMajor", 3), 
                    Config.Get<int>("ApiMinor", 3)
                )
            })
    {
        _userGame = userGame;
        this.TextInput += HandleConsoleTextInput;
    }

    protected override void OnLoad()
    {
        base.OnLoad(); 

        gameConsole = new GameConsole();

        bool vsync = Config.Get<bool>("VSync", false);
        this.VSync = vsync ? VSyncMode.On : VSyncMode.Off;

        CursorState = CursorState.Grabbed;
        if (_GameFullscreen) WindowState = WindowState.Fullscreen;

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend); 
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _textRenderer = new Text(
            "./Assets/fonts/VCR-OSD-MONO.ttf",
            32,
            "./Assets/shaders/text/shader.vert",
            "./Assets/shaders/text/shader.frag"
        );

        _model = Matrix4.Identity;
        _projection = Matrix4.CreatePerspectiveFieldOfView(_initialFov, Size.X / (float)Size.Y, 0.1f, 1000.0f);

        _userGame.Load(_projection);

        _activeCameraRef = _userGame.ActiveCamera;

        if (_activeCameraRef == null)
        {
            _activeCameraRef = new Camera(new Vector3(0, 5, 10), -90.0f, 0.0f);
            _userGame.ActiveCamera = _activeCameraRef;
        }

        _view = _activeCameraRef.GetViewMatrix();
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {

        if (gameConsole != null && !gameConsole.IsOpen){
            base.OnMouseMove(e);
            if (CursorState != CursorState.Grabbed) return;

            if (_activeCameraRef != null)
            {
                _activeCameraRef.ProcessMouseMovement(e.DeltaX, e.DeltaY);
                _view = _activeCameraRef.GetViewMatrix();
            }
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs e) {
        base.OnUpdateFrame(e);

        KeyboardState input = KeyboardState;
        MouseState mouse = MouseState;
        
        EngineValues.IsPaused = _GamePaused;
        if (gameConsole != null) EngineValues.IsConsoleOpen = gameConsole.IsOpen;
        EngineValues.IsDebugOpen = debug_menu;

        // Full Screen enable/disable
        if (input.IsKeyReleased(Keys.F11))
        {
            _GameFullscreen = !_GameFullscreen;
            WindowState = _GameFullscreen ? WindowState.Fullscreen : WindowState.Normal;
        }

        gameConsole?.ProcessInput(input);

        // Console open/close
        if (gameConsole != null && gameConsole.IsOpen)
        {
            CursorState = CursorState.Normal;
        }
        else
        {
            if (input.IsKeyReleased(Keys.F3)) { 
                debug_menu = !debug_menu;
            }

            if (input.IsKeyReleased(Keys.Escape))
            {
                _GamePaused = !_GamePaused;
            }

            CursorState = _GamePaused ? CursorState.Normal : CursorState.Grabbed;
        }

        // Script Update Data
        if ((gameConsole == null || !gameConsole.IsOpen) && !_GamePaused)
        {
            var FUV = new FrameUpdateVars(input, mouse, (float)e.Time);
            _userGame.Update(FUV);

            if (_userGame.ActiveCamera != null)
            {
                _activeCameraRef = _userGame.ActiveCamera;
                _view = _activeCameraRef.GetViewMatrix();
            }

            _model = Matrix4.Identity;
        }
        SetTitle();
    }

    private void HandleConsoleTextInput(TextInputEventArgs e)
    {
        if (gameConsole != null && gameConsole.IsOpen)
        {
            string text = e.AsString; 
            
            if (!string.IsNullOrEmpty(text) && text[0] >= 32)
            {
                gameConsole.AppendToCommand(text);
            }
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e) {   
        base.OnRenderFrame(e);

        EngineValues.DeltaTime = (float)e.Time;
        EngineValues.TotalTime += e.Time;
        EngineValues.CurrentKeyboard = KeyboardState;
        EngineValues.CurrentMouse = MouseState;

        GL.ClearColor(0, 0, 0, 1);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _userGame.Draw(_projection);

        if (gameConsole != null && gameConsole.IsOpen) {
            gameConsole.DrawConsole(Size.X, Size.Y);
        } 

        // Debug Menu
        if (_textRenderer != null && debug_menu && _activeCameraRef != null)
        {
            float TS = 0.5f;
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1, 1);
            
            _textRenderer.DrawString($"Unminal V0.2.1 {gameConsole?.IsOpen}", 10, 7, TS, ortho, new Vector4(Colors.White, 1f), 2f);
            _textRenderer.DrawString($"FPS: {1.0 / e.Time:F1}", 10, 63, TS, ortho, new Vector4(Colors.White, 1f), 1f); 
            
            string posText = string.Format(CultureInfo.InvariantCulture, 
                "Pos: {0:F1} {1:F1} {2:F1} | FOV: {3}", 

                _activeCameraRef.Position.X, _activeCameraRef.Position.Y, _activeCameraRef.Position.Z, 
                MathHelper.RadiansToDegrees(_activeCameraRef!.FOV));
            _textRenderer.DrawString(posText, 10, 91, TS, ortho, new Vector4(Colors.White, 1f), 1f);

            string dirText = string.Format(CultureInfo.InvariantCulture, 
                "Dir: {0:F1} {1:F1} {2:F1}", 
                _activeCameraRef.Front.X, _activeCameraRef.Front.Y, _activeCameraRef.Front.Z);
            _textRenderer.DrawString(dirText, 10, 119, TS, ortho, new Vector4(Colors.White, 1f), 1f); 

        }
        Context.SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        EngineValues.WindowSize = new Vector2i(Size.X, Size.Y);
        float currentFov = _activeCameraRef?.FOV ?? _initialFov;
        _projection = Matrix4.CreatePerspectiveFieldOfView(currentFov, Size.X / (float)Size.Y, 0.1f, 1000.0f);

    }

    protected override void OnUnload()
    {
        _textRenderer?.Dispose();
        base.OnUnload();
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        if (gameConsole != null && !gameConsole.IsOpen){
            if (_activeCameraRef != null)
            {
                _activeCameraRef.ProcessMouseScroll(e.OffsetY);
                _projection = Matrix4.CreatePerspectiveFieldOfView(_activeCameraRef.FOV, Size.X / (float)Size.Y, 0.1f, 100.0f);
            }
        } 
    }
    
    // Helper Metods
    private void SetTitle(){
        string BaseTitle = Config.Get<string>("Title", "Unminal Engine");
        if (this.WindowState != WindowState.Fullscreen){
            if (debug_menu) Title = BaseTitle + " (In Debug Menu)";
            else if (_GamePaused) Title = BaseTitle + " (In Pause)";
            else if (gameConsole != null && gameConsole.IsOpen) Title = BaseTitle + " (In Console)";
            else Title = BaseTitle;
        } else {
            if (Title != BaseTitle) Title = BaseTitle;
        }
    }
}

// Helper Classes

public class FrameUpdateVars
{
    public KeyboardState Keyboard { get; private set; }
    public MouseState Mouse { get; private set; }

    public float DeltaTime { get; private set; }

    public FrameUpdateVars(KeyboardState keyboard, MouseState mouse, float deltaTime)
    {
        Keyboard = keyboard;
        Mouse = mouse;
        DeltaTime = deltaTime;
    }
}