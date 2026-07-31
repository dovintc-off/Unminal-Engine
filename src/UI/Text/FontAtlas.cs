// TextRender/FontAtlas.cs
namespace Unminal.UI.TextRender.FontAtlas;

using System.Collections.Concurrent;

[SupportedOSPlatform("windows")]
public class Atlas : IDisposable
{   
    public int TextureID {get; private set;}
    public int Width {get; private set;}
    public int Height {get; private set;}

    private ConcurrentDictionary<char, GlyphData> _glyphs = new ConcurrentDictionary<char, GlyphData>();

    private const string Charset = Engine.LanguageChars.EN + Engine.LanguageChars.RU + """0123456789 .,!?-_+*/|\=()[]{}<>:;"'@#$%^""";

    public Atlas(string fontPath, int fontSize, int atlasSize = 2048)
    {
        Width = atlasSize;
        Height = atlasSize;
        GenerateAtlas(fontPath, fontSize);
    }

    private void GenerateAtlas(string fontPath, int fontSize)
    {
        using var pfc = new System.Drawing.Text.PrivateFontCollection();
        pfc.AddFontFile(fontPath);
        var family = pfc.Families[0];

        using var bitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);

        // graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        graphics.Clear(Color.Transparent);

        var format = StringFormat.GenericTypographic;
        format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        float currentX = 2;
        float currentY = 2;
        float maxHeightInRow = 0;
        float padding = 4.0f;

        using var drawFont = new Font(family, fontSize);

        foreach (char c in Charset) {
            string s = c.ToString();
            
            SizeF size = graphics.MeasureString(s, drawFont, PointF.Empty, format);
            
            float wPixels = MathF.Ceiling(size.Width);
            float hPixels = MathF.Ceiling(size.Height);
            
            if (currentX + wPixels > Width)
            {
                currentX = 2;
                currentY += MathF.Ceiling(maxHeightInRow) + padding;
                maxHeightInRow = 0;
            }
            
            if (currentY + hPixels > Height)
            {
                Console.CreateLog(Console.LogType.WARNING, $"Font atlas overflow! Symbol: '{c}'");
                break;
            }

            graphics.DrawString(s, drawFont, Brushes.White, currentX, currentY, format);

            float u = currentX / Width;
            float v = currentY / Height;
            float w = wPixels / Width;
            float h = hPixels / Height;
            
            float advance = wPixels; 

            _glyphs.TryAdd(c, new GlyphData(c, u, v, w, h, 0, 0, advance));

            currentX += wPixels + padding;
            if (hPixels > maxHeightInRow) maxHeightInRow = hPixels;
        }

        string outputPath = "debug/generated_atlas.png";
        string? dirName = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        bitmap.Save(outputPath, ImageFormat.Png);
        Console.CreateLog(Console.LogType.INFO, $"Font atlas saved to: {outputPath}");

        LoadTextureToOpenGL(bitmap);
    }

    private void LoadTextureToOpenGL(Bitmap bitmap)
    {
        TextureID = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, TextureID);

        var data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            bitmap.Width,
            bitmap.Height,
            0,
            PixelFormat.Bgra,
            PixelType.UnsignedByte,
            data.Scan0);

        bitmap.UnlockBits(data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public bool TryGetGlyph(char c, out GlyphData glyph)
    {
        return _glyphs.TryGetValue(c, out glyph);
    }

    public void Dispose()
    {
        if (TextureID != 0)
        {
            GL.DeleteTexture(TextureID);
            TextureID = 0;
        }
    }
}
