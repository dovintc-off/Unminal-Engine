namespace Unminal.Render.PrimitiveFigures._2D;

public class Square : Primitive2D
{
    public Square() : base() { }

    public Square(Vector2 position, Vector2 scale, Vector3 color, float alpha, float rotation) 
        : base(position, scale, color, alpha, rotation)
    {}
    
    public Square(Vector2 position, Vector2 scale) 
        : base(position, scale, Vector3.One, 1.0f, 0.0f)
    {}

    protected override float[] GetVertices()
    {
        return new float[]
        {
            -0.5f, -0.5f,
             0.5f, -0.5f,
             0.5f,  0.5f,
            -0.5f,  0.5f
        };
    }
}