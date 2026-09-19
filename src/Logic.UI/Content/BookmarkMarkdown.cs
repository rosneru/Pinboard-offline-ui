using System.Linq;

namespace Logic.UI.Content
{
  /// <summary>
  /// Rearranges the markdown of a bookmark before it is rendered.
  /// </summary>
  public static class BookmarkMarkdown
  {
    /// <summary>
    /// Returns the markdown with the picture inserted.
    ///
    /// A leading quote block introduces the bookmark and keeps its place on
    /// top, as does the italic byline that may follow it. Everything else
    /// moves below the picture.
    /// </summary>
    public static string InsertPicture(string markdown, string pictureUrl)
    {
      var picture = $"![]({pictureUrl}){{.bookmark-picture}}";
      var lines = markdown.Split('\n');
      var introLineCount = CountIntroLines(lines);

      if (introLineCount == 0)
      {
        return $"{picture}\n\n{markdown}";
      }

      var intro = string.Join("\n", lines.Take(introLineCount));
      var rest = string.Join("\n", lines.Skip(introLineCount)).TrimStart('\n');

      return $"{intro}\n\n{picture}\n\n{rest}";
    }

    private static int CountIntroLines(string[] lines)
    {
      if (lines.Length == 0 || !lines[0].StartsWith(">"))
      {
        return 0;
      }

      var behindQuote = SkipParagraph(lines, 0);
      var italicStart = SkipBlankLines(lines, behindQuote);

      return IsItalicParagraph(lines, italicStart)
        ? SkipParagraph(lines, italicStart)
        : behindQuote;
    }

    private static int SkipParagraph(string[] lines, int start)
    {
      var index = start;

      while (index < lines.Length && lines[index].Trim().Length > 0)
      {
        index++;
      }

      return index;
    }

    private static int SkipBlankLines(string[] lines, int start)
    {
      var index = start;

      while (index < lines.Length && lines[index].Trim().Length == 0)
      {
        index++;
      }

      return index;
    }

    private static bool IsItalicParagraph(string[] lines, int start)
    {
      if (start >= lines.Length
          || !lines[start].StartsWith("*")
          || lines[start].StartsWith("**"))
      {
        return false;
      }

      // The emphasis may span several lines, as in "*Von Jane Doe\n05.03.2026*".
      var lastLine = lines[SkipParagraph(lines, start) - 1];

      return lastLine.TrimEnd().EndsWith("*");
    }
  }
}
