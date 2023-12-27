using System;
using System.Text;
using System.Threading.Channels;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Npgsql;

class Program
{
    static void Main()
    {
        string pdfFilePath = "C:\\Users\\Laptop\\Desktop\\punkttabelle_rudolph_2023.pdf";
        StringBuilder text = new StringBuilder();
        List<string> nazwy = new List<string>() {"RudolphTable8YearsOldBoys", "RudolphTable9YearsOldBoys", "RudolphTable10YearsOldBoys", "RudolphTable11YearsOldBoys", "RudolphTable12YearsOldBoys", "RudolphTable13YearsOldBoys", "RudolphTable14YearsOldBoys", "RudolphTable15YearsOldBoys", "RudolphTable16YearsOldBoys", "RudolphTable17YearsOldBoys", "RudolphTable18YearsOldBoys", "RudolphTableOpenBoys", "RudolphTable8YearsOldGirls", "RudolphTable9YearsOldGirls", "RudolphTable10YearsOldGirls", "RudolphTable11YearsOldGirls", "RudolphTable12YearsOldGirls", "RudolphTable13YearsOldGirls", "RudolphTable14YearsOldGirls", "RudolphTable15YearsOldGirls", "RudolphTable16YearsOldGirls", "RudolphTable17YearsOldGirls", "RudolphTable18YearsOldGirls", "RudolphTableOpenGirls", };
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
        List<string> collumnNames = new List<string>() { "Punkty", "Freestyle_50m", "Freestyle_100m", "Freestyle_200m", "Freestyle_400m", "Freestyle_800m", "Freestyle_1500m", "Breaststroke_50m", "Breaststroke_100m", "Breaststroke_200m", "Butterfly_50m", "Butterfly_100m", "Butterfly_200m", "Backstroke_50m", "Backstroke_100m", "Backstroke_200m", "Medley_200m", "Medley_400m"};

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
        List<string> pages = text.ToString().Split("DieDisziplinen 400-1500F, 100/200S, 200R, 400Lsindstatistischunzureichendgesichertund solltenzurLeistungseinschätzung nichtherangezogenwerden.2").ToList();
        // Teraz 'text' zawiera tekst z całego dokumentu PDF
        List<double> times = new List<double>();
        int adder = 0;
        foreach(string page in pages)
        {
            List<string> w = page.Split("\n").ToList();
            for (int i = 0; i < w.Count; i++)
            {
                List<string> one = w[i].Split(" ").ToList();
                foreach (string word in one)
                {
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
        double newer = 0;
        string[] list = xd.Split(':');
        newer = Math.Round(((list.Length > 1) ? (Convert.ToDouble(list[0]) * 60) + Convert.ToDouble(list[1]) : Convert.ToDouble(list[0])),2);
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
        string connectionString = "Host=localhost;Username=postgres;Password=Mzkwcim181099!;Database=RudolphTable";

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
}