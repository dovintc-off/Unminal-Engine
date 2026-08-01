// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
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
