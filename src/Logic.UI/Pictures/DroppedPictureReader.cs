using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Logic.UI.Model;

namespace Logic.UI.Pictures
{
  /// <summary>
  /// Extracts a picture out of the data of a drag'n drop operation.
  ///
  /// Decides which of the offered sources to use, in descending order of
  /// reliability, and hands the raw bytes to ImageFormatDetector to find
  /// out what was actually dropped.
  /// </summary>
  public static class DroppedPictureReader
  {
    /// <summary>
    /// Returns the first supported picture found in the dropped data, or
    /// null if there is none.
    ///
    /// Must be called synchronously from the Drop handler because the
    /// dropped data object is released once that handler returns. For a
    /// drag out of a browser this includes downloading the picture and
    /// therefore blocks - see VirtualFileReader.
    /// </summary>
    public static DroppedPicture Read(IDataObject data)
    {
      if (data is null)
      {
        return null;
      }

      // A drag from the Explorer carries the file itself..
      if (data.GetDataPresent(DataFormats.FileDrop)
          && data.GetData(DataFormats.FileDrop) is string[] filePaths)
      {
        foreach (var filePath in filePaths)
        {
          var picture = ReadFromFile(filePath);

          if (picture is not null)
          {
            return picture;
          }
        }
      }

      // ..while a drag from a browser only announces a file which is
      // downloaded on demand while its contents are being read.
      if (VirtualFileReader.IsPresent(data))
      {
        var picture = ReadFromVirtualFiles(data);

        if (picture is not null)
        {
          return picture;
        }
      }

      // Sites that suppress dragging the picture itself still let their
      // link or the address bar be dragged, which offers the URL only.
      var url = GetUrl(data);

      if (url is not null)
      {
        return ReadFromUrl(url);
      }

      Debug.WriteLine($"No picture in dropped formats: {string.Join(", ", data.GetFormats())}");
      return null;
    }

    private static DroppedPicture ReadFromFile(string filePath)
    {
      try
      {
        var bytes = File.ReadAllBytes(filePath);
        var extension = ImageFormatDetector.GetExtension(bytes);

        return extension is null
          ? null
          : new DroppedPicture(Path.GetFileName(filePath), extension, bytes);
      }
      catch (Exception exception) when (exception is IOException
                                                  or UnauthorizedAccessException)
      {
        Debug.WriteLine($"Cannot read dropped file '{filePath}': {exception.Message}");
        return null;
      }
    }

    /// <summary>
    /// Returns the http(s) URL offered by the drop, or null if there is none.
    /// </summary>
    private static string GetUrl(IDataObject data)
    {
      string[] formats =
      [
        "UniformResourceLocatorW",
        "UniformResourceLocator",
        DataFormats.UnicodeText,
        DataFormats.Text
      ];

      foreach (var format in formats)
      {
        if (!data.GetDataPresent(format))
        {
          continue;
        }

        var candidate = AsText(data.GetData(format), format)?.Trim();

        if (Uri.TryCreate(candidate, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
          return candidate;
        }
      }

      return null;
    }

    private static string AsText(object value, string format)
    {
      if (value is string text)
      {
        return text;
      }

      if (value is not MemoryStream stream)
      {
        return null;
      }

      // The URL formats carry raw, null terminated bytes.
      var encoding = format == "UniformResourceLocatorW"
        ? Encoding.Unicode
        : Encoding.ASCII;

      return encoding.GetString(stream.ToArray()).TrimEnd('\0');
    }

    private static DroppedPicture ReadFromUrl(string url)
    {
      try
      {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        // Task.Run keeps the await off the blocked UI thread.
        var bytes = Task.Run(() => client.GetByteArrayAsync(url)).GetAwaiter().GetResult();
        var extension = ImageFormatDetector.GetExtension(bytes);

        if (extension is null)
        {
          return null;
        }

        var fileName = Path.GetFileName(new Uri(url).LocalPath);

        return new DroppedPicture(
          string.IsNullOrEmpty(fileName)
            ? $"picture{extension}"
            : Path.ChangeExtension(fileName, extension),
          extension,
          bytes);
      }
      catch (Exception exception) when (exception is HttpRequestException
                                                  or TaskCanceledException
                                                  or UriFormatException)
      {
        Debug.WriteLine($"Cannot download dropped url '{url}': {exception.Message}");
        return null;
      }
    }

    private static DroppedPicture ReadFromVirtualFiles(IDataObject data)
    {
      var fileNames = VirtualFileReader.GetFileNames(data);

      for (var i = 0; i < fileNames.Length; i++)
      {
        var bytes = VirtualFileReader.GetFileContents(data, i);
        var extension = ImageFormatDetector.GetExtension(bytes);

        if (extension is not null)
        {
          // The announced name is derived from the source URL, so its
          // extension is corrected to match the actual contents.
          return new DroppedPicture(
            Path.ChangeExtension(fileNames[i], extension),
            extension,
            bytes);
        }
      }

      return null;
    }
  }
}
