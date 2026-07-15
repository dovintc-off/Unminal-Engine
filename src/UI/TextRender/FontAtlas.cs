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

    private const string Charset = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,!?-+*/|\=()[]{}<>@#$%^АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

    public Atlas(string fontPath, int fontSize, int atlasSize = 512)
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

        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        graphics.Clear(Color.Transparent);

        var format = StringFormat.GenericTypographic;
        format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        float currentX = 2;
        float currentY = 2;
        float maxHeightInRow = 0;
        float padding = 4.0f;

        using var drawFont = new System.Drawing.Font(family, fontSize);

        foreach (char c in Charset)
        {
            string s = c.ToString();
            
            SizeF size = graphics.MeasureString(s, drawFont, PointF.Empty, format);
            
            if (currentX + size.Width > Width)
            {
                currentX = 2;
                currentY += maxHeightInRow + padding;
                maxHeightInRow = 0;
            }
            
            if (currentY + size.Height > Height)
            {
                GameConsole.GameConsole.Instance?.Log("Warning", $"Font atlas overflow! Symbol: '{c}'");
                break;
            }

            graphics.DrawString(s, drawFont, Brushes.White, currentX, currentY, format);

            float u = currentX / Width;
            float v = currentY / Height;
            float w = size.Width / Width;
            float h = size.Height / Height;
            
            float advance = size.Width; 

            _glyphs.TryAdd(c, new GlyphData(c, u, v, w, h, 0, 0, advance));

            currentX += size.Width + padding;
            if (size.Height > maxHeightInRow) maxHeightInRow = size.Height;
        }

        string outputPath = "debug/generated_atlas.png";
        string? dirName = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        bitmap.Save(outputPath, ImageFormat.Png);
        GameConsole.GameConsole.Instance?.Log("Debug", $"Font atlas saved to: {outputPath}");

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
