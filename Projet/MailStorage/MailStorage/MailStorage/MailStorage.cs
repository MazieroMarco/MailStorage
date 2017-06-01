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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit;
using CsharpAes;

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
        public Task initialSync;
        public Task globalRefresh;
        public Task getQuota;
        private bool blnAppCrash = false;
        private bool blnAppFirstSync = true;
        private readonly FileSystemWatcher rootFolderWatcher = new FileSystemWatcher();

        /// <summary>
        /// Class constructor
        /// </summary>
        public MailStorage()
        {
            // Initializes all the window components
            InitializeComponent();

            // Sets the window icon
            this.Icon = new Icon("appIcon.ico");

            // Adds this window to the globals
            Globals.mainWindow = this;

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
        public async void InitializeWindow()
        {
            // Sets the infos label text
            infosLabel.Text = "Dossier  " + Globals.ROOT_DIRECTORY + "\n" +
                              "Mail     " + Globals.USER_MAIL_ADDRESS;

            // Updates the disk space value
            UpdateMailboxSpace();

            // Checks for the initial sync
            if (Globals.NEED_INITIAL_SYNC)
            {
                // Starts the task and the synchronisation
                initialSync = Task.Factory.StartNew(FilesManager.InitialSynchronisation);

                await initialSync;

                // Sets the variable to false
                Globals.NEED_INITIAL_SYNC = false;
            }

            // Sets the monitoring folder to root
            rootFolderWatcher.Path = Globals.ROOT_DIRECTORY;

            // Sarts the monitoring
            if (!rootFolderWatcher.EnableRaisingEvents)
                rootFolderWatcher.EnableRaisingEvents = true;

            // Starts the first synchronisation
            SynchronizeFiles(null, null);

            // Updates the index mail
            FilesManager.UpdateDirectoriesIndex();
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

                // File status label
                currentFileLabel.BackColor = Color.NavajoWhite;
                currentFileLabel.Visible = true;

                // Status picture
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
        private async void ExitApplication(object sender, MouseEventArgs e)
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
                // Displays the warning message
                if (MessageBox.Show("Vous allez quitter l'application.\n\n" +
                                    "Voulez-vous effectuer une dernière synchronisation avant de quitter ?\n",
                        "Fermeture du programme",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Last sync
                    SynchronizeFiles(null, null);

                    // Freezes all the window elements
                    backButton.Enabled = false;
                    exitButton.Enabled = false;
                    minimizeButton.Enabled = false;

                    // Waits for the task to complete
                    await globalRefresh;
                }

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
            // returns if the task is already running
            if (getQuota != null && !getQuota.IsCompleted) return;
            
            // Get the mailbox informations
            var mailQuota = MailManager.GetMailboxQuota();

            // Checks if the quota is avaliable
            if (mailQuota != null)
            {
                // Updates the title
                if (spaceLabel.InvokeRequired)
                {
                    spaceLabel.BeginInvoke((MethodInvoker)delegate
                    {
                        // Updates the text
                        spaceLabel.Text = "Espace disponible - " + Math.Round((float)mailQuota.CurrentStorageSize / 1024 / 1024, 2) + " GB / " + Math.Round((float)mailQuota.StorageLimit / 1024 / 1024, 2) + " GB";

                        // Updates the label position
                        spaceValueLabel.Location = new Point(spaceBackLabel.Location.X + 1, spaceBackLabel.Location.Y + 1);


                        // Updates the label (0 - maxBacklabelSize)
                        spaceValueLabel.MinimumSize = new Size((int)(mailQuota.CurrentStorageSize * (spaceBackLabel.Width - 1) / mailQuota.StorageLimit), spaceBackLabel.Height - 1);
                    });
                }
                else
                {
                    // Updates the text
                    spaceLabel.Text = "Espace disponible - " + Math.Round((float)mailQuota.CurrentStorageSize / 1024 / 1024, 2) + " GB / " + Math.Round((float)mailQuota.StorageLimit / 1024 / 1024, 2) + " GB";

                    // Updates the label position
                    spaceValueLabel.Location = new Point(spaceBackLabel.Location.X + 1, spaceBackLabel.Location.Y + 1);


                    // Updates the label (0 - maxBacklabelSize)
                    spaceValueLabel.MinimumSize = new Size((int)(mailQuota.CurrentStorageSize * (spaceBackLabel.Width - 1) / mailQuota.StorageLimit), spaceBackLabel.Height - 1);
                }
            }
            else
            {
                // Quota not avaliable, updates the title
                if (spaceLabel.InvokeRequired)
                {
                    spaceLabel.BeginInvoke((MethodInvoker)delegate
                    {
                        // Updates the text
                        spaceLabel.Text = "Espace disponible - Données indisponibles";

                        // Updates the label (0 - 598)
                        spaceValueLabel.MinimumSize = new Size(0, 23);
                    });
                }
                else
                {
                    // Updates the text
                    spaceLabel.Text = "Espace disponible - Données indisponibles";

                    // Updates the label (0 - 598)
                    spaceValueLabel.MinimumSize = new Size(0, 23);
                }
            }
        }

        /// <summary>
        /// Updates the current file status label
        /// </summary>
        /// <param name="strText"></param>
        public void UpdateCurrentFile(string strText)
        {
            // Changes the current file text label
            if (currentFileLabel.InvokeRequired)
            {
                currentFileLabel.BeginInvoke((MethodInvoker)delegate
                {
                    currentFileLabel.Text = strText;
                });
            }
            else
            {
                currentFileLabel.Text = strText;
            }
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
            
            // Starts the task
            globalRefresh = Task.Factory.StartNew(() =>
            {
                // Waits before sync
                Thread.Sleep(3000);

                // Updates the index mail
                FilesManager.UpdateDirectoriesIndex();

                // Updates the lists
                FilesManager.UpdateLocalFiles();
                FilesManager.UpdateRemoteFiles();

                // If the app has just started, downloads all the remaining files from the mailbox
                if (blnAppFirstSync)
                {
                    // Download the missing files
                    FilesManager.StartUpdateFromMailBox();

                    // Updates the lists
                    FilesManager.UpdateLocalFiles();
                    FilesManager.UpdateRemoteFiles();

                    // Disables the boolean
                    blnAppFirstSync = false;
                }

                // Updates the files
                FilesManager.AddLocalFilesToMailBox();
                FilesManager.DeleteRemoteFilesFromLocal();

                // Updates the disk space value
                UpdateMailboxSpace();
            });
        }

        /// <summary>
        /// Refreshes the main windoe elements (status, backgrounds, etc...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshWindowElements(object sender, EventArgs e)
        {
            // Returns if the main window is hidden
            if (Globals.mainWindow.Visible == false) return;

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

                // File status label
                currentFileLabel.BackColor = Color.NavajoWhite;
                currentFileLabel.Visible = true;

                // Status picture
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

                // File status label
                currentFileLabel.BackColor = Color.SpringGreen;
                currentFileLabel.Text = "";
                currentFileLabel.Visible = false;

                // Status picture
                statusBackPictureBox.BackColor = Color.SpringGreen;
                statusPictureBox.BackColor = Color.SpringGreen;
                statusPictureBox.Image = Image.FromFile("activated.png");
            }
        }
    }
}
