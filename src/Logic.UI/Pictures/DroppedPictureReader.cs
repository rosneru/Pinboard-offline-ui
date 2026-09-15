using System;
using System.Diagnostics;
using System.IO;
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

      // TODO As a last resort the picture URL could be taken from the
      // UniformResourceLocatorW format and downloaded directly. That is
      // only needed for sources where FileContents stays empty, for
      // example pictures behind a blob: URL.
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
