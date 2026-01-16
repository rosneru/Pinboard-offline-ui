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
    private static string css = "";
    private bool isBookmarkChanging = false;
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

        isBookmarkChanging = true;
        wv.NavigateToString(htmlWithCss);
      }
    }

    private MainViewModel _mainViewModel;

    private async void WebView2_Loaded(object sender, RoutedEventArgs e)
    {
      if (wv.CoreWebView2 == null)
      {
        // Ensure WebView2 is fully initialized
        await wv.EnsureCoreWebView2Async();

        var fg = ThemeHelper.GetFluentThemeTextColor();
        css = $@"
              <style>
                  body {{
                      color: rgb({fg.R}, {fg.G}, {fg.B});
                      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                      font-size: 16px;
                      line-height: 1.6;
                  }}
                  a {{
                      color: #00BFFF;
                  }}
                  a:visited {{
                      color: #87CEEB;
                  }}
                  a:hover {{
                      color: #00DFFF;
                  }}
                  a:active {{
                      color: #00FFFF;
                  }}
              </style>";
        isBookmarkChanging = true;
        wv.NavigateToString($"<!DOCTYPE html><html><head>{css}</head><body></body></html>");
        UpdateWebViewBackground();
      }
    }

    private void WebView2_NavigationStarting(
      object sender,
      Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
    {
      if (isBookmarkChanging)
      {
        isBookmarkChanging = false;
        return;
      }

      Process.Start(new ProcessStartInfo
      {
        FileName = "opera",
        Arguments = e.Uri.ToString(),
        UseShellExecute = true
      });
      e.Cancel = true;
    }

    private Color? UpdateWebViewBackground()
    {
      if (wv.CoreWebView2 == null) return null;

      if (Application.Current.Resources["ControlFillColorDefaultBrush"] is SolidColorBrush brush)
      {
        var mediaColor = brush.Color;
        // Force alpha to 255 (fully opaque) since WebView2 doesn't support semi-transparent backgrounds
        var drawingColor = System.Drawing.Color.FromArgb(
            255,  // Use 255 instead of mediaColor.A
            mediaColor.R,
            mediaColor.G,
            mediaColor.B);

        wv.DefaultBackgroundColor = drawingColor;
        return mediaColor;
      }

      return null;
    }
  }
}
