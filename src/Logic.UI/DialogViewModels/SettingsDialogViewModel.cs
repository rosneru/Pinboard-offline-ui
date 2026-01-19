using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [ObservableProperty] private ThemeType _readerTheme;

    [ObservableProperty] private ObservableCollection<KeyValuePair<string, ThemeType>> _themeOptions;

    public SettingsDialogViewModel(ISettingsService settingsService,
                                   IDialogService dialogService)
    {
      _dialogService = dialogService;
      _settingsService = settingsService;

      JSONFileURL = _settingsService.AppSettings.JSONFileURL;
      AskBeforeAppExit = _settingsService.AppSettings.AskBeforeAppExit;
      ReaderTheme = _settingsService.AppSettings.ReaderTheme;

      ThemeOptions = new ObservableCollection<KeyValuePair<string, ThemeType>>
      {
        new KeyValuePair<string, ThemeType>("System", ThemeType.SYS),
        new KeyValuePair<string, ThemeType>("Gruvbox", ThemeType.GRUVBOX)
      };
    }

    private bool CanExecuteApply()
    {
      return (AskBeforeAppExit != _settingsService.AppSettings.AskBeforeAppExit) ||
             (ReaderTheme != _settingsService.AppSettings.ReaderTheme) ||
             (JSONFileURL != _settingsService.AppSettings.JSONFileURL);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteApply))]
    private void Apply()
    {
      _settingsService.AppSettings.JSONFileURL = JSONFileURL;
      _settingsService.AppSettings.AskBeforeAppExit = AskBeforeAppExit;
      _settingsService.AppSettings.ReaderTheme = ReaderTheme;

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
