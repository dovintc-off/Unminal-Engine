// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
// render/Mesh.cs
namespace Unminal.Render.MeshProgram;

using Main;

[SupportedOSPlatform("windows")]
public class Mesh : IDisposable {
    private int _vao, _vbo, _ebo, _indexCount;

    public Mesh(float[] vertices, uint[] indices, int[] attributes) {
        _indexCount = indices.Length;

        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 
                      vertices.Length * sizeof(float),
                      vertices,
                      BufferUsageHint.StaticDraw
        );

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, 
                      indices.Length * sizeof(uint),
                      indices,
                      BufferUsageHint.StaticDraw
        );

        int totalComponents = 0;
        foreach (int count in attributes) {
            totalComponents += count;
        }
        int stride = totalComponents * sizeof(float);

        int offset = 0;
        for (int i = 0; i < attributes.Length; i++) {
            int componentCount = attributes[i];
            
            GL.VertexAttribPointer(i, componentCount, VertexAttribPointerType.Float, false, stride, offset);
            GL.EnableVertexAttribArray(i);

            offset += componentCount * sizeof(float);
        }

        GL.BindVertexArray(0);

        if (Engine.CurrentWindow is Main main) main.RegisterMeshLoaded(_indexCount);
    }

    public Mesh(float[] vertices, uint[] indices) : this(vertices, indices, new int[] { 3, 3 }) {}

    public void Draw() {
        GL.BindVertexArray(_vao);

        if (Engine.CurrentWindow is Main main)
            main.RegisterDraw(_indexCount);

        GL.DrawElements(
            PrimitiveType.Triangles,
            _indexCount,
            DrawElementsType.UnsignedInt,
            IntPtr.Zero
        );

        GL.BindVertexArray(0);
    }

    public void Dispose() {
        if (_vao == 0) return;
        if (Engine.CurrentWindow is Main main) main.RegisterMeshUnloaded(_indexCount);

        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);

        _vao = 0;
        _vbo = 0;
        _ebo = 0;
    }
}
