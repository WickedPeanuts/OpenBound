
namespace OpenBound_Management_Tools.Forms
{
    partial class MainMenu
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
            this.installDockerContainersButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // installDockerContainersButton
            // 
            this.installDockerContainersButton.Location = new System.Drawing.Point(68, 34);
            this.installDockerContainersButton.Name = "installDockerContainersButton";
            this.installDockerContainersButton.Size = new System.Drawing.Size(161, 26);
            this.installDockerContainersButton.TabIndex = 0;
            this.installDockerContainersButton.Text = "Install Docker Containers";
            this.installDockerContainersButton.UseVisualStyleBackColor = true;
            this.installDockerContainersButton.Click += new System.EventHandler(this.InstallDockerContainersButton_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 465);
            this.Controls.Add(this.installDockerContainersButton);
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button installDockerContainersButton;
    }
}