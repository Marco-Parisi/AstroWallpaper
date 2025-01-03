using System;
using System.IO;

namespace AstroWallpaperUtils
{
    internal static class Logger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AstroWallpaper.log");

        public static void Log(string title, string message)
        {
            try
            {
                File.AppendAllText(LogFilePath, $"{DateTime.Now} - {title}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log error: {ex.Message}");
            }
        }

        public static void Log(string message) => Log("Info", message);
    }
}
