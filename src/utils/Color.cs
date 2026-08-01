// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Utils.Colors;

using System.Linq;
using System.Runtime.CompilerServices;

public static class Colors
{
    public static readonly Vector3 Black = new(0.0f, 0.0f, 0.0f);               // #000000
    public static readonly Vector3 White = new(1.0f, 1.0f, 1.0f);               // #FFFFFF
    public static readonly Vector3 Grey = new(0.5f, 0.5f, 0.5f);                // #808080
    public static readonly Vector3 LightGrey = new(0.75f, 0.75f, 0.75f);        // #C0C0C0
    public static readonly Vector3 DarkGrey = new(0.25f, 0.25f, 0.25f);         // #404040
    public static readonly Vector3 Silver = new(0.75f, 0.75f, 0.75f);           // #C0C0C0
    public static readonly Vector3 Red = new(1.0f, 0.0f, 0.0f);                 // #FF0000
    public static readonly Vector3 DarkRed = new(0.55f, 0.0f, 0.0f);            // #8B0000
    public static readonly Vector3 Crimson = new(0.86f, 0.08f, 0.24f);          // #DC143C
    public static readonly Vector3 IndianRed = new(0.8f, 0.36f, 0.36f);         // #CD5C5C
    public static readonly Vector3 Green = new(0.0f, 1.0f, 0.0f);               // #00FF00
    public static readonly Vector3 Lime = new(0.0f, 1.0f, 0.0f);                // #00FF00
    public static readonly Vector3 DarkGreen = new(0.0f, 0.39f, 0.0f);          // #006400
    public static readonly Vector3 ForestGreen = new(0.13f, 0.55f, 0.13f);      // #228B22
    public static readonly Vector3 Olive = new(0.5f, 0.5f, 0.0f);               // #808000
    public static readonly Vector3 Blue = new(0.0f, 0.0f, 1.0f);                // #0000FF
    public static readonly Vector3 DarkBlue = new(0.0f, 0.0f, 0.55f);           // #00008B
    public static readonly Vector3 Navy = new(0.0f, 0.0f, 0.5f);                // #000080
    public static readonly Vector3 SkyBlue = new(0.53f, 0.81f, 0.92f);          // #87CEEB
    public static readonly Vector3 CornflowerBlue = new(0.39f, 0.58f, 0.93f);   // #6495ED
    public static readonly Vector3 Yellow = new(1.0f, 1.0f, 0.0f);              // #FFFF00
    public static readonly Vector3 Gold = new(1.0f, 0.84f, 0.0f);               // #FFD700
    public static readonly Vector3 Orange = new(1.0f, 0.65f, 0.0f);             // #FFA500
    public static readonly Vector3 OrangeRed = new(1.0f, 0.27f, 0.0f);          // #FF4500
    public static readonly Vector3 Purple = new(0.5f, 0.0f, 0.5f);              // #800080
    public static readonly Vector3 Violet = new(0.93f, 0.51f, 0.93f);           // #EE82EE
    public static readonly Vector3 Magenta = new(1.0f, 0.0f, 1.0f);             // #FF00FF
    public static readonly Vector3 Indigo = new(0.29f, 0.0f, 0.51f);            // #4B0082
    public static readonly Vector3 Cyan = new(0.0f, 1.0f, 1.0f);                // #00FFFF
    public static readonly Vector3 Aqua = new(0.0f, 1.0f, 1.0f);                // #00FFFF
    public static readonly Vector3 Teal = new(0.0f, 0.5f, 0.5f);                // #008080
    public static readonly Vector3 Turquoise = new(0.25f, 0.88f, 0.82f);        // #40E0D0
    public static readonly Vector3 Brown = new(0.65f, 0.16f, 0.16f);            // #A52A2A
    public static readonly Vector3 Beige = new(0.96f, 0.96f, 0.86f);            // #F5F5DC
    public static readonly Vector3 Chocolate = new(0.82f, 0.41f, 0.12f);        // #D2691E
    public static readonly Vector3 Zero = new(0.0f, 0.0f, 0.0f);                // #000000

