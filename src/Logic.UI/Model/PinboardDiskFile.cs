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
  public class PinboardFile : ObservableObject
  {

  }

  public class PinboardDiskFile : PinboardFile
  {
    public string DateStr { get; private set; }

    public PinboardDiskFile(string appSettingsPath, string fileName)
    {
      _path = appSettingsPath;
      _fileName = fileName;
      _fileFullPath = Path.Combine(appSettingsPath, fileName);
      updateDateStr();
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
      int dayDifference = calculateDayDifference(writeTime, DateTime.Now);

      var dateStr = writeTime.ToString("dd.MM.yyyy");

      return $"{dateStr} ({dayDifference} days ago)";
    }

    private int calculateDayDifference(DateTime d1, DateTime d2)
    {
      TimeSpan span = d2.Subtract(d1);
      return (int)span.TotalDays;
    }

    string _path;
    string _fileName;
    string _fileFullPath;
  }

  public class PinboardMemoryFile : PinboardFile
  {
    public PinboardMemoryFile()
    {
          
    }

    public async Task<bool> Download(string downloadedFilePath,
                                     string jsonFileURL)
    {
      try
      {
        using var client = new HttpClient();
        using var s = await client.GetStreamAsync(jsonFileURL);
        using var fs = new FileStream(downloadedFilePath, FileMode.OpenOrCreate);
        await s.CopyToAsync(fs);
        return true;
      }
      catch
      {
        return false;
      }
    }

  }
}
