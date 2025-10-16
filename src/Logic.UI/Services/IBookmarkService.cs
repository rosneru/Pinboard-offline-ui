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
    /// <summary>
    /// List of all bookmarks
    /// </summary>
    List<Bookmark> AllBookmarks { get; }

    /// <summary>
    /// List of the currently displayed bookmarks
    /// </summary>
    List<Bookmark> FilteredBookmarks { get; }

    /// <summary>
    /// Collection of the all tag names of the currently displayed bookmarks
    /// </summary>
    ObservableCollection<string> FilteredTags { get; }

    /// <summary>
    /// Collection of the <names, occurrence count> of the currently displayed tags
    /// </summary>
    ObservableCollection<DisplayedTag> DisplayedTags { get; }

    event EventHandler FilteredBookmarksChanged;

    string BookmarkFileDateInfo { get;  }
    string LatestBookmarkDateInfo { get;  }

    void ToggleFilterTag(string tag);


    Task InitializeAsync(string appSettingsPath, string pinboardFileName);
  }
}
