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
    [ObservableProperty] private IBookmarkService _bookmarkService;
    [ObservableProperty] private Bookmark _selectedBookmark;
    [ObservableProperty] private string _selectedBookmarkHtml;
    [ObservableProperty] private string _statusBarText;
    
    [ObservableProperty] private string _bookmarkFileDateInfo;
    [ObservableProperty] private string _latestBookmarkDateInfo;

    partial void OnSelectedBookmarkChanged(Bookmark oldValue, Bookmark newValue)
    {
      if (newValue is null)
      {
        return;
      }

      var bookmarkContent = newValue!.Extended;

      // Translate the '==' around '==Highlighted==' passages with into
      // '<mark>Highlighted</mark>'.
      //
      // Because this is the syntax, `Markdig` understands and *does*
      // render highlighted.
      //
      // Regex explained:
      //  - '(?<!\=)' ensures there's no '=' before start of match
      //    (Negative Lookbehind)
      //  - '\={2}' matches exactly two '='
      //  - '(?!\=)' ensures there's no '=' after end of match (Negative
      //    Lookahead)
      int i = 0;
      var bookmarkContendTranslated = new Regex(@"(?<!\=)\={2}(?!\=)")
        .Replace(bookmarkContent, m => i++ % 2 == 0 ? "<mark>" : "</mark>");
      SelectedBookmarkHtml = Markdown.ToHtml(bookmarkContendTranslated);
    }


    public MainViewModel(IDialogService dialogService, 
                         IUiService uiService,
                         ISettingsService settingsService,
                         IBookmarkService bookmarkService,
                         SettingsDialogViewModel settingsDialogViewModel,
                         UpdateDialogViewModel updateDialogViewModel,
                         FilterByTagsDialogViewModel filterByTagsDialogViewModel)
    {
      _dialogService = dialogService;
      UiService = uiService;
      _settingsService = settingsService;
      BookmarkService = bookmarkService;
      _settingsDialogViewModel = settingsDialogViewModel;
      _updateDialogViewModel = updateDialogViewModel;
      _filterByTagsDialogViewModel = filterByTagsDialogViewModel;


      BookmarkService.FilteredBookmarksChanged += (sender, e) =>
      {
        StatusBarText = $"Displaying {BookmarkService.FilteredBookmarks.Count} " +
                        $"of {BookmarkService.AllBookmarks.Count} loaded bookmarks.";
      };
    }

    private bool CanExecuteOpenSelectedBookmarkUrl()
    {
      if(SelectedBookmark is null)
      {
        return false;
      }

      return !string.IsNullOrEmpty(SelectedBookmark.HRef);
    }

    [RelayCommand(CanExecute = nameof(CanExecuteOpenSelectedBookmarkUrl))]
    private void OpenSelectedBookmarkUrl(CancelEventArgs cancelEventArgs)
    {
      Process.Start(new ProcessStartInfo
      {
        FileName = "opera",
        Arguments = SelectedBookmark.HRef,
        UseShellExecute = true
      });
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
      Mouse.OverrideCursor = Cursors.Wait;
      StatusBarText = $"Loading bookmarks..";

      await BookmarkService.InitializeAsync(_settingsService.AppSettingsPath,
                                            _settingsService.PinboardFileName);

      BookmarkFileDateInfo = BookmarkService.BookmarkFileDateInfo;
      LatestBookmarkDateInfo = BookmarkService.LatestBookmarkDateInfo;

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
