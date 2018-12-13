using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using NUnit.Framework;
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

        /// <summary>
        /// Method automatically used on the start of the test suite to initialize the report
        /// </summary>
        /// <param name="reportName"></param>
        public void InitializeReport(string reportName)
        {
            if (ExtentWrapper == null)
            {
                ExtentWrapper = new ExtentReports();
                bool reportIsKlov = false;
                try
                {
                    reportIsKlov = ConfigurationManager.AppSettings["REPORT_IS_KLOV"].Equals("1");
                    if (reportIsKlov)
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
                catch (ConfigurationErrorsException)
                {

                }
            }
        }

        internal void AddTest(string category, string testName, string description)
        {
            try
            {
                ExtentTest testCase = this.ExtentWrapper.CreateTest(testName, description);
                testCase.AssignCategory(category);

                this.TestInExecution = testCase;
            }
            catch (InvalidOperationException)
            {

            }
        }

        internal void PassTest(string details)
        {
            try
            {
                this.TestInExecution.Pass(details);
            }
            catch (InvalidOperationException)
            {

            }
        }

        internal void FailTest(string details)
        {
            try
            {
                this.TestInExecution.Fail(details);
            }
            catch (InvalidOperationException)
            {

            }
        }

        internal void GenerateReport()
        {
            try
            {
                this.ExtentWrapper.Flush();

                bool reportIsKlov = false;
                try
                {
                    reportIsKlov = ConfigurationManager.AppSettings["REPORT_IS_KLOV"].Equals("1");
                }
                catch (ConfigurationErrorsException)
                {

                }

                if (!reportIsKlov)
                {
                    if (TestContext.CurrentContext.Test.Name.Contains("Z99999_SeleniumQuickStartTestFinishTasks"))
                    {
                        //Manter nome fixo para o dia, analisar validação para ver se é o último teste a ser executado para envio do e-mail
                        Console.WriteLine(TestContext.Parameters);
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
            catch (InvalidOperationException)
            {

            }
        }
    }
}