using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace VolumeHotKeys.Library
{
    public class Settings
    {
        public string VolumeUp { get; set; }
        public string VolumeDown { get; set; }
        public string VolumeMute { get; set; }
        public string HideUIOnAppStart { get; set; }
        public string StartAtWindowsStartUp { get; set; }
        public string ExitToTray { get; set; }

        public Settings()
        {
            VolumeUp = "None";
            VolumeDown = "None";
            VolumeMute = "None";
            HideUIOnAppStart = "No";
            StartAtWindowsStartUp = "No";
            ExitToTray = "No";
        }
    }

    static class SettingsManager
    {
        private static string LocalAppPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string SettingsFolderPath = Path.Combine(LocalAppPath, Application.ProductName);
        private static string SettingsXmlPath = Path.Combine(LocalAppPath, Application.ProductName, "settings.xml");

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="settings">Settings</param>
        public static void Save(Settings settings)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            using (var xmlFile = File.Create(SettingsXmlPath))
            {
                xmlSerializer.Serialize(xmlFile, settings);
            }
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns></returns>
        public static Settings Load()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            Settings settings = new Settings();

            if(!File.Exists(SettingsXmlPath))
            {
                Directory.CreateDirectory(SettingsFolderPath);
                using (var xmlFile = File.Create(SettingsXmlPath))
                {
                    xmlSerializer.Serialize(xmlFile, settings);
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(SettingsXmlPath))
                {
                    settings = (Settings)xmlSerializer.Deserialize(sr);
                    sr.Close();
                }
            }

            return settings;
        }
    }
}
