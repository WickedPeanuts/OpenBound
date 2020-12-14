using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Helper;
using OpenBound_Game_Launcher.Launcher.Connection;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Game_Launcher.Forms.GenericLoadingScreen
{
    public class LoginLoadingScreen : LoadingMenu
    {
        string login, password;

        LauncherRequestManager launcherRequestManager;

        public LoginLoadingScreen(string login, string password)
            : base()
        {
            this.login = login;
            this.password = password;
            launcherRequestManager = new LauncherRequestManager();
        }

        public new DialogResult ShowDialog()
        {
            launcherRequestManager.PrepareLoginThread(this, login, password);
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

        public void OnFailToFindPlayer()
        {
            timer1InvokeAndDestroyTickAction += () =>
            {
                Feedback.CreateWarningMessageBox(Language.PlayerNotFound);
                Close(DialogResult.Cancel);
            };
        }
    }
}
