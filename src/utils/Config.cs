// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
// Im take this code with another my project "SyncraRPC"
// now these two projects are running on the same config system
// im chnge it but this is soo cool))) 

using System.Text.Json;
using System.ComponentModel;

namespace Unminal.Utils.ConfigManager;

[SupportedOSPlatform("windows")]
public class Config {
    string fileConfig = GetPath.GetPath.GetCorrectPath(Engine.Paths.Config.MainConfig);

    public Config(string FileConfig = "") {
        if (!(FileConfig == "")) this.fileConfig = FileConfig;
        JsonRoot conf = ReadConfig(this.fileConfig);
    }

    public T ConvertTo<T>(object input) {
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
        string? newCanf3 = null
    )
    {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null) config.Changeable = new ChangeableData();
        if (config.Changeable.WindowSettings == null) config.Changeable.WindowSettings = new WindowSettings();
        config.Changeable.Title = newTitle ?? this.GetConfig<string>("Title") ?? "Title";
        config.Changeable.Debug = newDebug != null
            ? ConvertTo<bool>(newDebug)
            : this.GetConfig<bool>("Debug");
        config.Changeable.WindowSettings.Height = newHeight != null
            ? ConvertTo<int>(newHeight)
            : this.GetConfig<int>("Height");
        config.Changeable.WindowSettings.Width = newWidth != null
            ? ConvertTo<int>(newWidth)
            : this.GetConfig<int>("Width");
        config.Changeable.WindowSettings.VSync = newVSync != null
            ? ConvertTo<bool>(newVSync)
            : this.GetConfig<bool>("VSync");
        config.Changeable.WindowSettings.LocationX = newLocationX != null
            ? ConvertTo<int>(newLocationX)
            : this.GetConfig<int>("LocationX");
        config.Changeable.WindowSettings.LocationY = newLocationY != null
            ? ConvertTo<int>(newLocationY)
            : this.GetConfig<int>("LocationY");
        config.Changeable.LightType = newLightType != null
            ? newLightType.ToString()
            : this.GetConfig<string>("LightType");
        config.Changeable.Canf3 = newCanf3 != null
            ? ConvertTo<bool>(newCanf3)
            : this.GetConfig<bool>("Canf3");

        SaveToFile(config);
    }

    public T GetConfig<T>(string key)
    {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null) {throw new JsonException("[#red]in Config.cs null object");}
        if (config.Changeable.WindowSettings == null){throw new JsonException("[#red]in Config.cs null object");}

        object val = key switch {
            "Title" => config.Changeable.Title,
            "Debug" => config.Changeable.Debug,
            "Height" => config.Changeable.WindowSettings.Height,
            "Width" => config.Changeable.WindowSettings.Width,
            "VSync" => config.Changeable.WindowSettings.VSync,
            "LocationX" => config.Changeable.WindowSettings.LocationX,
            "LocationY" => config.Changeable.WindowSettings.LocationY,
            "LightType" => config.Changeable.LightType,
            "Canf3" => config.Changeable.Canf3,
            "ActiveBackend" => config.Changeable.Scripts.ActiveBackend,
            "LuaEntryFile" => config.Changeable.Scripts.LuaEntryFile,
            "CsharpEntryNameSpace" => config.Changeable.Scripts.CsharpEntryNameSpace,
            "ScriptDrawer" => config.Changeable.Scripts.Drawer,
            "ScriptLoader" => config.Changeable.Scripts.Loader,
            "ScriptUpdater" => config.Changeable.Scripts.Updater,
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
public class JsonRoot
{
    public ChangeableData? Changeable {get; set;}
    [System.Text.Json.Serialization.JsonPropertyName("User-defined")]
    public Dictionary<string, object>? UserDefined {get; set;}
}

public class ChangeableData
{
    public string Title {get; set;} = "Title";
    public bool Debug {get; set;}
    public bool Canf3 {get; set;}
    public WindowSettings WindowSettings {get; set;} = new WindowSettings();
    public string LightType {get; set;} = "Forward-Rendering-With-UBO";
    public Scripts Scripts = new Scripts();
}

public class WindowSettings
{
    public bool VSync {get; set;}
    public int Height {get; set;}
    public int Width {get; set;}
    public int LocationX {get; set;}
    public int LocationY {get; set;}
}

public class Scripts 
{
    public string ActiveBackend = "Csharp";
    public string CsharpEntryNameSpace = "Unminal.Game";
    public string LuaEntryFile = "scripts:/main.lua";
    public string Drawer {get; set;} = "Csharp";
    public string Updater {get; set;} = "Csharp";
    public string Loader {get; set;} = "Csharp";
}