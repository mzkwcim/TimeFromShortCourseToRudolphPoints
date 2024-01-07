using System.Text;
using HtmlAgilityPack;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Npgsql;
using System.Text.RegularExpressions;

class Formater
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
class Getter
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
class SQLQueryCreater
{
    public static string CreateTable(string name, List<string> collumnNames)
    {
        string createTableQueryTest = $"CREATE TABLE {name} (" +
                          "ID SERIAL PRIMARY KEY,";
        for (int i = 0; i < collumnNames.Count; i++)
        {
            createTableQueryTest += (i != collumnNames.Count - 1) ? $"\"{collumnNames[i].ToLower()}\" DOUBLE PRECISION, " : $"\"{collumnNames[i].ToLower()}\" DOUBLE PRECISION );";
        }
        return createTableQueryTest;
    }
    public static string AddValuesToQuery(string name, List<string> collumnNames, List<double> tableValues)
    {
        string AddValues = $"INSERT INTO {name} (";
        for (int j = 0; j < collumnNames.Count; j++)
        {
            AddValues += (j < collumnNames.Count - 1) ? $" {collumnNames[j].ToLower()}, " : $" {collumnNames[j].ToLower()} ) VALUES ";
        }
        for (int i = 0; i < 20; i++)
        {
            AddValues += $"( {20 - i}, ";
            for (int j = 0; j < 17; j++)
            {
                AddValues += (j < 16) ? $"{tableValues[(i * 17) + j],2}, " : $"{tableValues[(i * 17) + j],2} ";

            }
            AddValues += (i < 19) ? " ), " : " ); ";
        }
        return AddValues;
    }
    public static List<string> GetRudolphPointsQuery(string url)
    {
        Dictionary<string, double> records = DictionaryGetter.Calculator(DictionaryGetter.AthleteRecords(url), url);
        List<string> queries = new List<string>();
        foreach (var (key, value) in records)
        {
            queries.Add($"SELECT punkty FROM {Getter.GetTableName(url)} WHERE {key} >= {value} LIMIT 1;");
        }
        return queries;
    }
}
class DictionaryGetter
{
    public static Dictionary<string, double> Calculator(Dictionary<string, double> records, string url)
    {
        /* in this function program uses table of word records and table of certain athlete records, then by reversing pattern on fina points program gets certain time (equivalent of time on long course with certain amount of fina points) */
        // the basic pattern is FINA Points = ((World Record/Athlete Personal Best)^3)*1000
        List<double> wrs = ListGetter.GettingTimesV2(records, url);
        Dictionary<string, double> doubleded = new Dictionary<string, double>();
        int adder = 0;
        foreach (var (key, value) in records)
        {
            try
            {
                double doubled = Math.Round(((1 / Math.Pow((value / 1000), (1.0 / 3.0))) * wrs[adder]), 2);
                doubleded.Add(Formater.StrokeTranslation(key), doubled);
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
class ListGetter
{
    static List<string> GettingDistancesFromLinks(string url)
    {
        string url2 = (Getter.CheckGender(url) == "boys") ? "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=1&course=LCM&styleId=0" : "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=2&course=LCM&styleId=0";
        List<string> tab = new List<string>();
        List<string> distances = new List<string>() { "1", "2", "3", "5", "6", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };
        for (int i = 0; i < distances.Count; i++)
        {
            tab.Add(url2.Replace("styleId=0", $"styleId={distances[i]}"));
        }
        return tab;
    }
    public static List<double> GettingTimesV2(Dictionary<string, double> records, string urls)
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
                strings.Add(Formater.CSTD(htmlDocument.DocumentNode.SelectSingleNode("//a[@class='time']").InnerText));
            }
        }
        return strings;
    }
}
class Scraper
{
    public static HtmlAgilityPack.HtmlDocument Loader(string url)
    {
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        return htmlDocument;
    }
}
class SQL
{
    public static void Connection(string name, List<string> collumnNames, List<double> tableValues)
    {
        string connectionString = "Host=localhost;Username=postgres;xd;Database=postgresv2";
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                if (TableExists(connection, name))
                {
                    using (NpgsqlCommand command2 = new NpgsqlCommand(SQLQueryCreater.AddValuesToQuery(name, collumnNames, tableValues), connection))
                    {
                        command2.ExecuteNonQuery();
                        Console.WriteLine("Dane zostały dodane do tabeli");
                    }
                }
                else
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(SQLQueryCreater.CreateTable(name, collumnNames), connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Baza Utworzona");
                    }
                    using (NpgsqlCommand command2 = new NpgsqlCommand(SQLQueryCreater.AddValuesToQuery(name, collumnNames, tableValues), connection))
                    {
                        command2.ExecuteNonQuery();
                        Console.WriteLine("Dane zostały dodane do tabeli");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    public static bool TableExists(NpgsqlConnection connection, string tableName)
    {
        using (NpgsqlCommand command = new NpgsqlCommand())
        {
            command.Connection = connection;
            command.CommandText = $"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = @tableName)";
            command.Parameters.AddWithValue("@tableName", tableName.ToLower());
            object result = command.ExecuteScalar();
            return result != null && (bool)result;
        }
    }
    public static void DataBaseConnection(string query, string key)
    {
        string connectionString = "Host=localhost;Username=postgres;xd;Database=postgresv2";
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            object value1 = reader["punkty"];
                            Console.WriteLine($"Na dystansie {Formater.TranslateStrokeBack(key)} uzyskałeś {value1}pkt w skali Rudolpha");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Na dystansie {Formater.TranslateStrokeBack(key)} uzyskałeś/aś mniej niż 1 pkt");
                    }
                }
            }
        }
    }
}
class PDF
{
    public static List<string> GetTextFromPdf(string pdfFilePath)
    {
        StringBuilder text = new StringBuilder();
        using (PdfReader pdfReader = new PdfReader(pdfFilePath))
        {
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNumber), strategy);
                    text.Append(pageText);
                }
            }
        }
        return text.ToString().Split("DieDisziplinen 400-1500F, 100/200S, 200R, 400Lsindstatistischunzureichendgesichertund solltenzurLeistungseinschätzung nichtherangezogenwerden.2").ToList();
    }
}
class FromPdfToSQL
{
    public static void Creater(string pdfFilePath)
    {
        List<string> nazwy = new List<string>() { "RudolphTable8YearsOldBoys", "RudolphTable9YearsOldBoys", "RudolphTable10YearsOldBoys", "RudolphTable11YearsOldBoys", "RudolphTable12YearsOldBoys", "RudolphTable13YearsOldBoys", "RudolphTable14YearsOldBoys", "RudolphTable15YearsOldBoys", "RudolphTable16YearsOldBoys", "RudolphTable17YearsOldBoys", "RudolphTable18YearsOldBoys", "RudolphTableOpenBoys", "RudolphTable8YearsOldGirls", "RudolphTable9YearsOldGirls", "RudolphTable10YearsOldGirls", "RudolphTable11YearsOldGirls", "RudolphTable12YearsOldGirls", "RudolphTable13YearsOldGirls", "RudolphTable14YearsOldGirls", "RudolphTable15YearsOldGirls", "RudolphTable16YearsOldGirls", "RudolphTable17YearsOldGirls", "RudolphTable18YearsOldGirls", "RudolphTableOpenGirls", };
        List<List<double>> listalist = new List<List<double>>();
        List<string> collumnNames = new List<string>() { "Punkty", "Freestyle_50m", "Freestyle_100m", "Freestyle_200m", "Freestyle_400m", "Freestyle_800m", "Freestyle_1500m", "Breaststroke_50m", "Breaststroke_100m", "Breaststroke_200m", "Butterfly_50m", "Butterfly_100m", "Butterfly_200m", "Backstroke_50m", "Backstroke_100m", "Backstroke_200m", "Medley_200m", "Medley_400m" };
        for (int i = 0; i < nazwy.Count; i++)
        {
            listalist.Add(new List<double>());
        }
        List<string> pages = PDF.GetTextFromPdf(pdfFilePath);
        int adder = 0, inter = 0;
        foreach (string page in pages)
        {
            List<string> w = page.Split("\n").ToList();
            for (int i = 0; i < w.Count; i++)
            {
                List<string> one = w[i].Split(" ").ToList();
                foreach (string word in one)
                {
                    if (word.Contains(":"))
                    {
                        listalist[inter].Add(Formater.CSTD(word.Replace(",", ".")));
                        if ((adder + 1) % 340 == 0)
                        {
                            inter++;
                        }
                        adder++;
                    }
                }
            }
        }
        for (int i = 0; i < nazwy.Count; i++)
        {
            SQL.Connection(nazwy[i], collumnNames, listalist[i]);
        }
    }
}
class Program
{
    static void Main()
    {
        string pdfFilePath = "C:\\Users\\mzkwcim\\Desktop\\punkttabelle_rudolph_2023.pdf"; //following 2 lines should be used only when there is new rudolphtable to insert into database
        FromPdfToSQL.Creater(pdfFilePath);
        string url = "https://www.swimrankings.net/index.php?page=athleteDetail&athleteId=4426838"; // string url is a profile of an Athlete on swimrankings.net
        List<string> queryList = new List<string>();
        int adder = 0;
        Dictionary<string, double> records = DictionaryGetter.Calculator(DictionaryGetter.AthleteRecords(url), url);
        List<string> queries = SQLQueryCreater.GetRudolphPointsQuery(url);
        foreach (var (key, value) in records)
        {
            SQL.DataBaseConnection(queries[adder], key);
            adder++;
        }
    }
    
}
