// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
using StbImageSharp;
namespace Unminal.Render.Texture;

[SupportedOSPlatform("windows")]
public class Texture2D : IDisposable {
    public int Handle { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    private static readonly Dictionary<Vector4, Texture2D> _colorCache = new();
    private static readonly Dictionary<string, Texture2D> _fileCache = new();

    private Texture2D(int handle, int width, int height) {
        Handle = handle;
        Width = width;
        Height = height;
    }

    private static Texture2D LoadFromFile(string path) {
        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        StbImage.stbi_set_flip_vertically_on_load(1);
        int width, height;
        using (Stream stream = File.OpenRead(path)) {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            width = image.Width;
            height = image.Height;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                          width, height, 0, 
                          PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.BindTexture(TextureTarget.Texture2D, 0);
        return new Texture2D(handle, width, height);
    }

    public static Texture2D GetOrCreateFileTexture(string path) {
        if (_fileCache.TryGetValue(path, out var cachedTexture))
            return cachedTexture;

        Texture2D newTexture = LoadFromFile(path);
        _fileCache.Add(path, newTexture);
        return newTexture;
    }

    public static Texture2D GetOrCreateColorTexture(Vector4 color) {
        if (_colorCache.TryGetValue(color, out var cachedTexture))
            return cachedTexture;

        byte r = (byte)(Math.Clamp(color.X, 0f, 1f) * 255);
        byte g = (byte)(Math.Clamp(color.Y, 0f, 1f) * 255);
        byte b = (byte)(Math.Clamp(color.Z, 0f, 1f) * 255);
        byte a = (byte)(Math.Clamp(color.W, 0f, 1f) * 255);
        byte[] pixelData = { r, g, b, a };

        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                      1, 1, 0, 
                      PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        Texture2D newTexture = new Texture2D(handle, 1, 1);
        _colorCache.Add(color, newTexture);
        return newTexture;
    }

    public void Dispose() {
        GL.DeleteTexture(Handle);
    }

    public static void ClearAllCaches() {
        foreach (var tex in _colorCache.Values) tex.Dispose();
        foreach (var tex in _fileCache.Values)  tex.Dispose();
        _colorCache.Clear();
        _fileCache.Clear();
    }
}

