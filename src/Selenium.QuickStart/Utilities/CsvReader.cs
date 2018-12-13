using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Selenium.QuickStart.Utilities
{
    /// <summary>
    /// Static class for reading data from a default formatted CSV file (; delimiter and lines divided with carriage return and linefeed.)
    /// </summary>
    public static class CsvReader
    {
        /// <summary>
        /// Method to return data from a CSV file into a list of arrays of strings so you can build data driven with an IEnumerable with yield return new TestCaseData
        /// </summary>
        /// <param name="csvContent">A CSV file from your Properties.Resources</param>
        /// <param name="hasHeader">Optional parameter with default on true that defines if your CSV file has a header or not for skipping it into the tests</param>
        /// <returns>Returns a list of arrays of strings so you can implement data driven with an Enumerable with yield return new TestCaseData</returns>
        public static List<string[]> GetData(string csvContent, bool hasHeader = true)
        {
            List<string[]> data = new List<string[]>();
            MatchCollection matches = new Regex("(((?!\").)*\n)|(.*\".*\r\n.*\"\r\n)|(.*[^\"])").Matches(csvContent);
            foreach (Match m in matches)
            {
                if (hasHeader & matches[0] == m) { }
                else
                    data.Add(m.Value.Split(';'));
            }
            return data;
        }
    }
}
