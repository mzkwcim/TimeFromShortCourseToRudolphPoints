using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Getting_Rudolph_Table_From_PDF_To_PostgreSQL
{
    internal class TextCrawler
    {
        public static string GetAge(string url)
        {
            int date = DateTime.Now.Year - Convert.ToInt32(GetNumericValue(Scraper.Loader(url).DocumentNode.SelectSingleNode("//div[@id='name']").InnerText));
            return (date <= 18) ? Convert.ToString(date) : "open"; //this function return the Age of athlete or return open if an age is above 18
        }
        public static string GetNumericValue(string input)
        {
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(input);
            return (match.Success) ? match.Value : string.Empty; //this is helper function responsible for getting rid of non numeric strings
        }
        public static string GetTableName(string url) => (GetAge(url) == "open") ? $"rudolphtableopen{CheckGender(url)}" : $"rudolphtable{GetAge(url)}yearsold{CheckGender(url)}";
        public static string CheckGender(string url) => Scraper.Loader(url).DocumentNode.SelectSingleNode("//div[@id='name']").InnerHtml.Contains("gender1.png") ? "boys" : "girls";
    }
}
