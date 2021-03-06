﻿/// ETML
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
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using MimeKit;

namespace MailStorage
{
    /// <summary>
    /// Contains all the functions used to manages the files
    /// </summary>
    static class FilesManager
    {
        /// <summary>
        /// Downloads all the mails and create the corresponding files in the root folder
        /// </summary>
        public static void InitialSynchronisation()
        {
            // Checks if there're files in the root folder
            if (Directory.GetFiles(Globals.ROOT_DIRECTORY, "*", SearchOption.AllDirectories).Length > 0 || Directory.GetDirectories(Globals.ROOT_DIRECTORY, "*").Length > 0)
            {
                // Displays the question message about the files
                var userChoice = MessageBox.Show("Vous avez changé d'adresse mail ou de dossier racine.\n" +
                                "Voulez-vous que ces fichiers du dossier soient synchronisés avec la boite mail ou supprimés ?\n\n" +
                                "(Cliquez sur OUI pour synchroniser, sur NON pour les supprimer et sur ANNULER pour quitter l'application)",
                    "Fichiers dans le dossier racine",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                switch (userChoice)
                {
                    case DialogResult.No:

                        // Deletes all the files in the root folder
                        foreach (var file in Directory.GetFiles(Globals.ROOT_DIRECTORY, "*", SearchOption.AllDirectories))
                        {
                            // If no root folder, abort
                            if (!Directory.Exists(Globals.ROOT_DIRECTORY))
                                return;

                            // Tries to delete the file
                            try
                            {
                                // Gets the file information
                                var infos = new FileInfo(file);

                                // Sets the file status label
                                Globals.mainWindow.UpdateCurrentFile("Suppression du fichier\n" + infos.Name);

                                // Deletes the file
                                File.Delete(file);
                            }
                            catch (Exception e)
                            {
                                // Gets the file information
                                var infos = new FileInfo(file);

                                // Displays the error message
                                MessageBox.Show("Impossible de supprimer le fichier " + infos.Name + "\n\n" +
                                                "Erreur : " + e.Message);
                            }
                        }

                        // Deletes all the directories in the root folder
                        foreach (var folder in Directory.GetDirectories(Globals.ROOT_DIRECTORY, "*"))
                        {
                            // Tries to delete the file
                            try
                            {
                                // Gets the file information
                                var infos = new DirectoryInfo(folder);

                                // Sets the file status label
                                Globals.mainWindow.UpdateCurrentFile("Suppression du dossier\n" + infos.Name);

                                // Deletes the folder
                                Directory.Delete(folder, true);
                            }
                            catch (Exception e)
                            {
                                // Gets the file information
                                var infos = new DirectoryInfo(folder);

                                // Displays the error message
                                MessageBox.Show("Impossible de supprimer le dossier " + infos.Name + "\n\n" +
                                                "Erreur : " + e.Message);
                            }
                        }
                        break;

                    case DialogResult.Cancel:

                        // Exits the application
                        Application.Exit();

                        break;
                }
            }

            // Gets the directories list
            var allDirectories = MailManager.GetOneMail().TextBody.Split(new[] { "::" }, StringSplitOptions.None).ToList();

            // Creates all the directories in the root folder
            foreach (var folder in allDirectories)
            {
                // Sets the file status label
                Globals.mainWindow.UpdateCurrentFile("Création du dossier\n" + folder);

                // Creates the directory
                Directory.CreateDirectory(Globals.ROOT_DIRECTORY + folder.Substring(1));
            }

            // Gets all the mails from the mailbox
            var allMails = MailManager.GetAllMails();

            // Adds each file per mail
            foreach (var mail in allMails)
            {
                // If no root folder, abort
                if (!Directory.Exists(Globals.ROOT_DIRECTORY))
                    return;

                // Splits the mail subject to get info
                var mailInfos = mail.Subject.Split(new[] { "::" }, StringSplitOptions.None);

                // Gets the mail full path
                var fullPath = Globals.ROOT_DIRECTORY + mailInfos[1].Substring(1);

                // Creates the file from base64
                new FileInfo(fullPath).Directory.Create();
                File.WriteAllBytes(fullPath, Convert.FromBase64String(mail.TextBody));

                // Sets the file creation and edit time
                File.SetCreationTime(fullPath, DateTime.Parse(mailInfos[2]));
                File.SetLastWriteTime(fullPath, DateTime.Parse(mailInfos[3]));
            }
        }

        /// <summary>
        /// Updates the local folder state by downloading all the missing files from the mailbox and deleting the files that have to be deleted
        /// </summary>
        public static void StartUpdateFromMailBox()
        {
            // Goes through each local file
            foreach (var localFile in Globals.LOCAL_FILES)
            {
                // Checks if the file is not in the mail files
                if (!Globals.MAIL_FILES.Any(a => a.filePath == localFile.filePath &&
                                                 a.fileModificationDate.ToString() == localFile.fileModificationDate.ToString()))
                {
                    // Gets the mail full path
                    var fullPath = Globals.ROOT_DIRECTORY + localFile.filePath.Substring(1);

                    // Deletes the file
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch (Exception e)
                        {
                            // Gets the file information
                            var infos = new FileInfo(fullPath);

                            // Displays the error message
                            MessageBox.Show("Impossible de supprimer le fichier " + infos.Name + "\n\n" +
                                            "Erreur : " + e.Message);
                        }
                    }
                }
            }

            // Goes through each mail list element
            foreach (var remoteFile in Globals.MAIL_FILES)
            {
                // Checks if the remote file is in the local folder
                if (!Globals.LOCAL_FILES.Any(a => a.filePath == remoteFile.filePath &&
                                                  a.fileModificationDate.ToString() == remoteFile.fileModificationDate.ToString()))
                {
                    // The file doesn't exist or is more recent, downloads it
                    var downloadedFile = MailManager.GetOneMail(false, remoteFile);

                    // If no root folder, abort
                    if (!Directory.Exists(Globals.ROOT_DIRECTORY))
                        return;

                    // Splits the mail subject to get info
                    var mailInfos = downloadedFile.Subject.Split(new[] {"::"}, StringSplitOptions.None);

                    // Gets the mail full path
                    var fullPath = Globals.ROOT_DIRECTORY + mailInfos[1].Substring(1);

                    // Deletes the old file if exists
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch (Exception e)
                        {
                            // Gets the file information
                            var infos = new FileInfo(fullPath);

                            // Displays the error message
                            MessageBox.Show("Impossible de supprimer le fichier " + infos.Name + "\n\n" +
                                            "Erreur : " + e.Message);

                            // Sets a unique name for the file
                            Random rnd = new Random();
                            fullPath = Globals.ROOT_DIRECTORY + mailInfos[1].Substring(1, mailInfos[1].Length - 4) +
                                       "_more_recent_" + rnd.Next(10000, 90000) +
                                       mailInfos[1].Substring(mailInfos[1].Length - 4);
                        }
                    }

                    // Creates the file from base64
                    new FileInfo(fullPath).Directory.Create();
                    File.WriteAllBytes(fullPath, Convert.FromBase64String(downloadedFile.TextBody));

                    // Sets the file creation and edit time
                    File.SetCreationTime(fullPath, DateTime.Parse(mailInfos[2]));
                    File.SetLastWriteTime(fullPath, DateTime.Parse(mailInfos[3]));
                }
            }
        }

