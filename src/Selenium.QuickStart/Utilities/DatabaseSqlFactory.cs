using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Selenium.QuickStart.Utilities
{
    /// <summary>
    /// Static class to make access to the method of query execution into a database of a connection string "DatabaseConnectionString" or a Mydatabase.sqlite file at your solution / project folder
    /// </summary>
    public static class DatabaseSqlFactory
    {
        /// <summary>
        /// Executes a query into a database with connection defiend on a connection string "DatabaseConnectionString" or a Mydatabase.sqlite file at your solution / project folder
        /// </summary>
        /// <param name="sql">The sql query you want to execute</param>
        /// <returns>Returns a list of arrays of strings</returns>
        public static List<string[]> ExecuteQuery(string sql)
        {
            dynamic command;

            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "MyDatabase.sqlite"))
            {
                SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
                command = new SQLiteCommand(sql, m_dbConnection);
            }
            else
            {
                SqlConnection m_dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);
                m_dbConnection.Open();
                command = new SqlCommand(sql, m_dbConnection);
            }

            DataSet ds = new DataSet();
            List<string[]> lista = new List<string[]>();
            DataTable table = new DataTable();
            table.Load(command.ExecuteReader());
            ds.Tables.Add(table);
            command.Connection.Close();

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            try
            {
                for (int r = 0; r < ds.Tables[0].Rows.Count; r++)
                {
                    string[] a = new string[ds.Tables[0].Columns.Count];
                    for (int c = 0; c < ds.Tables[0].Columns.Count; c++)
                    {
                        a[c] = ds.Tables[0].Rows[r][c].ToString();
                    }
                    lista.Add(a);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            return lista;
        }
    }
}
