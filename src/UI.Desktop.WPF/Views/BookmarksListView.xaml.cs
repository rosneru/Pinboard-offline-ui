using System;
using System.Collections.Generic;
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
using Logic.UI.ViewModels;

namespace UI.Desktop.WPF.Views
{
  /// <summary>
  /// Interaction logic for BookmarkList.xaml
  /// </summary>
  public partial class BookmarksListView : UserControl
  {
    public BookmarksListView()
    {
      InitializeComponent();
    }

    private void BookmarkTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Console.WriteLine(e.AddedItems);
      if (DataContext is BookmarksListViewModel vm)
      {
        vm.AddFilterTag(e.AddedItems[0].ToString());
      }
    }
  }
}
