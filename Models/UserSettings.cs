using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace LBV_WPF.Models
{
    public class UserSettings
    {
        public static UserSettings? Instance { get; private set; }
        private static readonly string fileName = "usersettings.json";
        private static string? path = null;

        public UserSettings()
        {
            Instance = this;
        }

        private bool isDarkMode = false;

        public bool IsDarkMode
        {
            get { return isDarkMode; }
            set
            {
                isDarkMode = value;
                GlobalWinValues.OnWpfColorsUpdated();
            }
        }

        public static UserSettings LoadJson(UserSettings? currentInstance)
        {
            UserSettings newInstance = new();
            if (GlobalWinValues.DocumentsDirectory is not null)
            {
                path = GlobalWinValues.DocumentsDirectory + fileName;
                try
                {
                    if (File.Exists(path))
                    {
                        currentInstance = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(path, Encoding.Unicode)) ?? currentInstance ?? newInstance;
                    }
                }
                catch { }
            }
            UserSettings instance = currentInstance ?? newInstance;
            instance.SaveJson();
            return instance;
        }

        public void SaveJson()
        {
            if (path is not null)
            {
                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.Unicode);
                }
                catch (Exception exception) { }
            }
        }
    }
}
