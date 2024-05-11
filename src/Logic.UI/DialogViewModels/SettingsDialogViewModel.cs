using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public bool? DialogResult => dialogResult;

    public ICommand CmdApply { get; }
    public ICommand CmdCancel { get; }
    public ICommand CmdDownloadJSON { get; }

    public  string JSONFileURL { get; set; }
    public string CurrentPinboardFileState { get; set; } 
    public  bool AskBeforeAppExit { get; set; }

    public SettingsDialogViewModel(IDialogService dialogService,
                                   IAppSettings settings,
                                   string appSettingsPath)
    {
      JSONFileURL = settings.JSONFileURL;
      AskBeforeAppExit = settings.AskBeforeAppExit;

      var pinboardFilePath = Path.Combine(appSettingsPath, PINBOARD_FILE_NAME);
      checkPinboardFileState(pinboardFilePath);

      CmdApply = new RelayCommand(() =>
      {
        settings.AskBeforeAppExit = AskBeforeAppExit;

        dialogResult = true;
        dialogService.Close(this);
      }, () => AskBeforeAppExit != settings.AskBeforeAppExit);

      CmdCancel = new RelayCommand(() =>
      {
        dialogResult = false;
        dialogService.Close(this);
      }, () => true);

      CmdDownloadJSON = new RelayCommand(() =>
      {
        // TODO: - download the JSON file to 'downloaded.json'
        //       - call checkPinboardFileState(downloadedFilePath);
        //         (to have the date updated as success signal)
        //       - if download fails, then set CurrentPinboardFileState to "Download failed"
        //       - On success only if "Apply" is clicked,
        //         - delete current pinboard file
        //         - rename 'downloaded.json' to current pinboard file
      }, () => true);
    }

    private void checkPinboardFileState(string pinboardFilePath)
    {
      if (!File.Exists(pinboardFilePath))
      {
        CurrentPinboardFileState = "None";
        return;
      }

      var writeTime = File.GetLastWriteTime(pinboardFilePath);
      int dayDifference = calculateDayDifference(writeTime, DateTime.Now);

      var dateStr = writeTime.ToString("dd.MM.yyyy");

      CurrentPinboardFileState = $"downloaded on {dateStr} ({dayDifference} days ago)";
    }

    private int calculateDayDifference(DateTime d1, DateTime d2)
    {
      TimeSpan span = d2.Subtract(d1);
      return (int)span.TotalDays;
    }

    private bool? dialogResult;
  }
}
