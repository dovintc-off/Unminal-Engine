// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Render.Light;

using System.Runtime.InteropServices;

[SupportedOSPlatform("windows")]
public class LightManager : IDisposable {
    public const int MaxLights = 64; 
    public const int LightBlockBinding = 0; 
    private static readonly int LightStride = Marshal.SizeOf<GpuLight>();
    private readonly GpuLight[] _lightDataArray = new GpuLight[MaxLights];
    private readonly int[] _lightCountData = new int[4];

    private readonly List<LightData> _lights = new List<LightData>();
    private int _uboHandle;
    private bool _isDirty = true;

    public IReadOnlyList<LightData> Lights => _lights;

    public LightManager() {
        InitializeUBO();
    }

    private void InitializeUBO() {
        _uboHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, _uboHandle);
        
        int dataSize = MaxLights * LightStride + 16;
        GL.BufferData(BufferTarget.UniformBuffer, dataSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, LightBlockBinding, _uboHandle);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void AddLight(LightData light) {
        if (_lights.Count >= MaxLights) {
            Log.Create(Log.LogType.WARNING, $"Max lights limit ({MaxLights}) reached.");
            return;
        }
        _lights.Add(light);
        _isDirty = true;
    }

    public void RemoveLight(LightData light) {
        if (_lights.Remove(light)) _isDirty = true;
    }

    public void ClearLights() {
        _lights.Clear();
        _isDirty = true;
    }

    public void UpdateGPUData() {
        bool anyLightDirty = false;
        foreach (var light in _lights) {
            if (light.IsDirty) {
                anyLightDirty = true;
                break;
            }
        }

        if (!_isDirty && !anyLightDirty) return;

        GL.BindBuffer(BufferTarget.UniformBuffer, _uboHandle);

        for (int i = 0; i < _lights.Count; i++) {
            _lightDataArray[i] = _lights[i].Data;
        }

        int activeBytes = _lights.Count * LightStride;

        if (activeBytes > 0) {
            GL.BufferSubData(
                BufferTarget.UniformBuffer,
                IntPtr.Zero,
                activeBytes,
                _lightDataArray
            );
        }

        int countOffset = MaxLights * LightStride;

        _lightCountData[0] = _lights.Count;
        _lightCountData[1] = 0;
        _lightCountData[2] = 0;
        _lightCountData[3] = 0;

        GL.BufferSubData(
            BufferTarget.UniformBuffer,
            new IntPtr(countOffset),
            16,
            _lightCountData
        );

        foreach (var light in _lights) {
            light.MarkClean();
        }

        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        _isDirty = false;
    }

    public void Dispose() {
        if (_uboHandle != 0) {
            GL.DeleteBuffer(_uboHandle);
            _uboHandle = 0;
        }
    }
}
