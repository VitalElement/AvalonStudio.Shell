using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;

namespace AvalonStudio.Extensibility.Theme
{
    public class DefaultColorThemes : IActivatableExtension
    {
        public void Activation()
        {
        }

        public void BeforeActivation()
        {
            ColorTheme.Register(ColorTheme.VisualStudioDark);
            ColorTheme.Register(ColorTheme.VisualStudioLight);
        }
    }

    public class ColorTheme
    {
        private static List<ColorTheme> s_themes = new List<ColorTheme>();
        private static Dictionary<string, ColorTheme> s_themeIds = new Dictionary<string, ColorTheme>();
        private static readonly ColorTheme DefaultTheme = ColorTheme.VisualStudioLight;

        public static IEnumerable<ColorTheme> Themes => s_themes;

        public static void Register (ColorTheme theme)
        {
            s_themes.Add(theme);
            s_themeIds.Add(theme.Name, theme);
        }

        public static readonly ColorTheme VisualStudioLight = new ColorTheme
        {
            Name = "Visual Studio Light",
            WindowBorder = Brush.Parse("#9B9FB9"),
            Background = Brush.Parse("#EEEEF2"),
            Foreground = Brush.Parse("#1E1E1E"),
            ForegroundLow = Brush.Parse("#525252"),
            BorderLow = Brush.Parse("#FFCCCEDB"),
            BorderMid = Brush.Parse("#9B9FB9"),
            BorderHigh = Brush.Parse("#9B9FB9"),
            ControlLow = Brush.Parse("#686868"),
            ControlMid = Brush.Parse("#FFC2C3C9"),
            ControlHigh = Brush.Parse("#FFF5F5F5"),
            ControlBackground = Brush.Parse("#FFE6E7E8"),
            EditorBackground = Brush.Parse("#FFFFFFFF"),
            AccentLow = Brush.Parse("#FF007ACC"),
            AccentMid = Brush.Parse("#FF1C97EA"),
			AccentHigh = Brush.Parse("#52B0EF"),
			AccentForeground = Brush.Parse("#FFF0F0F0"),
            ErrorListError = Brush.Parse("#E34937"),
            ErrorListWarning = Brush.Parse("#D78A04"),
            ErrorListInfo = Brush.Parse("#1C7CD2")
        };

        public static readonly ColorTheme VisualStudioDark = new ColorTheme
        {
            Name = "Visual Studio Dark",
            WindowBorder = Brush.Parse("#FF004C8A"),
            Background = Brush.Parse("#FF2D2D30"),
            Foreground = Brush.Parse("#FFC4C4C4"),
            ForegroundLow = Brush.Parse("#FF808080"),
            BorderLow = Brush.Parse("#FF3E3E42"),
            BorderMid = Brush.Parse("#FF888888"),
            BorderHigh = Brush.Parse("#FFAAAAAA"),
            ControlLow = Brush.Parse("#FF2D2D30"),
            ControlMid = Brush.Parse("#FF3E3E42"),
            ControlHigh = Brush.Parse("#FF888888"),
            ControlBackground = Brush.Parse("#FF252526"),
            EditorBackground = Brush.Parse("#FF1E1E1E"),
			AccentLow = Brush.Parse("#FF007ACC"),
			AccentMid = Brush.Parse("#FF1C97EA"),
			AccentHigh = Brush.Parse("#52B0EF"),
			AccentForeground = Brush.Parse("#FFF0F0F0"),
            ErrorListError = Brush.Parse("#E34937"),
            ErrorListWarning = Brush.Parse("#D78A04"),
            ErrorListInfo = Brush.Parse("#1C7CD2")
        };

        public static ColorTheme LoadTheme (string name)
        {
            if (!string.IsNullOrEmpty(name) && s_themeIds.ContainsKey(name))
            {
                LoadTheme(s_themeIds[name]);

                return s_themeIds[name];
            }
            else
            {
                LoadTheme(DefaultTheme);

                return DefaultTheme;
            }
        }

        public static void LoadTheme(ColorTheme theme)
        {
            if (CurrentTheme != theme)
            {
                Application.Current.Resources["ThemeBackgroundBrush"] = theme.Background;
                Application.Current.Resources["ThemeControlBackgroundBrush"] = theme.ControlBackground;
                Application.Current.Resources["ThemeControlHighBrush"] = theme.ControlHigh;
                Application.Current.Resources["ThemeControlMidBrush"] = theme.ControlMid;
                Application.Current.Resources["ThemeControlLowBrush"] = theme.ControlLow;
                Application.Current.Resources["ThemeForegroundBrush"] = theme.Foreground;
                Application.Current.Resources["ThemeBorderHighBrush"] = theme.BorderHigh;
				Application.Current.Resources["ThemeBorderMidBrush"] = theme.BorderMid;
				Application.Current.Resources["ThemeBorderLowBrush"] = theme.BorderLow;
                Application.Current.Resources["ThemeEditorBackground"] = theme.EditorBackground;
                Application.Current.Resources["ApplicationAccentBrushLow"] = theme.AccentLow;
                Application.Current.Resources["ApplicationAccentBrushMed"] = theme.AccentMid;
				Application.Current.Resources["ApplicationAccentBrushHigh"] = theme.AccentHigh;
				Application.Current.Resources["ApplicationAccentForegroundBrush"] = theme.AccentForeground;
                Application.Current.Resources["ErrorListError"] = theme.ErrorListError;
                Application.Current.Resources["ErrorListWarning"] = theme.ErrorListWarning;
                Application.Current.Resources["ErrorListInfo"] = theme.ErrorListInfo;

                CurrentTheme = theme;
            }
        }

        public static ColorTheme CurrentTheme { get; private set; }

        public string Name { get; private set; }

        public IBrush AccentLow { get; set; }

        public IBrush AccentMid { get; set; }

		public IBrush AccentHigh { get; set; }

        public IBrush AccentForeground { get; set; }

        public IBrush WindowBorder { get; set; }

        public IBrush Background { get; set; }

        public IBrush Foreground { get; set; }

        public IBrush ForegroundLow { get; set; }

        public IBrush BorderLow { get; set; }

        public IBrush BorderMid { get; set; }

        public IBrush BorderHigh { get; set; }

        public IBrush ControlLow { get; set; }

        public IBrush ControlMid { get; set; }

        public IBrush ControlHigh { get; set; }

        public IBrush ControlBackground { get; set; }

        public IBrush EditorBackground { get; set; }

        public IBrush ErrorListError { get; set; }

        public IBrush ErrorListWarning { get; set; }

        public IBrush ErrorListInfo { get; set; }
    }
}
