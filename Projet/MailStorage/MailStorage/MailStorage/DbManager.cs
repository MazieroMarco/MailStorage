/// ETML
/// Author      : Marco Maziero
/// Date        : 11.05.2017
/// Description : Manages all the interactions with the database

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MailStorage
{
    /// <summary>
    /// Manages all the interaction with the SQLite database
    /// </summary>
    class DbManager
    {
        private readonly SQLiteConnection dbConnection;

        /// <summary>
        /// Class constructor, creates the database if it doesn't exist
        /// </summary>
        public DbManager()
        {
            // Creates the database file if it doesn't exist
            if (!File.Exists("mailstoragedb.sqlite"))
            {
                // Creates the file
                SQLiteConnection.CreateFile("mailstoragedb.sqlite");

                // Connects to the database
                dbConnection = new SQLiteConnection("Data Source=mailstoragedb.sqlite;Version=3;");

                // Opens the connection
                dbConnection.Open();

                // Creates the table
                string sqlQuery =
                    "CREATE TABLE appData" +
                    "(servername VARCHAR(100)," +
                    "serverport VARCHAR(4)," +
                    "usermail VARCHAR(250)," +
                    "userpassword VARCHAR(250)," +
                    "rootpath VARCHAR(250))";

                // Creates the command
                var sqlCommand = new SQLiteCommand(sqlQuery, dbConnection);

                // Executees the query
                sqlCommand.ExecuteNonQuery();

                dbConnection.Close();
            }
            else
            {
                // Connects to the database
                dbConnection = new SQLiteConnection("Data Source=mailstoragedb.sqlite;Version=3;");
            }
        }

        /// <summary>
        /// Inserts all the data into the database
        /// </summary>
        /// <param name="serverName">The IMAP server name</param>
        /// <param name="serverPort">The server port</param>
        /// <param name="userMail">The user mail address</param>
        /// <param name="userPassword">The user password</param>
        /// <param name="dirPath">The root directory path</param>
        public void UpdateUserData(string serverName, string serverPort, string userMail, string userPassword, string dirPath)
        {
            // Opens the connection to the database
            dbConnection.Open();

            // Gets the old path in the database
            var sqlCommand = new SQLiteCommand("SELECT * FROM appData LIMIT 1", dbConnection);
            var sqlReader = sqlCommand.ExecuteReader();

            // If ther're no rows, sets up the inital sync
            if (!sqlReader.HasRows)
            {
                Globals.NEED_INITIAL_SYNC = true;
            }

            // If ther're rows, checks if the path is the same
            while (sqlReader.Read())
            {
                if (sqlReader["rootpath"].ToString() != dirPath)
                    Globals.NEED_INITIAL_SYNC = true;
            }

            // Clears the data table
            executeSQLQuery("DELETE FROM appData");

            // Inserts the values to the table
            executeSQLQuery("INSERT INTO appData VALUES ('" + serverName + "', '" + serverPort + "', '" + userMail + "', '" + userPassword + "', '" + dirPath + "')");

            // Closes the connection
            dbConnection.Close();
        }

        /// <summary>
        /// Gets the registered user from the database
        /// </summary>
        /// <returns>Returns a list with all the data</returns>
        public List<string> GetCurrentUserData()
        {
            // Variables declaration
            var liData = new List<string>();

            // Opens the connection to the database
            dbConnection.Open();

            // Prepares the query
            var sqlCommand = new SQLiteCommand("SELECT * FROM appData LIMIT 1", dbConnection);
            var sqlReader = sqlCommand.ExecuteReader();

            // Reads the data
            while (sqlReader.Read())
            {
                liData.Add(sqlReader["servername"].ToString());
                liData.Add(sqlReader["serverport"].ToString());
                liData.Add(sqlReader["usermail"].ToString());
                liData.Add(sqlReader["userpassword"].ToString());
                liData.Add(sqlReader["rootpath"].ToString());
            }

            // Returns the data
            return liData;
        }

        /// <summary>
        /// Executes a SQL query on the database
        /// </summary>
        /// <param name="strQuery"></param>
        private void executeSQLQuery(string strQuery)
        {
            // Prepares the query
            var sqlCommand = new SQLiteCommand(strQuery, dbConnection);

            // Executees the query
            sqlCommand.ExecuteNonQuery();
        }
    }
}
