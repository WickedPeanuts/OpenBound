using System;
using System.Drawing;
using System.Windows.Forms;
using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Forms.GenericLoadingScreen;
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


        public SignUpForm()
        {
            InitializeComponent();

#if DEBUG
            btnRegisterDebug.Visible = true;
            btnRegisterDebug.Enabled = true;
#endif

            TickAction = new AsynchronousAction();
        }

        private void RegisterAccount(PlayerDTO newPlayer)
        {
            SetEnableInterfaceElements(false);
            SignUpLoadingScreen suls = new SignUpLoadingScreen(newPlayer);
            if (suls.ShowDialog() == DialogResult.OK)
            {
                Close();
                return;
            }
            SetEnableInterfaceElements(true);
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
            TextBox tBox = (TextBox)sender;

            ttpValidation.Show(
                tBox.Tag.ToString(),
                gpbAccount, 
                new Point(tBox.Location.X + tBox.Width, tBox.Location.Y));
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
                Nickname = "accountName",
                Email = "account@name.com",
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
    }
}
