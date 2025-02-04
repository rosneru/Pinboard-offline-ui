using System.Diagnostics;
using System.Windows;
using Logic.UI.ViewModels;

namespace UI.Desktop.WPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      Loaded += MainWindow_Loaded;

    }


    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      await wv.EnsureCoreWebView2Async();

      _mainViewModel = DataContext as MainViewModel;
      _mainViewModel.PropertyChanged += Vm_PropertyChanged;

      wv.NavigateToString("<!DOCTYPE html>\r\n<html>\r\n    <head>\r\n        <title>Example</title>\r\n    </head>\r\n    <body>\r\n        <p>This is an example of a simple HTML page with one paragraph.</p>\r\n    </body>\r\n</html>");
    }
    private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MainViewModel.SelectedBookmarkHtml))
      {
        wv.NavigateToString(_mainViewModel.SelectedBookmarkHtml);
      }
    }


    private MainViewModel _mainViewModel;
  }
}
