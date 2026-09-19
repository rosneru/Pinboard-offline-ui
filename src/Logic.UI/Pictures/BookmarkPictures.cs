using System.IO;

namespace Logic.UI.Pictures
{
  /// <summary>
  /// Finds the picture that belongs to a bookmark and builds the URL the
  /// WebView2 loads it from.
  /// </summary>
  public static class BookmarkPictures
  {
    /// <summary>
    /// The host name the picture directory is mapped to.
    ///
    /// A picture cannot be referenced as file:// because content set via
    /// NavigateToString has no origin of its own and is therefore not
    /// allowed to read local files.
    /// </summary>
    public const string VirtualHost = "pictures.local";

    /// <summary>
    /// Returns the picture URL for the given bookmark hash, or null if no
    /// picture was saved for it.
    /// </summary>
    public static string FindUrl(string pictureDirectory, string bookmarkHash)
    {
      if (string.IsNullOrEmpty(pictureDirectory)
          || !Directory.Exists(pictureDirectory))
      {
        return null;
      }

      var pictures = Directory.GetFiles(pictureDirectory, bookmarkHash + ".*");

      return pictures.Length == 0
        ? null
        : $"https://{VirtualHost}/{Path.GetFileName(pictures[0])}";
    }
  }
}
