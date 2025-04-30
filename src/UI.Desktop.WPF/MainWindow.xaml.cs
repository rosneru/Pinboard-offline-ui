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

    private MainViewModel _mainViewModel;

    private async void WebView2_Loaded(object sender, RoutedEventArgs e)
    {
      if (wv.CoreWebView2 == null)
      {
        // Ensure WebView2 is fully initialized
        await wv.EnsureCoreWebView2Async();

        // Handle the NewWindowRequested event
        wv.CoreWebView2.NewWindowRequested += (s, args) =>
        {
          // Prevent the WebView2 from opening a new window
          args.Handled = true;

          // Open the requested URL in the Opera browser
          Process.Start(new ProcessStartInfo
          {
            FileName = "opera", // Specify Opera as the browser
            Arguments = args.Uri, // Pass the URL as an argument
            UseShellExecute = true
          });
        };

        // Set the default background color based on the theme using
        // 'fake' colors: When the dark/bright theme changes, these
        // colors won't fit anymore. Also, on the fly theme switch
        // isn't detected here for the WebView2 control. TODO: fix.
        var brightBackground = System.Drawing.Color.FromArgb(240, 244, 249);
        var darkBackground = System.Drawing.Color.FromArgb(30, 32, 35);

        if (ThemeHelper.IsDarkModeEnabled())
        {
          wv.DefaultBackgroundColor = darkBackground;
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
        else
        {
          wv.DefaultBackgroundColor = brightBackground;
        }
      }
    }
  }
}
