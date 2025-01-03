using AstroWallpaperUtils.Parser;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AstroWallpaperUtils
{
    public class HubbleParser : IParser
    {
        public string ParserName => "HubbleParser";

        public async Task<string> GetImageUrl()
        {
            try
            {
                Logger.Log(ParserName, $"Started");

                Random random = new Random();

                // Create the base URL for the page
                string baseUrl = "https://esahubble.org/images/archive/";
                string[] category = new string[] { "category/nebulae", "category/galaxies", "category/starclusters" };

                baseUrl += category[random.Next(category.Length)];

                // Download the first page to get the total number of pages
                HttpClient client = new HttpClient();
                string firstPageHtml = await client.GetStringAsync(baseUrl + "/page/1");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(firstPageHtml);

                // Find the number of pages from the pagination section
                HtmlNode paginationNode = doc.DocumentNode.SelectSingleNode("//ul[contains(@class, 'pagination')]/li[last()]/a");
                if (paginationNode == null)
                {
                    return null; // Not found, error retrieving the total number of pages
                }

                // Extract the total number of pages
                int totalPages = int.Parse(paginationNode.InnerText.Trim());

                // Choose a random page between 1 and totalPages
                int randomPage = random.Next(1, totalPages + 1);

                baseUrl = baseUrl + "/page/" + randomPage + "/?sort=-release_date";

                Logger.Log(ParserName, $"Getting image from: {baseUrl}");

                // Download a random page
                string pageHtml = await client.GetStringAsync(baseUrl);
                doc.LoadHtml(pageHtml);

                // Find the script block that contains the image JSON
                var scriptNode = doc.DocumentNode.SelectSingleNode("//script[contains(text(), 'var images =')]");

                if (scriptNode == null)
                {
                    return null; // Not found, error retrieving the JSON block
                }

                // Extract the JavaScript text that contains the JSON
                string scriptText = scriptNode.InnerText;

                // Find the JSON block in the text (remove everything before and after the JSON)
                int startIndex = scriptText.IndexOf("var images =") + "var images =".Length;
                int endIndex = scriptText.IndexOf("];", startIndex) + 1;
                string json = scriptText.Substring(startIndex, endIndex - startIndex);

                // Deserialize the JSON
                JArray images = JArray.Parse(json);

                // Choose a random image
                JToken randomImage = images[random.Next(images.Count)];

                // Get the large image link and replace "thumb" + 4 characters with "large"
                string imageUrl = randomImage["src"].ToString();
                string fullImageUrl = Regex.Replace(imageUrl, @"thumb\w{4}", "large");

                Logger.Log(ParserName, $"Image url: {fullImageUrl}");
                return fullImageUrl;
            }
            catch (Exception ex) 
            {
                Logger.Log(ParserName, $"Errore: {ex.Message}");
                return null;
            }
        }
    }
}
