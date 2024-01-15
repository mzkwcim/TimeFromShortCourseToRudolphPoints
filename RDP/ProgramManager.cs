using System.Text;
using HtmlAgilityPack;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Npgsql;
using System.Text.RegularExpressions;
using Getting_Rudolph_Table_From_PDF_To_PostgreSQL;

class ProgramManager
{
    static void Main()
    {
        string pdfFilePath = "Set your own path to downloaded rudolphtable pdf file"; //following 2 lines should be used only when there is new rudolphtable to insert into database
        PdfDataExtractor.Creater(pdfFilePath);
        string url = "https://www.swimrankings.net/index.php?page=athleteDetail&athleteId=4426838"; // string url is a profile of an Athlete on swimrankings.net
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
