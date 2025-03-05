using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Config.Net;
using Logic.UI.DialogViewModels;
using Logic.UI.Model;
using Logic.UI.Services;
using Markdig;
using Markdig.Parsers;
using MvvmDialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Logic.UI.ViewModels
{
  public partial class MainViewModel : ObservableObject
  {
    [ObservableProperty] private UiService _uiService;
    [ObservableProperty] private BookmarksListViewModel _bookmarksListViewModel;


    public MainViewModel(IDialogService dialogService, ISettingsService settingsService)
    {
      _dialogService = dialogService;
      _settingsService = settingsService;


      UiService = new UiService();
      BookmarksListViewModel = new(UiService, settingsService);
    }

    [RelayCommand]
    void OpenSettings()
    {
      var openDialog = new SettingsDialogViewModel(_settingsService,
                                                   _dialogService);
      var success = _dialogService.ShowDialog(this, openDialog);
      if (success == true)
      {
        // Open the device e.g. by opening openDialog.Id from database
        // TODO Load content from JSON
      }
    }

    [RelayCommand]
    async Task OpenUpdate()
    {
      var settingsDialog = new UpdateDialogViewModel(_dialogService,
                                                     _settingsService);
      var success = _dialogService.ShowDialog(this, settingsDialog);
      if (success == true)
      {
        await Loaded();
      }
    }

    [RelayCommand]
    async Task Loaded()
    {
      await BookmarksListViewModel.Load();
    }

    private bool CanExecuteExit()
    {
      // Not allowed if shutdown is already in progress.
      return !_isAlreadyShutdown;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteExit))]
    private void Exit(CancelEventArgs cancelEventArgs)
    {
      if (cancelEventArgs != null)
      {
        // The command was probably triggered by a WindowCLose event. We
        // cancel that WindowsClose because shutdown is done manually
        // depending on the MessageBox result below.
        cancelEventArgs.Cancel = true;
      }

      MessageBoxResult result = MessageBoxResult.Yes;
      if (_appSettings.AskBeforeAppExit)
      {
        result = _dialogService
          .ShowMessageBox(this,
                          "Do you really want to quit the application?",
                          "Really exit?",
                          MessageBoxButton.YesNo,
                          MessageBoxImage.Warning);
      }

      if (result == MessageBoxResult.Yes)
      {
        // Shutdown() will trigger another invocation of this command.
        // So we mark that the shutdown process has already started to
        // block the re-invocation.
        _isAlreadyShutdown = true;
        Application.Current.Shutdown();
      }
    }


    IDialogService _dialogService;
    ISettingsService _settingsService;
    bool _isAlreadyShutdown = false;
    private readonly string _appSettingsPath;
    private readonly IAppSettings _appSettings;
  }
}
