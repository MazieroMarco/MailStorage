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

namespace MailStorage
{
    static class MailManager
    {
        private static ImapClient imapCli = new ImapClient();       // The IMAP client

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
    }
}

