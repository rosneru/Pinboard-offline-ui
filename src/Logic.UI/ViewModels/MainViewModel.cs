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
    [ObservableProperty] private IUiService _uiService;
    [ObservableProperty] private BookmarksListViewModel _bookmarksListViewModel;


    public MainViewModel(IDialogService dialogService, 
                         IUiService uiService,
                         ISettingsService settingsService,
                         BookmarksListViewModel bookmarksViewModel,
                         SettingsDialogViewModel settingsDialogViewModel,
                         UpdateDialogViewModel updateDialogViewModel,
                         FilterByTagsDialogViewModel filterByTagsDialogViewModel)
    {
      _dialogService = dialogService;
      UiService = uiService;
      _settingsService = settingsService;
      BookmarksListViewModel = bookmarksViewModel;
      _settingsDialogViewModel = settingsDialogViewModel;
      _updateDialogViewModel = updateDialogViewModel;
      _filterByTagsDialogViewModel = filterByTagsDialogViewModel;
    }

    [RelayCommand]
    void OpenSettings()
    {
      _dialogService.ShowDialog(this, _settingsDialogViewModel);
    }

    [RelayCommand]
    async Task OpenUpdate()
    {
      var success = _dialogService.ShowDialog(this, _updateDialogViewModel);
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
      if (_settingsService.AppSettings.AskBeforeAppExit)
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

    [RelayCommand]
    void OpenTagFilter()
    {
      var success = _dialogService.ShowDialog(this, _filterByTagsDialogViewModel);
      if (success == true)
      {
        // ??? await Filtered
      }
    }

    IDialogService _dialogService;
    ISettingsService _settingsService;
    SettingsDialogViewModel _settingsDialogViewModel;
    UpdateDialogViewModel _updateDialogViewModel;
    FilterByTagsDialogViewModel _filterByTagsDialogViewModel;

    bool _isAlreadyShutdown = false;
  }
}
