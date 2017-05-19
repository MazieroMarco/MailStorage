/// ETML
/// Author      : Marco Maziero
/// Date        : 10.05.2017
/// Description : Main window, files management and scanning

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailStorage
{
    /// <summary>
    /// The window main class
    /// </summary>
    public partial class MailStorage : Form
    {
        // Window interaction variables
        public const int WM_NCLBUTTONDOWN = 0xA1;

        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // Class variables declaration
        private Task initialSync;
        private Task globalRefresh;
        private bool blnAppCrash = false;
        private FileSystemWatcher rootFolderWatcher = new FileSystemWatcher();

        /// <summary>
        /// Class constructor
        /// </summary>
        public MailStorage()
        {
            // Initializes all the window components
            InitializeComponent();

            // Adds this window to the globals
            Globals.mainWindow = this;

            // Updates the storage size
            UpdateMailboxSpace();

            // Starts the root folder monitoring
            rootFolderWatcher.Path = Globals.ROOT_DIRECTORY;
            rootFolderWatcher.IncludeSubdirectories = true;
            rootFolderWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            // Adds the events
            rootFolderWatcher.Changed += SynchronizeFiles;
            rootFolderWatcher.Created += SynchronizeFiles;
            rootFolderWatcher.Deleted += SynchronizeFiles;
            rootFolderWatcher.Renamed += SynchronizeFiles;

            // Starts the monitoring
            rootFolderWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Initializes the window when called
        /// </summary>
        public void InitializeWindow()
        {
            // Checks for the initial sync
            if (Globals.NEED_INITIAL_SYNC)
            {
                // Starts the task and the synchronisation
                initialSync = Task.Factory.StartNew(FilesManager.InitialSynchronisation);

                // Sets the variable to false
                Globals.NEED_INITIAL_SYNC = false;
            }

            // Sarts the monitoring
            if (!rootFolderWatcher.EnableRaisingEvents)
                rootFolderWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Called when the bakc button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButtonClick(object sender, EventArgs e)
        {
            // Checks if tasks are running
            if (globalRefresh != null && !globalRefresh.IsCompleted || initialSync != null && !initialSync.IsCompleted)
            {
                // Displays the warning message
                MessageBox.Show("Un synchronisation est en cours.\n" +
                                "Impossible de retourner à l'écran de login pour le moment.\n" +
                                "Veuillez patienter pendant quelques secondes.",
                    "Synchronisation en cours",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            // Hides the main window
            Globals.mainWindow.Hide();

            // Stops the monitoring
            if (rootFolderWatcher.EnableRaisingEvents)
                rootFolderWatcher.EnableRaisingEvents = false;

            // Disconnects from the mail server
            MailManager.DisconnectIMAP();

            // Places the login window
            Globals.loginWindow.Location = Globals.mainWindow.Location;

            // Displays the login window
            Globals.loginWindow.Show();
        }

        /// <summary>
        /// Called when the synchronisation button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncButton(object sender, EventArgs e)
        {
            // Checks if already in this state
            if (statusLabel.Text != "Synchronisation")
            {
                // Snyc mode
                statusLabel.Text = "Synchronisation";
                statusLabel.BackColor = Color.NavajoWhite;
                statusLabel.ForeColor = Color.Peru;
                statusBackPictureBox.BackColor = Color.NavajoWhite;
                statusPictureBox.BackColor = Color.NavajoWhite;
                statusPictureBox.Image = Image.FromFile("sync.gif");
            }

            // Manual synchronisation
            SynchronizeFiles(null, null);
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
            // Checks if tasks are running
            if (globalRefresh != null && !globalRefresh.IsCompleted || initialSync != null && !initialSync.IsCompleted)
            {
                // Displays the warning message
                if (MessageBox.Show("Un synchronisation est en cours.\n" +
                                    "Fermer l'application maintenant pourrait endommager vos fichiers.\n" +
                                    "Etes vous sur de vouloir quitter ?", "Synchronisation en cours",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    // Exits the application
                    Application.Exit();
                }
            }
            else
            {
                // Exits the application
                Application.Exit();
            }
        }

        /// <summary>
        /// Minimizes the window when the minimize button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeWindow(object sender, MouseEventArgs e)
        {
            // Sets the window state to minimized
            WindowState = FormWindowState.Minimized;

            // Hides the window in the task bar
            this.Hide();

            // Displays the ballon tip
            appNotifyIcon.ShowBalloonTip(5, "MailStorage", "MailStorage s'exécute en arrière plan", ToolTipIcon.Info);
        }

        /// <summary>
        /// Maximizes the window when the notify icon is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeWindow(object sender, EventArgs e)
        {
            // Shows the application
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Updates the mailbox storage size elements
        /// </summary>
        private void UpdateMailboxSpace()
        {
            // Get the mailbox informations
            var mailQuota = MailManager.GetMailboxQuota();

            // Updates the title
            spaceLabel.Text = "Espace disponible - " + Math.Round((float)mailQuota.CurrentStorageSize / 1024 / 1024, 2) + " GB / " + Math.Round((float)mailQuota.StorageLimit / 1024 / 1024, 2) + " GB";

            // Updates the label (0 - 598)
            spaceValueLabel.MinimumSize = new Size((int)(mailQuota.CurrentStorageSize * 598 / mailQuota.StorageLimit), 23);
        }

        /// <summary>
        /// Synchronizes the files between the local folder and the user mailbox
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void SynchronizeFiles(object source, FileSystemEventArgs e)
        {
            // returns if the task is already running
            if (globalRefresh != null && !globalRefresh.IsCompleted || initialSync != null && !initialSync.IsCompleted) return;

            // Updates the storage size
            UpdateMailboxSpace();

            // Starts the task
            globalRefresh = Task.Factory.StartNew(() =>
            {
                // Updates the lists
                FilesManager.UpdateLocalFiles();
                FilesManager.UpdateRemoteFiles();

                // Updates the files
                FilesManager.AddLocalFilesToMailBox();
                FilesManager.DeleteRemotesFilesFromLocal();

            });
        }

        /// <summary>
        /// Refreshes the main windoe elements (status, backgrounds, etc...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshWindowElements(object sender, EventArgs e)
        {
            // Checks if the root folder still exists
            if (!Directory.Exists(Globals.ROOT_DIRECTORY) && !blnAppCrash)
            {
                // Sets the boolean to true
                blnAppCrash = true;

                // Displays the error message
                MessageBox.Show("Oups ...\n\n" +
                                "Il semblerait que le dossier racine ne soit plus trouvable.\n" +
                                "Veuillez spécifier un nouveau dossier valide.",
                    "Dossier racine inexistant",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Exits the application
                Application.Exit();
            }

            // Checks if the application is still connected to the IMAP server
            if (!MailManager.imapCli.IsConnected || !MailManager.imapCli.IsAuthenticated)
            {
                // Reconnects the program to the IMAP server
                if (!MailManager.ConnectIMAP(Globals.IMAP_SERVER_NAME, Globals.IMAP_SERVER_PORT, Globals.USER_MAIL_ADDRESS, Globals.USER_MAIL_PASSWORD))
                {
                    // Sets the boolean to true
                    blnAppCrash = true;

                    // Displays the error message
                    MessageBox.Show("Oups ...\n\n" +
                                    "Il semblerait que vous ayez été déconnecté du serveur mail.\n" +
                                    "Veuillez vous reconnecter depuis la page de connexion.",
                        "Connexion perdue",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Exits the application
                    Application.Exit();
                }
            }

            // Checks if it's a synchronisation
            if (globalRefresh != null && !globalRefresh.IsCompleted || initialSync != null && !initialSync.IsCompleted)
            {
                // Checks if already in this state
                if (statusLabel.Text == "Synchronisation") return;

                // Snyc mode
                statusLabel.Text = "Synchronisation";
                statusLabel.BackColor = Color.NavajoWhite;
                statusLabel.ForeColor = Color.Peru;

                statusBackPictureBox.BackColor = Color.NavajoWhite;
                statusPictureBox.BackColor = Color.NavajoWhite;
                statusPictureBox.Image = Image.FromFile("sync.gif");
            }
            else
            {
                // Checks if already in this state
                if (statusLabel.Text == "Activé") return;

                // Activated mode
                statusLabel.Text = "Activé";
                statusLabel.BackColor = Color.SpringGreen;
                statusLabel.ForeColor = Color.DarkGreen;

                statusBackPictureBox.BackColor = Color.SpringGreen;
                statusPictureBox.BackColor = Color.SpringGreen;
                statusPictureBox.Image = Image.FromFile("activated.png");
            }
        }
    }
}
