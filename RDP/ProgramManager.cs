using Getting_Rudolph_Table_From_PDF_To_PostgreSQL;

class ProgramManager
{
    static void Main()
    {
        
        string answer;
        do
        {
            Console.WriteLine("Would you like to to set up a new rudolphtable database? (yes/no)");
            answer = Console.ReadLine();
        } while (answer != "yes" && answer != "no" && answer != "Yes" && answer != "No");
        if (answer == "yes")
        {
            string pdfFilePath = "C:\\Users\\mzkwcim\\Desktop\\punkttabelle_rudolph_2023.pdf"; //following 2 lines should be used only when there is new rudolphtable to insert into database
            PdfDataExtractor.Creater(pdfFilePath);
            OverAllProgram();
        }
        else
        {
            OverAllProgram();
        }
    }
    static void OverAllProgram()
    {
        string url = LinkGettingSystem.Link();
        int adder = 0;
        Dictionary<string, double> records = DictionaryBuilder.CalculatePointsFromShortCourseToLongCourseTime(DictionaryBuilder.AthleteRecords(url), url);
        List<string> queries = PostgreSQLQueryBuilder.GetRudolphPointsQuery(url);
        foreach (var (key, value) in records)
        {
            SqlDataManager.DataBaseConnection(queries[adder], key);
            adder++;
        }
    }
}


