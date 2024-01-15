using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getting_Rudolph_Table_From_PDF_To_PostgreSQL
{
    internal class DataFormat
    {
        public static string StrokeTranslation(string distance)
        {
            // this part of code is responsible for translation of distance to collumnName
            string[] words = distance.Split(' ');
            switch (words[1])
            {
                case "Freestyle": return "freestyle_" + words[0];
                case "Backstroke": return "backstroke_" + words[0];
                case "Breaststroke": return "breaststroke_" + words[0];
                case "Butterfly": return "butterfly_" + words[0];
                case "Medley": return "medley_" + words[0];
                default: return "";
            }
        }
        public static double CSTD(string xd)
        {
            // this function grabs time like 3:21.55 and convert it to 201.55 so it can be easy written as a double
            string[] list = xd.Split(':');
            return Math.Round((list.Length > 1) ? (Convert.ToDouble(list[0]) * 60) + Convert.ToDouble(list[1]) : Convert.ToDouble(list[0]), 2);
        }
        public static string TranslateStrokeBack(string key)
        {
            string[] words = key.Split('_');
            switch (words[0])
            {
                case "freestyle": return words[1] + " Dowolnym";
                case "backstroke": return words[1] + " Grzbietowym";
                case "breaststroke": return words[1] + " Klasycznym";
                case "butterfly": return words[1] + " Motylkowym";
                case "medley": return words[1] + " Zmiennym";
                default: return "";
            }
        }
    }
}
