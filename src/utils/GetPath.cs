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
        if (string.IsNullOrWhiteSpace(virtualPath))
            throw new ArgumentException("Path cannot be null or empty.", nameof(virtualPath));
        ValidateNoTraversal(virtualPath);

        string baseDir = Path.GetFullPath(Path.Combine(Engine.Paths.BaseFolder, ".."));
        string fullpath = Path.Combine(baseDir, virtualPath);

        if (Logging) Log.Create(Log.LogType.INFO, $"Load file: {fullpath}", file:file, line:line);
        return fullpath;
    }

    private static void ValidateNoTraversal(string path) {
        if (path.Contains("..", StringComparison.Ordinal))
            throw new Exception($"Path traversal ('..') is not allowed. Got: '{path}'");
    }
}
