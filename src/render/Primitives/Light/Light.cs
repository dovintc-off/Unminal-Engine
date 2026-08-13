// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
using System.Runtime.InteropServices;

namespace Unminal.Render.Light;

/// <summary>
/// Represents a single light source data structure for GPU transfer.
/// The layout matches std140 alignment rules in GLSL (each vec3 + float = 16 bytes).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct GpuLight
{
    public Vector3 Position;
    public float Constant;   // Attenuation constant factor

    public Vector3 Color;
    public float Linear;     // Attenuation linear factor

    public Vector3 Ambient;
    public float Quadratic;  // Attenuation quadratic factor

    public GpuLight(Vector3 position, Vector3 color, float intensity = 1.0f)
    {
        Position = position;
        Color = color * intensity;
        
        // Standard attenuation presets for a point light
        Constant = 1.0f;
        Linear = 0.09f;
        Quadratic = 0.032f;
        
        // Base ambient light for this source
        Ambient = color * 0.1f * intensity; 
    }
}

/// <summary>
/// High-level light object for user interaction.
/// </summary>
public class LightData {
    private GpuLight _data;
    
    public bool IsDirty { get; private set; } = true;

    public Vector3 Position 
    { 
        get => _data.Position; 
        set 
        { 
            _data.Position = value; 
            IsDirty = true; 
        } 
    }
    
    public Vector3 Color 
    { 
        get => _data.Color; 
        set 
        { 
            _data.Color = value; 
            IsDirty = true; 
        } 
    }

    public GpuLight Data => _data;

    public LightData(Vector3 position, Vector3 color, float intensity = 1.0f)
    {
        _data = new GpuLight(position, color, intensity);
    }

    public void MarkClean() => IsDirty = false;
}
