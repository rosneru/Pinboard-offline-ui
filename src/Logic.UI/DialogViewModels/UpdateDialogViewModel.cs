using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.Model;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public class UpdateDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    private const string PINBOARD_FILE_NAME = "pinboard_backup.json";

    public bool? DialogResult => _dialogResult;
    
    public ICommand CmdDownloadJSON { get; }
    public ICommand CmdClose { get; }
    
    public string PinboardFileDate { get; set; }

    public UpdateDialogViewModel(IDialogService dialogService,
                                 IAppSettings settings,
                                 string appSettingsPath)
    {
      CmdDownloadJSON = new RelayCommand(async () =>
      {
        var pinboardFilePath = Path.Combine(appSettingsPath, PINBOARD_FILE_NAME);
        var currentFileDateStr = getPinboardFileDate(pinboardFilePath);
        displayPinboardFileDate(currentFileDateStr, "");

        // download the JSON file to 'downloaded.json'
        var downloadedFilePath = Path.Combine(appSettingsPath, "download.json");
        _isDownloading = true;
        var didDownloadSucceed = await downloadFile(downloadedFilePath,
                                                    settings.JSONFileURL);
        _isDownloading = false;
        if (didDownloadSucceed)
        {
          PinboardFileDate = "Download failed.";
          return;
        }

        // Call checkPinboardFileState(downloadedFilePath) (to have the date updated as success signal)
        getPinboardFileDate(downloadedFilePath);
      }, () => !_isDownloading);
    }

    private void displayPinboardFileDate(string currentFileDate, string newFileDate)
    {
      if (string.IsNullOrEmpty(currentFileDate))
      {
        PinboardFileDate = $"None";
      }
      else
      {
        PinboardFileDate = $"File from download date {currentFileDate}";
      }
    }

    private bool _isDownloading = false;
    private async Task<bool> downloadFile(string downloadedFilePath,
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

    private string getPinboardFileDate(string pinboardFilePath)
    {
      if (!File.Exists(pinboardFilePath))
      {
        return "";
      }

      var writeTime = File.GetLastWriteTime(pinboardFilePath);
      int dayDifference = calculateDayDifference(writeTime, DateTime.Now);

      var dateStr = writeTime.ToString("dd.MM.yyyy");

      return $"{dateStr} ({dayDifference} days ago)";
    }

    private int calculateDayDifference(DateTime d1, DateTime d2)
    {
      TimeSpan span = d2.Subtract(d1);
      return (int)span.TotalDays;
    }

    private bool? _dialogResult;

  }
}
