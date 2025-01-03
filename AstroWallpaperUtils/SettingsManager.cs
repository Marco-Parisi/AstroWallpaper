using AstroWallpaperUtils.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AstroWallpaperUtils
{
    public static class SettingsManager
    {
        private static readonly string SettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.xml");

        public static bool APODEnabled { get; set; }
        public static bool HubbleEnabled { get; set; }
        public static bool ESOEnabled { get; set; }
        public static int IntervalMinutes { get; set; }

        // Get the list of enabled parsers
        public static List<IParser> GetEnabledParsers()
        {
            List<IParser> parsers = new List<IParser>();

            if (APODEnabled)
            {
                parsers.Add(new APODParser());
            }
            if (HubbleEnabled)
            {
                parsers.Add(new HubbleParser());
            }
            if (ESOEnabled)
            {
                parsers.Add(new ESOParser());
            }

            return parsers;
        }

        static SettingsManager()
        {
            LoadSettings();
        }

        public static void LoadSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                CreateDefaultSettings();
            }

            var doc = XDocument.Load(SettingsFilePath);
            APODEnabled = bool.Parse(doc.Descendants("APODEnabled").FirstOrDefault()?.Value ?? "false");
            HubbleEnabled = bool.Parse(doc.Descendants("HubbleEnabled").FirstOrDefault()?.Value ?? "false");
            ESOEnabled = bool.Parse(doc.Descendants("ESOEnabled").FirstOrDefault()?.Value ?? "false");
            IntervalMinutes = int.TryParse(doc.Descendants("IntervalMinutes").FirstOrDefault()?.Value, out var interval) ? interval : 60;
        }

        public static void SaveSettings()
        {
            var doc = new XDocument(
                new XElement("Settings",
                    new XElement("APODEnabled", APODEnabled),
                    new XElement("HubbleEnabled", HubbleEnabled),
                    new XElement("ESOEnabled", ESOEnabled),
                    new XElement("IntervalMinutes", IntervalMinutes)
                )
            );

            doc.Save(SettingsFilePath);
        }

        private static void CreateDefaultSettings()
        {
            var doc = new XDocument(
                new XElement("Settings",
                    new XElement("APODEnabled", true),
                    new XElement("HubbleEnabled", true),
                    new XElement("ESOEnabled", true),
                    new XElement("IntervalMinutes", 60)
                )
            );

            doc.Save(SettingsFilePath);
        }
    }
}