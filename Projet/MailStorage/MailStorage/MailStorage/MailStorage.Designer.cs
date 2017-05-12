﻿namespace MailStorage
{
    partial class MailStorage
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailStorage));
            this.barPictureBox = new System.Windows.Forms.PictureBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.mailPictureBox = new System.Windows.Forms.PictureBox();
            this.minimizeButton = new System.Windows.Forms.Button();
            this.appNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.backButton = new System.Windows.Forms.Button();
            this.syncButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.spaceLabel = new System.Windows.Forms.Label();
            this.spaceBackLabel = new System.Windows.Forms.Label();
            this.spaceValueLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.barPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // barPictureBox
            // 
            this.barPictureBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("barPictureBox.BackgroundImage")));
            this.barPictureBox.Location = new System.Drawing.Point(0, 0);
            this.barPictureBox.Name = "barPictureBox";
            this.barPictureBox.Size = new System.Drawing.Size(700, 50);
            this.barPictureBox.TabIndex = 0;
            this.barPictureBox.TabStop = false;
            this.barPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MoveWindow);
            // 
            // exitButton
            // 
            this.exitButton.BackColor = System.Drawing.Color.LightCoral;
            this.exitButton.FlatAppearance.BorderColor = System.Drawing.Color.LightCoral;
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.ForeColor = System.Drawing.Color.DarkRed;
            this.exitButton.Location = new System.Drawing.Point(600, 0);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(100, 50);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "X";
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ExitApplication);
            // 
            // mailPictureBox
            // 
            this.mailPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("mailPictureBox.Image")));
            this.mailPictureBox.Location = new System.Drawing.Point(20, 0);
            this.mailPictureBox.Name = "mailPictureBox";
            this.mailPictureBox.Size = new System.Drawing.Size(90, 50);
            this.mailPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.mailPictureBox.TabIndex = 2;
            this.mailPictureBox.TabStop = false;
            // 
            // minimizeButton
            // 
            this.minimizeButton.BackColor = System.Drawing.Color.NavajoWhite;
            this.minimizeButton.FlatAppearance.BorderColor = System.Drawing.Color.NavajoWhite;
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimizeButton.ForeColor = System.Drawing.Color.SaddleBrown;
            this.minimizeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.minimizeButton.Location = new System.Drawing.Point(530, 0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(70, 50);
            this.minimizeButton.TabIndex = 4;
            this.minimizeButton.Text = "-";
            this.minimizeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.minimizeButton.UseVisualStyleBackColor = false;
            this.minimizeButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MinimizeWindow);
            // 
            // appNotifyIcon
            // 
            this.appNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.appNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("appNotifyIcon.Icon")));
            this.appNotifyIcon.Text = "MailStorage";
            this.appNotifyIcon.Visible = true;
            this.appNotifyIcon.Click += new System.EventHandler(this.MaximizeWindow);
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.Wheat;
            this.backButton.FlatAppearance.BorderSize = 0;
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.backButton.ForeColor = System.Drawing.Color.SaddleBrown;
            this.backButton.Location = new System.Drawing.Point(0, 50);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(250, 60);
            this.backButton.TabIndex = 5;
            this.backButton.Text = "Retour";
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.BackButtonClick);
            // 
            // syncButton
            // 
            this.syncButton.BackColor = System.Drawing.Color.MediumTurquoise;
            this.syncButton.FlatAppearance.BorderSize = 0;
            this.syncButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.syncButton.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syncButton.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.syncButton.Location = new System.Drawing.Point(250, 50);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(450, 60);
            this.syncButton.TabIndex = 6;
            this.syncButton.Text = "Synchroniser";
            this.syncButton.UseVisualStyleBackColor = false;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Berlin Sans FB", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.titleLabel.Location = new System.Drawing.Point(0, 110);
            this.titleLabel.MinimumSize = new System.Drawing.Size(700, 300);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(700, 300);
            this.titleLabel.TabIndex = 7;
            this.titleLabel.Text = "MailStorage";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.BackColor = System.Drawing.Color.SpringGreen;
            this.statusLabel.Font = new System.Drawing.Font("Berlin Sans FB", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.DarkGreen;
            this.statusLabel.Location = new System.Drawing.Point(0, 410);
            this.statusLabel.MinimumSize = new System.Drawing.Size(700, 200);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(700, 200);
            this.statusLabel.TabIndex = 8;
            this.statusLabel.Text = "Status : Activé";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // spaceLabel
            // 
            this.spaceLabel.AutoSize = true;
            this.spaceLabel.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spaceLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.spaceLabel.Location = new System.Drawing.Point(50, 625);
            this.spaceLabel.MinimumSize = new System.Drawing.Size(600, 50);
            this.spaceLabel.Name = "spaceLabel";
            this.spaceLabel.Size = new System.Drawing.Size(600, 50);
            this.spaceLabel.TabIndex = 9;
            this.spaceLabel.Text = "Espace disponible - 5.4 GB";
            this.spaceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spaceBackLabel
            // 
            this.spaceBackLabel.AutoSize = true;
            this.spaceBackLabel.BackColor = System.Drawing.Color.Azure;
            this.spaceBackLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spaceBackLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spaceBackLabel.Font = new System.Drawing.Font("Berlin Sans FB", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spaceBackLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.spaceBackLabel.Location = new System.Drawing.Point(50, 685);
            this.spaceBackLabel.MinimumSize = new System.Drawing.Size(600, 25);
            this.spaceBackLabel.Name = "spaceBackLabel";
            this.spaceBackLabel.Size = new System.Drawing.Size(600, 25);
            this.spaceBackLabel.TabIndex = 10;
            this.spaceBackLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spaceValueLabel
            // 
            this.spaceValueLabel.AutoSize = true;
            this.spaceValueLabel.BackColor = System.Drawing.Color.Teal;
            this.spaceValueLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spaceValueLabel.Font = new System.Drawing.Font("Berlin Sans FB", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spaceValueLabel.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.spaceValueLabel.Location = new System.Drawing.Point(51, 686);
            this.spaceValueLabel.MinimumSize = new System.Drawing.Size(98, 23);
            this.spaceValueLabel.Name = "spaceValueLabel";
            this.spaceValueLabel.Size = new System.Drawing.Size(98, 23);
            this.spaceValueLabel.TabIndex = 11;
            this.spaceValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MailStorage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(700, 750);
            this.Controls.Add(this.spaceValueLabel);
            this.Controls.Add(this.spaceBackLabel);
            this.Controls.Add(this.spaceLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.syncButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.minimizeButton);
            this.Controls.Add(this.mailPictureBox);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.barPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MailStorage";
            this.Text = "MailStorage";
            ((System.ComponentModel.ISupportInitialize)(this.barPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox barPictureBox;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.PictureBox mailPictureBox;
        private System.Windows.Forms.Button minimizeButton;
        private System.Windows.Forms.NotifyIcon appNotifyIcon;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button syncButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label spaceLabel;
        private System.Windows.Forms.Label spaceBackLabel;
        private System.Windows.Forms.Label spaceValueLabel;
    }
}

