using System.Windows.Media;

namespace UI.Desktop.WPF
{
  internal class ThemeColors
  {
    public enum ThemeType
    {
      SYS,
      GRUVBOX
    }

    public Color ForegroundColor { get; }
    public System.Drawing.Color BackgroundColor { get; }
    public Color LinkColor { get; }
    public Color LinkVisitedColor { get; }
    public Color LinkHoverColor { get; }
    public Color LinkActiveColor { get; }

    public ThemeColors(ThemeType themeType)
    {
      var colors = themeType switch
      {
        ThemeType.SYS => SysTheme,
        ThemeType.GRUVBOX => ThemeHelper.IsDarkModeEnabled() ? GruvboxDarkTheme : GruvboxLightTheme,
        _ => throw new System.NotImplementedException()
      };

      ForegroundColor = colors.ForegroundColor;
      BackgroundColor = colors.BackgroundColor;
      LinkColor = colors.LinkColor;
      LinkVisitedColor = colors.LinkVisitedColor;
      LinkHoverColor = colors.LinkHoverColor;
      LinkActiveColor = colors.LinkActiveColor;
    }

    private static readonly ThemeColors SysTheme = new(
      foregroundColor: ThemeHelper.GetFluentThemeTextColor(),
      backgroundColor: ThemeHelper.GetFluentThemeBackgroundDrawingColor(),
      linkColor: Color.FromRgb(0, 191, 255),
      linkVisitedColor: Color.FromRgb(135, 206, 235),
      linkHoverColor: Color.FromRgb(0, 223, 255),
      linkActiveColor: Color.FromRgb(0, 255, 255));

    private static readonly ThemeColors GruvboxDarkTheme = new(
      foregroundColor: Color.FromRgb(235, 219, 178),
      backgroundColor: System.Drawing.Color.FromArgb(255, 40, 40, 40),
      linkColor: Color.FromRgb(69, 133, 136),
      linkVisitedColor: Color.FromRgb(135, 206, 235),
      linkHoverColor: Color.FromRgb(104, 157, 106),
      linkActiveColor: Color.FromRgb(142, 192, 124));

    private static readonly ThemeColors GruvboxLightTheme = new(
      foregroundColor: Color.FromRgb(60, 56, 54),
      backgroundColor: System.Drawing.Color.FromArgb(255, 251, 241, 199),
      linkColor: Color.FromRgb(7, 102, 120),
      linkVisitedColor: Color.FromRgb(66, 123, 88),
      linkHoverColor: Color.FromRgb(60, 56, 44),
      linkActiveColor: Color.FromRgb(157, 0, 6));

    public ThemeColors GetCurrentTheme()
    {
      // Return the current instance instead of making this static
      return this;
    }

    private ThemeColors(
      Color foregroundColor,
      System.Drawing.Color backgroundColor,
      Color linkColor,
      Color linkVisitedColor,
      Color linkHoverColor,
      Color linkActiveColor)
    {
      ForegroundColor = foregroundColor;
      BackgroundColor = backgroundColor;
      LinkColor = linkColor;
      LinkVisitedColor = linkVisitedColor;
      LinkHoverColor = linkHoverColor;
      LinkActiveColor = linkActiveColor;
    }
  }
}
