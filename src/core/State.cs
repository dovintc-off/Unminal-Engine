// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.State;

using Unminal.Core.PlayerCamera;
using Unminal.Render.Light;
using Unminal.Utils.ConfigManager;
using Unminal.Core.Input.Mouse.VisualState;

[SupportedOSPlatform("windows")]
public static class Engine {
    // Global states
    public static string[] args = {};
    public static string ScriptingSystem = "";
    public static Main.Main? CurrentWindow {get; internal set;}
    // Window state
    public static LightManager? LightManager {get; set;}
    public static ILightingPipeline? LightingPipeline {get; set;}
    public static Config ConfigManager {get; set;} = new Config();
    public static Vector2i WindowSize {get; set;}
    public static float DeltaTime {get; set;}
    public static double TotalTime {get; set;}
    public static KeyboardState? CurrentKeyboard {get; set;}
    public static MouseState? CurrentMouse {get; set;}
    public static CursorState CurrentCursorState {get; set;}
    // Menu state
    public static bool CanF3 {get; set;} = true;
    public static bool IsDebug {get; set;}
    public static bool ShowLight { get; set; } = false;
    // Matrix
    public static Matrix4 Projection {get; set;}
    public static Matrix4 View {get; set;}
    public static Matrix4 Ortho {get; set;}    
    // idk why im added this but perhaps in the 
    // future the meaning and uniqueness engine will be 
    // that it is easy to create a network division (maybe)
    public static class Player {
        public static string? userName {get; set;}
        public static string? id {get; set;}
        public static string? language {get; set;}
        // Original camera object
        public static Camera? CameraObj {get; set;}
    }

    public static class ExtensionData {
        // here a engine "extensions" data (maybe)
    };

    // Global window state
    public static class GlobalWindowState {
        public static bool InConsole {get; set;}
        public static bool IsFullScreen {get; set;}
        public static bool InDebugMenu {get; set;}
        public static bool InPause {get; set;}
        public static class InputState {
            public static CursorVisualState CurrentCursorVisual { get; set; } = CursorVisualState.Normal;
        }
    }

    public static class Paths {
        // path to folder where located Unminal.exe file
        public static string BaseFolder {get; set;} = "";
        public static string Assets = @"Assets\";
        public static string Objects = @"Assets\objects\";
        public static string Data = @"Assets\data\";
        public static string Font = @"Assets\fonts\";
        public static string Shader = @"Assets\shaders\";
        public static string Textures = @"Assets\textures\";
        public static readonly string[] BaseSkyBoxAssets = {
            "./Assets/SkyBox/right.png",
            "./Assets/SkyBox/left.png",
            "./Assets/SkyBox/top.png",
            "./Assets/SkyBox/bottom.png",
            "./Assets/SkyBox/front.png",
            "./Assets/SkyBox/back.png"
        };
        public class Config {
            private static readonly Dictionary<string, string> _ = new() {
                ["CommandConfig"] = "./Assets/data/CommandExecutorConfig.json", 
                ["ConsoleHistory"] = "./Assets/data/ConsoleHistory.log",
                ["MainConfig"] = "./Assets/data/config.json"
            };
            public static string CommandConfig => _["CommandConfig"];
            public static string ConsoleHistory => _["ConsoleHistory"]; 
            public static string MainConfig => _["MainConfig"];
        }
        public class Shaders {
            private static readonly Dictionary<string, string> _ = new() {
                ["mainV"] = "./Assets/shaders/main.vert",
                ["mainF"] = "./Assets/shaders/main.frag",
                ["skyboxV"] = "./Assets/shaders/skybox.vert",
                ["skyboxF"] = "./Assets/shaders/skybox.frag",
                ["textV"] = "./Assets/shaders/text.vert",
                ["textF"] = "./Assets/shaders/text.frag",
                ["baseV"] = "./Assets/shaders/base.vert",
                ["baseF"] = "./Assets/shaders/base.frag",
                ["billboardV"] = "./Assets/shaders/billboard.vert",
                ["billboardF"] = "./Assets/shaders/billboard.frag"
            };
            public static string mainV => _["mainV"];
            public static string mainF => _["mainF"];
            public static string skyboxV => _["skyboxV"];
            public static string skyboxF => _["skyboxF"];
            public static string textV => _["textV"];
            public static string textF => _["textF"];
            public static string baseV => _["baseV"];
            public static string baseF => _["baseF"];
            public static string billboardV => _["billboardV"];
            public static string billboardF => _["billboardF"];
        }

        public class Fonts {
            private static readonly Dictionary<string, string> _ = new(){
                ["Metroplex_Shadow"] = "./Assets/fonts/Metroplex-Shadow.ttf",
                ["PFAgoraSlabPro_Bold"] = "./Assets/fonts/PFAgoraSlabPro-Bold.ttf",
                ["VCR_OSD_MONO"] = "./Assets/fonts/VCR-OSD-MONO.ttf",
                ["Arial"] = "./Assets/fonts/Arial/arialmt.ttf"
            };
            public static string Metroplex_Shadow => _["Metroplex_Shadow"];
            public static string PFAgoraSlabPro_Bold => _["PFAgoraSlabPro_Bold"];
            public static string VCR_OSD_MONO => _["VCR_OSD_MONO"];
            public static string Arial => _["Arial"];
        }
    }
    
    public class LanguageChars {
        public const string EN = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public const string RU = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    }

    public static void LoadEngineStates(string[] args) {
        Engine.Paths.BaseFolder = AppDomain.CurrentDomain.BaseDirectory;
        Engine.args = args;
        Engine.ConfigManager = new();
    }
}
// yeah im soooo love word "maybe" 
