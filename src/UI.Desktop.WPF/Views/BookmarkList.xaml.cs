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

    private void BookmarksListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (sender is ListView listView)
      {
        // Number of rows to scroll
        int scrollAmount = 1;

        // Get the ScrollViewer of Bookmarks ListView
        var scrollViewer = FindVisualChild<ScrollViewer>(listView);
        if (scrollViewer != null)
        {
          if (e.Delta > 0)
          {
            scrollViewer.LineUp();
          }
          else
          {
            scrollViewer.LineDown();
          }

          // Scroll up to `scrollAmount` lines
          for (int i = 1; i < scrollAmount; i++)
          {
            if (e.Delta > 0)
            {
              scrollViewer.LineUp();
            }
            else
            {
              scrollViewer.LineDown();
            }
          }

          // Prevent default mouse wheel behavior
          e.Handled = true;
        }
      }
    }

    private void TagsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    private void TagsListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      var parentListView = FindParent<ListView>((DependencyObject)sender);
      if (parentListView != null)
      {
        // Forward the wheel scroll event to the parent ListView
        var scrollViewer = FindVisualChild<ScrollViewer>(parentListView);
        if (scrollViewer != null)
        {
          if (e.Delta > 0)
          {
            scrollViewer.LineUp();
          }
          else
          {
            scrollViewer.LineDown();
          }

          // Prevent the child ListView from processing the event
          e.Handled = true;
        }
      }
    }

    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        if (child is T t)
        {
          return t;
        }

        var childOfChild = FindVisualChild<T>(child);
        if (childOfChild != null)
        {
          return childOfChild;
        }
      }
      return null;
    }
  }
}
