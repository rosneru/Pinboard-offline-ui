using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Logic.UI.Model;
using Logic.UI.ViewModels;
using Markdig;
using Markdig.Extensions.Footnotes;

namespace UI.Desktop.WPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    static string css = "";
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
    }

    private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MainViewModel.SelectedBookmarkHtml))
      {
        if (!string.IsNullOrEmpty(_mainViewModel.SelectedBookmarkHtml))
        {
          string htmlWithCss = $@"
             <!DOCTYPE html>
             <html>
             <head>
                 {css}
             </head>
             <body>
                 {_mainViewModel.SelectedBookmarkHtml}
             </body>
             </html>";
          wv.NavigateToString(htmlWithCss);
        }
      }
    }

    private MainViewModel _mainViewModel;

    private async void WebView2_Loaded(object sender, RoutedEventArgs e)
    {
      if (wv.CoreWebView2 == null)
      {
        // Ensure WebView2 is fully initialized
        await wv.EnsureCoreWebView2Async();

        if (ThemeHelper.IsDarkModeEnabled())
        {
          //var fg = Color.FromArgb(235, 219, 178); // Gruvbox "white".
          var fg = ThemeHelper.GetFluentThemeTextColor();
          css = $@"
              <style>
                  body {{
                      color: rgb({fg.R}, {fg.G}, {fg.B});
                      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                      font-size: 16px;
                      line-height: 1.6;
                  }}
              </style>";
          wv.NavigateToString($"<!DOCTYPE html><html><head>{css}</head><body></body></html>");
        }
      }
    }
  }
}
