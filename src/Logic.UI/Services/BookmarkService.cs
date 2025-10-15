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
    [ObservableProperty] private ObservableCollection<KeyValuePair<string, int>> _displayedTags = [];

    public string BookmarkFileDateInfo { get; private set; }
    public string LatestBookmarkDateInfo { get; private set; }

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

      var latestBookmarkDate = DateTime.MinValue;
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

        if(bookmark.DateTime > latestBookmarkDate)
        {
          latestBookmarkDate = bookmark.DateTime;
        }

        // And now overwrite the the bookmark.Time with a better
        // readable format from the DateTimeValue
        bookmark.Time = bookmark.DateTime.ToString("dd.MM.yyyy, HH:mm");
      }

      var bookmarkFileLastWriteTime = GetBookMarkFileDate(bookmarkFilePath);
      if (bookmarkFileLastWriteTime.HasValue)
      {
        var writeTime = bookmarkFileLastWriteTime.Value;
        int dayDifference = (int)DateTime.Now.Subtract(writeTime).TotalDays;
        var dateStr = writeTime.ToString("dd.MM.yyyy");

        BookmarkFileDateInfo = $"Bookmark file from {dateStr} ({dayDifference} days ago)";
      }

      if(latestBookmarkDate > DateTime.MinValue)
      {
        int dayDifference = (int)DateTime.Now.Subtract(latestBookmarkDate).TotalDays;
        var dateStr = latestBookmarkDate.ToString("dd.MM.yyyy");
        LatestBookmarkDateInfo = $"Latest bookmark from {dateStr} ({dayDifference} days ago)";
      }

      AllBookmarks = bookmarks;
      DisplayedTags = new ObservableCollection<KeyValuePair<string, int>>(
        AllBookmarks
          // Flatten the list of tagsArray arrays from all bookmarks to a single list of all tags
          // create something like ["tag1", "tag2", "tag3", "tag1", "tag4", ...]
          .SelectMany(bm => bm.TagsArray)
          //   Group the same tags together, case-insensitive to something like:
          //   { "tag1": ["tag1", "tag1"] }
          //   { "tag2": ["tag2", "tag2", "tag2"]}
          //   { "tag3": ["tag3"] }
          .GroupBy(tag => tag, StringComparer.OrdinalIgnoreCase)
          // Create a countable structure from the tag groups. `g`is an
          // `IGrouping<string, string>`, i.e. a group of tags with the
          // same name. Example:
          //      g.Key = "culture"
          //      g = ["culture", "Culture", "CULTURE"]
          // We create a KeyValuePair<string, int> from it, where the key
          // is the tag name and the value is the count of tags in the
          // group:
          //      new KeyValuePair<string, int>(g.Key, g.Count())
          // which results in:
          //      new KeyValuePair<string, int>("culture", 3)
          .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
          // Sort for value, `count`, primarily
          .OrderByDescending(kvp => kvp.Value)
          // and by key, `tag name`, secondarily
          .ThenBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
          .ToList()
      );

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

      DisplayedTags.Clear();
      foreach(var kvp in FilteredBookmarks
        .SelectMany(bm => bm.TagsArray)
        .Where(t => !FilteredTags.Contains(t, StringComparer.OrdinalIgnoreCase))
        .GroupBy(tag => tag, StringComparer.OrdinalIgnoreCase)
        .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
        .OrderByDescending(kvp => kvp.Value)
        .ThenBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase)
        .ToList())
      {
        DisplayedTags.Add(kvp);
      }

      ApplyFilters();
    }

    private DateTime? GetBookMarkFileDate(string bookmarkFilePath)
    {
      try
      {
        return System.IO.File.GetLastWriteTime(bookmarkFilePath);
      }
      catch(Exception e)
      {
        return null;
      }
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
