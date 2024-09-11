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
    
    public ICommand CmdDownload { get; }
    public ICommand CmdSave { get; }
    public ICommand CmdCancel { get; }

    public PinboardDiskFile CurrentFile { get; private set; }
    public PinboardMemoryFile NewFile { get; private set; }

    public bool HasDownloadSucceeded { get; private set; } = false;

    public bool HasDownloadFailed { get; private set; } = false;

    public string JSONFileURL { get; set; }


    public UpdateDialogViewModel(IDialogService dialogService,
                                 IAppSettings settings,
                                 string appSettingsPath)
    {
      JSONFileURL = settings.JSONFileURL;
      CurrentFile = new PinboardDiskFile(appSettingsPath, PINBOARD_FILE_NAME);
      NewFile = new PinboardMemoryFile();

      CmdDownload = new RelayCommand(async () =>
      {

        // download the JSON file to 'downloaded.json'
        var downloadedFilePath = Path.Combine(appSettingsPath, "download.json");
        HasDownloadSucceeded = await NewFile.Download(downloadedFilePath, settings.JSONFileURL);
        HasDownloadFailed = !HasDownloadSucceeded;

      }, () => !HasDownloadSucceeded);

      CmdSave= new RelayCommand(() =>
      {
        _dialogResult = true;
        dialogService.Close(this);
      }, () => HasDownloadSucceeded);

      CmdCancel = new RelayCommand(() =>
      {
        _dialogResult = false;
        dialogService.Close(this);
      }, () => true);
    }


    private bool? _dialogResult;

  }
}
