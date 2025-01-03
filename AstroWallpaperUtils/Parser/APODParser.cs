using System;
using System.Net.Http;
using System.Threading.Tasks;
using AstroWallpaperUtils.Parser;
using HtmlAgilityPack;

namespace AstroWallpaperUtils
{
    public class APODParser : IParser
    {
        public string ParserName => "APODParser";

        public async Task<string> GetImageUrl()
        {
            try
            {
                Logger.Log(ParserName, $"Started");

                Random random = new Random();

                // Generate a random date between January 1, 2015 and the current date
                DateTime startDate = new DateTime(2015, 1, 1);
                DateTime endDate = DateTime.Now;

                // Generate a random number of days to add to the start date
                int daysRange = (endDate - startDate).Days;
                DateTime randomDate = startDate.AddDays(random.Next(daysRange));

                // Create the APOD URL for that date (e.g., ap240103.html for January 3, 2024)
                string formattedDate = randomDate.ToString("yyMMdd");
                string url = $"https://apod.nasa.gov/apod/ap{formattedDate}.html";

                // Download the HTML content of the page
                HttpClient client = new HttpClient();
                string html = await client.GetStringAsync(url);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Extract the image link from the page
                HtmlNode imageNode = doc.DocumentNode.SelectSingleNode("//a[@href and contains(@href, 'image')]");

                if (imageNode != null)
                {
                    string imageUrl = "https://apod.nasa.gov/apod/" + imageNode.GetAttributeValue("href", "");
                    Logger.Log(ParserName, $"Image url: {imageUrl}");
                    return imageUrl;
                }
                else 
                {
                    throw new ArgumentNullException("imageNode");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ParserName, $"Errore: {ex.Message}");
                return null;
            }
        }
    }
}