// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Render.Light;

using Unminal.Render.ShaderProgram;


[SupportedOSPlatform("windows")]
public class ForwardUBOPipeline : ILightingPipeline {
    private readonly LightManager _lightManager;
    private readonly Dictionary<int, int> _lightBlockIndices = new();

    public ForwardUBOPipeline(LightManager lightManager) {
        _lightManager = lightManager;
    }

    public void Initialize() { }

    public void BeginFrame() {
        _lightManager.UpdateGPUData();
    }

    public void ApplyLighting(Shader shader) {
        if (!_lightBlockIndices.TryGetValue(shader.Handle, out int blockIndex)) {
            blockIndex = GL.GetUniformBlockIndex(shader.Handle, "LightBlock");
            _lightBlockIndices[shader.Handle] = blockIndex;
        }

        if (blockIndex != -1) {
            GL.UniformBlockBinding(shader.Handle, blockIndex, LightManager.LightBlockBinding);
        }
    }

    public void EndFrame() { }
    public void Dispose() {
        _lightBlockIndices.Clear();
    }
}
