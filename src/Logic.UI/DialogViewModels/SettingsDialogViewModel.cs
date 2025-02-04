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
  public partial class SettingsDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    [ObservableProperty] private bool? _dialogResult;

    [ObservableProperty] private string _jSONFileURL;
    [ObservableProperty] private bool _askBeforeAppExit;

    public SettingsDialogViewModel(IDialogService dialogService,
                                   IAppSettings appSettings,
                                   string appSettingsPath)
    {
      _dialogService = dialogService;
      _appSettings = appSettings;

      JSONFileURL = appSettings.JSONFileURL;
      AskBeforeAppExit = appSettings.AskBeforeAppExit;
    }

    private bool CanExecuteApply()
    {
      return (AskBeforeAppExit != _appSettings.AskBeforeAppExit) ||
             (JSONFileURL != _appSettings.JSONFileURL);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteApply))]
    private void Apply()
    {
      _appSettings.JSONFileURL = JSONFileURL;
      _appSettings.AskBeforeAppExit = AskBeforeAppExit;

      DialogResult = true;
      _dialogService.Close(this);
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
