using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using Selenium.QuickStart.Attributes;
using Selenium.QuickStart.Utilities;

namespace Selenium.QuickStart.Core
{

    /// <summary>
    /// Base class for common background tasks which should be inherited by each of your test classes
    /// <remarks>
    /// e.g:
    ///  using NUnit.Framework;
    ///  using Selenium.QuickStart.Attributes;
    ///  using Selenium.QuickStart.Core;
    ///  namespace YourSolutionProject.Tests
    ///  {
    ///  	[TestFixture]
    ///  	public class LoginTests : TestBase
    ///  	{
    ///       	[PageObject] LoginPage _LoginPage;
    ///       	
    ///         [Test]
    ///         public void Test_ValidaNavegacaoAoLoginPage()
    ///         {
    ///          		_LoginPage.NavigateToPage();
    ///          		Assert.That(_LoginPage.IsOnLoginPage());
    ///         }
    ///  	}
    ///  }
    /// </remarks>
    /// </summary>
    public class TestBase
    {
        internal ProxyGenerator ProxyGenerator;

        /// <summary>
        /// Customized constructor to automatically collect and inject page objects of each page class that inherit this class
        /// </summary>
        public TestBase()
        {

            this.ProxyGenerator = new ProxyGenerator();

            InjectPageObjects(CollectPageObjects(), null);

        }

        internal void InjectPageObjects(List<FieldInfo> fields, IInterceptor proxy)
        {
            foreach (FieldInfo field in fields)
            {
                field.SetValue(this, ProxyGenerator.CreateClassProxy(field.FieldType, proxy));
            }
        }

        internal List<FieldInfo> CollectPageObjects()
        {
            List<FieldInfo> fields = new List<FieldInfo>();

            foreach (FieldInfo field in this.GetType().GetRuntimeFields())
            {
                if (field.GetCustomAttribute(typeof(PageObject)) != null)
                    fields.Add(field);
            }

            return fields;
        }
        
        [OneTimeSetUp]
        internal void OneTimeSetup()
        {
            Reporter.GetInstance().InitializeReport(this.ToString().Split('.')[0]);
        }

        [SetUp]
        internal void SetupTest()
        {
            string testCategory = TestContext.CurrentContext.Test.ClassName;
            string testName = TestContext.CurrentContext.Test.Name;
            string testDescription = (string)TestContext.CurrentContext.Test.Properties.Get("Description");
            Reporter.GetInstance().AddTest(testCategory, testName, testDescription);
        }

        [TearDown]
        internal void TearDownTest()
        {

            if (ConfigurationManager.AppSettings["VIDEO_RECORDING_ENABLED"].Equals("1"))
                VideoRecorder.EndRecording();

            //Prepara result block para o report
            string testResult = String.Format("<p>{0}</p>", TestContext.CurrentContext.Result.Message);
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            string stackTrace = String.Format("<p>{0}</p>",TestContext.CurrentContext.Result.StackTrace);
            string screenshotBytes = ScreenShot.CaptureAsBase64EncodedString();
            string imgTag = "<br/>" +
                            "<img src='data:image/jpg; base64, " + screenshotBytes+ "' " +
                            "style='width:100%'>";
            string fullTestResult =
                    testResult +
                    stackTrace +
                    imgTag;

            if (ConfigurationManager.AppSettings["DEFAULT_TIMEOUT"].Equals("1"))
            {
                string videoTag = "<br/>" +
                                  "<video controls style='width:100%'> " +
                                  "<source type='video/mp4' src='data:video/mp4;base64," + VideoRecorder.GetVideoRecordedAsBase64StringAndDeleteLocalFile() + "'> " +
                                  "</video>";
                fullTestResult +=
                    videoTag;
            }

            switch (status)
            {
                case NUnit.Framework.Interfaces.TestStatus.Failed:
                    Reporter.GetInstance().FailTest(fullTestResult);
                    Reporter.GetInstance().failedTests++;
                    break;
                default:
                    Reporter.GetInstance().PassTest(fullTestResult);
                    Reporter.GetInstance().passedTests++;
                    break;
            }

            WebDriverHooks.Driver.Quit();
        }

        [OneTimeTearDown]
        internal void OneTimeTearDown()
        {
            Reporter.GetInstance().GenerateReport();
        }
    }
}