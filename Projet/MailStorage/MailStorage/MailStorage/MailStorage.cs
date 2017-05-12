/// ETML
/// Author      : Marco Maziero
/// Date        : 10.05.2017
/// Description : Main window, files management and scanning

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        /// <summary>
        /// Class constructor
        /// </summary>
        public MailStorage()
        {
            // Initializes all the window components
            InitializeComponent();

            // Adds this window to the globals
            Globals.mainWindow = this;

            FilesManager.UpdateLocalFiles();

            foreach (var variable in Globals.LOCAL_FILES)
            {
                MessageBox.Show(variable.filePath);
            }

            MessageBox.Show("Update");

            FilesManager.UpdateLocalFiles();

            foreach (var variable in Globals.LOCAL_FILES)
            {
                MessageBox.Show(variable.filePath);
            }
        }

        /// <summary>
        /// Called when the bakc button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButtonClick(object sender, EventArgs e)
        {
            // Hides the main window
            Globals.mainWindow.Hide();

            // Disconnects from the mail server
            MailManager.DisconnectIMAP();

            // Places the login window
            Globals.loginWindow.Location = Globals.mainWindow.Location;

            // Displays the login window
            Globals.loginWindow.Show();
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
    }
}
