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

            // Deletes the files that doesn't exist anymore
            var nbFiles = Globals.LOCAL_FILES.Count;

            for (var i = 0; i < nbFiles; i++)
            {
                // Goes through all the files in the list
                foreach (var localFile in Directory.GetFiles(Globals.ROOT_DIRECTORY, "*", SearchOption.AllDirectories))
                {
                    // Gets the file id
                    var stream = File.Open(localFile, FileMode.Open);
                    BY_HANDLE_FILE_INFORMATION hInfo;
                    GetFileInformationByHandle(stream.SafeFileHandle, out hInfo);
                    var fileId = hInfo.FileIndexHigh.ToString() + hInfo.FileIndexLow.ToString();
                    stream.Close();

                    // Checks if the files still exists
                    if (Globals.LOCAL_FILES.All(a => a.fileUniqueId != fileId))
                        Globals.LOCAL_FILES.Remove(Globals.LOCAL_FILES.Find(a => a.fileUniqueId == fileId));
                }
            }
        }

        /// <summary>
        /// Adds a new local file to the list
        /// </summary>
        /// <param name="strFilePath">The file path</param>
        private static void AddLocalFileToList(string strFilePath)
        {
            // Gets the relative file path
            var relativePath = strFilePath.Replace(Globals.ROOT_DIRECTORY, @".");

            // Gets the file unique id
            var stream = File.Open(strFilePath, FileMode.Open);
            BY_HANDLE_FILE_INFORMATION hInfo;
            GetFileInformationByHandle(stream.SafeFileHandle, out hInfo);
            var fileId = hInfo.FileIndexHigh.ToString() + hInfo.FileIndexLow.ToString();
            stream.Close();

            // Gets the file's name and creation date
            var fileInformations = new FileInfo(strFilePath);
            var fileName = fileInformations.Name;
            var fileCreationDate = fileInformations.CreationTime;

            // Checks if the file is already in the list
            if (Globals.LOCAL_FILES.All(a => a.fileUniqueId != fileId))
            {
                // The file does not exist, adds it
                var newFile = new AppFile(fileName, relativePath, fileId, fileCreationDate);
                Globals.LOCAL_FILES.Add(newFile);
            }
        }

        private static void AddMailFileToList(string strFilePath)
        {
            
        }
        public static void GetMailboxFiles()
        {
            
        }
    }
}
