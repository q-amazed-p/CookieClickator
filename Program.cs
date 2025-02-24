using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace CookieClickator
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "Cookie Clickator";
            Console.WriteLine("Welcome to Cookie Clickator!");
            Console.WriteLine("-            --            -");
            Console.WriteLine("");
            Console.WriteLine("Press N to begin a new session");
            Console.WriteLine("Press I to import an existing session from Import.txt");
            Console.WriteLine("Press any other key to quit");
            Console.WriteLine("");

            int appMode = 0;        // 0 - No Session (terminate)     1 - New Session     2 - Import Session
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.N:
                    appMode = 1;
                    break;

                case ConsoleKey.I:
                    appMode = 2;
                    break;
            }

            if(appMode > 0)
            {
                int delay = 0;
                int t = 0;

                bool enabledAutoClick = true;

                bool enabledBuildingAutoBuy = false;
                bool enabledUpgradeAutoBuy = false;

                IWebDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl("https://orteil.dashnet.org/cookieclicker/");

                driver.FindElement(By.CssSelector(".fc-primary-button")).Click();

                WaitToFindElement("#langSelect-EN").Click();

                if (appMode == 2)
                {
                    Console.WriteLine("");
                    string saveCode;

                    if (File.Exists("Import.txt") && (saveCode = File.ReadAllText("Import.txt")) != "")
                    {

                        WaitToFindElement("#prefsButton").FindElement(By.CssSelector(".subButton")).Click();
                        driver.FindElement(By.CssSelector("[onclick^=\"Game.ImportSave\"]")).Click();
                        driver.FindElement(By.Id("textareaPrompt")).SendKeys(saveCode);
                        driver.FindElement(By.Id("promptOption0")).Click();
                        WaitToFindElement("#prefsButton").FindElement(By.CssSelector(".subButton")).Click();

                        File.WriteAllText("ExportedType.txt", saveCode);

                        Console.WriteLine("mported session from Import.txt");
                    }
                    else
                    {
                        Console.WriteLine("Import.txt not found or empty");
                        appMode = 1;
                    }
                    Console.WriteLine("");
                }

                IWebElement theCookie = WaitToFindElement("#bigCookie", 20);

                Console.WriteLine("");
                if (appMode == 1) Console.Write("So it begun at ");
                if (appMode == 2) Console.Write("Welcome back to ");
                Console.WriteLine(driver.FindElement(By.Id("bakeryName")).Text + "!");
                Console.WriteLine("");
                driver.Manage().Window.Minimize();
                ListOptions();

                void ListOptions()
                {
                    Console.WriteLine("Press h to see this list again");
                    Console.WriteLine("Press - to reduce autoclicking speed");
                    Console.WriteLine("Press + to restore autoclicking speed");
                    Console.WriteLine("Press p to toggle autoclicking pause");
                    Console.WriteLine("Press b to toggle autobuy buildings");
                    Console.WriteLine("Press u to toggle autobuy upgrades");
                    Console.WriteLine("Press r to get a report");
                    Console.WriteLine("Press e to export current session to Exported.txt");
                    Console.WriteLine("Press q to end");
                    Console.WriteLine("");
                }

                while (appMode > 0)
                {
                    if (t > 0) t--;
                    else
                    {
                        if (enabledAutoClick)
                        {
                            try
                            {
                                theCookie.Click();
                            }
                            catch (StaleElementReferenceException _)
                            {
                                (theCookie = driver.FindElement(By.Id("bigCookie"))).Click();
                            }
                        }

                        if (enabledBuildingAutoBuy)
                        {
                            if(driver.FindElements(By.CssSelector(".product.enabled")).Count != 0) 
                                driver.FindElements(By.CssSelector(".product.enabled")).LastOrDefault()?.Click();
                        }

                        if (enabledUpgradeAutoBuy)
                        {
                            if(driver.FindElements(By.CssSelector(".upgrade.enabled")).Count != 0)
                            {
                                driver.FindElements(By.CssSelector(".upgrade.enabled")).LastOrDefault()?.Click();
                            }
                                
                        }

                        t = delay;
                    }

                    if (Console.KeyAvailable)
                    {
                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.R:
                                Console.WriteLine("eporting:");
                                Console.WriteLine(driver.FindElement(By.Id("cookies")).Text);
                                Console.WriteLine("");
                                break;

                            case ConsoleKey.P:

                                Console.Write("ause autoclick");
                                enabledAutoClick ^= true;
                                Console.WriteLine(enabledAutoClick ? " off" : " on");
                                Console.WriteLine("");
                                break;

                            case ConsoleKey.Subtract:
                                Console.WriteLine(" slowed");
                                Console.WriteLine("");
                                delay += 1000;
                                break;

                            case ConsoleKey.Add:
                                if (delay > 0)
                                {
                                    Console.WriteLine(" restored");
                                    delay -= 1000;
                                }
                                else Console.WriteLine(" at max");
                                Console.WriteLine("");
                                break;

                            case ConsoleKey.B:
                                Console.Write("uilding autobuy");
                                enabledBuildingAutoBuy ^= true;
                                Console.WriteLine(enabledBuildingAutoBuy ? " on" : " off");
                                Console.WriteLine("");
                                break;

                            case ConsoleKey.U:
                                Console.Write("pgrade autobuy");
                                enabledUpgradeAutoBuy ^= true;
                                Console.WriteLine(enabledUpgradeAutoBuy ? " on" : " off");
                                Console.WriteLine("");
                                break;

                            case ConsoleKey.E:
                                IWebElement optionsButton = driver.FindElement(By.Id("prefsButton")).FindElement(By.CssSelector(".subButton"));
                                optionsButton.Click();
                                driver.FindElement(By.CssSelector("[onclick^=\"Game.ExportSave\"]")).Click();
                                string saveCode = driver.FindElement(By.Id("textareaPrompt")).Text;
                                driver.FindElement(By.Id("promptOption0")).Click();
                                optionsButton.Click();

                                File.WriteAllText("Exported.txt", saveCode);
                                Console.WriteLine("xported session to Exported.txt");
                                Console.WriteLine("");
                                break;

                            case ConsoleKey.H:
                                ListOptions();
                                break;

                            case ConsoleKey.Q:
                                QuitVerification(driver);
                                break;

                            case ConsoleKey.Escape:
                                QuitVerification(driver);
                                break;

                        }
                    }
                }

                IWebElement WaitToFindElement(string uniqueCssSelector, int customTimeout = 10)
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(customTimeout));
                    wait.Until(d => driver.FindElements(By.CssSelector(uniqueCssSelector)).Count > 0);

                    return driver.FindElement(By.CssSelector(uniqueCssSelector));
                }
            }

            void QuitVerification(IWebDriver currendDriver)
            {
                Console.WriteLine("uit?---");
                Console.WriteLine("Press Q to leave immediately. Press any key to return.");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Q:
                        currendDriver.Quit();
                        appMode = 0;
                        break;
                    default:
                        Console.WriteLine("");
                        break;
                }
            }
            Environment.Exit(0);
        }
    }
}
