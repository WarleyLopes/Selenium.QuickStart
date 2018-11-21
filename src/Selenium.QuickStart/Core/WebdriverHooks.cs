using System;
using System.Configuration;
using System.Net;
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
    /// <summary>
    /// Class to interact with the WebDriver in a more dynamic and easier way with built in features
    /// </summary>
    public static class WebDriverHooks
    {
        private static IWebDriver w_Driver;
        /// <summary>
        /// Common Selenium WebDriver object for webdriver interactions
        /// </summary>
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

        private static readonly string UrlBase = ConfigurationManager.AppSettings["URL_BASE"];

        internal static IWebDriver Initialize()
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

            if(ConfigurationManager.AppSettings["VIDEO_RECORDING_ENABLED"].Equals("1"))
                VideoRecorder.CreateRecording(
                    TestContext.CurrentContext.Test.ClassName +
                    " - " + TestContext.CurrentContext.Test.Name +
                    " - " + DateTime.Now.ToString("dd-MM-yyyyTHH-mm-ss"));

            return driver;
        }


        private static IWebDriver GetFirefoxDriver()
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            IWebDriver driver = new FirefoxDriver();
            try
            {
                driver = new RemoteWebDriver(
                    new Uri(ConfigurationManager.AppSettings["SELENIUM_GRID_URL"]),
                    firefoxOptions);
            }
            catch (WebException e)
            {
                if(e.Message.ToUpper().Contains("UNABLE TO CONNECT"))
                {
                    try
                    {
                        driver = new FirefoxDriver(Environment.GetEnvironmentVariable("GeckoWebDriver"));
                    }
                    catch (DriverServiceNotFoundException)
                    {
                        driver = new FirefoxDriver();
                    }
                }
            }
            return driver;
        }

        private static IWebDriver GetChromeDriver()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            IWebDriver driver = new ChromeDriver();
            try
            {
                driver = new RemoteWebDriver(
                    new Uri(ConfigurationManager.AppSettings["SELENIUM_GRID_URL"]),
                    chromeOptions);
            }
            catch (WebException e)
            {
                if (e.Message.ToUpper().Contains("UNABLE TO CONNECT"))
                {
                    try
                    {
                        driver = new ChromeDriver(Environment.GetEnvironmentVariable("ChromeWebDriver"));
                    }
                    catch (DriverServiceNotFoundException)
                    {
                        driver = new ChromeDriver();
                    }
                }
            }
            return driver;
        }

        private static IWebDriver GetIEDriver()
        {
            InternetExplorerOptions ieOptions = new InternetExplorerOptions();
            IWebDriver driver = new InternetExplorerDriver();
            try
            {
                driver = new RemoteWebDriver(
                new Uri(ConfigurationManager.AppSettings["SELENIUM_GRID_URL"]),
                ieOptions);
            }
            catch (WebException e)
            {
                if (e.Message.ToUpper().Contains("UNABLE TO CONNECT"))
                {
                    try
                    {
                        driver = new InternetExplorerDriver(Environment.GetEnvironmentVariable("IEWebDriver"));
                    }
                    catch (DriverServiceNotFoundException)
                    {
                        driver = new InternetExplorerDriver();
                    }
                }
            }
            return driver;
        }

        /// <summary>
        /// Improved WebDriver.FindElement with a WebDriverWait Until the DEFAULT_TIMEOUT defined on the app.config of your project
        /// </summary>
        /// <param name="by">A common OpenQA.Selenium.By type parameter for finding your page element by the possible ways of such object</param>
        /// <returns></returns>
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

        /// <summary>
        /// Simplifier for executing javascript codes as needed
        /// </summary>
        /// <param name="script">
        /// The javascript code to execute on the page.
        /// </param>
        /// <returns></returns>
        public static object ExecuteJavaScript(string script)
        {
            return ((IJavaScriptExecutor)Driver).ExecuteScript(script);
        }
    }
}
