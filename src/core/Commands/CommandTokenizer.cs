// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
namespace Unminal.Core.Commands.Tokenizer;

public static class CommandTokenizer {
    public static List<string> GetArgs(string input) {
        var tokens = new List<string>();
        var buffer = new System.Text.StringBuilder();
        bool inQuotes = false;

        foreach (char c in input) {
            if (c == '"') { inQuotes = !inQuotes; } 
            else if (c == ' ' && !inQuotes) {
                if (buffer.Length > 0) {
                    tokens.Add(buffer.ToString());
                    buffer.Clear();
                }
            } else { buffer.Append(c); }
        }
        if (buffer.Length > 0) tokens.Add(buffer.ToString());
        
        return tokens;
    }
}
