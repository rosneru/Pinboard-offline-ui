using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [ObservableProperty] private List<Bookmark> _displayedBookmarks = [];
    [ObservableProperty] private ObservableCollection<string> _filteredTags = [];
    [ObservableProperty] private Bookmark _selectedBookmark;
    [ObservableProperty] private string _selectedBookmarkHtml;

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




    public BookmarksListViewModel(IUiService uiService, ISettingsService settingsService)
    {
      _uiService = uiService;
      _settingsService = settingsService;
    }

    public void ToggleFilterTag(string tag)
    {
      if (string.IsNullOrEmpty(tag))
      {
        return;
      }

      if (FilteredTags.Contains(tag))
      {
        FilteredTags.Remove(tag);
      }
      else
      {
        FilteredTags.Add(tag);
      }

      applyFilters();
    }


    public async Task Load()
    {
      Mouse.OverrideCursor = Cursors.Wait;

      _uiService.StatusBar.StatusText = $"Loading bookmarks..";
      var bookmarks = await loadBookmarks();
      if (bookmarks != null)
      {
        _uiService.StatusBar.StatusText = $"Processing bookmarks..";
        _allBookmarks = await splitBookmarksTags(bookmarks);
        DisplayedBookmarks = _allBookmarks;
      }
      else
      {
        _uiService.StatusBar.StatusText = $"No bookmarks found.";
      }

        applyFilters();
      Mouse.OverrideCursor = null;
    }

    private void applyFilters()
    {
      if(_allBookmarks == null)
      {
        return;
      }

      var resultList = _allBookmarks;

      if (FilteredTags.Count > 0)
      {
        resultList = resultList.Where(item =>
          FilteredTags.All(tag =>
            item.TagsArray.Contains(tag))).ToList();
      }

      DisplayedBookmarks = resultList;

      _uiService.StatusBar.StatusText =
        $"Displaying {DisplayedBookmarks.Count} " +
        $"of {_allBookmarks.Count} loaded bookmarks.";
    }

    private Task<List<Bookmark>> loadBookmarks()
    {
      return Task.Run(() =>
      {
        string bookmarkFilePath = Path.Combine(_settingsService.AppSettingsPath,
                                               _settingsService.PinboardFileName);
        try
        {
          string json = File.ReadAllText(bookmarkFilePath);
          return JsonConvert.DeserializeObject<List<Bookmark>>(json);
        }
        catch (FileNotFoundException e)
        {
          return null;
        }
      });
    }

    private Task<List<Bookmark>> splitBookmarksTags(List<Bookmark> bookmarks)
    {
      if (bookmarks == null)
      {
        return null;
      }

      return Task.Run(() =>
      {
        foreach (var bookmark in bookmarks)
        {
          bookmark.TagsArray = bookmark.Tags.Split(' ');
        }

        return bookmarks;
      });
    }


    private List<Bookmark> _allBookmarks;
    private IUiService _uiService;
    private ISettingsService _settingsService;
  }
}
