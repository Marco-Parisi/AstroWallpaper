using System.Drawing;
using System.Drawing.Drawing2D;

namespace AstroWallpaperUtils
{
    public static class ImageResizer
    {
        public static void ResizeImage(string inputPath, string outputPath, int desiredWidth)
        {
            using (Image originalImage = Image.FromFile(inputPath))
            {
                int newHeight = (int)(originalImage.Height * ((float)desiredWidth / originalImage.Width));
                using (Bitmap resizedImage = new Bitmap(desiredWidth, newHeight))
                {
                    using (Graphics graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

                        graphics.DrawImage(originalImage, new Rectangle(0, 0, desiredWidth, newHeight));
                    }

                    resizedImage.Save(outputPath, originalImage.RawFormat);
                }
            }
        }
    }
}
