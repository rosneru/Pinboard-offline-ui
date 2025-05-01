using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Logic.UI.Model;
using Newtonsoft.Json;

namespace Logic.UI.Services
{
  public partial class BookmarkService : ObservableObject, IBookmarkService
  {
    [ObservableProperty] private List<Bookmark> _allBookmarks = [];
    [ObservableProperty] private List<Bookmark> _FilteredBookmarks = [];
    [ObservableProperty] private ObservableCollection<string> _filteredTags = [];

    public event EventHandler FilteredBookmarksChanged;

    public BookmarkService()
    {

    }

    public async Task InitializeAsync(string appSettingsPath, string pinboardFileName)
    {
      var bookmarkFilePath = Path.Combine(appSettingsPath, pinboardFileName);

      var bookmarks = await LoadBookmarks(bookmarkFilePath);
      if(bookmarks == null)
      {
        return;
      }

      foreach (var bookmark in bookmarks)
      {
        // Split the tags
        bookmark.TagsArray = bookmark.Tags.Split(' ');

        // `bookmark.Time` contains something like "2025-03-14T07:43:18Z"
        // Convert it to a DateTimeValue
        string format = "yyyy-MM-ddTHH:mm:ssZ";
        bookmark.DateTime = DateTime.ParseExact(
          bookmark.Time,
          format,
          CultureInfo.InvariantCulture,
          DateTimeStyles.AssumeUniversal);

        // And now overwrite the the bookmark.Time with a better
        // readable format from the DateTimeValue
        bookmark.Time = bookmark.DateTime.ToString("dd.MM.yyyy, HH:mm");
      }

      AllBookmarks = bookmarks;
      ApplyFilters();
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

      ApplyFilters();
    }

    private Task<List<Bookmark>> LoadBookmarks(string bookmarkFilePath)
    {
      return Task.Run(() =>
      {
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

    private void ApplyFilters()
    {
      if (AllBookmarks == null)
      {
        return;
      }

      var resultList = AllBookmarks;

      if (FilteredTags.Count > 0)
      {
        resultList = resultList.Where(item =>
          FilteredTags.All(tag =>
            item.TagsArray.Contains(tag, StringComparer.OrdinalIgnoreCase))).ToList();
      }

      FilteredBookmarks = resultList;
      FilteredBookmarksChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
