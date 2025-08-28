using System.Windows.Media;

namespace LBV_WPF
{
    public static class WpfColors
    {
        public static Dictionary<string, SolidColorBrush> Dictionary = new()
        {
            { "Transparent", Brushes.Transparent },
            { "Background", Brushes.Black },
            { "Foreground", Brushes.White },
            { "Detail1", Brushes.Gray },
            { "Detail2", Brushes.Gray },
            { "Detail3", Brushes.Gray },
            { "On", Brushes.Green },
            { "Off", Brushes.Red },
            { "PersonalBest", Brushes.Green },
            { "OverallBest", Brushes.Magenta },
            { "Background2", Brushes.DarkGray },
            { "Background3", Brushes.LightGray }
        };
    }
}
