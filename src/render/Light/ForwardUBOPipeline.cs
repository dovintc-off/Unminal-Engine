// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Render.Light;

using Unminal.Render.ShaderProgram;


[SupportedOSPlatform("windows")]
public class ForwardUBOPipeline : ILightingPipeline {
    private readonly LightManager _lightManager;

    public ForwardUBOPipeline(LightManager lightManager) {
        _lightManager = lightManager;
    }

    public void Initialize() { }

    public void BeginFrame() {
        _lightManager.UpdateGPUData();
    }

    public void ApplyLighting(Shader shader) {
        int blockIndex = GL.GetUniformBlockIndex(shader.Handle, "LightBlock");
        if (blockIndex != -1) {
            GL.UniformBlockBinding(shader.Handle, blockIndex, LightManager.LightBlockBinding);
        }
    }

    public void EndFrame() { }
    public void Dispose() { }
}
