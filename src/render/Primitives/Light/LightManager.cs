// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
using System.Runtime.InteropServices;

namespace Unminal.Render.Light;

[SupportedOSPlatform("windows")]
public class LightManager : IDisposable
{
    public const int MaxLights = 1000; 
    public const int LightBlockBinding = 0; 

    private readonly List<LightData> _lights = new List<LightData>();
    private int _uboHandle;
    private bool _isDirty = true;

    public IReadOnlyList<LightData> Lights => _lights;

    public LightManager()
    {
        InitializeUBO();
    }

    private void InitializeUBO()
    {
        _uboHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, _uboHandle);
        
        int dataSize = (MaxLights * Marshal.SizeOf<GpuLight>()) + 16; 
        GL.BufferData(BufferTarget.UniformBuffer, dataSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, LightBlockBinding, _uboHandle);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void AddLight(LightData light)
    {
        if (_lights.Count >= MaxLights)
        {
            Console.CreateLog(Console.LogType.WARNING, $"Max lights limit ({MaxLights}) reached.");
            return;
        }
        _lights.Add(light);
        _isDirty = true;
    }

    public void RemoveLight(LightData light)
    {
        if (_lights.Remove(light)) _isDirty = true;
    }

    public void ClearLights()
    {
        _lights.Clear();
        _isDirty = true;
    }

    public void UpdateGPUData()
    {
        bool anyLightDirty = false;
        foreach (var light in _lights)
        {
            if (light.IsDirty)
            {
                anyLightDirty = true;
                break;
            }
        }

        if (!_isDirty && !anyLightDirty) return;

        GL.BindBuffer(BufferTarget.UniformBuffer, _uboHandle);

        GpuLight[] lightDataArray = new GpuLight[MaxLights];
        for (int i = 0; i < _lights.Count; i++)
        {
            lightDataArray[i] = _lights[i].Data;
            _lights[i].MarkClean();
        }

        int lightDataSize = lightDataArray.Length * Marshal.SizeOf<GpuLight>();
        GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, lightDataSize, lightDataArray);

        int count = _lights.Count;
        GL.BufferSubData(BufferTarget.UniformBuffer, lightDataSize, sizeof(int), new int[] { count, 0, 0, 0 });

        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        _isDirty = false;
    }

    public void Dispose()
    {
        if (_uboHandle != 0)
        {
            GL.DeleteBuffer(_uboHandle);
            _uboHandle = 0;
        }
    }
}
