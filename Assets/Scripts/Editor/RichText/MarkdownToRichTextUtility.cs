using System.Text.RegularExpressions;

public static class MarkdownToRichTextUtility
{
    public static string Convert(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var result = markdown;
        
        result = Regex.Replace(result, @"#####\s*(.+)", "<b>$1</b>");
        result = Regex.Replace(result, @"####\s*(?!#)(.+)", "<size=26><b>$1</b></size>");
        result = Regex.Replace(result, @"###\s*(?!#)(.+)", "<size=30><b>$1</b></size>");

        result = Regex.Replace(result, @"\*\*(.+?)\*\*", "<b>$1</b>");
        result = Regex.Replace(result, @"\[(.+?)\]\((.+?)\)", "<color=#338a87><u><link=$2>$1</link></u></color>");
        result = result.Replace("  \n", "<br>");

        return result.Trim();
    }
}