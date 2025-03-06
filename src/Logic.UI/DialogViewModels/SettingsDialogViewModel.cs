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
using Logic.UI.Services;
using MvvmDialogs;

namespace Logic.UI.DialogViewModels
{
  public partial class SettingsDialogViewModel : ObservableObject, IModalDialogViewModel
  {
    [ObservableProperty] private bool? _dialogResult;

    [ObservableProperty] private string _jSONFileURL;
    [ObservableProperty] private bool _askBeforeAppExit;

    public SettingsDialogViewModel(ISettingsService settingsService,
                                   IDialogService dialogService)
    {
      _dialogService = dialogService;
      _settingsService = settingsService;

      JSONFileURL = _settingsService.AppSettings.JSONFileURL;
      AskBeforeAppExit = _settingsService.AppSettings.AskBeforeAppExit;
    }

    private bool CanExecuteApply()
    {
      return (AskBeforeAppExit != _settingsService.AppSettings.AskBeforeAppExit) ||
             (JSONFileURL != _settingsService.AppSettings.JSONFileURL);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteApply))]
    private void Apply()
    {
      _settingsService.AppSettings.JSONFileURL = JSONFileURL;
      _settingsService.AppSettings.AskBeforeAppExit = AskBeforeAppExit;

      DialogResult = true;
      _dialogService.Close(this);
    }

    [RelayCommand]
    private void Cancel()
    {
      DialogResult = false;
      _dialogService.Close(this);
    }

    ISettingsService _settingsService;
    IDialogService _dialogService;
  }
}
