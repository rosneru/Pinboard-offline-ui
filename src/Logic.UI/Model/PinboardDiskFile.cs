using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Logic.UI.Model
{
  public partial class PinboardFile : ObservableObject
  {

  }

  public partial class PinboardDiskFile : PinboardFile
  {
    [ObservableProperty] private string _dateStr;

    public PinboardDiskFile(string appSettingsPath, string fileName)
    {
      _path = appSettingsPath;
      _fileName = fileName;
      _fileFullPath = Path.Combine(appSettingsPath, fileName);
      updateDateStr();
    }

    public bool SaveContentsFromMemoryFile(PinboardMemoryFile memoryFile)
    {
      try
      {
        using(var fs = new FileStream(_fileFullPath, FileMode.Create, FileAccess.Write))
        {
          memoryFile.Stream.CopyTo(fs);
        }

        return true;
      }
      catch(Exception)
      {
        return false;
      }
    }

    private void updateDateStr()
    {
      string currentFileDate = getPinboardFileDate();
      if (string.IsNullOrEmpty(currentFileDate))
      {
        DateStr = "None";
      }
      else
      {
        DateStr = currentFileDate;
      }
    }

    private string getPinboardFileDate()
    {
      if (!File.Exists(_fileFullPath))
      {
        return "";
      }

      var writeTime = File.GetLastWriteTime(_fileFullPath);
      int dayDifference = (int)DateTime.Now.Subtract(writeTime).TotalDays;
      var dateStr = writeTime.ToString("dd.MM.yyyy");

      return $"{dateStr} ({dayDifference} days ago)";
    }

    string _path;
    string _fileName;
    string _fileFullPath;
  }

  public class PinboardMemoryFile : PinboardFile, IDisposable
  {
    public Stream Stream { get; private set; } = null;

    public async Task<bool> Download(string jsonFileURL)
    {
      try
      {
        using var client = new HttpClient();
        Stream = await client.GetStreamAsync(jsonFileURL);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public void Dispose()
    {
      Stream = null;
    }
  }

}
