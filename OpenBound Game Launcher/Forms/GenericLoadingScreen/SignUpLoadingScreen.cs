using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Helper;
using OpenBound_Game_Launcher.Launcher.Connection;
using OpenBound_Network_Object_Library.ValidationModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Forms.GenericLoadingScreen
{
    public class SignUpLoadingScreen : LoadingMenu
    {
        private PlayerDTO playerDTO;
        private LauncherRequestManager launcherRequestManager;

        public SignUpLoadingScreen(PlayerDTO playerDTO)
            : base()
        {
            this.playerDTO = playerDTO;
            launcherRequestManager = new LauncherRequestManager();
        }

        public new DialogResult ShowDialog()
        {
            launcherRequestManager.PrepareRegistrationThread(this, playerDTO);
            return base.ShowDialog();
        }

        public void OnFailToStablishConnection()
        {
            timer1InvokeAndDestroyTickAction += () =>
            {
                Feedback.CreateWarningMessageBox(Language.FailToEstabilishConnection);
                Close(DialogResult.Cancel);
            };
        }

        public void OnFailToCreateAccount()
        {
            timer1InvokeAndDestroyTickAction += () =>
            {
                Feedback.CreateWarningMessageBox(Language.RegisterFailureMessage);
                Close(DialogResult.Cancel);
            };
        }

        public void OnFailToCreateAccount(string reason)
        {
            timer1InvokeAndDestroyTickAction += () =>
            {
                Feedback.CreateWarningMessageBox($"{Language.RegisterFailureMessage}{reason}");
                Close(DialogResult.Cancel);
            };
        }

        public void OnCreateAccount()
        {
            timer1InvokeAndDestroyTickAction += () =>
            {
                Feedback.CreateInformationMessageBox(Language.RegisterSuccess);
                Close(DialogResult.OK);
            };
        }
    }
}
