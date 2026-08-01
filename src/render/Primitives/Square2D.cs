namespace Unminal.Render.Primitive._2D;

[SupportedOSPlatform("windows")]
public class Square : Primitive2D {
    public Square(Vector2 position, Vector2 scale, Vector4 color, float rotation) {
        Position = position;
        Scale = scale;
        Rotation = rotation;
        Color = color;
        
        Pivot = new Vector2(scale.X / 2f, scale.Y / 2f);

        float[] vertices = {
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            0.0f, 1.0f
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

    public bool Contains(Vector2 point) {
        Vector2 localPoint = point - Position;

        if (Rotation != 0.0f) {
            Vector2 pivotPixels = new Vector2(Pivot.X * Scale.X, Pivot.Y * Scale.Y);
            localPoint -= pivotPixels;

            float angleRad = -MathHelper.DegreesToRadians(Rotation);
            float cos = MathF.Cos(angleRad);
            float sin = MathF.Sin(angleRad);

            localPoint = new Vector2(localPoint.X * cos - localPoint.Y * sin, localPoint.X * sin + localPoint.Y * cos);
        } 

        return localPoint.X >= 0.0f && localPoint.X <= Scale.X &&
               localPoint.Y >= 0.0f && localPoint.Y <= Scale.Y;
    }
}