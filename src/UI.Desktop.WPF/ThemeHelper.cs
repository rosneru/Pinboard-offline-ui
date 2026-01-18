using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace UI.Desktop.WPF
{
  public static class ThemeHelper
  {
    public static bool IsDarkModeEnabled()
    {
      const string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
      const string registryValue = "AppsUseLightTheme";

      object value = Registry.GetValue(registryKey, registryValue, null);
      return value is int intValue && intValue == 0; // 0 = Dark Mode, 1 = Light Mode
    }

    public static Color GetFluentThemeTextColor()
    {
      // Try to retrieve the primary text color from the Fluent Theme resources
      if (Application.Current.Resources["TextFillColorPrimaryBrush"] is SolidColorBrush brush)
      {
        return brush.Color;
      }

      // Fallback to a default color if the resource is not found
      return Colors.Black;
    }

    public static System.Drawing.Color GetFluentThemeBackgroundDrawingColor()
    {
      if (Application.Current.Resources["ControlFillColorDefaultBrush"] is SolidColorBrush brush)
      {
        // WebView2 doesn't support transparency. Set alpha to full 255.
        var webViewColor = System.Drawing.Color.FromArgb(
          255,
          brush.Color.R,
          brush.Color.G,
          brush.Color.B);

        return webViewColor;
      }

      // TODO: The above seems not to work. Consider to change it in DarkMode to:


      //// Set the default background color based on the theme using
      //// 'fake' colors: When the dark/bright theme changes, these
      //// colors won't fit anymore. Also, on the fly theme switch
      //// isn't detected here for the WebView2 control. TODO: fix.
      //var brightBackground = System.Drawing.Color.FromArgb(235, 246, 249);
      //var darkBackground = System.Drawing.Color.FromArgb(30, 32, 35);

      return System.Drawing.Color.IndianRed;
    }
  }
}
