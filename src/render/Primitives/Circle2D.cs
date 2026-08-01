// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Render.Primitive._2D;

[SupportedOSPlatform("windows")]
public class Circle : Primitive2D {
    private readonly short _segments;
    private readonly float _radius;

    public Circle(Vector2 center, short segments, Vector4 color, float radius) {
        _segments = segments;
        _radius = radius;
        Position = center;
        Color = color;
        Pivot = Vector2.Zero;
        
        LoadCustomGeometry();
        InitializeShader();
    }

    private void LoadCustomGeometry() {
        float[] vertices = new float[(_segments + 2) * 2];
        vertices[0] = 0.0f; vertices[1] = 0.0f; 

        for (int i = 0; i <= _segments; i++)
        {
            float angle = (float)(i * 2.0 * Math.PI / _segments);
            vertices[(i + 1) * 2] = (float)(Math.Cos(angle) * _radius);
            vertices[(i + 1) * 2 + 1] = (float)(Math.Sin(angle) * _radius);
        }

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
    }

    protected override float[] GetVertices() => Array.Empty<float>();
}
