/// ETML
/// Auteur      : Marco Maziero
/// Date        : 11.05.2017
/// Description : Manages all the connexions to the mails servers, send and receive

using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace MailStorage
{
    /// <summary>
    /// This class manages all the user mailbox interactions and connections
    /// </summary>
    static class MailManager
    {
        private static readonly ImapClient imapCli = new ImapClient();       // The IMAP client

        /// <summary>
        /// Connects the application to the IMAP server with the given data
        /// </summary>
        /// <param name="strServer">The IMAP server name</param>
        /// <param name="strPort">The IMAP server port</param>
        /// <param name="strUser">The user mail address</param>
        /// <param name="strPassword">The user password</param>
        /// <returns>Returns a boolean, true if the connection succedded</returns>
        public static bool ConnectIMAP(string strServer, string strPort, string strUser, string strPassword)
        {
            // Tries to connect to the server
            try
            {
                // Connects to the IMAP4 server
                imapCli.Connect(strServer, Int32.Parse(strPort), true);
                imapCli.AuthenticationMechanisms.Remove("XOAUTH2");
            }
            catch (Exception ex)
            {
                // Displays a messagebox with the error
                MessageBox.Show("Il y a eu un problème lors de la connexion IMAP4 au serveur mail.\n\n" +
                                "Vérifiez le nom du serveur IMAP et le numéro de port puis réessayez.\n\n" +
                                "Erreur : \n" + ex.Message,
                    "Erreur de connexion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Returns false
                return false;
            }

            // Tries to authenticate the user
            try
            {
                // Authenticates the user
                imapCli.Authenticate(strUser, strPassword);
            }
            catch (Exception ex)
            {
                // Displays a messagebox with the error
                MessageBox.Show("Il y a eu un problème lors de l'authentification de l'utilisateur.\n\n" +
                                "Vérifiez l'adresse mail et le mot de passe puis réessayez.\n\n" +
                                "Erreur : \n" + ex.Message,
                    "Erreur d'authentification",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Returns false
                return false;
            }

            // Returns true, the connection to the mailbox succedded
            return true;
        }

        /// <summary>
        /// Disconnects the IMAP client from the mail server
        /// </summary>
        public static void DisconnectIMAP()
        {
            // Disconnects the IMAP client
            if (imapCli.IsConnected)
                imapCli.Disconnect(true);
        }

        /// <summary>
        /// Creates the storage folder in the mailbox
        /// </summary>
        public static void CreateStorageFolder()
        {
            // Creates the MailStorage folder in the user's mailbox
            foreach (var folder in imapCli.GetFolder(imapCli.PersonalNamespaces[0]).GetSubfolders().ToList())
            {
                // If the folder already exists, returns
                if (folder.Name == Globals.MAILBOX_FOLDER)
                    return;
            }

            // Creates the folder
            imapCli.GetFolder(imapCli.PersonalNamespaces[0]).Create(Globals.MAILBOX_FOLDER, true);
        }

        /// <summary>
        /// Appends a new mail to the MailStorage folder in the user's mailbox
        /// </summary>
        /// <param name="strSubject">The mail subject</param>
        /// <param name="strBody">The mail body</param>
        public static void SendMailToStorage(string strSubject, string strBody)
        {
            // Creates the message
            var messageToSend = new MimeMessage();
            messageToSend.From.Add(new MailboxAddress(Globals.USER_MAIL_ADDRESS));
            messageToSend.To.Add(new MailboxAddress(Globals.USER_MAIL_ADDRESS));

            // Creates the message body with a builder
            var bBuilder = new BodyBuilder { TextBody = strBody };

            // Sets the subject and the body
            messageToSend.Subject = strSubject;
            messageToSend.Body = bBuilder.ToMessageBody();

            // Appends the message to the mailbox
            var messageslist = new List<MimeMessage> { messageToSend };
            var messageFlags = new List<MessageFlags> { MessageFlags.Recent };
            imapCli.GetFolder("MailStorage").Append(FormatOptions.Default, messageslist, messageFlags);
        }

        /// <summary>
        /// Deletes a specified file in the mailbox
        /// </summary>
        /// <param name="fileToDelete">The file to delete</param>
        public static void DeleteMailInStorage(AppFile fileToDelete)
        {
            // Opens the MailStorage folder
            var mailStorageFolder = imapCli.GetFolder("MailStorage");
            mailStorageFolder.Open(FolderAccess.ReadWrite);

            // Goes through all the mails informations
            foreach (var mailInfo in mailStorageFolder.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId))
            {
                // Checks if the paths matches
                if (mailInfo.Envelope.Subject.Split(new [] {"::"}, StringSplitOptions.None)[1] == fileToDelete.filePath)
                {
                    // Deletes the file
                    mailStorageFolder.AddFlags(new [] { mailInfo.UniqueId }, MessageFlags.Deleted, true);

                    // Applies the delete and closes the folder
                    mailStorageFolder.Close(true);

                    // Returns
                    return;
                }
            }
        }

        /// <summary>
        /// Gets only the subject of all the MailStorage folder mails
        /// </summary>
        /// <returns>List with all the subject strings</returns>
        public static List<string> GetAllMailSubjects()
        {
            // Variables declaration
            var subjectList = new List<string>();

            // Opens the MailStorage folder
            var mailStorageFolder = imapCli.GetFolder("MailStorage");
            mailStorageFolder.Open(FolderAccess.ReadOnly);

            // Goes through all the mails informations
            foreach (var mailInfo in mailStorageFolder.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId))
                subjectList.Add(mailInfo.Envelope.Subject);

            // Closes the folder
            mailStorageFolder.Close(false);

            // Returns the subjects list
            return subjectList;
        }

        /// <summary>
        /// Returns the mailbox informations
        /// </summary>
        /// <returns>The mailbox quota</returns>
        public static FolderQuota GetMailboxQuota() => imapCli.GetFolder("MailStorage").GetQuota();
    }
}

