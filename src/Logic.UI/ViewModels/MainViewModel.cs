using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Config.Net;
using Logic.UI.DialogViewModels;
using Logic.UI.Model;
using Logic.UI.Tools;
using Markdig;
using MvvmDialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Logic.UI.ViewModels
{
  public partial class MainViewModel : ObservableObject
  {
    private const string PINBOARD_FILE_NAME = "pinboard_backup.json";

    [ObservableProperty] private UITools _uiTools;
    [ObservableProperty] private List<Bookmark> bookmarks = [];
    [ObservableProperty] private Bookmark _selectedBookmark;
    [ObservableProperty] private string _selectedBookmarkHtml = "<!DOCTYPE html>\r\n<html>\r\n    <head>\r\n        <title>Example</title>\r\n    </head>\r\n    <body>\r\n        <p>This is an example of a simple HTML page with one paragraph.</p>\r\n    </body>\r\n</html>";

    partial void OnSelectedBookmarkChanged(Bookmark oldValue, Bookmark newValue)
    {
      SelectedBookmarkHtml = Markdown.ToHtml(newValue.Extended);
    }
    public MainViewModel(IDialogService dialogService)
    {
      var appDataRoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var developerName = "tysw";
      var appName = "Pinboard-offline-ui";

      _appSettingsPath = Path.Combine(appDataRoamingPath, developerName, appName);

      if (!Directory.Exists(_appSettingsPath))
      {
        Directory.CreateDirectory(_appSettingsPath);
      }

      var appSettingsFile = Path.Combine(_appSettingsPath, "settings.json");
      _appSettings = new ConfigurationBuilder<IAppSettings>()
        .UseJsonFile(appSettingsFile)
        .Build();

      UiTools = new UITools(dialogService);
    }

    [RelayCommand]
    void OpenSettings()
    {
      var openDialog = new SettingsDialogViewModel(UiTools.DialogService,
                                                   _appSettings,
                                                   _appSettingsPath);
      var success = UiTools.DialogService.ShowDialog(this, openDialog);
      if (success == true)
      {
        // Open the device e.g. by opening openDialog.Id from database
        // TODO Load content from JSON
      }
    }

    [RelayCommand]
    async Task OpenUpdate()
    {
      var settingsDialog = new UpdateDialogViewModel(UiTools.DialogService,
                                                    _appSettings,
                                                    _appSettingsPath,
                                                    PINBOARD_FILE_NAME);
      var success = UiTools.DialogService.ShowDialog(this, settingsDialog);
      if (success == true)
      {
        await Loaded();
      }
    }

    [RelayCommand]
    async Task Loaded()
    {
      Mouse.OverrideCursor = Cursors.Wait;
      UiTools.StatusBar.StatusText = $"Loading bookmarks..";
      Bookmarks = await Task.Run(() =>
      {
        string bookmarkFilePath = Path.Combine(_appSettingsPath, PINBOARD_FILE_NAME);
        string json = File.ReadAllText(bookmarkFilePath);
        return JsonConvert.DeserializeObject<List<Bookmark>>(json);
      });
      UiTools.StatusBar.StatusText = $"{Bookmarks.Count} bookmarks loaded.";
      Mouse.OverrideCursor = null;
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
        result = UiTools
          .DialogService
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



    bool _isAlreadyShutdown = false;
    private readonly string _appSettingsPath;
    private readonly IAppSettings _appSettings;
  }
}
