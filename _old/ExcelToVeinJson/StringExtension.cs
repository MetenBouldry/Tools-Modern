﻿using System.Text;

namespace TFG_1._20.x_ExcelToVeinJson;

public static class StringExtension
{
    public static string ToSnakeCase(this string text)
    {
        if(text == null) {
            throw new ArgumentNullException(nameof(text));
        }
        if(text.Length < 2) {
            return text;
        }
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for(var i = 1; i < text.Length; ++i) {
            var c = text[i];
            if(char.IsUpper(c)) {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            } else {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}