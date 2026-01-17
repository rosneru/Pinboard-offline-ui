using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Logic.UI.ViewModels;

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

        Color fg; // = ThemeHelper.GetFluentThemeTextColor();
        System.Drawing.Color bg;
        Color link;
        Color linkVisited;
        Color linkHover;
        Color linkActive;
        if (ThemeHelper.IsDarkModeEnabled())
        {
          fg = Color.FromRgb(235, 219, 178);
          bg = System.Drawing.Color.FromArgb(
              255,  // WebView2 doens't support semi-transparence
              40,
              40,
              40);
          link = Color.FromRgb(69, 133, 136);
          linkVisited = Color.FromRgb(131, 165, 152);
          linkHover = Color.FromRgb(104, 157, 106);
          linkActive = Color.FromRgb(142, 192, 124);
        }
        else
        {
          fg = Color.FromRgb(60, 56, 54);
          bg = System.Drawing.Color.FromArgb(
              255,  // WebView2 doens't support semi-transparence
              251,
              241,
              199);
          link = Color.FromRgb(7, 102, 120);
          linkVisited = Color.FromRgb(66, 123, 88);
          linkHover = Color.FromRgb(60, 56, 44);
          linkActive = Color.FromRgb(157, 0, 6);
        }

        css = $@"
              <style>
                  body {{
                      background-color: rgb({bg.R}, {bg.G}, {bg.B});
                      color: rgb({fg.R}, {fg.G}, {fg.B});
                      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                      font-size: 16px;
                      line-height: 1.6;
                  }}
                  a {{
                      color: rgb({link.R}, {link.G}, {link.B});
                  }}
                  a:visited {{
                      color: rgb({linkVisited.R}, {linkVisited.G}, {linkVisited.B});
                  }}
                  a:hover {{
                      color: rgb({linkHover.R}, {linkHover.G}, {linkHover.B});
                  }}
                  a:active {{
                      color: rgb({linkActive.R}, {linkActive.G}, {linkActive.B});
                  }}
              </style>";

        isBookmarkChanging = true;
        wv.NavigateToString($"<!DOCTYPE html><html><head>{css}</head><body></body></html>");
        wv.DefaultBackgroundColor = bg;
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
