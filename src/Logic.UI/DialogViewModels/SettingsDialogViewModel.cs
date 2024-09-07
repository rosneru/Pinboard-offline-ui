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
  public class SettingsDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    private const string PINBOARD_FILE_NAME = "pinboard_backup.json";
    public bool? DialogResult => _dialogResult;

    public ICommand CmdApply { get; }
    public ICommand CmdCancel { get; }
    public ICommand CmdDownloadJSON { get; }

    public  string JSONFileURL { get; set; }
    public string PinboardFileDate { get; set; }
    public  bool AskBeforeAppExit { get; set; }

    public bool HasNewPinboardFileDownloaded { get; set; } = false;

    public SettingsDialogViewModel(IDialogService dialogService,
                                   IAppSettings settings,
                                   string appSettingsPath)
    {
      JSONFileURL = settings.JSONFileURL;
      AskBeforeAppExit = settings.AskBeforeAppExit;

      var pinboardFilePath = Path.Combine(appSettingsPath, PINBOARD_FILE_NAME);
      var currentFileDateStr = getPinboardFileDate(pinboardFilePath);
      displayPinboardFileDate(currentFileDateStr);

      CmdApply = new RelayCommand(() =>
      {
        settings.AskBeforeAppExit = AskBeforeAppExit;

        // TODO: (if a new pinboard file was downloaded)
        //       - if download fails, then set CurrentPinboardFileState to "Download failed"
        //       - On success only if "Apply" is clicked,
        //         - delete current pinboard file
        //         - rename 'downloaded.json' to current pinboard file

        _dialogResult = true;
        dialogService.Close(this);
      }, () => (AskBeforeAppExit != settings.AskBeforeAppExit) || HasNewPinboardFileDownloaded);

      CmdCancel = new RelayCommand(() =>
      {
        _dialogResult = false;
        dialogService.Close(this);
      }, () => true);

      CmdDownloadJSON = new RelayCommand(async () =>
      {

        // download the JSON file to 'downloaded.json'
        var downloadedFilePath = Path.Combine(appSettingsPath, "download.json");
        HasNewPinboardFileDownloaded = false;
        _isDownloading = true;
        var didDownloadSucceed = await downloadFile(downloadedFilePath);
        _isDownloading = false;
        if (didDownloadSucceed)
        {
          PinboardFileDate = "Download failed.";
          return;
        }

        // Call checkPinboardFileState(downloadedFilePath) (to have the date updated as success signal)
        getPinboardFileDate(downloadedFilePath);
        HasNewPinboardFileDownloaded = true;
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
    private async Task<bool> downloadFile(string downloadedFilePath)
    {
      try
      {
        using var client = new HttpClient();
        using var s = await client.GetStreamAsync(JSONFileURL);
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
