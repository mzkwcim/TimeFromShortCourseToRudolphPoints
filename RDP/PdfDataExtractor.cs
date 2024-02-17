using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Getting_Rudolph_Table_From_PDF_To_PostgreSQL
{
    internal class PdfDataExtractor
    {
        public static void Creater(string pdfFilePath)
        {
            List<string> nazwy = [ "RudolphTable8YearsOldBoys", "RudolphTable9YearsOldBoys", "RudolphTable10YearsOldBoys", "RudolphTable11YearsOldBoys", "RudolphTable12YearsOldBoys", "RudolphTable13YearsOldBoys", "RudolphTable14YearsOldBoys", "RudolphTable15YearsOldBoys", "RudolphTable16YearsOldBoys", "RudolphTable17YearsOldBoys", "RudolphTable18YearsOldBoys", "RudolphTableOpenBoys", "RudolphTable8YearsOldGirls", "RudolphTable9YearsOldGirls", "RudolphTable10YearsOldGirls", "RudolphTable11YearsOldGirls", "RudolphTable12YearsOldGirls", "RudolphTable13YearsOldGirls", "RudolphTable14YearsOldGirls", "RudolphTable15YearsOldGirls", "RudolphTable16YearsOldGirls", "RudolphTable17YearsOldGirls", "RudolphTable18YearsOldGirls", "RudolphTableOpenGirls", ];
            List<List<double>> listalist = [];
            List<string> collumnNames = [ "Punkty", "Freestyle_50m", "Freestyle_100m", "Freestyle_200m", "Freestyle_400m", "Freestyle_800m", "Freestyle_1500m", "Breaststroke_50m", "Breaststroke_100m", "Breaststroke_200m", "Butterfly_50m", "Butterfly_100m", "Butterfly_200m", "Backstroke_50m", "Backstroke_100m", "Backstroke_200m", "Medley_200m", "Medley_400m" ];
            for (int i = 0; i < nazwy.Count; i++)
            {
                listalist.Add([]);
            }
            List<string> pages = GetTextFromPdf(pdfFilePath);
            int adder = 0, inter = 0;
            foreach (string page in pages)
            {
                List<string> w = [..page.Split("\n")];
                for (int i = 0; i < w.Count; i++)
                {
                    List<string> one = [..w[i].Split(" ")];
                    foreach (string word in one)
                    {
                        if (word.Contains(':'))
                        {
                            listalist[inter].Add(DataFormat.ConvertTimeStringToDouble(word.Replace(",", ".")));
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
                SqlDataManager.Connection(nazwy[i], collumnNames, listalist[i]);
            }
        }
        public static List<string> GetTextFromPdf(string pdfFilePath)
        {
            StringBuilder text = new ();
            using (PdfReader pdfReader = new (pdfFilePath))
            {
                using (PdfDocument pdfDocument = new (pdfReader))
                {
                    for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        string pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNumber), strategy);
                        text.Append(pageText);
                    }
                }
            }
            return [..text.ToString().Split("DieDisziplinen 400-1500F, 100/200S, 200R, 400Lsindstatistischunzureichendgesichertund solltenzurLeistungseinschätzung nichtherangezogenwerden.2")];
        }
    }
}
