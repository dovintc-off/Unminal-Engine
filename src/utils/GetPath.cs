// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Utils.GetPath;
using System.Runtime.CompilerServices;

[SupportedOSPlatform("windows")]
public static class GetPath {
    public static string GetCorrectPath(string virtualPath, bool Logging = false, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        if (string.IsNullOrWhiteSpace(virtualPath))
            throw new ArgumentException("Path cannot be null or empty.", nameof(virtualPath));
        ValidateNoTraversal(virtualPath);
        string fullpath = Engine.Paths.BaseFolder + virtualPath;

        if (Logging) Log.Create(Log.LogType.INFO, $"Load file: {fullpath}", file:file, line:line);
        return fullpath;
    }

    public static string[] GetCorrectPath(string[] virtualPaths) {
        if (virtualPaths == null || virtualPaths.Length == 0)
            return Array.Empty<string>();

        string[] result = new string[virtualPaths.Length];

        for (int i = 0; i < virtualPaths.Length; i++) {
            try {
                result[i] = GetCorrectPath(virtualPaths[i]);
            } catch (Exception ex) {
                throw new InvalidOperationException($"Failed to resolve path at index [{i}] ('{virtualPaths[i]}'): {ex.Message}", ex);
            }
        }
        return result;
    }

    public static string GetLuaPath(string virtualPath, bool Logging = false, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {
        string FolderFrom = Path.GetFullPath(Directory.GetCurrentDirectory());

        if (string.IsNullOrWhiteSpace(virtualPath)) {
            Log.Create(Log.LogType.ERROR, "Virtual LUA path is null or white space");
            return "";
        }

        string path = Path.GetFullPath(Path.Combine(FolderFrom, virtualPath));

        string relative = Path.GetRelativePath(FolderFrom, path);

        if (relative == ".." || relative.StartsWith(".." + Path.DirectorySeparatorChar,StringComparison.Ordinal) || Path.IsPathRooted(relative)) {
            Log.Create(Log.LogType.WARNING, "Going outside the allowed folder");
            return "";
        }

        if (File.Exists(path)) {
            if (Logging) Log.Create(Log.LogType.INFO, $"Load file: {path}", file:file, line:line);
            return path;   
        } else {
            Log.Create(Log.LogType.ERROR, $"{path} is not exists.");
            return "";
        }
    }

    private static void ValidateNoTraversal(string path) {
        if (path.Contains("..", StringComparison.Ordinal))
            Log.Create(Log.LogType.ERROR, $"Path traversal ('..') is not allowed. Got: '{path}'");
    }
}
