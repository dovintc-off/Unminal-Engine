// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
// Im take this code with another my project "SyncraRPC"
// now these two projects are running on the same config system
// im chnge it but this is soo cool))) 
namespace Unminal.Utils.ConfigManager;

using System.Text.Json;
using System.ComponentModel;

[SupportedOSPlatform("windows")]
public class Config {
    string fileConfig = GetPath.GetPath.GetCorrectPath(Engine.Paths.Config.MainConfig, true);

    public Config(string FileConfig = "") {
        if (!(FileConfig == "")) this.fileConfig = FileConfig;
        JsonRoot conf = ReadConfig(this.fileConfig);
    }

    public static T ConvertTo<T>(object input) {
        if (input == null || input == DBNull.Value) throw new Exception("[#red][ERROR] Value is null");;
        try {
            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (targetType == typeof(bool)) {
                string str = (input.ToString() ?? "").Trim();
                if (str == "1") return (T)(object)true;
                if (str == "0") return (T)(object)false;
                return (T)(object)bool.Parse(str);
            }
            if (targetType == typeof(string)) {
                if (input is bool b) return (T)(object)(b ? "true" : "false");
                string str = (input.ToString() ?? "").Trim();
                if (str == "1") return (T)(object)"true";
                if (str == "0") return (T)(object)"false";
            }
            return (T)Convert.ChangeType(input, targetType);
        } catch {
            throw new Exception("[#red][ERROR] Value is Unknown");
        }
    }

    public void SetConfig(
        string? newTitle = null,
        string? newDebug = null,
        string? newHeight = null,
        string? newWidth = null,
        string? newVSync = null,
        string? newLocationX = null,
        string? newLocationY = null,
        string? newLightType = null,
        string? newCanf3 = null,
        string? newShowLights = null,
        string? newLanguage = null
    )
    {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null) config.Changeable = new ChangeableData();
        if (config.Changeable.EngineSettings.WindowSettings == null) config.Changeable.EngineSettings.WindowSettings = new WindowSettings();

        config.Changeable.Graphics.LightType = newLightType != null
            ? newLightType.ToString() : this.GetConfig<string>("LightType");
        config.Changeable.EngineSettings.Debug = newDebug != null
            ? ConvertTo<bool>(newDebug) : this.GetConfig<bool>("Debug");
        config.Changeable.EngineSettings.Canf3 = newCanf3 != null
            ? ConvertTo<bool>(newCanf3) : this.GetConfig<bool>("Canf3");
        config.Changeable.EngineSettings.ShowLights = newShowLights != null
            ? ConvertTo<bool>(newShowLights) : this.GetConfig<bool>("ShowLights");
        config.Changeable.EngineSettings.Language = newLanguage != null
            ? ConvertTo<string>(newLanguage) : this.GetConfig<string>("Language");
        config.Changeable.EngineSettings.WindowSettings.VSync = newVSync != null
            ? ConvertTo<bool>(newVSync) : this.GetConfig<bool>("VSync");
        config.Changeable.EngineSettings.WindowSettings.Height = newHeight != null
            ? ConvertTo<int>(newHeight) : this.GetConfig<int>("Height");
        config.Changeable.EngineSettings.WindowSettings.Width = newWidth != null
            ? ConvertTo<int>(newWidth) : this.GetConfig<int>("Width");
        config.Changeable.EngineSettings.WindowSettings.Title = newTitle 
            ?? this.GetConfig<string>("Title") ?? "Title";
        config.Changeable.EngineSettings.WindowSettings.LocationX = newLocationX != null
            ? ConvertTo<int>(newLocationX) : this.GetConfig<int>("LocationX");
        config.Changeable.EngineSettings.WindowSettings.LocationY = newLocationY != null
            ? ConvertTo<int>(newLocationY) : this.GetConfig<int>("LocationY");

        SaveToFile(config);
    }

    public T GetConfig<T>(string key) {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null) {throw new JsonException($"[#red]in Config.cs null object {fileConfig}");}
        if (config.Changeable.EngineSettings.WindowSettings == null){throw new JsonException("[#red]in Config.cs null object");}

        object val = key switch {
            "LightType" => config.Changeable.Graphics.LightType,
            "Debug" => config.Changeable.EngineSettings.Debug,
            "Canf3" => config.Changeable.EngineSettings.Canf3,
            "ShowLights" => config.Changeable.EngineSettings.ShowLights,
            "Language" => config.Changeable.EngineSettings.Language,
            "VSync" => config.Changeable.EngineSettings.WindowSettings.VSync,
            "Height" => config.Changeable.EngineSettings.WindowSettings.Height,
            "Width" => config.Changeable.EngineSettings.WindowSettings.Width,
            "Title" => config.Changeable.EngineSettings.WindowSettings.Title,
            "LocationX" => config.Changeable.EngineSettings.WindowSettings.LocationX,
            "LocationY" => config.Changeable.EngineSettings.WindowSettings.LocationY,
            _ => throw new Exception($"{key} not found in config")
        };
        return To<T>(val);
    }

    private T To<T>(object? value){
        if (value == null || value == DBNull.Value) {
            throw new Exception("[#red][ERROR] Value is null 2");
        }

        if (value is T strictValue)
            return strictValue;

        Type target = typeof(T);
        Type undertype = Nullable.GetUnderlyingType(target) ?? target;

        try {
            if (undertype.IsEnum) {
                if (value is string s) return (T)Enum.Parse(undertype, s, true);
                return (T)Enum.ToObject(undertype, value);
            }
            var converter = TypeDescriptor.GetConverter(undertype);
            if (converter != null && converter.CanConvertFrom(value.GetType())) {
                return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value)!;
            }

            return (T)Convert.ChangeType(value, undertype, CultureInfo.InvariantCulture);
        } catch {
            throw new Exception("[#red][ERROR] Value is unknown");
        }
    }
        
    private JsonRoot ReadConfig(string PathToFile)
    {   
        if (!File.Exists(PathToFile)) {
            return new JsonRoot();
        }

        using FileStream stream = File.OpenRead(PathToFile);
        JsonRoot data = JsonSerializer.Deserialize<JsonRoot>(stream)!;
        return data;
    }

    private void SaveToFile(JsonRoot config)
    {
        var options = new JsonSerializerOptions { WriteIndented = true};
        string jsonString = JsonSerializer.Serialize(config, options);
        File.WriteAllText(this.fileConfig, jsonString);
    }
}

// Config structure
public class JsonRoot {
    public ChangeableData? Changeable {get; set;}
    [System.Text.Json.Serialization.JsonPropertyName("User-defined")]
    public Dictionary<string, object>? UserDefined {get; set;}
}

public class ChangeableData {
    public Graphics Graphics {get; set;}= new Graphics();
    public EngineSettings EngineSettings {get; set;} = new EngineSettings();
}

public class Graphics {
    public string LightType {get; set;} = "Forward-Rendering-With-UBO";
}

public class EngineSettings {
    public bool Debug {get; set;}
    public bool Canf3 {get; set;}
    public bool ShowLights {get; set;}
    public string Language {get; set;} = "EN";
    public WindowSettings WindowSettings {get; set;} = new WindowSettings();
}

public class WindowSettings {
    public bool VSync {get; set;}
    public int Height {get; set;}
    public int Width {get; set;}
    public string Title {get; set;} = "Game on Unminal Engine";
    public int LocationX {get; set;}
    public int LocationY {get; set;}
}

public class Extension {
    string? Name {get; set;}
    string? Path {get; set;}
}

public class ExtensionSetting {
    string? Name {get; set;}
    string? Namespace {get; set;}
    object? values {get; set;}
}