    private static readonly Dictionary<string, Vector4> NamedColors = new(StringComparer.OrdinalIgnoreCase) {
        ["black"] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f),                        // #000000FF
        ["white"] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f),                        // #FFFFFFFF
        ["grey"] = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),                         // #808080FF
        ["lightgrey"] = new Vector4(0.75f, 0.75f, 0.75f, 1.0f),                 // #C0C0C0FF
        ["darkgrey"] = new Vector4(0.25f, 0.25f, 0.25f, 1.0f),                  // #404040FF
        ["silver"] = new Vector4(0.75f, 0.75f, 0.75f, 1.0f),                    // #C0C0C0FF
        ["red"] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),                          // #FF0000FF
        ["darkred"] = new Vector4(0.55f, 0.0f, 0.0f, 1.0f),                     // #8B0000FF
        ["crimson"] = new Vector4(0.86f, 0.08f, 0.24f, 1.0f),                   // #DC143CFF
        ["indianred"] = new Vector4(0.8f, 0.36f, 0.36f, 1.0f),                  // #CD5C5CFF
        ["green"] = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),                        // #00FF00FF
        ["lime"] = new Vector4(0.0f, 1.0f, 0.0f, 1.0f),                         // #00FF00FF
        ["darkgreen"] = new Vector4(0.0f, 0.39f, 0.0f, 1.0f),                   // #006400FF
        ["forestgreen"] = new Vector4(0.13f, 0.55f, 0.13f, 1.0f),               // #228B22FF
        ["olive"] = new Vector4(0.5f, 0.5f, 0.0f, 1.0f),                        // #808000FF
        ["blue"] = new Vector4(0.0f, 0.0f, 1.0f, 1.0f),                         // #0000FFFF
        ["darkblue"] = new Vector4(0.0f, 0.0f, 0.55f, 1.0f),                    // #00008BFF
        ["navy"] = new Vector4(0.0f, 0.0f, 0.5f, 1.0f),                         // #000080FF
        ["skyblue"] = new Vector4(0.53f, 0.81f, 0.92f, 1.0f),                   // #87CEEBFF
        ["cornflowerblue"] = new Vector4(0.39f, 0.58f, 0.93f, 1.0f),            // #6495EDFF
        ["yellow"] = new Vector4(1.0f, 1.0f, 0.0f, 1.0f),                       // #FFFF00FF
        ["gold"] = new Vector4(1.0f, 0.84f, 0.0f, 1.0f),                        // #FFD700FF
        ["orange"] = new Vector4(1.0f, 0.65f, 0.0f, 1.0f),                      // #FFA500FF
        ["orangered"] = new Vector4(1.0f, 0.27f, 0.0f, 1.0f),                   // #FF4500FF
        ["purple"] = new Vector4(0.5f, 0.0f, 0.5f, 1.0f),                       // #800080FF
        ["violet"] = new Vector4(0.93f, 0.51f, 0.93f, 1.0f),                    // #EE82EEFF
        ["magenta"] = new Vector4(1.0f, 0.0f, 1.0f, 1.0f),                      // #FF00FFFF
        ["indigo"] = new Vector4(0.29f, 0.0f, 0.51f, 1.0f),                     // #4B0082FF
        ["cyan"] = new Vector4(0.0f, 1.0f, 1.0f, 1.0f),                         // #00FFFFFF
        ["aqua"] = new Vector4(0.0f, 1.0f, 1.0f, 1.0f),                         // #00FFFFFF
        ["teal"] = new Vector4(0.0f, 0.5f, 0.5f, 1.0f),                         // #008080FF
        ["turquoise"] = new Vector4(0.25f, 0.88f, 0.82f, 1.0f),                 // #40E0D0FF
        ["brown"] = new Vector4(0.65f, 0.16f, 0.16f, 1.0f),                     // #A52A2AFF
        ["beige"] = new Vector4(0.96f, 0.96f, 0.86f, 1.0f),                     // #F5F5DCFF
        ["chocolate"] = new Vector4(0.82f, 0.41f, 0.12f, 1.0f),                 // #D2691EFF
        ["zero"] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f)                          // #000000FF
    };

    /// <summary>
    /// Converts RGB code to normalized (0 - 1)
    /// </summary>
    public static Vector3 NormalizeRGB(byte r, byte g, byte b) {
        return new Vector3(r / 255f, g / 255f, b / 255f);
    }

    public static Vector4 NormalizeRGBA(byte r, byte g, byte b, byte a) {
        return new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public static Vector4 HEXtoRGBA(string row) {
        byte r, g, b;
        if (row.Length is int len && 
           (len == 6 || len == 8) && 
           row.All(c => "0123456789ABCDEFabcdef".Contains(c))) 
        {
            ReadOnlySpan<char> span = row.AsSpan();
            r = FastHexToByte(span[0], span[1]);
            g = FastHexToByte(span[2], span[3]); 
            b = FastHexToByte(span[4], span[5]);
            byte a = len == 8 ? FastHexToByte(span[6], span[7]) : (byte)255;
            return NormalizeRGBA(r, g, b, a);
        }
        return new Vector4(1, 1, 1, 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte FastHexToByte(char h1, char h2) {
        int v1 = h1 - '0' < 10 ? h1 - '0' : (h1 | 0x20) - 'a' + 10;
        int v2 = h2 - '0' < 10 ? h2 - '0' : (h2 | 0x20) - 'a' + 10;
        return (byte)((v1 << 4) | v2);
    }

    public static Vector4 HEXtoVEC4(string input) {
        if (string.IsNullOrEmpty(input)) return new Vector4(1f, 1f, 1f, 1f);
        if (TryGetNamedColor(input, out var namedColor)) return namedColor;
        var isValid = IsValidHEX(input);
        if (!isValid) return new Vector4(1f, 1f, 1f, 1f);
        return HEXtoRGBA(input);
    }

    public static bool IsValidRGB(int r, int g, int b) 
        => r is >= 0 and < 256 && g is >= 0 and < 256 && b is >= 0 and < 256;

    public static bool IsValidRGBA(int r, int g, int b, int a) 
        => IsValidRGB(r, g, b) && a is >= 0 and < 256;

    public static bool IsValidHEX(string row)  {
        if (string.IsNullOrEmpty(row)) return false;
        int len = row.Length;
        if (len == 6 || len == 8) {
            bool allValid = true;
            foreach (char c in row) {
                if (!"0123456789ABCDEFabcdef".Contains(c)) {
                    allValid = false;
                    break;
                }
            }   
            if (allValid) return true;
        }
        return false;
    }

    public static Vector3 VEC3toRGB(Vector3 color) {
        byte r = (byte)(color[0] * 255);
        byte g = (byte)(color[1] * 255);
        byte b = (byte)(color[2] * 255);
        return new Vector3(r, g, b);
    }

    public static bool IsValidHEX(Vector4 color) {
        bool valid = color.X is >= 0f and <= 1f &&
                    color.Y is >= 0f and <= 1f &&
                    color.Z is >= 0f and <= 1f &&
                    color.W is >= 0f and <= 1f;
        
        return valid;
    }

    public static bool TryGetNamedColor(string name, out Vector4 color) 
        => NamedColors.TryGetValue(name, out color);
}
