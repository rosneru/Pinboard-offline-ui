using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Logic.UI.Pictures
{
  /// <summary>
  /// Reads the virtual files a browser offers during a drag'n drop
  /// operation.
  ///
  /// When an image is dragged out of a browser there is no FileDrop
  /// format. Instead the browser announces a file it has not downloaded
  /// yet, using the two shell formats FileGroupDescriptorW (the file
  /// names) and FileContents (the bytes). The latter cannot be read
  /// through the WPF DataObject because an IStream with an item index
  /// sits behind it, so the COM IDataObject is used directly.
  /// </summary>
  public static class VirtualFileReader
  {
    public const string FileGroupDescriptorFormat = "FileGroupDescriptorW";
    public const string FileContentsFormat = "FileContents";

    /// <summary>
    /// Returns true if the dropped data announces virtual files.
    /// </summary>
    public static bool IsPresent(IDataObject data)
    {
      return data is not null
             && data.GetDataPresent(FileGroupDescriptorFormat)
             && data.GetDataPresent(FileContentsFormat);
    }

    /// <summary>
    /// Returns the announced file names. They carry the original file
    /// extension, but as they are derived from the source URL they are a
    /// hint only and should not be trusted - see ImageFormatDetector.
    /// </summary>
    public static string[] GetFileNames(IDataObject data)
    {
      if (data?.GetData(FileGroupDescriptorFormat) is not MemoryStream stream)
      {
        return Array.Empty<string>();
      }

      using var reader = new BinaryReader(stream);

      // FILEGROUPDESCRIPTORW starts with the item count, followed by one
      // FILEDESCRIPTORW per item.
      var count = reader.ReadInt32();
      var names = new string[count];

      for (var i = 0; i < count; i++)
      {
        var descriptor = reader.ReadBytes(FileDescriptorSize);

        var name = Encoding.Unicode.GetString(
          descriptor,
          FileNameOffset,
          FileDescriptorSize - FileNameOffset);

        var end = name.IndexOf('\0');
        names[i] = end >= 0 ? name.Substring(0, end) : name;
      }

      return names;
    }

    /// <summary>
    /// Returns the bytes of the announced file at the given index, or
    /// null if they cannot be read.
    ///
    /// Note that this blocks: the browser starts downloading the file
    /// only while the stream is being read. It must still be called
    /// synchronously from the Drop handler because the dropped data
    /// object is released once the handler returns.
    /// </summary>
    public static byte[] GetFileContents(IDataObject data, int index)
    {
      if (data is not ComTypes.IDataObject comData)
      {
        return null;
      }

      var format = new ComTypes.FORMATETC
      {
        cfFormat = (short)DataFormats.GetDataFormat(FileContentsFormat).Id,
        dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
        lindex = index,
        ptd = IntPtr.Zero,
        tymed = ComTypes.TYMED.TYMED_ISTREAM | ComTypes.TYMED.TYMED_HGLOBAL
      };

      ComTypes.STGMEDIUM medium;

      try
      {
        comData.GetData(ref format, out medium);
      }
      catch (COMException)
      {
        return null;
      }

      try
      {
        return medium.tymed switch
        {
          ComTypes.TYMED.TYMED_ISTREAM => ReadStream(medium.unionmember),
          ComTypes.TYMED.TYMED_HGLOBAL => ReadHGlobal(medium.unionmember),
          _ => null
        };
      }
      finally
      {
        ReleaseStgMedium(ref medium);
      }
    }

    private static byte[] ReadStream(IntPtr handle)
    {
      if (handle == IntPtr.Zero)
      {
        return null;
      }

      var stream = (ComTypes.IStream)Marshal.GetObjectForIUnknown(handle);

      try
      {
        // Chromium reports a size of zero while its download is still
        // running, so read until the stream is exhausted instead of
        // trusting STATSTG.cbSize.
        using var result = new MemoryStream();

        var buffer = new byte[ReadBufferSize];
        var readCount = Marshal.AllocCoTaskMem(sizeof(int));

        try
        {
          while (true)
          {
            stream.Read(buffer, buffer.Length, readCount);
            var count = Marshal.ReadInt32(readCount);

            if (count <= 0)
            {
              break;
            }

            result.Write(buffer, 0, count);
          }
        }
        finally
        {
          Marshal.FreeCoTaskMem(readCount);
        }

        return result.Length > 0 ? result.ToArray() : null;
      }
      finally
      {
        Marshal.ReleaseComObject(stream);
      }
    }

    private static byte[] ReadHGlobal(IntPtr handle)
    {
      if (handle == IntPtr.Zero)
      {
        return null;
      }

      var pointer = GlobalLock(handle);

      if (pointer == IntPtr.Zero)
      {
        return null;
      }

      try
      {
        var size = (int)GlobalSize(handle);

        if (size <= 0)
        {
          return null;
        }

        var bytes = new byte[size];
        Marshal.Copy(pointer, bytes, 0, size);
        return bytes;
      }
      finally
      {
        GlobalUnlock(handle);
      }
    }

    // Layout of FILEDESCRIPTORW: the struct is 592 bytes long and its
    // cFileName member sits at offset 72, holding MAX_PATH UTF-16 chars.
    private const int FileDescriptorSize = 592;
    private const int FileNameOffset = 72;

    private const int ReadBufferSize = 64 * 1024;

    [DllImport("ole32.dll")]
    private static extern void ReleaseStgMedium(ref ComTypes.STGMEDIUM medium);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GlobalLock(IntPtr handle);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalUnlock(IntPtr handle);

    [DllImport("kernel32.dll")]
    private static extern UIntPtr GlobalSize(IntPtr handle);
  }
}
