/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using System;
using System.Threading;
using System.Windows.Forms;
using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Launcher.Connection;
using OpenBound_Game_Launcher.Properties;
using OpenBound_Network_Object_Library.Entity;

namespace OpenBound_Game_Launcher.Forms
{
    public partial class GameLauncher : Form
    {
        /// <summary>
        /// AsynchronousAction calls any instructions that were appended during the execution of any other thread.
        /// This variable is Thread-safe. Be careful, do not invoke Value inside it's action, this will cause a deadlock.
        /// On <see cref="timer1_Tick"/> all accumulated actions are going to be disposed.
        /// 
        /// Most of the actions used here are generated on <see cref="LauncherRequestManager.OnFailToEstabilishConnection"/>.
        /// </summary>
        public AsynchronousAction TickAction;

        private LauncherRequestManager launcherRequestManager;

        public GameLauncher()
        {
            InitializeComponent();
            Parameter.Initialize();

            TickAction = new AsynchronousAction();
            launcherRequestManager = new LauncherRequestManager();

            txtNickname.Text = Parameter.GameClientSettingsInformation.SavedNickname;
        }

        public LauncherInformation OpenDialog()
        {
            DialogResult dr = ShowDialog();

            if (dr == DialogResult.OK)
                return new LauncherInformation(LauncherOperationStatus.AuthConfirmed,
                    Parameter.GameClientSettingsInformation,
                    Parameter.Player);
            else
                return new LauncherInformation(LauncherOperationStatus.Closed,
                    null, null);
        }

        #region Element Actions
        private void GameLauncher_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Resources.LauncherFrame;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            //Disable Login Button
            SetEnableTextBox(false);
            SetEnableInterfaceButtons(false);

            launcherRequestManager.PrepareLoginThread(this, txtNickname.Text, txtPassword.Text);
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            new SignUpForm().ShowDialog();
        }

        /// <summary>
        /// This method is called once every 100ms. TickAction brings all external
        /// inputs into the Window.Update thread, preventing any exceptions caused by
        /// other threads influence
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            TickAction.AsynchronousInvokeAndDestroy();
        }
        #endregion

        #region Screen Element State Manipulation
        public void SetEnableInterfaceButtons(bool isEnabled)
        {
            btnLogin.Enabled = btnGameSettings.Enabled = btnSignup.Enabled =
                button1.Enabled = button3.Enabled = button4.Enabled =
                button5.Enabled = isEnabled;
        }

        public void SetEnableTextBox(bool isEnabled)
        {
            txtPassword.Enabled = txtNickname.Enabled = isEnabled;
        }

        private void LoginTextbox_TextChanged(object sender, EventArgs e)
        {
            btnLogin.Enabled = txtNickname.Text.Length > 2 && txtPassword.Text.Length > 2;
        }
        #endregion

        #region Screen Manupulation
        public new void Close()
        {
            DialogResult = DialogResult.OK;
            base.Close();
        }
        #endregion

        #region Button Actions
        private void btnGameSettings_Click(object sender, EventArgs e)
        {
            GameSettings gs = new GameSettings();
            gs.ShowDialog();
        }
        #endregion
    }
}
