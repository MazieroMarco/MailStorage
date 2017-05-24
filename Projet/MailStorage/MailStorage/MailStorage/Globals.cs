/// ETML
/// Auteur      : Marco Maziero
/// Date        : 11.05.2017
/// Description : Contains all the program globals and constants

using System.Collections.Generic;

namespace MailStorage
{
    /// <summary>
    /// Here are all the program globals and constans
    /// </summary>
    static class Globals
    {
        public static MailStorage mainWindow = null;         // The main window
        public static LoginWindow loginWindow = null;        // the login window

        public static string IMAP_SERVER_NAME = "";          // The IMAP server name
        public static string IMAP_SERVER_PORT = "";          // The IMAP server port
        public static string USER_MAIL_ADDRESS = "";         // The user mail address
        public static string USER_MAIL_PASSWORD = "";        // The user mail password
        public static string ROOT_DIRECTORY = "";            // The chosen root floder

        public static string MAILBOX_FOLDER = "MailStorage"; // The name of the folder in the user's mailbox
        public static string PASSWORD_KEY = "ugAFZdmmPyBZXzQvXLsptFFNAYVx7sZrzrHquTmU"; // The password decrypt key

        public static List<AppFile> LOCAL_FILES = new List<AppFile>();              // The list with all the local files paths
        public static List<AppFile> MAIL_FILES = new List<AppFile>();               // The list with all the remote mail files paths

        public static bool NEED_INITIAL_SYNC = false;        // Used to execute the initial sync in the root folder
    }
}
