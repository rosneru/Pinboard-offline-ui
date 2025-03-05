using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Logic.UI.Model;
using Logic.UI.Services;
using Markdig;
using Newtonsoft.Json;

namespace Logic.UI.ViewModels
{
  public partial class BookmarksListViewModel : ObservableObject
  {
    [ObservableProperty] private List<Bookmark> bookmarks = [];
    [ObservableProperty] private Bookmark _selectedBookmark;
    [ObservableProperty] private string _selectedBookmarkHtml;

    partial void OnSelectedBookmarkChanged(Bookmark oldValue, Bookmark newValue)
    {
      if (newValue is null)
      {
        return;
      }

      var bookmarkContent = newValue!.Extended;

      // Translate the '==' around '==Highlighted==' passages with
      // into  '<mark>Highlighted</mark>'. Because this is the syntax,
      // `Markdig` understands and *does* render highlighted.
      //
      // Regex explained:
      //  - '(?<!\=)' ensures there's no '=' before start of match (Negative Lookbehind)
      //  - '\={2}' matches exactly two '='
      //  - '(?!\=)' ensures there's no '=' after end of match (Negative Lookahead)
      int i = 0;
      var bookmarkContendTranslated = new Regex(@"(?<!\=)\={2}(?!\=)")
        .Replace(bookmarkContent, m => i++ % 2 == 0 ? "<mark>" : "</mark>");
      SelectedBookmarkHtml = Markdown.ToHtml(bookmarkContendTranslated);
    }




    public BookmarksListViewModel(UiService uiService, ISettingsService settingsService)
    {
      _uiService = uiService;
      _appSettingsPath = settingsService.AppSettingsPath;
      _pinboardFileName = settingsService.PinboardFileName;
    }




    public async Task Load()
    {
      Mouse.OverrideCursor = Cursors.Wait;

      _uiService.StatusBar.StatusText = $"Loading bookmarks..";
      var bookmarks = await loadBookmarks();
      _uiService.StatusBar.StatusText = $"Processing bookmarks..";
      Bookmarks = await splitBookmarksTags(bookmarks);
      _uiService.StatusBar.StatusText = $"{Bookmarks.Count} bookmarks loaded.";

      Mouse.OverrideCursor = null;
    }

    private Task<List<Bookmark>> loadBookmarks()
    {
      return Task.Run(() =>
      {
        string bookmarkFilePath = Path.Combine(_appSettingsPath, _pinboardFileName);
        string json = File.ReadAllText(bookmarkFilePath);
        return JsonConvert.DeserializeObject<List<Bookmark>>(json);
      });
    }

    private Task<List<Bookmark>> splitBookmarksTags(List<Bookmark> bookmarks)
    {
      return Task.Run(() =>
      {
        foreach (var bookmark in bookmarks)
        {
          bookmark.TagsArray = bookmark.Tags.Split(' ');
        }

        return bookmarks;
      });
    }



    private UiService _uiService;
    private string _appSettingsPath;
    private string _pinboardFileName;
  }
}
