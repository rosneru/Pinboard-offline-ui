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
  }
}
