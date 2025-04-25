using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

      // Split the tags
      foreach (var bookmark in bookmarks)
      {
        bookmark.TagsArray = bookmark.Tags.Split(' ');
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
            item.TagsArray.Contains(tag))).ToList();
      }

      FilteredBookmarks = resultList;
    }
  }
}
