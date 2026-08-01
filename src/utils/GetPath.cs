// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Utils.GetPath;
using System.Runtime.CompilerServices;

[SupportedOSPlatform("windows")]
public static class GetPath {
    public static string GetCorrectPath(string virtualPath, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) {

        if (string.IsNullOrWhiteSpace(virtualPath))
            throw new ArgumentException("Path cannot be null or empty.", nameof(virtualPath));

        ValidateNoTraversal(virtualPath);

        string normalized = virtualPath.Trim();
        bool isAssets = normalized.StartsWith("assets:/", StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(@"assets:\", StringComparison.OrdinalIgnoreCase);
        bool isData = normalized.StartsWith("data:/", StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(@"data:\", StringComparison.OrdinalIgnoreCase);
        bool isFont = normalized.StartsWith("font:/", StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(@"font:\", StringComparison.OrdinalIgnoreCase);
        bool isObj = normalized.StartsWith("obj:/", StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(@"obj:\", StringComparison.OrdinalIgnoreCase);
        bool isShader = normalized.StartsWith("shader:/", StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(@"shader:\", StringComparison.OrdinalIgnoreCase);
        bool isTexture = normalized.StartsWith("texture:/", StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(@"texture:\", StringComparison.OrdinalIgnoreCase);

        string cleanPath;
        string basePath;

        if (isAssets) {
            cleanPath = normalized.Substring(8);
            basePath = Engine.Paths.Assets;
        } else if (isData) {
            cleanPath = normalized.Substring(6);
            basePath = Engine.Paths.Data;
        } else if (isFont) {
            cleanPath = normalized.Substring(6);
            basePath = Engine.Paths.Font;
        } else if (isObj) {
            cleanPath = normalized.Substring(5);
            basePath = Engine.Paths.Objects;
        } else if (isShader) {
            cleanPath = normalized.Substring(8);
            basePath = Engine.Paths.Shader;
        } else if (isTexture) {
            cleanPath = normalized.Substring(9);
            basePath = Engine.Paths.Textures;
        } else {
            cleanPath = normalized;
            if (cleanPath.StartsWith("./", StringComparison.Ordinal) || cleanPath.StartsWith(@".\", StringComparison.Ordinal))
                cleanPath = cleanPath.Substring(2);

            basePath = Engine.Paths.BaseFolder;
        }

        Console.CreateLog(Console.LogType.INFO, $"Load file: {Path.Combine(basePath, cleanPath.Trim())}", file:file, line:line);
        return Path.Combine(basePath, cleanPath.Trim());
    }

    public static string[] GetCorrectPath(string[] virtualPaths) {
        if (virtualPaths == null || virtualPaths.Length == 0)
            return Array.Empty<string>();

        string[] result = new string[virtualPaths.Length];

        for (int i = 0; i < virtualPaths.Length; i++) {
            try {
                result[i] = GetCorrectPath(virtualPaths[i]);
            } catch (Exception ex) when (ex is ArgumentException or Exception) {
                throw new InvalidOperationException($"Failed to resolve path at index [{i}] ('{virtualPaths[i]}'): {ex.Message}", ex);
            }
        }
        return result;
    }

    private static void ValidateNoTraversal(string path) {
        if (path.Contains("..", StringComparison.Ordinal))
            throw new Exception($"Path traversal ('..') is not allowed. Got: '{path}'");
    }
}
