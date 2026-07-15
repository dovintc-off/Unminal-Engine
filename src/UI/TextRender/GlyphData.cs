// TextRender/GlyphData.cs
namespace Unminal.UI.TextRender.Glyph;

public struct GlyphData
{
    public char Character;

    public float U, V;
    public float Width, Height;

    public float BearingX, BearingY;

    public float Advance;

    public GlyphData(char character, float u, float v, float w, float h, float bx, float by, float adv)
    {
        Character = character;
        U = u;
        V = v;
        Width = w;
        Height = h;
        BearingX = bx;
        BearingY = by;
        Advance = adv;
    }
}