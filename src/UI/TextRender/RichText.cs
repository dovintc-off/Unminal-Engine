namespace Unminal.UI.TextRender.RichText;

[SupportedOSPlatform("windows")]
public class RichTextSegment {
    public string? RawText {get; private set;}
    public Vector4 defaultColor;

    public RichTextSegment(Vector4 DefaultColor) {
        defaultColor = DefaultColor;
    }

    private List<TextPart> ParseColor(string text, Vector4 DefaultColor)  {
        var Parts = new List<TextPart>{};
        string parsedText = text;
        var (IsValidColor, LenColor) = Colors.IsValidHEX(DefaultColor); 
        if (!IsValidColor) DefaultColor = new Vector4(1, 1, 1, 1);
        
        while(!string.IsNullOrWhiteSpace(parsedText)) {
            int entry_color = parsedText.IndexOf("[#");
            int close_color = parsedText.IndexOf("]");
            if (entry_color == -1) return new List<TextPart> { new TextPart { Text = parsedText, TextColor = DefaultColor } };
            if (entry_color == -1 || close_color == -1 || close_color <= entry_color) break;
            if (entry_color > 0) {
                Parts.Add(new TextPart {Text = parsedText[..entry_color], TextColor = DefaultColor});
                parsedText = parsedText[entry_color..];
            } else {
                string color = parsedText[(entry_color+2)..close_color];
                parsedText = parsedText[(close_color+1)..];
                if (parsedText.IndexOf("[#") is int next_close_color && next_close_color != -1) {
                    string PartText = parsedText[..next_close_color]; 
                    parsedText = parsedText[next_close_color..]; 
                    Parts.Add(new TextPart {Text = PartText, TextColor = Colors.HEXtoVEC4(color)});
                } else {
                    Parts.Add(new TextPart {Text = parsedText, TextColor = Colors.HEXtoVEC4(color)});
                    break;
                }
            }
        }
        return Parts;
    } 

    public void Draw(Text renderer, string text, float x, float y, float scale, Matrix4 projection) {
        List<TextPart> Parts = ParseColor(text, defaultColor);
        float cur_x = x;
        if (Parts == null) return;
        foreach (TextPart part in Parts) {
            if (string.IsNullOrEmpty(part.Text)) continue;
            renderer.DrawString(part.Text, cur_x, y, scale, projection, part.TextColor);
            cur_x += renderer.MeasureWidth(part.Text, scale);
        }
    }

    private struct TextPart {
        public string Text;
        public Vector4 TextColor;
    }   
}