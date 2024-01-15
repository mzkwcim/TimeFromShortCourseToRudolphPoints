using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getting_Rudolph_Table_From_PDF_To_PostgreSQL
{
    internal class DictionaryBuilder
    {
        public static Dictionary<string, double> CalculatePointsFromShortCourseToLongCourseTime(Dictionary<string, double> records, string url)
        {
            /* in this function program uses table of word records and table of certain athlete records, then by reversing pattern on fina points program gets certain time (equivalent of time on long course with certain amount of fina points) */
            // the basic pattern is FINA Points = ((World Record/Athlete Personal Best)^3)*1000
            List<double> wrs = ListBuilder.GetTimes(records, url);
            Dictionary<string, double> doubleded = new Dictionary<string, double>();
            int adder = 0;
            foreach (var (key, value) in records)
            {
                try
                {
                    double doubled = Math.Round(((1 / Math.Pow((value / 1000), (1.0 / 3.0))) * wrs[adder]), 2);
                    doubleded.Add(DataFormat.StrokeTranslation(key), doubled);
                    adder++;
                }
                catch
                {

                }
            }
            return doubleded;
        }
        public static Dictionary<string, double> AthleteRecords(string url)
        {
            Dictionary<string, double> records = new Dictionary<string, double>();
            var rawRecords = Scraper.Loader(url).DocumentNode.SelectNodes("//td[@class='code']"); //rawRecords gets fina points
            var distances = Scraper.Loader(url).DocumentNode.SelectNodes("//td[@class='event']//a"); //distances gets event name
            var pool = Scraper.Loader(url).DocumentNode.SelectNodes("//td[@class='course']"); // pool gets pool length 25m or 50m
            for (int i = 0; i < rawRecords.Count; i++)
            {
                //if statement here is kind of filter that handles empty htmls, only allows 25m distances, discards 100m Medley, couse there is no such an event on long course. There is also clausule that doesn't allow 25m distances, couse it is kids only distance and it coused some exceptions
                if (rawRecords[i].InnerText != "-" && pool[i].InnerText == "25m" && distances[i].InnerText != "100m Medley" && !distances[i].InnerText.Contains("25m") && !records.ContainsKey(distances[i].InnerText))
                {
                    records.Add(distances[i].InnerText, Convert.ToDouble(rawRecords[i].InnerText));
                }
            }
            return records;
        }
    }
}
