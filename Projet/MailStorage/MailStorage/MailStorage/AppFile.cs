/// ETML
/// Author      : Marco Maziero
/// Date        : 12.05.2017
/// Description : Represents a file used in the app

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailStorage
{
    /// <summary>
    /// Files used in the app
    /// </summary>
    class AppFile
    {
        // Class variables declaration
        public string fileName;
        public string filePath;
        public DateTime fileCreationDate;
        public DateTime fileModificationDate;

        /// <summary>
        /// Class constructor, creates the file
        /// </summary>
        public AppFile(string strFileName, string strFilePath, DateTime dtCreationDate, DateTime dtModificationDate)
        {
            // Sets the variables
            fileName = strFileName;
            filePath = strFilePath;
            fileCreationDate = dtCreationDate;
            fileModificationDate = dtModificationDate;
        }
    }
}
