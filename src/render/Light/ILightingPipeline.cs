// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Render.Light;

using Unminal.Render.ShaderProgram;

public interface ILightingPipeline : IDisposable {
    void Initialize();
    void BeginFrame();
    void ApplyLighting(Shader shader);
    void EndFrame();
}
