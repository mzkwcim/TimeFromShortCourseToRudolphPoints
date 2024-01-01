using System;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using HtmlAgilityPack;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Npgsql;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        //following 2 lines should be used only when there is new rudolphtable to insert into database
        string pdfFilePath = "C:\\Users\\mzkwcim\\Desktop\\punkttabelle_rudolph_2023.pdf";
        Creater(pdfFilePath);
        // string url is a profile of an Athlete on swimrankings.net
        string url = "https://www.swimrankings.net/index.php?page=athleteDetail&athleteId=4657304";
        List<string> queryList = new List<string>();
        int adder = 0;
        Dictionary<string, double> records = Calculator(AthleteRecords(url), url);
        Console.WriteLine(GetTableName(url));
        List<string> queries = GetRudolphPointsQuery(url);
        foreach(var (key, value) in records)
        {
            DataBaseConnection(queries[adder], key);
            adder++;
        }
        
    }
    static string StrokeTranslation(string distance)
    {
        // this part of code is responsible for translation of stroke
        string stroke;
        string[] words = distance.Split(' ');
        switch (words[1])
        {
            case "Freestyle":
                stroke = "freestyle_" + words[0];
                return stroke;
            case "Backstroke":
                stroke = "backstroke_" + words[0];
                return stroke;
            case "Breaststroke":
                stroke = "breaststroke_" + words[0];
                return stroke;
            case "Butterfly":
                stroke = "butterfly_" + words[0];
                return stroke;
            case "Medley":
                stroke = "medley_" + words[0];
                return stroke;
            default:
                return "";
        }
    }
    static Dictionary<string, double> Calculator(Dictionary<string, double> records, string url)
    {
        /* in this function program uses table of word records and table of certain athlete records, then by reversing pattern on fina points program gets certain time (equivalent of time on long course with certain amount of fina points) */
        // the basic pattern is FINA Points = ((World Record/Athlete Personal Best)^3)*1000
        List<double> wrs = GettingTimesV2(records, url);
        Dictionary<string, double> doubleded = new Dictionary<string, double>();
        int adder = 0;
        foreach (var (key,value) in records)
        {
            try
            {
                double doubled = Math.Round(((1 / Math.Pow((value/1000), (1.0 / 3.0))) * wrs[adder]), 2);
                doubleded.Add(StrokeTranslation(key), doubled);
                Console.WriteLine(key + " " + value);
                adder++;
            }
            catch (Exception ex)
            {

            }
        }
        foreach(var (key, value) in doubleded)
        {
            Console.WriteLine(key + " " + value);
        }
        return doubleded;
    }
    static Dictionary<string, double> AthleteRecords(string url)
    {
        Dictionary<string, double> records = new Dictionary<string, double>();  
        //rawRecords gets fina points
        var rawRecords = Loader(url).DocumentNode.SelectNodes("//td[@class='code']");
        //distances gets event name
        var distances = Loader(url).DocumentNode.SelectNodes("//td[@class='event']//a");
        // pool gets pool length 25m or 50m
        var pool = Loader(url).DocumentNode.SelectNodes("//td[@class='course']");
        for (int i = 0; i <  rawRecords.Count; i++)
        {
            //if statement here is kind of filter that handles empty htmls, only allows 25m distances, discards 100m Medley, couse there is no such an event on long course. There is also clausule that doesn't allow 25m distances, couse it is kids only distance and it coused some exceptions
            if (rawRecords[i].InnerText != "-" && pool[i].InnerText == "25m"  && distances[i].InnerText != "100m Medley" && !distances[i].InnerText.Contains("25m"))
            {
                records.Add(distances[i].InnerText,Convert.ToDouble(rawRecords[i].InnerText));
            }
        }
        return records;
    }
    static void Creater(string pdfFilePath)
    {
        //in the near future i will shorten this part of code, i was lazy, couse my initial idea on how to prepare this code with class had a lot of errors.
        StringBuilder text = new StringBuilder();
        List<string> nazwy = new List<string>() { "RudolphTable8YearsOldBoys", "RudolphTable9YearsOldBoys", "RudolphTable10YearsOldBoys", "RudolphTable11YearsOldBoys", "RudolphTable12YearsOldBoys", "RudolphTable13YearsOldBoys", "RudolphTable14YearsOldBoys", "RudolphTable15YearsOldBoys", "RudolphTable16YearsOldBoys", "RudolphTable17YearsOldBoys", "RudolphTable18YearsOldBoys", "RudolphTableOpenBoys", "RudolphTable8YearsOldGirls", "RudolphTable9YearsOldGirls", "RudolphTable10YearsOldGirls", "RudolphTable11YearsOldGirls", "RudolphTable12YearsOldGirls", "RudolphTable13YearsOldGirls", "RudolphTable14YearsOldGirls", "RudolphTable15YearsOldGirls", "RudolphTable16YearsOldGirls", "RudolphTable17YearsOldGirls", "RudolphTable18YearsOldGirls", "RudolphTableOpenGirls", };
        List<double> rudolphTable8YearsOldBoys = new List<double>();
        List<double> rudolphTable9YearsOldBoys = new List<double>();
        List<double> rudolphTable10YearsOldBoys = new List<double>();
        List<double> rudolphTable11YearsOldBoys = new List<double>();
        List<double> rudolphTable12YearsOldBoys = new List<double>();
        List<double> rudolphTable13YearsOldBoys = new List<double>();
        List<double> rudolphTable14YearsOldBoys = new List<double>();
        List<double> rudolphTable15YearsOldBoys = new List<double>();
        List<double> rudolphTable16YearsOldBoys = new List<double>();
        List<double> rudolphTable17YearsOldBoys = new List<double>();
        List<double> rudolphTable18YearsOldBoys = new List<double>();
        List<double> rudolphTableOpenBoys = new List<double>();
        List<double> rudolphTable8YearsOldGirls = new List<double>();
        List<double> rudolphTable9YearsOldGirls = new List<double>();
        List<double> rudolphTable10YearsOldGirls = new List<double>();
        List<double> rudolphTable11YearsOldGirls = new List<double>();
        List<double> rudolphTable12YearsOldGirls = new List<double>();
        List<double> rudolphTable13YearsOldGirls = new List<double>();
        List<double> rudolphTable14YearsOldGirls = new List<double>();
        List<double> rudolphTable15YearsOldGirls = new List<double>();
        List<double> rudolphTable16YearsOldGirls = new List<double>();
        List<double> rudolphTable17YearsOldGirls = new List<double>();
        List<double> rudolphTable18YearsOldGirls = new List<double>();
        List<double> rudolphTableOpenGirls = new List<double>();
        List<List<double>> listalist = new List<List<double>>() { rudolphTable8YearsOldBoys, rudolphTable9YearsOldBoys, rudolphTable10YearsOldBoys, rudolphTable11YearsOldBoys, rudolphTable12YearsOldBoys, rudolphTable13YearsOldBoys, rudolphTable14YearsOldBoys, rudolphTable15YearsOldBoys, rudolphTable16YearsOldBoys, rudolphTable17YearsOldBoys, rudolphTable18YearsOldBoys, rudolphTableOpenBoys, rudolphTable8YearsOldGirls, rudolphTable9YearsOldGirls, rudolphTable10YearsOldGirls, rudolphTable11YearsOldGirls, rudolphTable12YearsOldGirls, rudolphTable13YearsOldGirls, rudolphTable14YearsOldGirls, rudolphTable15YearsOldGirls, rudolphTable16YearsOldGirls, rudolphTable17YearsOldGirls, rudolphTable18YearsOldGirls, rudolphTableOpenGirls, };
        List<string> collumnNames = new List<string>() { "Punkty", "Freestyle_50m", "Freestyle_100m", "Freestyle_200m", "Freestyle_400m", "Freestyle_800m", "Freestyle_1500m", "Breaststroke_50m", "Breaststroke_100m", "Breaststroke_200m", "Butterfly_50m", "Butterfly_100m", "Butterfly_200m", "Backstroke_50m", "Backstroke_100m", "Backstroke_200m", "Medley_200m", "Medley_400m" };

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
        // every split in this function was based on the structure of pdf
        List<string> pages = text.ToString().Split("DieDisziplinen 400-1500F, 100/200S, 200R, 400Lsindstatistischunzureichendgesichertund solltenzurLeistungseinschätzung nichtherangezogenwerden.2").ToList();
        List<double> times = new List<double>();
        int adder = 0;
        foreach (string page in pages)
        {
            List<string> w = page.Split("\n").ToList();
            for (int i = 0; i < w.Count; i++)
            {
                List<string> one = w[i].Split(" ").ToList();
                foreach (string word in one)
                {
                    // the following lines of code, was my lazyness, i will replace it with shorter code
                    if (word.Contains(":"))
                    {
                        if (adder < 340)
                        {
                            rudolphTable8YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 340 && adder < 680)
                        {
                            rudolphTable9YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 680 && adder < 1020)
                        {
                            rudolphTable10YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 1020 && adder < 1360)
                        {
                            rudolphTable11YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 1360 && adder < 1700)
                        {
                            rudolphTable12YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 1700 && adder < 2040)
                        {
                            rudolphTable13YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 2040 && adder < 2380)
                        {
                            rudolphTable14YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 2380 && adder < 2720)
                        {
                            rudolphTable15YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 2720 && adder < 3060)
                        {
                            rudolphTable16YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 3060 && adder < 3400)
                        {
                            rudolphTable17YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 3400 && adder < 3740)
                        {
                            rudolphTable18YearsOldBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 3740 && adder < 4080)
                        {
                            rudolphTableOpenBoys.Add(CSTD(word.Replace(",", ".")));
                        }
                        if (adder >= 4080 && adder < 4420)
                        {
                            rudolphTable8YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 4420 && adder < 4760)
                        {
                            rudolphTable9YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 4760 && adder < 5100)
                        {
                            rudolphTable10YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 5100 && adder < 5440)
                        {
                            rudolphTable11YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 5440 && adder < 5780)
                        {
                            rudolphTable12YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 5780 && adder < 6120)
                        {
                            rudolphTable13YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 6120 && adder < 6460)
                        {
                            rudolphTable14YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 6460 && adder < 6800)
                        {
                            rudolphTable15YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 6800 && adder < 7140)
                        {
                            rudolphTable16YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 7140 && adder < 7480)
                        {
                            rudolphTable17YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 7480 && adder < 7820)
                        {
                            rudolphTable18YearsOldGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        else if (adder >= 7820 && adder < 8160)
                        {
                            rudolphTableOpenGirls.Add(CSTD(word.Replace(",", ".")));
                        }
                        adder++;
                    }
                }
            }
        }
        for (int i = 0; i < nazwy.Count; i++)
        {
            Connection(nazwy[i], collumnNames, listalist[i]);
        }
    }
    static double CSTD (string xd)
    {
        // this function grabs time like 3:21.55 and convert it to 201.55 so it can be easy written as a double
        string[] list = xd.Split(':');
        double newer = Math.Round(((list.Length > 1) ? (Convert.ToDouble(list[0]) * 60) + Convert.ToDouble(list[1]) : Convert.ToDouble(list[0])),2);
        return newer;
    }
    static string CreateTable(string name, List<string> collumnNames)
    {
        string createTableQueryTest = $"CREATE TABLE {name} (" +
                          "ID SERIAL PRIMARY KEY,";
        for (int i = 0; i < collumnNames.Count; i++)
        {
            createTableQueryTest += (i != collumnNames.Count - 1) ? $"\"{collumnNames[i].ToLower()}\" DOUBLE PRECISION, " : $"\"{collumnNames[i].ToLower()}\" DOUBLE PRECISION );";
        }
        Console.WriteLine(createTableQueryTest);
        return createTableQueryTest;
    }
    static string AddValuesToQuery(string name, List<string> collumnNames, List<double> tableValues)
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
                AddValues += (j < 16) ? $"{tableValues[(i * 17) + j], 2}, " : $"{tableValues[(i * 17) + j], 2} ";
                
            }
            AddValues += (i < 19) ? " ), " : " ); ";
        }
        return AddValues;
    }
    static void Connection(string name, List<string> collumnNames, List<double> tableValues)
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Mzkwcim181099!;Database=postgresv2";

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                Console.WriteLine("haha");
                connection.Open();
                if (TableExists(connection, name))
                {
                    using (NpgsqlCommand command2 = new NpgsqlCommand(AddValuesToQuery(name, collumnNames, tableValues), connection))
                    {
                        command2.ExecuteNonQuery();
                        Console.WriteLine("Powodzenie");
                    }
                }
                else
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(CreateTable(name, collumnNames), connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Baza Utworzona");
                    }
                    using (NpgsqlCommand command2 = new NpgsqlCommand(AddValuesToQuery(name, collumnNames, tableValues), connection))
                    {
                        command2.ExecuteNonQuery();
                        Console.WriteLine("Powodzenie");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    static bool TableExists(NpgsqlConnection connection, string tableName)
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
    static HtmlAgilityPack.HtmlDocument Loader(string url)
    {
        var httpClient = new HttpClient();
        var html = httpClient.GetStringAsync(url).Result;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        return htmlDocument;
    }
    static List<double> GettingLongCurseWorldRecordsMen()
    {
        
        string url = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=1&course=LCM&styleId=1";
        var currentTimes = Loader(url).DocumentNode.SelectSingleNode("//td[@class='swimtime']");
        List<double> tab = new List<double>();
        for (int i = 0; i < 1; i++)
        {
            tab.Add(CSTD(currentTimes.InnerText));
            Console.WriteLine(tab[i]);
        }
        return tab;
    }
    static List<string> GettingDistancesFromLinks(string url)
    {
        string url2 = "";
        if (CheckGender(url) == "boys")
        {
            url2 = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=1&course=LCM&styleId=0";
        }
        else
        {
            url2 = "https://www.swimrankings.net/index.php?page=recordDetail&recordListId=50001&gender=2&course=LCM&styleId=0";
        }
        
        List<string> tab = new List<string>();
        List<string> distances = new List<string>() {"1", "2", "3", "5", "6", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };
        for (int i = 0; i < distances.Count; i++)
        {
            tab.Add(url2.Replace("styleId=0", $"styleId={distances[i]}"));
        }
        return tab;
    }
    static List<string> GettingDistances(string urls)
    {
        List<string> url = GettingDistancesFromLinks(urls);
        List<string> strings = new List<string>();

        foreach (var u in url)
        {
            var htmlDocument = Loader(u);
            if (!String.IsNullOrEmpty(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText) && !(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText == "Record history for 300m Freestyle") && !(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText == "Record history for 1000m Freestyle"))
            {
                strings.Add(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText.Replace("Record history for ", ""));
            }
        }
        for (int i = 0; i < strings.Count; i++)
        {
            Console.WriteLine(strings[i]);
        }
        return strings;
    }
    static List<double> GettingTimes(string urls)
    {
        // for now i do not use this function and it can be removed
        List<string> url = GettingDistancesFromLinks(urls);
        List<double> strings = new List<double>();
        foreach (var u in url)
        {
            var htmlDocument = Loader(u);
            if (!String.IsNullOrEmpty(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText) && !(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText == "Record history for 300m Freestyle") && !(htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText == "Record history for 1000m Freestyle"))
            {
                strings.Add(CSTD(htmlDocument.DocumentNode.SelectSingleNode("//a[@class='time']").InnerText));
            }
        }
        for (int i = 0; i < strings.Count; i++)
        {
            Console.WriteLine(strings[i]);
        }
        return strings;
    }
    static List<double> GettingTimesV2(Dictionary<string,double> records, string urls)
    {
        List<string> url = GettingDistancesFromLinks(urls);
        List<double> strings = new List<double>();
        List<string> helper = new List<string>();
        foreach(var key in  records)
        {
            helper.Add(key.Key);
        }
        foreach (var u in url)
        {
            var htmlDocument = Loader(u);
            if (helper.Any(help => help == htmlDocument.DocumentNode.SelectSingleNode("//b").InnerText.Replace("Record history for ","")))
            {
                strings.Add(CSTD(htmlDocument.DocumentNode.SelectSingleNode("//a[@class='time']").InnerText));
            }
        }
        return strings;
    }
    static string CheckGender(string url)
    {
        var gender = Loader(url).DocumentNode.SelectSingleNode("//div[@id='name']").InnerHtml;
        return (gender.Contains("gender1.png")) ? "boys" : "girls";
    }
    static string GetAge(string url)
    {
        int date = DateTime.Now.Year - Convert.ToInt32(GetNumericValue(Loader(url).DocumentNode.SelectSingleNode("//div[@id='name']").InnerText));
        return (date <= 18) ? Convert.ToString(date) : "open";
    }
    static string GetNumericValue(string input)
    {
        Regex regex = new Regex(@"\d+");
        Match match = regex.Match(input);
        return (match.Success) ? match.Value : string.Empty;
    }
    static string GetTableName(string url)
    {
        return (GetAge(url) == "open") ? $"rudolphtableopen{CheckGender(url)}" : $"rudolphtable{GetAge(url)}yearsold{CheckGender(url)}";
    }
    static List<string> GetRudolphPointsQuery(string url)
    {
        Dictionary<string,double> records = Calculator(AthleteRecords(url), url);
        string tableName = GetTableName(url);
        List<string> queries = new List<string>();
        foreach(var (key, value) in records)
        {
            queries.Add($"SELECT punkty FROM {GetTableName(url)} WHERE {key} >= {value} LIMIT 1;");
        }
        return queries;
    }

    static void DataBaseConnection(string query, string key)
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Mzkwcim181099!;Database=postgresv2";
        // Utwórz obiekt NpgsqlConnection
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            // Otwórz połączenie
            connection.Open();
            // Utwórz obiekt NpgsqlCommand
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    // Sprawdź, czy istnieją wyniki
                    if (reader.HasRows)
                    {
                        // Przetwarzaj wyniki zapytania
                        while (reader.Read())
                        {
                            // Odczytaj wartości z wyników
                            object value1 = reader["punkty"];

                            Console.WriteLine($"{key}: {value1}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Na dystansie {key} uzyskałeś/aś mniej niż 1 pkt");
                    }
                }
            }
        }
    }
}
