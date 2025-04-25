using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Logic.UI.Model;
using Logic.UI.ViewModels;
using Markdig;

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

      if (DataContext is MainViewModel vm)
      {
        _mainViewModel = vm;
        _mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
      }

      wv.NavigateToString("<!DOCTYPE html>\r\n<html>\r\n    <head>\r\n        <title>Example</title>\r\n    </head>\r\n    <body>\r\n        <p>This is an example of a simple HTML page with one paragraph.</p>\r\n    </body>\r\n</html>");
    }

    private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MainViewModel.SelectedBookmarkHtml))
      {
        wv.NavigateToString(_mainViewModel.SelectedBookmarkHtml);
      }
    }

    private MainViewModel _mainViewModel;
  }
}
