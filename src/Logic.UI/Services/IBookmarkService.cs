using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logic.UI.Model;
using Logic.UI.ViewModels.Controls;

namespace Logic.UI.Services
{
  public interface IBookmarkService
  {
    List<Bookmark> AllBookmarks { get; }
    List<Bookmark> FilteredBookmarks { get; }

    ObservableCollection<string> FilteredTags { get; }
    List<KeyValuePair<string, int>> AllTags { get; }

    event EventHandler FilteredBookmarksChanged;

    string BookmarkFileDateInfo { get;  }
    string LatestBookmarkDateInfo { get;  }

    void ToggleFilterTag(string tag);


    Task InitializeAsync(string appSettingsPath, string pinboardFileName);
  }
}
