/// ETML
/// Author      : Marco Maziero
/// Date        : 10.05.2017
/// Description : Login window, IMAP connect and saves user

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsharpAes;

namespace MailStorage
{
    /// <summary>
    /// The window main class
    /// </summary>
    public partial class LoginWindow : Form
    {
        // Window interaction variables
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // Class variables
        private Task connectImap;

        /// <summary>
        /// Class constructor
        /// </summary>
        public LoginWindow()
        {
            // Initializes all the window components
            InitializeComponent();

            // Sets the window icon
            this.Icon = new Icon("appIcon.ico");

            // Adds this window to the globals
            Globals.loginWindow = this;
        }

        /// <summary>
        /// Called when the window is first shown to the screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            // Checks the existing database user
            FillFormWithExistingUser();
        }

        /// <summary>
        /// Validates all the form fields for the connexion to the mailbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ValidateConnection(object sender, EventArgs e)
        {
            // Variables declaration
            var blnFormIsvalid = true;      // If true, the connection form is valid

            // Resets all the error messages
            serverErrorLabel.Text = "";
            portErrorLabel.Text = "";
            mailErrorLabel.Text = "";
            passwordErrorLabel.Text = "";
            pathErrorLabel.Text = "";

            // Validates the server name
            if (serverTextBox.Text == "")
            {
                // Displays the error message and disables the valid boolean
                serverErrorLabel.Text = "Entrez un serveur";
                blnFormIsvalid = false;
            }

            // Validates the port number
            if (portTextBox.Text == "")
            {
                // Displays the error message and disables the valid boolean
                portErrorLabel.Text = "Entrez un port";
                blnFormIsvalid = false;
            }

            // Validates the mail address
            if (mailTextBox.Text == "")
            {
                // Displays the error message and disables the valid boolean
                mailErrorLabel.Text = "Entrez une adresse mail";
                blnFormIsvalid = false;
            }
            else
            {
                // Checks if the mail address is valid
                Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                        + "@"
                                        + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
                Match match = regex.Match(mailTextBox.Text);

                // Displays the error message
                if (!match.Success)
                {
                    mailErrorLabel.Text = "Cette adresse mail n'est pas valide";
                    blnFormIsvalid = false;
                }
            }

            // Validates the user password
            if (passwordTextBox.Text == "")
            {
                // Displays the error message and disables the valid boolean
                passwordErrorLabel.Text = "Entrez un mot de passe";
                blnFormIsvalid = false;
            }

            // Validates the root directory path
            if (pathTextBox.Text == "")
            {
                // Displays the error message and disables the valid boolean
                pathErrorLabel.Text = "Entrez un chemin de dossier racine";
                blnFormIsvalid = false;
            }
            else
            {
                // Checks if the path is valid
                if (!Directory.Exists(pathTextBox.Text))
                {
                    // Displays the error message
                    pathErrorLabel.Text = "Ce chemin n'est pas valide";
                    blnFormIsvalid = false;
                }
            }

            // Checks the valid boolean
            if (blnFormIsvalid)
            {
                // Displays the loading elements on the screen
                loadingBack.Visible = true;
                loadingImage.Visible = true;
                loadingLabel.Visible = true;

                // Defines the connection result variable
                bool connectionResult = false;

                // Starts the connect task
                connectImap = Task.Factory.StartNew(() =>
                {
                    // Connects to the imap server and gets the bool result
                    connectionResult = MailManager.ConnectIMAP(serverTextBox.Text, portTextBox.Text, mailTextBox.Text, passwordTextBox.Text);
                });

                // Waits for the task to finish
                await connectImap;

                // If the connection result is true, goes on, if not, goes back to the login window. Also creates the 
                if (connectionResult && MailManager.CreateStorageFolder())
                {
                    // Crypts the user password
                    var aesPwdKey = new PasswordAes(Globals.PASSWORD_KEY);
                    var cryptedPwd = aesPwdKey.Encrypt(passwordTextBox.Text); 

                    // Connection successful, saves the user data into the database
                    var db = new DbManager();
                    db.UpdateUserData(serverTextBox.Text, portTextBox.Text, mailTextBox.Text, cryptedPwd, pathTextBox.Text);

                    // Hides this window and displays the main one
                    Globals.loginWindow.Hide();

                    // Adds the values to the globals
                    Globals.IMAP_SERVER_NAME = serverTextBox.Text;
                    Globals.IMAP_SERVER_PORT = portTextBox.Text;
                    Globals.USER_MAIL_ADDRESS = mailTextBox.Text;
                    Globals.USER_MAIL_PASSWORD = cryptedPwd;
                    Globals.ROOT_DIRECTORY = pathTextBox.Text;

                    // Creates the window if it doesn't exist
                    if (Globals.mainWindow == null)
                        Globals.mainWindow = new MailStorage();
                    
                    // Places the login window
                    Globals.mainWindow.Location = Globals.loginWindow.Location;

                    // Shows the window
                    Globals.mainWindow.Show();

                    // Initializes the main window
                    Globals.mainWindow.InitializeWindow();
                }
                else
                {
                    // Disconnects the client if connected
                    MailManager.DisconnectIMAP();
                }

                // Hides the loading elements
                loadingBack.Visible = false;
                loadingImage.Visible = false;
                loadingLabel.Visible = false;
            }
        }

        /// <summary>
        /// Checks in the database for any existing profile
        /// </summary>
        private void FillFormWithExistingUser()
        {
            // Connects to the database
            var db = new DbManager();

            // Gets the data into the list
            List<string> liData = db.GetCurrentUserData();

            if (liData.Count > 0)
            {
                // Decrypts the password
                var aesPwdKey = new PasswordAes(Globals.PASSWORD_KEY);
                var decryptedPwd = aesPwdKey.Decrypt(liData[3]);

                // Fills the fields
                serverTextBox.Text = liData[0];
                portTextBox.Text = liData[1];
                mailTextBox.Text = liData[2];
                passwordTextBox.Text = decryptedPwd;
                pathTextBox.Text = liData[4];

                // Calls the validate function
                ValidateConnection(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Displays a window where the user can select a local folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDirectory(object sender, EventArgs e)
        {
            // New FolderBrowserDialog instance
            var folderDialog = new FolderBrowserDialog();

            // Sets initial selected folder
            folderDialog.SelectedPath = pathTextBox.Text;

            // Shows the "Make new folder" button
            folderDialog.ShowNewFolderButton = true;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                // Successful select
                pathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Used to move the window when the left click is pressed on the upper bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveWindow(object sender, MouseEventArgs e)
        {
            // When the left button is pressed, moves the window
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        /// <summary>
        /// Stops the application execution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApplication(object sender, MouseEventArgs e)
        {
            // Exits the application
            Application.Exit();
        }
    }
}
