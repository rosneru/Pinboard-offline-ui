using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
  }
}
