/// ETML
/// Author      : Marco Maziero
/// Date        : 12.05.2017
/// Description : Manages all the files manipulations

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace MailStorage
{
    /// <summary>
    /// Contains all the functions used to manages the files
    /// </summary>
    static class FilesManager
    {
        // Files informations
        struct BY_HANDLE_FILE_INFORMATION
        {
            public uint FileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }

        // Files informatiosn dlls
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        /// <summary>
        /// Updates the local files list, adds the new files and removes the deleted ones
        /// </summary>
        public static void UpdateLocalFiles()
        {
            // Adds the new files to the list
            foreach (var file in Directory.GetFiles(Globals.ROOT_DIRECTORY, "*", SearchOption.AllDirectories))
                AddLocalFileToList(file);

            // Gets the files paths from the list
            var pathList = Globals.LOCAL_FILES.Select(a=>a.filePath).ToList();

            // Goes through each file in the list
            foreach (var listFilePath in pathList)
            {
                var blnExists = false;

                // Goes through all the files in the directory
                foreach (var localFile in Directory.GetFiles(Globals.ROOT_DIRECTORY, "*", SearchOption.AllDirectories))
                {
                    // Gets the file informations
                    var fileInformations = new FileInfo(localFile);

                    // Checks if the files still exists
                    if (listFilePath == localFile.Replace(Globals.ROOT_DIRECTORY, @".") && fileInformations.LastWriteTime == Globals.LOCAL_FILES.FirstOrDefault(a=>a.filePath == listFilePath).fileModificationDate)
                        blnExists = true;
                }

                // Deletes the file if it doesn't exist anymore
                if (!blnExists)
                    Globals.LOCAL_FILES.Remove(Globals.LOCAL_FILES.FirstOrDefault(a => a.filePath == listFilePath));
            }
        }

        /// <summary>
        /// Updates the remote files list, adds the mail files in the list and removes the old ones
        /// </summary>
        public static void UpdateRemoteFiles()
        {
            // Gets all the mails subjects from the mailbox
            var mailSubjects = MailManager.GetAllMailSubjects();

            // Splits each subject and adds the file to the global list
            foreach (var subject in mailSubjects)
            {
                // Splits
                var mailInfos = subject.Split(new [] {"::"}, StringSplitOptions.None);

                // Creates the new file
                var newFile = new AppFile(mailInfos[0], mailInfos[1], DateTime.Parse(mailInfos[2]), DateTime.Parse(mailInfos[3]));

                // Checks if the file is already in the list
                if (Globals.MAIL_FILES.All(a => a.filePath != newFile.filePath))
                {
                    // The file does not exist, adds it
                    Globals.MAIL_FILES.Add(newFile);
                }
            }

            // Gets the files paths from the list
            var pathsList = Globals.MAIL_FILES.Select(a => a.filePath).ToList();

            // Goes through each file in the list
            foreach (var listFilePath in pathsList)
            {
                var blnExists = false;

                // Goes through all the files in the mailbox
                foreach (var remoteFile in mailSubjects)
                {
                    // Gets the file path
                    var filePath = remoteFile.Split(new [] { "::" }, StringSplitOptions.None)[1];

                    // Checks if the files still exists
                    if (listFilePath == filePath)
                        blnExists = true;
                }

                // Deletes the file if it doesn't exist anymore
                if (!blnExists)
                    Globals.MAIL_FILES.Remove(Globals.MAIL_FILES.FirstOrDefault(a => a.filePath == listFilePath));
            }
        }

        /// <summary>
        /// Adds the new local files to the user mailbox
        /// </summary>
        public static void AddLocalFilesToMailBox()
        {
            // Goes through the local files list
            foreach (var localFile in Globals.LOCAL_FILES)
            {
                var blnFileExists = false;

                // Goes through the remote files list
                foreach (var remoteFile in Globals.MAIL_FILES)
                {
                    if (remoteFile.filePath == localFile.filePath)
                    {
                        // Sets the add booelan to true
                        blnFileExists = true;
                    }
                }

                // If the file is not in the mailbox, adds id
                if (!blnFileExists)
                    MailManager.SendMailToStorage(localFile.fileName + "::" + localFile.filePath + "::" + localFile.fileCreationDate + "::" + localFile.fileModificationDate, ConvertFileTo64(localFile));
            }
        }

        /// <summary>
        /// Deletes the files in the mailbox that are not present in the local directory 
        /// </summary>
        public static void DeleteRemotesFilesFromLocal()
        {
            // Goes through all the mail files list
            foreach (var remoteFile in Globals.MAIL_FILES)
            {
                var blnFileExists = false;

                // Goes through all the local file list
                foreach (var localFile in Globals.LOCAL_FILES)
                {
                    // If the files exists in the other list
                    if (localFile.filePath == remoteFile.filePath)
                        blnFileExists = true;
                }

                // Deletes the file if it's not in local
                if (!blnFileExists)
                    MailManager.DeleteMailInStorage(remoteFile);
            }
        }

        /// <summary>
        /// Converts a file to base64 string
        /// </summary>
        /// <param name="fileToConvert">The file to convert</param>
        /// <returns>The file in string</returns>
        public static string ConvertFileTo64(AppFile fileToConvert)
        {
            for (;;)
            {
                try
                {
                    return Convert.ToBase64String(File.ReadAllBytes(Globals.ROOT_DIRECTORY + fileToConvert.filePath.Substring(1)));
                }
                catch
                {
                    // Ignored
                }
            }
        }

        public static void ConvertFileFrom64()
        {
            
        }

        /// <summary>
        /// Adds a new local file to the list
        /// </summary>
        /// <param name="strFilePath">The file path</param>
        private static void AddLocalFileToList(string strFilePath)
        {
            // Gets the relative file path
            var relativePath = strFilePath.Replace(Globals.ROOT_DIRECTORY, @".");

            // Gets the file's name and creation date
            var fileInformations = new FileInfo(strFilePath);
            var fileName = fileInformations.Name;
            var fileCreationDate = fileInformations.CreationTime;
            var fileModificationDate = fileInformations.LastWriteTime;

            // Checks if the file is already in the list
            if (Globals.LOCAL_FILES.All(a => a.filePath != relativePath))
            {
                // The file does not exist, adds it
                var newFile = new AppFile(fileName, relativePath, fileCreationDate, fileModificationDate);
                Globals.LOCAL_FILES.Add(newFile);
            }
        }
    }
}
