using AstroWallpaperUtils.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace AstroWallpaperUtils
{
    public class WallpaperService
    {
        private Timer _timer;
        private readonly int _intervalMinutes;

        public WallpaperService(int intervalMinutes)
        {
            _intervalMinutes = intervalMinutes;
            _timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(_intervalMinutes));
        }

        public async void ExecuteTask(object state)
        {
            List<IParser> Parsers = SettingsManager.GetEnabledParsers();
            IParser parser = Parsers.First();

            if (Parsers.Count > 1)
            {
                parser = Parsers[new Random().Next(Parsers.Count)];
            }

            string imageUrl = await parser.GetImageUrl();

            if (imageUrl != null)
            {
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "astro_wallpaper_temp.jpg");
                await DownloadImageAsync(imageUrl, imagePath);

                // Ottieni la larghezza dello schermo
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;

                // Path per il file ridimensionato
                string resizedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "astro_wallpaper.jpg");

                // Ridimensiona l'immagine
                ImageResizer.ResizeImage(imagePath, resizedPath, screenWidth);

                // Imposta l'immagine ridimensionata come sfondo
                Wallpaper.Set(resizedPath);

                File.Delete(imagePath);
            }
        }

        private async Task DownloadImageAsync(string imageUrl, string savePath)
        {
            HttpClient client = new HttpClient();
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
            await Task.Run(() => File.WriteAllBytes(savePath, imageBytes));
        }
    }

    public static class Wallpaper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 0x01;
        public const int SPIF_SENDCHANGE = 0x02;

        public static void Set(string path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }
    }


}
