using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getting_Rudolph_Table_From_PDF_To_PostgreSQL
{
    internal class ListBuilder
    {
        static List<string> GettingDistancesFromLinks(string url)
        {
            string url2 = (TextCrawler.CheckGender(url) == "boys") ? "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=1&course=LCM&styleId=0" : "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=2&course=LCM&styleId=0";
            List<string> tab = new List<string>();
            List<string> distances = new List<string>() { "1", "2", "3", "5", "6", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };
            for (int i = 0; i < distances.Count; i++)
            {
                tab.Add(url2.Replace("styleId=0", $"styleId={distances[i]}"));
            }
            return tab;
        }
        public static List<double> GetTimes(Dictionary<string, double> records, string urls)
        {
            List<string> url = GettingDistancesFromLinks(urls);
            List<double> strings = new List<double>();
            List<string> helper = new List<string>();
            foreach (var key in records)
            {
                helper.Add(key.Key);
            }
            foreach (var u in url)
            {
                var htmlDocument = Scraper.Loader(u);
                if (helper.Any(help => help == htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText.Replace("Record history for ", "")))
                {
                    strings.Add(DataFormat.CSTD(htmlDocument.DocumentNode.SelectSingleNode("//a[@class='time']").InnerText));
                }
            }
            return strings;
        }
    }
}
