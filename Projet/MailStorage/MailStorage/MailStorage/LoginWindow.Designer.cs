namespace MailStorage
{
    partial class LoginWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginWindow));
            this.barPictureBox = new System.Windows.Forms.PictureBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.mailPictureBox = new System.Windows.Forms.PictureBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.serverLabel = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.mailTextBox = new System.Windows.Forms.TextBox();
            this.mailLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.pathLabel = new System.Windows.Forms.Label();
            this.pathButton = new System.Windows.Forms.Button();
            this.validateButton = new System.Windows.Forms.Button();
            this.mailErrorLabel = new System.Windows.Forms.Label();
            this.pathErrorLabel = new System.Windows.Forms.Label();
            this.passwordErrorLabel = new System.Windows.Forms.Label();
            this.serverErrorLabel = new System.Windows.Forms.Label();
            this.portErrorLabel = new System.Windows.Forms.Label();
            this.loadingImage = new System.Windows.Forms.PictureBox();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.loadingBack = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.barPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingImage)).BeginInit();
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
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Berlin Sans FB", 25.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.titleLabel.Location = new System.Drawing.Point(0, 50);
            this.titleLabel.MinimumSize = new System.Drawing.Size(700, 130);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(700, 130);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "Connexion IMAP";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.serverLabel.Location = new System.Drawing.Point(50, 183);
            this.serverLabel.MinimumSize = new System.Drawing.Size(290, 50);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(290, 50);
            this.serverLabel.TabIndex = 4;
            this.serverLabel.Text = "Serveur IMAP";
            this.serverLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // serverTextBox
            // 
            this.serverTextBox.BackColor = System.Drawing.Color.GhostWhite;
            this.serverTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverTextBox.ForeColor = System.Drawing.Color.SteelBlue;
            this.serverTextBox.Location = new System.Drawing.Point(50, 233);
            this.serverTextBox.MaxLength = 230;
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.Size = new System.Drawing.Size(290, 38);
            this.serverTextBox.TabIndex = 5;
            // 
            // mailTextBox
            // 
            this.mailTextBox.BackColor = System.Drawing.Color.GhostWhite;
            this.mailTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mailTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mailTextBox.ForeColor = System.Drawing.Color.SteelBlue;
            this.mailTextBox.Location = new System.Drawing.Point(50, 358);
            this.mailTextBox.MaxLength = 240;
            this.mailTextBox.Name = "mailTextBox";
            this.mailTextBox.Size = new System.Drawing.Size(600, 38);
            this.mailTextBox.TabIndex = 7;
            // 
            // mailLabel
            // 
            this.mailLabel.AutoSize = true;
            this.mailLabel.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mailLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.mailLabel.Location = new System.Drawing.Point(50, 308);
            this.mailLabel.MinimumSize = new System.Drawing.Size(600, 50);
            this.mailLabel.Name = "mailLabel";
            this.mailLabel.Size = new System.Drawing.Size(600, 50);
            this.mailLabel.TabIndex = 6;
            this.mailLabel.Text = "Adresse mail";
            this.mailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // portTextBox
            // 
            this.portTextBox.BackColor = System.Drawing.Color.GhostWhite;
            this.portTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portTextBox.ForeColor = System.Drawing.Color.SteelBlue;
            this.portTextBox.Location = new System.Drawing.Point(360, 233);
            this.portTextBox.MaxLength = 230;
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(290, 38);
            this.portTextBox.TabIndex = 9;
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.portLabel.Location = new System.Drawing.Point(360, 183);
            this.portLabel.MinimumSize = new System.Drawing.Size(290, 50);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(290, 50);
            this.portLabel.TabIndex = 8;
            this.portLabel.Text = "Port";
            this.portLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.BackColor = System.Drawing.Color.GhostWhite;
            this.passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTextBox.ForeColor = System.Drawing.Color.SteelBlue;
            this.passwordTextBox.Location = new System.Drawing.Point(50, 483);
            this.passwordTextBox.MaxLength = 240;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(600, 38);
            this.passwordTextBox.TabIndex = 11;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.passwordLabel.Location = new System.Drawing.Point(50, 433);
            this.passwordLabel.MinimumSize = new System.Drawing.Size(600, 50);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(600, 50);
            this.passwordLabel.TabIndex = 10;
            this.passwordLabel.Text = "Mot de passe";
            this.passwordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathTextBox
            // 
            this.pathTextBox.BackColor = System.Drawing.Color.GhostWhite;
            this.pathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pathTextBox.Enabled = false;
            this.pathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathTextBox.ForeColor = System.Drawing.Color.SteelBlue;
            this.pathTextBox.Location = new System.Drawing.Point(50, 608);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(400, 38);
            this.pathTextBox.TabIndex = 13;
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.pathLabel.Location = new System.Drawing.Point(50, 558);
            this.pathLabel.MinimumSize = new System.Drawing.Size(600, 50);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(600, 50);
            this.pathLabel.TabIndex = 12;
            this.pathLabel.Text = "Dossier racine";
            this.pathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathButton
            // 
            this.pathButton.BackColor = System.Drawing.Color.AliceBlue;
            this.pathButton.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this.pathButton.FlatAppearance.BorderSize = 2;
            this.pathButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pathButton.Font = new System.Drawing.Font("Berlin Sans FB", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathButton.ForeColor = System.Drawing.Color.SteelBlue;
            this.pathButton.Location = new System.Drawing.Point(457, 602);
            this.pathButton.Name = "pathButton";
            this.pathButton.Size = new System.Drawing.Size(193, 51);
            this.pathButton.TabIndex = 14;
            this.pathButton.Text = "Parcourir";
            this.pathButton.UseVisualStyleBackColor = false;
            this.pathButton.Click += new System.EventHandler(this.SelectDirectory);
            // 
            // validateButton
            // 
            this.validateButton.BackColor = System.Drawing.Color.SpringGreen;
            this.validateButton.FlatAppearance.BorderSize = 0;
            this.validateButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.validateButton.Font = new System.Drawing.Font("Berlin Sans FB", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.validateButton.ForeColor = System.Drawing.Color.DarkGreen;
            this.validateButton.Location = new System.Drawing.Point(0, 690);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(700, 60);
            this.validateButton.TabIndex = 0;
            this.validateButton.Text = "Suivant";
            this.validateButton.UseVisualStyleBackColor = false;
            this.validateButton.Click += new System.EventHandler(this.ValidateConnection);
            // 
            // mailErrorLabel
            // 
            this.mailErrorLabel.AutoSize = true;
            this.mailErrorLabel.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mailErrorLabel.ForeColor = System.Drawing.Color.Tomato;
            this.mailErrorLabel.Location = new System.Drawing.Point(250, 324);
            this.mailErrorLabel.MinimumSize = new System.Drawing.Size(400, 25);
            this.mailErrorLabel.Name = "mailErrorLabel";
            this.mailErrorLabel.Size = new System.Drawing.Size(400, 26);
            this.mailErrorLabel.TabIndex = 15;
            this.mailErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pathErrorLabel
            // 
            this.pathErrorLabel.AutoSize = true;
            this.pathErrorLabel.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathErrorLabel.ForeColor = System.Drawing.Color.Tomato;
            this.pathErrorLabel.Location = new System.Drawing.Point(250, 574);
            this.pathErrorLabel.MinimumSize = new System.Drawing.Size(400, 25);
            this.pathErrorLabel.Name = "pathErrorLabel";
            this.pathErrorLabel.Size = new System.Drawing.Size(400, 26);
            this.pathErrorLabel.TabIndex = 16;
            this.pathErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // passwordErrorLabel
            // 
            this.passwordErrorLabel.AutoSize = true;
            this.passwordErrorLabel.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordErrorLabel.ForeColor = System.Drawing.Color.Tomato;
            this.passwordErrorLabel.Location = new System.Drawing.Point(250, 449);
            this.passwordErrorLabel.MinimumSize = new System.Drawing.Size(400, 25);
            this.passwordErrorLabel.Name = "passwordErrorLabel";
            this.passwordErrorLabel.Size = new System.Drawing.Size(400, 26);
            this.passwordErrorLabel.TabIndex = 17;
            this.passwordErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // serverErrorLabel
            // 
            this.serverErrorLabel.AutoSize = true;
            this.serverErrorLabel.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverErrorLabel.ForeColor = System.Drawing.Color.Tomato;
            this.serverErrorLabel.Location = new System.Drawing.Point(51, 166);
            this.serverErrorLabel.MinimumSize = new System.Drawing.Size(400, 25);
            this.serverErrorLabel.Name = "serverErrorLabel";
            this.serverErrorLabel.Size = new System.Drawing.Size(400, 26);
            this.serverErrorLabel.TabIndex = 18;
            this.serverErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // portErrorLabel
            // 
            this.portErrorLabel.AutoSize = true;
            this.portErrorLabel.Font = new System.Drawing.Font("Berlin Sans FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portErrorLabel.ForeColor = System.Drawing.Color.Tomato;
            this.portErrorLabel.Location = new System.Drawing.Point(361, 166);
            this.portErrorLabel.MinimumSize = new System.Drawing.Size(400, 25);
            this.portErrorLabel.Name = "portErrorLabel";
            this.portErrorLabel.Size = new System.Drawing.Size(400, 26);
            this.portErrorLabel.TabIndex = 19;
            this.portErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loadingImage
            // 
            this.loadingImage.Image = ((System.Drawing.Image)(resources.GetObject("loadingImage.Image")));
            this.loadingImage.Location = new System.Drawing.Point(150, 150);
            this.loadingImage.Name = "loadingImage";
            this.loadingImage.Size = new System.Drawing.Size(400, 400);
            this.loadingImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.loadingImage.TabIndex = 21;
            this.loadingImage.TabStop = false;
            this.loadingImage.Visible = false;
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoSize = true;
            this.loadingLabel.Font = new System.Drawing.Font("Berlin Sans FB", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadingLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.loadingLabel.Location = new System.Drawing.Point(0, 625);
            this.loadingLabel.MinimumSize = new System.Drawing.Size(700, 0);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(700, 35);
            this.loadingLabel.TabIndex = 22;
            this.loadingLabel.Text = "Connexion à la boite mail ...";
            this.loadingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.loadingLabel.Visible = false;
            // 
            // loadingBack
            // 
            this.loadingBack.AutoSize = true;
            this.loadingBack.Location = new System.Drawing.Point(0, 50);
            this.loadingBack.MinimumSize = new System.Drawing.Size(700, 700);
            this.loadingBack.Name = "loadingBack";
            this.loadingBack.Size = new System.Drawing.Size(700, 700);
            this.loadingBack.TabIndex = 23;
            // 
            // LoginWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(700, 750);
            this.Controls.Add(this.loadingLabel);
            this.Controls.Add(this.loadingImage);
            this.Controls.Add(this.loadingBack);
            this.Controls.Add(this.portErrorLabel);
            this.Controls.Add(this.serverErrorLabel);
            this.Controls.Add(this.passwordErrorLabel);
            this.Controls.Add(this.pathErrorLabel);
            this.Controls.Add(this.mailErrorLabel);
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.pathButton);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.mailTextBox);
            this.Controls.Add(this.mailLabel);
            this.Controls.Add(this.serverTextBox);
            this.Controls.Add(this.serverLabel);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.mailPictureBox);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.barPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginWindow";
            this.Text = "MailStorage";
            this.Shown += new System.EventHandler(this.OnShown);
            ((System.ComponentModel.ISupportInitialize)(this.barPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox barPictureBox;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.PictureBox mailPictureBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.TextBox mailTextBox;
        private System.Windows.Forms.Label mailLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Button pathButton;
        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.Label mailErrorLabel;
        private System.Windows.Forms.Label pathErrorLabel;
        private System.Windows.Forms.Label passwordErrorLabel;
        private System.Windows.Forms.Label serverErrorLabel;
        private System.Windows.Forms.Label portErrorLabel;
        private System.Windows.Forms.PictureBox loadingImage;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.Label loadingBack;
    }
}