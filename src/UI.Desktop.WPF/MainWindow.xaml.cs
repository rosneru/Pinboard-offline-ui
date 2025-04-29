using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
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

    private async void WebView2_Loaded(object sender, RoutedEventArgs e)
    {
      if (wv.CoreWebView2 == null)
      {
        // Ensure WebView2 is fully initialized
        await wv.EnsureCoreWebView2Async();

        // Retrieve the PrimaryBackgroundColor from the current resources
        var bg = Application.Current.Resources["SystemControlBackgroundBaseHighBrush"]
              ?? Color.FromRgb(170, 170, 170);  // Fallback

        if (bg is System.Windows.Media.Color primaryBackgroundColor)
        {
          wv.DefaultBackgroundColor = System.Drawing.Color.FromArgb(
              primaryBackgroundColor.A,
              primaryBackgroundColor.R,
              primaryBackgroundColor.G,
              primaryBackgroundColor.B);
        }

        var fg = Application.Current.Resources["SystemControlForegroundBaseHighBrush"]
              ?? Color.FromRgb(235, 235, 235);  // Fallback
        if (fg is System.Windows.Media.Color primaryForegroundColor)
        {
          // Inject CSS to set the foreground color
          string css = $@"
                    <style>
                        body {{
                            color: rgb({primaryForegroundColor.R}, {primaryForegroundColor.G}, {primaryForegroundColor.B});
                        }}
                    </style>";
          wv.NavigateToString($"<!DOCTYPE html><html><head>{css}</head><body></body></html>");
        }
      }
    }

    public static Brush GetCurrentBackgroundBrush()
    {
      return Application.Current.Resources["SystemControlBackgroundBaseHighBrush"] as Brush;
    }

    public static Brush GetCurrentForegroundBrush()
    {
      return Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as Brush;
    }
  }
}
