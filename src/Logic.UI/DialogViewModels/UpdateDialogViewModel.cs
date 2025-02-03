using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.UI.Model;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public partial class UpdateDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    [ObservableProperty] private bool? _dialogResult;

    [ObservableProperty] private PinboardDiskFile _currentFile;
    [ObservableProperty] private PinboardMemoryFile _newFile;
    [ObservableProperty] private bool _isDownloadInProgress;
    [ObservableProperty] private bool _hasDownloadSucceeded;
    [ObservableProperty] private bool _hasDownloadFailed;
    [ObservableProperty] private bool _hasURL;
    [ObservableProperty] private string _jSONFileURL;


    public UpdateDialogViewModel(IDialogService dialogService,
                                 IAppSettings appSettings,
                                 string appSettingsPath,
                                 string pinboardFileName)
    {
      _dialogService = dialogService;
      _appSettings = appSettings;

      JSONFileURL = appSettings.JSONFileURL;
      CurrentFile = new PinboardDiskFile(appSettingsPath, pinboardFileName);
      NewFile = new PinboardMemoryFile();
      HasURL = !string.IsNullOrEmpty(JSONFileURL);
    }

    private bool CanExecuteDownload()
    {
      return HasURL && !IsDownloadInProgress && !HasDownloadSucceeded;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteDownload))]
    private async Task Download()
    {
      HasDownloadFailed = false;
      IsDownloadInProgress = true;
      Mouse.OverrideCursor = Cursors.Wait;

      HasDownloadSucceeded = await NewFile.Download(_appSettings.JSONFileURL);

      Mouse.OverrideCursor = null;
      IsDownloadInProgress = false;
      HasDownloadFailed = !HasDownloadSucceeded;
    }

    private bool CanExecuteSave()
    {
      return HasDownloadSucceeded;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteSave))]
    private void Save()
    {
      if (CurrentFile.SaveContentsFromMemoryFile(NewFile))
      {
        DialogResult = true;
        _dialogService.Close(this);
      }
    }

    [RelayCommand]
    private void Cancel()
    {
      DialogResult = false;
      _dialogService.Close(this);
    }

    IDialogService _dialogService;
    IAppSettings _appSettings;
  }
}
