using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Helper;
using OpenBound_Game_Launcher.Launcher.Connection;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.ValidationModel;

namespace OpenBound_Game_Launcher.Forms
{
    public partial class SignUpForm : Form
    {
        public AsynchronousAction TickAction;

        private LauncherRequestManager launcherRequestManager;

        public SignUpForm()
        {
            InitializeComponent();

            launcherRequestManager = new LauncherRequestManager();
#if DEBUG
            btnRegisterDebug.Visible = true;
            btnRegisterDebug.Enabled = true;
#endif

            TickAction = new AsynchronousAction();
        }

        private void RegisterAccount(PlayerDTO newPlayer)
        {
            launcherRequestManager.PrepareRegistrationThread(this, newPlayer);
        }

        #region Screen Element State Manipulaton
        public void SetEnableInterfaceElements(bool isEnabled)
        {
            txtEmail.Enabled = txtNickname.Enabled =
                txtPassword.Enabled = txtPasswordConfirmation.Enabled =
                btnClose.Enabled = btnRegister.Enabled = btnRegisterDebug.Enabled =
                rdbFemale.Enabled = rdbMale.Enabled = isEnabled;
        }
        #endregion

        #region Element Actions
        private void Textbox_TextChanged(object sender, EventArgs e)
        {
            ttpValidation.Show(((TextBox)sender).Tag.ToString(), gpbAccount,
                new Point(((TextBox)sender).Location.X + ((TextBox)sender).Width, ((TextBox)sender).Location.Y));
        }

        private void Textbox_OnDeselect(object sender, EventArgs e) =>
            ttpValidation.Hide((TextBox)sender);

        private void BtnClose_Click(object sender, EventArgs e) => Close();

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            PlayerDTO newPlayer = new PlayerDTO
            {
                Email = txtEmail.Text,
                Nickname = txtNickname.Text,
                Gender = rdbMale.Checked ? Gender.Male : Gender.Female,
                Password = txtPassword.Text,
                PasswordConfirmation = txtPasswordConfirmation.Text
            };

            if (newPlayer.Validate())
            {
                SetEnableInterfaceElements(false);
                RegisterAccount(newPlayer);
            }
            else
            {
                Feedback.CreateWarningMessageBox($"{Language.RegisterFailureInvalidCredentials}\n{newPlayer.ValidationErrorsToString()}");
            }
        }

        private void BtnRegisterDebug_Click(object sender, EventArgs e)
        {
            RegisterAccount(new PlayerDTO()
            {
                Nickname = "Sopa",
                Email = "sopa@hotmail.com",
                Password = "123456",
                PasswordConfirmation = "123456",
                Gender = Gender.Female
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TickAction.AsynchronousInvokeAndDestroy();
        }
        #endregion

        private void SignUpForm_Load(object sender, EventArgs e)
        {

        }
    }
}
