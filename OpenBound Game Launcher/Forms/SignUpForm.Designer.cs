namespace OpenBound_Game_Launcher.Forms
{
    partial class SignUpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignUpForm));
            this.gpbAccount = new System.Windows.Forms.GroupBox();
            this.txtPasswordConfirmation = new System.Windows.Forms.TextBox();
            this.lblPasswordConfirmation = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.gbxGender = new System.Windows.Forms.GroupBox();
            this.rdbFemale = new System.Windows.Forms.RadioButton();
            this.rdbMale = new System.Windows.Forms.RadioButton();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtNickname = new System.Windows.Forms.TextBox();
            this.lblNickname = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.ttpValidation = new System.Windows.Forms.ToolTip(this.components);
            this.btnRegisterDebug = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gpbAccount.SuspendLayout();
            this.gbxGender.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpbAccount
            // 
            this.gpbAccount.Controls.Add(this.txtPasswordConfirmation);
            this.gpbAccount.Controls.Add(this.lblPasswordConfirmation);
            this.gpbAccount.Controls.Add(this.txtPassword);
            this.gpbAccount.Controls.Add(this.lblPassword);
            this.gpbAccount.Controls.Add(this.gbxGender);
            this.gpbAccount.Controls.Add(this.txtEmail);
            this.gpbAccount.Controls.Add(this.lblEmail);
            this.gpbAccount.Controls.Add(this.txtNickname);
            this.gpbAccount.Controls.Add(this.lblNickname);
            this.gpbAccount.Location = new System.Drawing.Point(21, 12);
            this.gpbAccount.Name = "gpbAccount";
            this.gpbAccount.Size = new System.Drawing.Size(223, 263);
            this.gpbAccount.TabIndex = 0;
            this.gpbAccount.TabStop = false;
            this.gpbAccount.Text = "Account information";
            // 
            // txtPasswordConfirmation
            // 
            this.txtPasswordConfirmation.Location = new System.Drawing.Point(8, 228);
            this.txtPasswordConfirmation.Name = "txtPasswordConfirmation";
            this.txtPasswordConfirmation.PasswordChar = '•';
            this.txtPasswordConfirmation.Size = new System.Drawing.Size(206, 23);
            this.txtPasswordConfirmation.TabIndex = 8;
            this.txtPasswordConfirmation.Tag = "At least 6 characters. The character | is not allowed.";
            this.txtPasswordConfirmation.TextChanged += new System.EventHandler(this.Textbox_TextChanged);
            this.txtPasswordConfirmation.Leave += new System.EventHandler(this.Textbox_OnDeselect);
            // 
            // lblPasswordConfirmation
            // 
            this.lblPasswordConfirmation.AutoSize = true;
            this.lblPasswordConfirmation.Location = new System.Drawing.Point(5, 209);
            this.lblPasswordConfirmation.Name = "lblPasswordConfirmation";
            this.lblPasswordConfirmation.Size = new System.Drawing.Size(129, 15);
            this.lblPasswordConfirmation.TabIndex = 7;
            this.lblPasswordConfirmation.Text = "Password confirmation";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(8, 183);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.Size = new System.Drawing.Size(206, 23);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.Tag = "At least 6 characters. The character | is not allowed.";
            this.txtPassword.TextChanged += new System.EventHandler(this.Textbox_TextChanged);
            this.txtPassword.Leave += new System.EventHandler(this.Textbox_OnDeselect);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(5, 164);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 15);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password";
            // 
            // gbxGender
            // 
            this.gbxGender.Controls.Add(this.rdbFemale);
            this.gbxGender.Controls.Add(this.rdbMale);
            this.gbxGender.Location = new System.Drawing.Point(8, 116);
            this.gbxGender.Name = "gbxGender";
            this.gbxGender.Size = new System.Drawing.Size(206, 44);
            this.gbxGender.TabIndex = 4;
            this.gbxGender.TabStop = false;
            this.gbxGender.Text = "Gender";
            // 
            // rdbFemale
            // 
            this.rdbFemale.AutoSize = true;
            this.rdbFemale.Location = new System.Drawing.Point(64, 17);
            this.rdbFemale.Name = "rdbFemale";
            this.rdbFemale.Size = new System.Drawing.Size(63, 19);
            this.rdbFemale.TabIndex = 1;
            this.rdbFemale.Text = "Female";
            this.rdbFemale.UseVisualStyleBackColor = true;
            // 
            // rdbMale
            // 
            this.rdbMale.AutoSize = true;
            this.rdbMale.Checked = true;
            this.rdbMale.Location = new System.Drawing.Point(7, 17);
            this.rdbMale.Name = "rdbMale";
            this.rdbMale.Size = new System.Drawing.Size(51, 19);
            this.rdbMale.TabIndex = 0;
            this.rdbMale.TabStop = true;
            this.rdbMale.Text = "Male";
            this.rdbMale.UseVisualStyleBackColor = true;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(8, 90);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(206, 23);
            this.txtEmail.TabIndex = 3;
            this.txtEmail.Tag = "Please enter a valid email address.";
            this.txtEmail.TextChanged += new System.EventHandler(this.Textbox_TextChanged);
            this.txtEmail.Leave += new System.EventHandler(this.Textbox_OnDeselect);
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(5, 71);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(36, 15);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "Email";
            // 
            // txtNickname
            // 
            this.txtNickname.Location = new System.Drawing.Point(8, 46);
            this.txtNickname.Name = "txtNickname";
            this.txtNickname.Size = new System.Drawing.Size(206, 23);
            this.txtNickname.TabIndex = 1;
            this.txtNickname.Tag = "Just alpha numeric characters are allowed. At least 4 characters.";
            this.txtNickname.TextChanged += new System.EventHandler(this.Textbox_TextChanged);
            this.txtNickname.Leave += new System.EventHandler(this.Textbox_OnDeselect);
            // 
            // lblNickname
            // 
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(5, 27);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(61, 15);
            this.lblNickname.TabIndex = 0;
            this.lblNickname.Text = "Nickname";
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(21, 281);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(223, 26);
            this.btnRegister.TabIndex = 0;
            this.btnRegister.Text = "Create an account";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(166, 313);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(78, 26);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // ttpValidation
            // 
            this.ttpValidation.AutomaticDelay = 100;
            this.ttpValidation.AutoPopDelay = 1000;
            this.ttpValidation.InitialDelay = 100;
            this.ttpValidation.ReshowDelay = 0;
            this.ttpValidation.ShowAlways = true;
            this.ttpValidation.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ttpValidation.ToolTipTitle = "Validation rules";
            // 
            // btnRegisterDebug
            // 
            this.btnRegisterDebug.Enabled = false;
            this.btnRegisterDebug.Location = new System.Drawing.Point(21, 313);
            this.btnRegisterDebug.Name = "btnRegisterDebug";
            this.btnRegisterDebug.Size = new System.Drawing.Size(75, 26);
            this.btnRegisterDebug.TabIndex = 2;
            this.btnRegisterDebug.Text = "Debug Acc";
            this.btnRegisterDebug.UseVisualStyleBackColor = true;
            this.btnRegisterDebug.Visible = false;
            this.btnRegisterDebug.Click += new System.EventHandler(this.BtnRegisterDebug_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SignUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 357);
            this.Controls.Add(this.btnRegisterDebug);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.gpbAccount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SignUpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sign Up";
            this.gpbAccount.ResumeLayout(false);
            this.gpbAccount.PerformLayout();
            this.gbxGender.ResumeLayout(false);
            this.gbxGender.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpbAccount;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtNickname;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.TextBox txtPasswordConfirmation;
        private System.Windows.Forms.Label lblPasswordConfirmation;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox gbxGender;
        private System.Windows.Forms.RadioButton rdbFemale;
        private System.Windows.Forms.RadioButton rdbMale;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.ToolTip ttpValidation;
        private System.Windows.Forms.Button btnRegisterDebug;
        private System.Windows.Forms.Timer timer1;
    }
}