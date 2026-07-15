namespace Unminal.Utils.Config;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text.Json;

[SupportedOSPlatform("windows")]
public static class Config
{
    private static readonly ConfigManager _manager = new ConfigManager("config.json");
    public static bool IsLoaded { get; private set; } = false;
    public static void Init()
    {
        _manager.Load();
        IsLoaded = true;
    }

    public static T Get<T>(string key, T? defaultValue = default)
    {
        if (!IsLoaded) Init();
        
        var value = _manager.Get(key, defaultValue);

        #pragma warning disable CS8603
        return value ?? defaultValue;
        #pragma warning restore CS8603
    }
    
    public static void Save()
    {
        _manager.Save();
    }
}

/// <summary>
/// Manages configuration settings by saving and loading them from a JSON file.
/// </summary>
[SupportedOSPlatform("windows")]
public class ConfigManager
{
    private readonly Dictionary<string, object> _settings = new Dictionary<string, object>();
    
    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigManager"/> class with a specified file path.
    /// </summary>
    /// <param name="filePath">The path to the configuration JSON file. Default is "config.json".</param>
    public ConfigManager(string filePath = "config.json")
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Loads configuration settings from the JSON file. If the file does not exist, a default empty file is created.
    /// </summary>
    public void Load()
    {
        if (!File.Exists(_filePath))
        {
            GameConsole.Instance?.Log("Error", $"File '{_filePath}' not found. Creating default empty config.");
            Save();
            return;
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, options);

            if (data != null)
            {
                _settings.Clear();
                foreach (var kvp in data)
                {
                    _settings[kvp.Key] = ConvertJsonElementToObject(kvp.Value);
                }
            }
            
            GameConsole.Instance?.Log("Debug", "Loaded successfully.");
        }
        catch (Exception ex)
        {
            GameConsole.Instance?.Log("Error", $"Error loading file: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves the current configuration settings into the JSON file with indentation formatting.
    /// </summary>
    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        string json = JsonSerializer.Serialize(_settings, options);
        File.WriteAllText(_filePath, json);
    }

    /// <summary>
    /// Retrieves a configuration value associated with the specified key, casting or converting it to the requested type.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value to return.</typeparam>
    /// <param name="key">The unique key of the setting.</param>
    /// <param name="defaultValue">The value to return if the key is not found or type conversion fails.</param>
    /// <returns>The stored setting value converted to type <typeparamref name="T"/>, or <paramref name="defaultValue"/>.</returns>
    public T Get<T>(string key, T defaultValue = default!)
    {
        if (_settings.TryGetValue(key, out object? value))
        {
            try
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                return (T)Convert.ChangeType(value, typeof(T))!;
            }
            catch (Exception ex)
            {
                GameConsole.Instance?.Log("Error", $"Type mismatch for key '{key}'. Expected {typeof(T)}, got {value.GetType()}. Error: {ex.Message}");
                return defaultValue;
            }
        }
        else
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Converts a <see cref="JsonElement"/> to its corresponding standard C# primitive object type.
    /// </summary>
    /// <param name="element">The JSON element to convert.</param>
    /// <returns>A primitive object (string, int, long, double, bool, DBNull, or raw JSON string).</returns>
    private object ConvertJsonElementToObject(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString() ?? string.Empty;
            case JsonValueKind.Number:
                if (element.TryGetInt32(out int i)) return i;
                if (element.TryGetInt64(out long l)) return l;
                if (element.TryGetDouble(out double d)) return d;
                return element.GetDouble();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Null:
                return DBNull.Value;
            default:
                return element.GetRawText(); 
        }
    }
}