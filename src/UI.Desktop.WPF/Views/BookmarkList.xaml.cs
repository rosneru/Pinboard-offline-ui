using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Logic.UI.Model;
using Logic.UI.ViewModels;

namespace UI.Desktop.WPF.Views
{
  /// <summary>
  /// Interaction logic for BookmarkList.xaml
  /// </summary>
  public partial class BookmarkList : UserControl
  {
    public BookmarkList()
    {
      InitializeComponent();
    }

    private void BookmarkTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (DataContext is MainViewModel vm)
      {
        vm.BookmarkService.ToggleFilterTag(e.AddedItems[0].ToString());
      }
    }

    private void TagsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      // Suche das Parent-ListView
      var parentListView = FindParent<ListView>((DependencyObject)sender);

      if (parentListView != null)
      {
        var clickedItem = e.OriginalSource as FrameworkElement;
        if(clickedItem.DataContext is Bookmark bookmark)
        {
          parentListView.SelectedItem = bookmark;
        }
      }
    }

    // Hilfsmethode, um das Parent-Element zu finden
    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject parent = VisualTreeHelper.GetParent(child);

      while (parent != null && !(parent is T))
      {
        parent = VisualTreeHelper.GetParent(parent);
      }

      return parent as T;
    }
  }
}