        /// <summary>
        /// Updates the local files list, adds the new files and removes the deleted ones
        /// </summary>
        public static void UpdateLocalFiles()
        {
            // Gets the files paths from the list
            var pathList = Globals.LOCAL_FILES.Select(a=>a.filePath).ToList();

            // Goes through each file in the list
            foreach (var listFilePath in pathList)
            {
                var blnExists = false;

                // Gets all the visible files in the directory
                DirectoryInfo directory = new DirectoryInfo(Globals.ROOT_DIRECTORY);
                FileInfo[] allFiles = directory.GetFiles("*", SearchOption.AllDirectories);
                var filteredFiles = allFiles.Where(a => !a.Attributes.HasFlag(FileAttributes.Hidden));

                // Goes through all the visible files in the directory
                foreach (var localFileInfo in filteredFiles)
                {
                    // Gets the file path
                    var localFile = localFileInfo.ToString();

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

            // Adds the new files to the list
            foreach (var file in Directory.GetFiles(Globals.ROOT_DIRECTORY, "*", SearchOption.AllDirectories))
                AddLocalFileToList(file);
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
                if (!Globals.MAIL_FILES.Any(a => a.filePath == newFile.filePath && a.fileModificationDate == newFile.fileModificationDate))
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
                    var fileInformations = remoteFile.Split(new [] { "::" }, StringSplitOptions.None);

                    // Checks if the files still exists
                    if (listFilePath == fileInformations[1] && Globals.MAIL_FILES.FirstOrDefault(a => a.filePath == listFilePath).fileModificationDate.ToString() == fileInformations[3])
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
                    if (remoteFile.filePath == localFile.filePath && remoteFile.fileModificationDate.ToString() == localFile.fileModificationDate.ToString())
                    {
                        // Sets the add booelan to true
                        blnFileExists = true;
                    }
                }

                // If the file is not in the mailbox, adds id
                if (!blnFileExists)
                {
                    // Sets the file status label
                    Globals.mainWindow.UpdateCurrentFile("Envoi du fichier\n" + localFile.fileName);

                    // Sends the file to the storage
                    MailManager.SendMailToStorage(localFile.fileName +
                                                  "::" +
                                                  localFile.filePath +
                                                  "::" +
                                                  localFile.fileCreationDate +
                                                  "::" +
                                                  localFile.fileModificationDate,
                                                  ConvertFileTo64(localFile));
                }
            }
        }

