using System;

namespace Logic.UI.Pictures
{
  /// <summary>
  /// Determines the image format from the leading bytes of a file.
  ///
  /// The file name a browser supplies on drag'n drop is derived from the
  /// source URL and regularly does not match the actual content, so the
  /// format is taken from the bytes themselves.
  /// </summary>
  public static class ImageFormatDetector
  {
    /// <summary>
    /// Returns the matching file extension (including the leading dot)
    /// or null if the bytes are not a supported image.
    /// </summary>
    public static string GetExtension(byte[] bytes)
    {
      if (bytes is null || bytes.Length < 12)
      {
        return null;
      }

      if (StartsWith(bytes, 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A))
      {
        return ".png";
      }

      if (StartsWith(bytes, 0xFF, 0xD8, 0xFF))
      {
        return ".jpg";
      }

      if (StartsWith(bytes, 0x47, 0x49, 0x46, 0x38))
      {
        return ".gif";
      }

      if (StartsWith(bytes, 0x42, 0x4D))
      {
        return ".bmp";
      }

      // RIFF....WEBP
      if (StartsWith(bytes, 0x52, 0x49, 0x46, 0x46)
          && MatchesAt(bytes, 8, 0x57, 0x45, 0x42, 0x50))
      {
        return ".webp";
      }

      // ....ftypavif / ....ftypavis
      if (MatchesAt(bytes, 4, 0x66, 0x74, 0x79, 0x70)
          && MatchesAt(bytes, 8, 0x61, 0x76, 0x69))
      {
        return ".avif";
      }

      return null;
    }

    public static bool IsSupportedImage(byte[] bytes)
    {
      return GetExtension(bytes) is not null;
    }

    private static bool StartsWith(byte[] bytes, params byte[] signature)
    {
      return MatchesAt(bytes, 0, signature);
    }

    private static bool MatchesAt(byte[] bytes, int offset, params byte[] signature)
    {
      if (bytes.Length < offset + signature.Length)
      {
        return false;
      }

      for (var i = 0; i < signature.Length; i++)
      {
        if (bytes[offset + i] != signature[i])
        {
          return false;
        }
      }

      return true;
    }
  }
}
