// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
using Unminal.Render.Texture;

namespace Unminal.Render.Billboards;

using Unminal.Core.State;
using Unminal.Render.Texture;
using Unminal.Render.MeshProgram;
using Unminal.Render.ShaderProgram;

[SupportedOSPlatform("windows")]
public class Billboard {
    private static Shader? _sharedShader;
    private static Mesh? _sharedMesh;
    private static bool _isInitialized = false;
    private static int _locPos = -1;
    private static int _locScale = -1;
    private static int _locTexture = -1;

    private Vector3 _position;
    private Vector2 _scale;
    private Texture2D? _activeTexture;

    public Billboard() {
        _position = Vector3.Zero;
        _scale = new Vector2(1.0f, 1.0f);
        
        Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
    }

    public static void Initialize(string vertPath, string fragPath) {
        if (_isInitialized) return;

        _sharedShader = new Shader(vertPath, fragPath);

        float[] vertices = {
            -0.5f, -0.5f,  0.0f,    0.0f, 0.0f,
             0.5f, -0.5f,  0.0f,    1.0f, 0.0f,
             0.5f,  0.5f,  0.0f,    1.0f, 1.0f,
            -0.5f,  0.5f,  0.0f,    0.0f, 1.0f 
        };

        uint[] indices = { 0, 1, 2, 2, 3, 0 };

        _sharedMesh = new Mesh(vertices, indices, new int[] { 3, 2 });

        _locPos = GL.GetUniformLocation(_sharedShader.Handle, "billboardPos");
        _locScale = GL.GetUniformLocation(_sharedShader.Handle, "scale");
        _locTexture = GL.GetUniformLocation(_sharedShader.Handle, "billboardTex");

        _isInitialized = true;
    }

    public Billboard Position(Vector3 position) {
        _position = position; 
        return this; 
    }

    public Billboard Scale(Vector2 scale) { 
        _scale = scale; 
        return this; 
    }

    public Billboard Color(Vector4 color) {
        _activeTexture = Texture2D.GetOrCreateColorTexture(color);
        return this;
    }

    public Billboard Texture(string texturePath) {
        _activeTexture = Texture2D.GetOrCreateFileTexture(GetPath.GetCorrectPath(texturePath));
        return this;
    }

    public void Draw() {
        if (!_isInitialized || _sharedShader == null || _sharedMesh == null || _activeTexture == null) 
            return;

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        GL.DepthMask(true);

        _sharedShader.Use();
        _sharedShader.SetMatrix4("view", Engine.View);
        _sharedShader.SetMatrix4("projection", Engine.Projection);

        if (_locPos != -1) GL.Uniform3(_locPos, _position);
        if (_locScale != -1) GL.Uniform2(_locScale, _scale.X, _scale.Y);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _activeTexture.Handle);
        if (_locTexture != -1) GL.Uniform1(_locTexture, 0);

        _sharedMesh.Draw();

        GL.Disable(EnableCap.Blend);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public static void Dispose() 
    {
        _sharedShader?.Dispose();
        _sharedMesh?.Dispose();
        Texture2D.ClearAllCaches();

        _sharedShader = null;
        _sharedMesh = null;
        _isInitialized = false;
    }
}

