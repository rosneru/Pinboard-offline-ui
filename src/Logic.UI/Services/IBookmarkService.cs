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
    List<Bookmark> DisplayedBookmarks { get; }

    ObservableCollection<string> FilteredTags { get; }

    Task InitializeAsync(string appSettingsPath, string pinboardFileName);

    void ToggleFilterTag(string tag);

  }
}
