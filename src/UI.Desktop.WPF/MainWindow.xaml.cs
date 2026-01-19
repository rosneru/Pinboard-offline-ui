using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Logic.UI.Model;
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
        ReloadWebViewTheme(_mainViewModel.CurrentTheme);
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
      else if (e.PropertyName == nameof(MainViewModel.CurrentTheme))
      {
        ReloadWebViewTheme(_mainViewModel.CurrentTheme);
      }
    }

    private MainViewModel _mainViewModel;

    private async void WebView2_Loaded(object sender, RoutedEventArgs e)
    {
      if (wv.CoreWebView2 == null)
      {
        // Ensure WebView2 is fully initialized
        await wv.EnsureCoreWebView2Async();

        var themeColors = new ThemeColors(ThemeType.SYS);
        var colors = themeColors.GetCurrentTheme();

        css = GenerateThemeCss(colors);

        isBookmarkChanging = true;
        wv.NavigateToString($"<!DOCTYPE html><html><head>{css}</head><body></body></html>");
        wv.DefaultBackgroundColor = colors.BackgroundColor;
      }
    }

    private void ReloadWebViewTheme(ThemeType themeType)
    {
      if (wv.CoreWebView2 == null)
      {
        return;
      }

      var themeColors = new ThemeColors(themeType);
      var colors = themeColors.GetCurrentTheme();

      css = GenerateThemeCss(colors);
      wv.DefaultBackgroundColor = colors.BackgroundColor;

      // Reload current content with new theme
      if (!string.IsNullOrEmpty(_mainViewModel?.SelectedBookmarkHtml))
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

    private string GenerateThemeCss(ThemeColors colors)
    {
      return $@"
              <style>
                  body {{
                      background-color: rgb({colors.BackgroundColor.R}, {colors.BackgroundColor.G}, {colors.BackgroundColor.B});
                      color: rgb({colors.ForegroundColor.R}, {colors.ForegroundColor.G}, {colors.ForegroundColor.B});
                      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                      font-size: 16px;
                      line-height: 1.6;
                  }}
                  a {{
                      color: rgb({colors.LinkColor.R}, {colors.LinkColor.G}, {colors.LinkColor.B});
                  }}
                  a:visited {{
                      color: rgb({colors.LinkVisitedColor.R}, {colors.LinkVisitedColor.G}, {colors.LinkVisitedColor.B});
                  }}
                  a:hover {{
                      color: rgb({colors.LinkHoverColor.R}, {colors.LinkHoverColor.G}, {colors.LinkHoverColor.B});
                  }}
                  a:active {{
                      color: rgb({colors.LinkActiveColor.R}, {colors.LinkActiveColor.G}, {colors.LinkActiveColor.B});
                  }}
              </style>";
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
  }
}
