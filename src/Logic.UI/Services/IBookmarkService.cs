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
    /// List of the currently displayed bookmarks - a subset of `AllBookmarks`
    /// created by a filtering (selection by tag) operation.
    /// </summary>
    List<Bookmark> SelectedBookmarks { get; }


    /// <summary>
    /// Tag names that are selected. They are used to create `SelectedBookmarks`
    /// as a subset of `AllBookmarks` where only bookmarksw which contain these
    /// tags are included.
    /// </summary>
    ObservableCollection<string> SelectedTags { get; }

    /// <summary>
    /// Tags of the remaining bookmarks after the filtering (tag selection)
    /// operation.
    /// </summary>
    ObservableCollection<string> AvailableTagNames { get; }

    /// <summary>
    /// Top ten tags of the currently selected bookmarks, sorted by
    /// `Occurrence`. Also contains the `Occurrence` normalized to [1..100] in
    /// the `Height` field. This can be used to draw the tags.
    /// </summary>
    ObservableCollection<DisplayedTag> TopTenTags { get; }

    /// <summary>
    /// Event is sent when either the bookmarks are loaded newly or when as a
    /// result of a filtering operation the displayed bookmarks changed,
    /// </summary>
    event EventHandler DisplayedBookmarksChanged;

    string BookmarkFileDateInfo { get;  }
    string LatestBookmarkDateInfo { get;  }

    void ToggleFilterTag(string tag);


    Task InitializeAsync(string appSettingsPath, string pinboardFileName);
  }
}
