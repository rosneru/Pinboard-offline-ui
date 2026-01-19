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
      // Try to retrieve the primary text color from WPF Fluent Theme resources
      // Note: WPF uses different resource keys than WinUI 3
      string[] textColorKeys = new[]
      {
        "SystemControlForegroundBaseHighBrush",   // WPF Fluent primary text
        "SystemControlPageTextBaseHighBrush",     // WPF page text
        "TextFillColorPrimaryBrush"               // WinUI 3 style (if custom resources)
      };

      foreach (var key in textColorKeys)
      {
        if (Application.Current.Resources[key] is SolidColorBrush brush)
        {
          return brush.Color;
        }
      }

      // Fallback to system text color based on theme
      return IsDarkModeEnabled() ? Colors.White : Colors.Black;
    }

    public static System.Drawing.Color GetFluentThemeBackgroundDrawingColor()
    {
      // Try these in order of preference:
      string[] backgroundKeys = new[]
      {
          "LayerFillColorDefaultBrush",           // Primary background layer
          "SolidBackgroundFillColorBaseBrush",    // Base solid background
          "ApplicationPageBackgroundThemeBrush"   // Legacy fallback
      };

      foreach (var key in backgroundKeys)
      {
          if (Application.Current.Resources[key] is SolidColorBrush brush)
          {
              return System.Drawing.Color.FromArgb(
                  255,
                  brush.Color.R,
                  brush.Color.G,
                  brush.Color.B);
          }
      }

      // Final fallback based on dark mode detection
      return IsDarkModeEnabled() 
          ? System.Drawing.Color.FromArgb(255, 32, 32, 32)  // Dark background
          : System.Drawing.Color.FromArgb(255, 243, 243, 243); // Light background
    }
  }
}
