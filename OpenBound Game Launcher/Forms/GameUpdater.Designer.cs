
namespace OpenBound_Game_Launcher.Forms
{
    partial class GameUpdater
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameUpdater));
            this.currentProgressBar = new System.Windows.Forms.ProgressBar();
            this.totalProgressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.downloadLabel3 = new System.Windows.Forms.Label();
            this.downloadLabel2 = new System.Windows.Forms.Label();
            this.downloadLabel1 = new System.Windows.Forms.Label();
            this.abortButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.logGroupBox = new System.Windows.Forms.GroupBox();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.toggleLogButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.logGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // currentProgressBar
            // 
            this.currentProgressBar.Location = new System.Drawing.Point(74, 95);
            this.currentProgressBar.Name = "currentProgressBar";
            this.currentProgressBar.Size = new System.Drawing.Size(582, 23);
            this.currentProgressBar.TabIndex = 0;
            // 
            // totalProgressBar
            // 
            this.totalProgressBar.Location = new System.Drawing.Point(74, 124);
            this.totalProgressBar.Name = "totalProgressBar";
            this.totalProgressBar.Size = new System.Drawing.Size(582, 23);
            this.totalProgressBar.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Current:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Total:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.downloadLabel3);
            this.groupBox1.Controls.Add(this.downloadLabel2);
            this.groupBox1.Controls.Add(this.downloadLabel1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.currentProgressBar);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.totalProgressBar);
            this.groupBox1.Location = new System.Drawing.Point(12, 188);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(662, 156);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Update Progress";
            // 
            // downloadLabel3
            // 
            this.downloadLabel3.AutoSize = true;
            this.downloadLabel3.Location = new System.Drawing.Point(6, 68);
            this.downloadLabel3.Name = "downloadLabel3";
            this.downloadLabel3.Size = new System.Drawing.Size(94, 15);
            this.downloadLabel3.TabIndex = 6;
            this.downloadLabel3.Text = "downloadLabel3";
            // 
            // downloadLabel2
            // 
            this.downloadLabel2.AutoSize = true;
            this.downloadLabel2.Location = new System.Drawing.Point(6, 45);
            this.downloadLabel2.Name = "downloadLabel2";
            this.downloadLabel2.Size = new System.Drawing.Size(94, 15);
            this.downloadLabel2.TabIndex = 5;
            this.downloadLabel2.Text = "downloadLabel2";
            // 
            // downloadLabel1
            // 
            this.downloadLabel1.AutoSize = true;
            this.downloadLabel1.Location = new System.Drawing.Point(6, 22);
            this.downloadLabel1.Name = "downloadLabel1";
            this.downloadLabel1.Size = new System.Drawing.Size(94, 15);
            this.downloadLabel1.TabIndex = 4;
            this.downloadLabel1.Text = "downloadLabel1";
            // 
            // abortButton
            // 
            this.abortButton.Location = new System.Drawing.Point(591, 495);
            this.abortButton.Name = "abortButton";
            this.abortButton.Size = new System.Drawing.Size(82, 26);
            this.abortButton.TabIndex = 7;
            this.abortButton.Text = "Abort";
            this.abortButton.UseVisualStyleBackColor = true;
            this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(503, 495);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(82, 26);
            this.updateButton.TabIndex = 8;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // logGroupBox
            // 
            this.logGroupBox.Controls.Add(this.logListBox);
            this.logGroupBox.Location = new System.Drawing.Point(12, 350);
            this.logGroupBox.Name = "logGroupBox";
            this.logGroupBox.Size = new System.Drawing.Size(661, 139);
            this.logGroupBox.TabIndex = 9;
            this.logGroupBox.TabStop = false;
            this.logGroupBox.Text = "Update Log";
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 15;
            this.logListBox.Location = new System.Drawing.Point(6, 22);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(649, 109);
            this.logListBox.TabIndex = 0;
            // 
            // toggleLogButton
            // 
            this.toggleLogButton.Location = new System.Drawing.Point(415, 495);
            this.toggleLogButton.Name = "toggleLogButton";
            this.toggleLogButton.Size = new System.Drawing.Size(82, 26);
            this.toggleLogButton.TabIndex = 10;
            this.toggleLogButton.Text = "Hide Log";
            this.toggleLogButton.UseVisualStyleBackColor = true;
            this.toggleLogButton.Click += new System.EventHandler(this.toggleLogButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 12);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(660, 170);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // GameUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 533);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.toggleLogButton);
            this.Controls.Add(this.logGroupBox);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.abortButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.Name = "GameUpdater";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Game Updater";
            this.Load += new System.EventHandler(this.GameUpdater_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.logGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar currentProgressBar;
        private System.Windows.Forms.ProgressBar totalProgressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label downloadLabel2;
        private System.Windows.Forms.Label downloadLabel1;
        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label downloadLabel3;
        private System.Windows.Forms.GroupBox logGroupBox;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.Button toggleLogButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer2;
    }
}