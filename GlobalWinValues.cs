using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

using LBV_Basics;
using LBV_WPF.Models;

namespace LBV_WPF
{
    public static class GlobalWinValues
    {
        public static readonly double screenWidth = SystemParameters.PrimaryScreenWidth;
        public static readonly double screenHeight = SystemParameters.FullPrimaryScreenHeight + SystemParameters.WindowCaptionHeight;
        public static readonly string Currency = "€";
        private static readonly int transparencySteps = 10;

        private static string? documentsDirectory = null;
        public static string? DocumentsDirectory { get { return documentsDirectory; } }
        public static Dictionary<StateBackgroundWorker, Brush> ColorsStateBackgroundWorker = new()
        {
            { StateBackgroundWorker.Off, WpfColors.Dictionary["Transparent"] },
            { StateBackgroundWorker.On, WpfColors.Dictionary["Detail1"] },
            { StateBackgroundWorker.Wait, WpfColors.Dictionary["Off"] },
            { StateBackgroundWorker.Run, WpfColors.Dictionary["On"] },
            { StateBackgroundWorker.RunWait, WpfColors.Dictionary["Detail2"] }
        };

        public static void SetDocumentsDirectory(string applicationName)
        {
            documentsDirectory = SpecialDirectories.MyDocuments + "\\" + applicationName + "\\";
            try
            {
                if (!Directory.Exists(documentsDirectory))
                {
                    Directory.CreateDirectory(documentsDirectory);
                }
            }
            catch (Exception exception)
            {
                documentsDirectory = null;
            }
        }

        public static void SetCultureInfo()
        {
            var newCulture = new CultureInfo(Thread.CurrentThread.CurrentUICulture.Name);
            newCulture.DateTimeFormat.FullDateTimePattern = "dd MM yyyy HH mm ss";

            CultureInfo.DefaultThreadCurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;

            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        public static void UpdateWpfColors(Window _window, List<DesignColor>? designColors = null)
        {
            designColors ??= [];
            foreach (DesignColor designColor in designColors)
            {
                SolidColorBrush _color = new(Color.FromArgb(byte.MaxValue, designColor.Red, designColor.Green, designColor.Blue));
                WpfColors.Dictionary[designColor.Purpose] = _color;
            }
            List<string> purposes = [];
            foreach (string purpose in WpfColors.Dictionary.Keys) { purposes.Add(purpose); }
            foreach (string purpose in purposes)
            {
                _window.Resources["color" + purpose] = WpfColors.Dictionary[purpose];
                for (int transparency = 1; transparency < transparencySteps; transparency++)
                {
                    _window.Resources["color" + purpose + "_Transp" + ((int)(100 * transparency / transparencySteps)).ToString()] = new SolidColorBrush(Color.FromArgb((byte)
                        (WpfColors.Dictionary[purpose].Color.A / transparencySteps * (transparencySteps - transparency)), WpfColors.Dictionary[purpose].Color.R, WpfColors.Dictionary[purpose].Color.G, WpfColors.Dictionary[purpose].Color.B));
                }
            }
            ColorsStateBackgroundWorker = new()
            {
                { StateBackgroundWorker.Off, WpfColors.Dictionary["Transparent"] },
                { StateBackgroundWorker.On, WpfColors.Dictionary["Detail1"] },
                { StateBackgroundWorker.Wait, WpfColors.Dictionary["Off"] },
                { StateBackgroundWorker.Run, WpfColors.Dictionary["On"] },
                { StateBackgroundWorker.RunWait, WpfColors.Dictionary["Detail2"] }
            };
            OnWpfColorsUpdated();
        }

        public static event Notify? WpfColorsUpdated;

        public static void OnWpfColorsUpdated() { WpfColorsUpdated?.Invoke(); }
    }
}
