// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
// render/Mesh.cs
namespace Unminal.Render.MeshProgram;

[SupportedOSPlatform("windows")]
public class Mesh : IDisposable
{
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
    }

    public Mesh(float[] vertices, uint[] indices) 
        : this(vertices, indices, new int[] { 3, 3 }) {}

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
    }
}
