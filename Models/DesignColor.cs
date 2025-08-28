using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Windows.Media;

namespace LBV_WPF.Models
{
    public class DesignColor
    {
        private static readonly string fileName = "design_colors.json";

        private static string? path = null;

        public static readonly string DefaultPurpose = nameof(Color);
        public static readonly List<DesignColor> List = [];
        public static readonly byte minDelta = 15;

        public DesignColor()
        {
            List.Add(this);
        }

        public DesignColor(string purpose, bool isDarkMode, byte red, byte green, byte blue, bool temporal = false)
        {
            Purpose = purpose;
            IsDarkMode = isDarkMode;
            Red = red;
            Green = green;
            Blue = blue;
            if (!temporal && IsValid())
            {
                List.Add(this);
            }
        }

        public string Purpose { get; set; } = DefaultPurpose;
        public bool IsDarkMode { get; set; } = true;
        public byte Red { get; set; } = byte.MinValue;
        public byte Green { get; set; } = byte.MinValue;
        public byte Blue { get; set; } = byte.MinValue;

        [JsonIgnore] public Color Preview
        {
            get { return Color.FromArgb(255, Red, Green, Blue); }
            set { }
        }

        public bool IsValid()
        {
            int indexThis = -1;
            if (List.Contains(this)) { indexThis = List.IndexOf(this); }    //Es dürfen keine Farben doppelt gespeichert werden
            for (int index = 0; index < List.Count; index++)                //Es dürfen keine Farben für den selben Modus gespeichert werden, die zu wenig Kontrast aufweisen
            {
                if (index != indexThis && List[index].IsDarkMode == IsDarkMode && Math.Abs(List[index].Red - Red) < minDelta && Math.Abs(List[index].Green - Green) < minDelta && Math.Abs(List[index].Blue - Blue) < minDelta)
                {
                    return false;
                }
            }
            return true;
        }

        public static void MakeValid()
        {
            for (int index = List.Count - 1; index >= 0; index--)
            {   //Alle Farben löschen, die nicht den Anforderungen entsprechen
                if (!List[index].IsValid()) { List.RemoveAt(index); }
            }
            //Fehlende Farben aus WpfColors ergänzen
            foreach (string _purpose in WpfColors.Dictionary.Keys)
            {
                bool foundDarkMode = false;
                bool foundBrightMode = false;
                foreach (DesignColor _designColor in List)
                {
                    if (_designColor.Purpose == _purpose)
                    {
                        if (_designColor.IsDarkMode) { foundDarkMode = true; }
                        else if (_designColor.IsDarkMode) { foundBrightMode = true; }
                    }
                }
                if (!foundDarkMode) { _ = new DesignColor(_purpose, true, WpfColors.Dictionary[_purpose].Color.R, WpfColors.Dictionary[_purpose].Color.G, WpfColors.Dictionary[_purpose].Color.B); }
                if (!foundBrightMode) { _ = new DesignColor(_purpose, false, WpfColors.Dictionary[_purpose].Color.R, WpfColors.Dictionary[_purpose].Color.G, WpfColors.Dictionary[_purpose].Color.B); }
            }
        }

        public static void LoadJson()
        {
            if (GlobalWinValues.DocumentsDirectory is not null)
            {
                path = GlobalWinValues.DocumentsDirectory + fileName;
                try
                {
                    if (File.Exists(path))
                    {
                        List.Clear();
                        _ = JsonConvert.DeserializeObject<List<DesignColor>>(File.ReadAllText(path, Encoding.Unicode)) ?? [];
                    }
                }
                catch { }
            }
            SaveJson();
            GlobalWinValues.OnWpfColorsUpdated();
        }

        public static void SaveJson()
        {
            MakeValid();
            if (path is not null)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(List, Formatting.Indented), Encoding.Unicode);
                }
                catch (Exception exception) { }
            }
        }
    }
}
