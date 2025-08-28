using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using LBV_Basics;

namespace LBV_WPF
{
    public static class GlobalWinValues
    {
        public static readonly double screenWidth = SystemParameters.PrimaryScreenWidth;
        public static readonly double screenHeight = SystemParameters.FullPrimaryScreenHeight + SystemParameters.WindowCaptionHeight;
        public static Dictionary<StateBackgroundWorker, Brush> ColorsStateBackgroundWorker = new()
        {
            { StateBackgroundWorker.Off, WpfColors.Dictionary["Transparent"] },
            { StateBackgroundWorker.On, WpfColors.Dictionary["Detail1"] },
            { StateBackgroundWorker.Wait, WpfColors.Dictionary["Off"] },
            { StateBackgroundWorker.Run, WpfColors.Dictionary["On"] },
            { StateBackgroundWorker.RunWait, WpfColors.Dictionary["Detail2"] }
        };
        private static readonly int transparencySteps = 10;

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

        public static void UpdateWpfColors(Window _window, List<GTRC_Basics.Models.Color>? colors = null)
        {
            colors ??= [];
            foreach (GTRC_Basics.Models.Color color in colors)
            {
                SolidColorBrush _color = new(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));
                WpfColors.Dictionary[color.Purpose] = _color;
                for (int transparency = 1; transparency < transparencySteps; transparency++)
                {
                    WpfColors.Dictionary[color.Purpose + "_Transp" + ((int)(100 * transparency / transparencySteps)).ToString()] = new SolidColorBrush(
                        Color.FromArgb((byte)(color.Alpha / transparencySteps * (transparencySteps - transparency)), color.Red, color.Green, color.Blue));
                }
            }
            List<string> purposes = [];
            foreach (string purpose in WpfColors.Dictionary.Keys) { purposes.Add(purpose); }
            foreach (string purpose in purposes)
            {
                _window.Resources["color" + purpose] = WpfColors.Dictionary[purpose];
                if (colors.Count == 0)
                {
                    for (int transparency = 1; transparency < transparencySteps; transparency++)
                    {
                        _window.Resources["color" + purpose + "_Transp" + ((int)(100 * transparency / transparencySteps)).ToString()] = new SolidColorBrush(Color.FromArgb((byte)
                            (WpfColors.Dictionary[purpose].Color.A / transparencySteps * (transparencySteps - transparency)), WpfColors.Dictionary[purpose].Color.R, WpfColors.Dictionary[purpose].Color.G, WpfColors.Dictionary[purpose].Color.B));
                    }
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
            OnStateBackgroundWorkerColorsUpdated();
        }

        public static event Notify? StateBackgroundWorkerColorsUpdated;

        public static void OnStateBackgroundWorkerColorsUpdated() { StateBackgroundWorkerColorsUpdated?.Invoke(); }
    }
}
