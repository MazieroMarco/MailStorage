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
        public string fileUniqueId;
        public DateTime fileCreationDate;

        /// <summary>
        /// Class constructor, creates the file
        /// </summary>
        public AppFile(string strFileName, string strFilePath, string strFileId, DateTime dtCreationDate)
        {
            // Sets the variables
            fileName = strFileName;
            filePath = strFilePath;
            fileUniqueId = strFileId;
            fileCreationDate = dtCreationDate;
        }
    }
}
