using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Getting_Rudolph_Table_From_PDF_To_PostgreSQL
{
    internal class LinkGettingSystem
    {
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public static string Link()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(chromeOptions);
            Athlete athlete = new Athlete();
            athlete.AthleteInserter();

            try
            {
                driver.Navigate().GoToUrl("https://www.swimrankings.net/index.php?page=athleteSelect&nationId=0&selectPage=SEARCH&language=pl");

                IWebElement searchInput = driver.FindElement(By.CssSelector("input#athlete_lastname.inputMedium"));
                IWebElement searchInputName = driver.FindElement(By.CssSelector("input#athlete_firstname.inputMedium"));

                searchInput.SendKeys(athlete.Nazwisko);
                searchInputName.SendKeys(athlete.Imie);

                searchInputName.SendKeys(Keys.Enter);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("td.name")));

                var nameElements = driver.FindElements(By.CssSelector("td.name"));

                foreach (var nameElement in nameElements)
                {
                    string athleteLink = nameElement.FindElement(By.CssSelector("td.name a")).GetAttribute("href");
                    string name = nameElement.FindElement(By.CssSelector("td.name a")).Text.Replace(",", "");

                    return athleteLink;
                }
            }
            finally
            {
                autoResetEvent.Set();
            }
            autoResetEvent.WaitOne();
            driver.Quit();
            return "The link hasn't been found, please check athlete name spelling";
        }
    }
}
