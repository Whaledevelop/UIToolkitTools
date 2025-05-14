using System.Text.RegularExpressions;

public static class MarkdownToRichTextUtility
{
    public static string Convert(string markdown, string linkColor = null, string headerColor = null, string spanColor = null)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var result = markdown;

        result = Regex.Replace(result, @"<h6\s+class=""colored_header"">(.+?)</h6>", $"<size=26><color={headerColor}><b>$1</b></color></size><br>");
        result = Regex.Replace(result, @"<span\s+class=""colored"">(.+?)</span>", $"<color={spanColor}>$1</color>");

        result = Regex.Replace(result, @"#####\s*(.+)", "<b>$1</b>");
        result = Regex.Replace(result, @"####\s*(?!#)(.+)", "<size=26><b>$1</b></size>");
        result = Regex.Replace(result, @"###\s*(?!#)(.+)", "<size=30><b>$1</b></size>");

        result = Regex.Replace(result, @"\*\*(.+?)\*\*", "<b>$1</b>");
        
        result = Regex.Replace(result, @"\[(.+?)\]\((.+?)\)", $"<color={linkColor}><u><link=$2>$1</link></u></color>");

        result = result.Replace("  \n", "<br>");

        return result.Trim();
    }
}