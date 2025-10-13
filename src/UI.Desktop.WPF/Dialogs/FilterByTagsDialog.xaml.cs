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
using System.Windows.Shapes;
using Logic.UI.DialogViewModels;
using Logic.UI.ViewModels;

namespace UI.Desktop.WPF.Dialogs
{
  /// <summary>
  /// Interaction logic for FilterByTagsDialog.xaml
  /// </summary>
  public partial class FilterByTagsDialog : Window
  {
    public FilterByTagsDialog()
    {
      InitializeComponent();
    }

    private void TagsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (DataContext is FilterByTagsDialogViewModel vm)
      {
        if (e.AddedItems.Count == 0)
        {
          return;
        }

        string tag = e.AddedItems[0].ToString();
        vm.BookmarkService.ToggleFilterTag(tag);
        //tagToAddTextbox.Focus();
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      //tagToAddTextbox.Focus();
    }

    private void cbxTagToAdd_SelectionEffectivelyChanged(CustomControls.FilteringCombobox arg1, object arg2)
    {
      var vm = DataContext as FilterByTagsDialogViewModel;
      vm.AddTag(cbxTagToAdd.EffectivelySelectedItem);

      cbxTagToAdd.SelectedIndex = -1;
    }
  }
}
