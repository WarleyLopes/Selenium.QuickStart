using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using Selenium.QuickStart.Utilities;
using System;
using System.Configuration;

namespace Selenium.QuickStart.Core
{
    internal class Reporter
    {
        private static Reporter Instance;
        private ExtentReports ExtentWrapper;
        private ExtentHtmlReporter Html;
        private ExtentTest TestInExecution;
        internal int passedTests, failedTests;

        internal static Reporter GetInstance()
        {
            if (Instance == null)
                Instance = new Reporter();

            return Instance;
        }

        internal void InitializeReport(string reportName)
        {
            if (ExtentWrapper == null)
            {
                ExtentWrapper = new ExtentReports();

                if (ConfigurationManager.AppSettings["REPORT_IS_KLOV"].Equals("1"))
                {
                    var klovReporter = new KlovReporter();

                    // specify mongoDb connection
                    klovReporter.InitMongoDbConnection("localhost", 27017);

                    // specify project ! you must specify a project, other a "Default project will be used"
                    klovReporter.ProjectName = reportName;

                    // you must specify a reportName otherwise a default timestamp will be used
                    klovReporter.ReportName =
                        ConfigurationManager.AppSettings["REPORT_DOCUMENT_TITLE"] + " - " +
                        ConfigurationManager.AppSettings["REPORT_NAME"];

                    // URL of the KLOV server
                    klovReporter.KlovUrl = "http://localhost";

                    ExtentWrapper.AttachReporter(klovReporter);
                }
                else
                {
                    string reportPath = ConfigurationManager.AppSettings["REPORT_FILE_PATH"];

                    System.IO.Directory.CreateDirectory(reportPath);

                    string reportFullPath = String.Format(reportPath + @"{0}.html", reportName + " - Automated Tests - Execution Date Time " + DateTime.Now.ToString("dd-MM-yyyyTHH-mm-ss"));

                    Html = new ExtentHtmlReporter(reportFullPath);
                    Html.Configuration().FilePath = reportFullPath;
                    Html.Configuration().DocumentTitle = ConfigurationManager.AppSettings["REPORT_DOCUMENT_TITLE"];
                    Html.Configuration().ReportName = ConfigurationManager.AppSettings["REPORT_NAME"];
                    Html.Configuration().ChartVisibilityOnOpen = true;
                    Html.Configuration().ChartLocation = ChartLocation.Top;

                    ExtentWrapper.AttachReporter(Html);
                }
            }
        }

        internal void AddTest(string category, string testName, string description)
        {
            ExtentTest testCase = this.ExtentWrapper.CreateTest(testName, description);
            testCase.AssignCategory(category);

            this.TestInExecution = testCase;
        }

        internal void PassTest(string details)
        {
            this.TestInExecution.Pass(details);
        }

        internal void FailTest(string details)
        {
            this.TestInExecution.Fail(details);
        }

        internal void GenerateReport()
        {
            if (passedTests == 0 && failedTests == 0)
            {
                this.ExtentWrapper.Flush();
            }
            else
            {
                this.ExtentWrapper.Flush();
                if (ConfigurationManager.AppSettings["REPORT_IS_KLOV"].Equals("0"))
                {
                    int porcentagem = (passedTests * 100 / (failedTests + passedTests) * 100) / 100;
                    var totalTestsFinalInfo = "(" + porcentagem + "% - " + passedTests + " de " + (passedTests + failedTests) + ") ";
                    var initialPath = Html.Configuration().FilePath;
                    var finalPathWithTestResults = Html.Configuration().FilePath.Insert(this.Html.Configuration().FilePath.IndexOf("-"), totalTestsFinalInfo);
                    System.IO.File.Move(initialPath, finalPathWithTestResults);
                    EmailSender.SendEmail(
                        ConfigurationManager.AppSettings["EMAIL_DESTINATIONS"].Split(';'),
                        totalTestsFinalInfo + " - " + ConfigurationManager.AppSettings["REPORT_DOCUMENT_TITLE"] + " - " + ConfigurationManager.AppSettings["REPORT_NAME"] + " - " + DateTime.Now,
                        finalPathWithTestResults,
                        ConfigurationManager.AppSettings["EMAIL_BODY"]
                    );
                }
            }
        }
    }
}