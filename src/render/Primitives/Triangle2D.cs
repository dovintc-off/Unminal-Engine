// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Render.Primitive._2D;

[SupportedOSPlatform("windows")]
public class Triangle : Primitive2D {
    public Triangle(Vector2 position, float scale, Vector4 color, float rotation, Vector2? pivotPoint = null) {
        Position = position;
        Scale = Vector2.One;
        Rotation = rotation;
        Color = color;

        Vector2 v0 = new Vector2(0.0f, 0.5f);
        Vector2 v1 = new Vector2(-0.5f, -0.5f);
        Vector2 v2 = new Vector2(0.5f, -0.5f);

        Vector2 pivot = pivotPoint ?? ((v0 + v1 + v2) / 3.0f);
        Pivot = pivot;

        v0 -= pivot;
        v1 -= pivot;
        v2 -= pivot;

        v0 *= scale;
        v1 *= scale;
        v2 *= scale;

        float[] vertices = {
            v0.X, v0.Y,
            v1.X, v1.Y,
            v2.X, v2.Y,
            v0.X, v0.Y 
        };

        VertexCount = vertices.Length / 2;
        VAO = GL.GenVertexArray();
        int vbo = GL.GenBuffer();
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        
        InitializeShader();
    }

    protected override float[] GetVertices() => Array.Empty<float>();
}
