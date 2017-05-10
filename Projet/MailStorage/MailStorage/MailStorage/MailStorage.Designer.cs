namespace MailStorage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailStorage));
            this.barPictureBox = new System.Windows.Forms.PictureBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.mailPictureBox = new System.Windows.Forms.PictureBox();
            this.minimizeButton = new System.Windows.Forms.Button();
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
            // 
            // MailStorage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(700, 750);
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

        }

        #endregion

        private System.Windows.Forms.PictureBox barPictureBox;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.PictureBox mailPictureBox;
        private System.Windows.Forms.Button minimizeButton;
    }
}

