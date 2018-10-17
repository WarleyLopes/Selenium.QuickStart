using System;
using System.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.QuickStart.Utilities;

namespace Selenium.QuickStart.Core
{
    public class WebDriverHooks
    {
        private static IWebDriver w_Driver;
        public static IWebDriver Driver
        {
            get
            {
                if (w_Driver != null && (((OpenQA.Selenium.Remote.RemoteWebDriver)w_Driver).SessionId != null))
                    return w_Driver;
                else
                {
                    w_Driver = Initialize();
                    return w_Driver;
                }
            }
            set { w_Driver = Initialize(); }
        }

        private static string UrlBase = ConfigurationManager.AppSettings["URL_BASE"];

        public static IWebDriver Initialize()
        {
            IWebDriver driver = null;
            var browser = ConfigurationManager.AppSettings["BROWSER"];

            if (browser.Equals("Firefox"))
            {
                driver = GetFirefoxDriver();

            }
            else if (browser.Equals("Chrome"))
            {
                driver = GetChromeDriver();

            }
            else if (browser.Equals("InternetExplorer"))
            {
                driver = GetIEDriver();
            }


            // Initialize base URL and maximize browser
            driver.Url = UrlBase;
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Double.Parse(ConfigurationManager.AppSettings["DEFAULT_TIMEOUT"]));

            if(ConfigurationManager.AppSettings["DEFAULT_TIMEOUT"].Equals("1"))
                VideoRecorder.CreateRecording(
                    TestContext.CurrentContext.Test.ClassName +
                    " - " + TestContext.CurrentContext.Test.Name +
                    " - " + DateTime.Now.ToString("dd-MM-yyyyTHH-mm-ss"));

            return driver;
        }


        private static IWebDriver GetFirefoxDriver()
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            IWebDriver driver = new RemoteWebDriver(
                new Uri(ConfigurationManager.AppSettings["SELENIUM_GRID_URL"]),
                firefoxOptions);
            return driver;
        }

        private static IWebDriver GetChromeDriver()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            IWebDriver driver = new RemoteWebDriver(
                new Uri(ConfigurationManager.AppSettings["SELENIUM_GRID_URL"]),
                chromeOptions);
            return driver;
        }

        private static IWebDriver GetIEDriver()
        {
            InternetExplorerOptions ieOptions = new InternetExplorerOptions();
            IWebDriver driver = new RemoteWebDriver(
                new Uri(ConfigurationManager.AppSettings["SELENIUM_GRID_URL"]),
                ieOptions);
            return driver;
        }

        public static IWebElement WaitForAndFindElement(By by)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(Double.Parse(ConfigurationManager.AppSettings["DEFAULT_TIMEOUT"])));

            return wait.Until(webDriver =>
            {
                if (webDriver.FindElement(by).Displayed)
                {
                    return webDriver.FindElement(by);
                }
                return null;
            });
        }

        public static object ExecuteJavaScript(string script)
        {
            return ((IJavaScriptExecutor)Driver).ExecuteScript(script);
        }
    }
}