        /// <summary>
        /// Deletes the files in the mailbox that are not present in the local directory 
        /// </summary>
        public static void DeleteRemoteFilesFromLocal()
        {
            // Goes through all the mail files list
            foreach (var remoteFile in Globals.MAIL_FILES)
            {
                var blnFileExists = false;

                // Goes through all the local file list
                foreach (var localFile in Globals.LOCAL_FILES)
                {
                    // If the files exists in the other list
                    if (localFile.filePath == remoteFile.filePath && remoteFile.fileModificationDate.ToString() == localFile.fileModificationDate.ToString())
                        blnFileExists = true;
                }

                // Deletes the file if it's not in local
                if (!blnFileExists)
                {
                    // Sets the file status label
                    Globals.mainWindow.UpdateCurrentFile("Suppression du fichier\n" + remoteFile.fileName);

                    // Deletes the file
                    MailManager.DeleteMailInStorage(remoteFile);
                }
            }
        }

        /// <summary>
        /// Gets all the directories in the folder and crats the updates version of the index mail
        /// </summary>
        public static void UpdateDirectoriesIndex()
        {
            // Sets the file status label
            Globals.mainWindow.UpdateCurrentFile("Indexation des dossiers");

            // Gets all the folders in the root
            var allDirectories = Directory.GetDirectories(Globals.ROOT_DIRECTORY, "*", SearchOption.AllDirectories);

            // Defines the index mail content
            string strMailContent = "";

            // Adds each folder in the content
            foreach (var folder in allDirectories)
            {
                strMailContent += folder.Replace(Globals.ROOT_DIRECTORY, @".") + "::";
            }

            // Deletes the last "::"
            if (strMailContent.Length > 0)
                strMailContent = strMailContent.Substring(0, strMailContent.Length - 2);

            // Sends the mail to the mailbox
            MailManager.UpdateIndexMail(strMailContent);
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
                    byte[] fileData;

                    using (var fs = File.Open(Globals.ROOT_DIRECTORY + fileToConvert.filePath.Substring(1), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var binaryReader = new BinaryReader(fs);
                        fileData = binaryReader.ReadBytes((int)fs.Length);
                    }

                    return Convert.ToBase64String(fileData);
                }
                catch(Exception ex)
                {
                    // Ignored
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

            // Gets the file's name and creation date
            var fileInformations = new FileInfo(strFilePath);
            var fileName = fileInformations.Name;
            var fileCreationDate = fileInformations.CreationTime;
            var fileModificationDate = fileInformations.LastWriteTime;
            var fileMbSize = fileInformations.Length / 1024 / 1024;

            // Checks if the file size is bigger than 10 MB
            if (fileMbSize > 10)
            {
                // Displays the error message
                MessageBox.Show("Le fichier \"" + fileName + "\" fait plus de 10 MO et ne peut être synchronisé avec la boite mail.\n\n" +
                                "Veuillez le retirer du dossier racine pour éviter d'autres messages d'erreur.",
                                "Fichier trop volumineux",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                return;
            }

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
