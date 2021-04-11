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

using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Launcher.Connection;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using OpenBound_Network_Object_Library.WebRequest;
using System;
using System.IO;
using System.Windows.Forms;
using OpenBound_Game_Launcher.Forms.GenericLoadingScreen;
using System.Threading;

namespace OpenBound_Game_Launcher.Forms
{
    public partial class GameLauncher : Form
    {
        /// <summary>
        /// AsynchronousAction calls any instructions that were appended during the execution of any other thread.
        /// This variable is Thread-safe. Be careful, do not invoke Value inside it's action, this will cause a deadlock.
        /// On <see cref="timer1_Tick"/> all accumulated actions are going to be disposed.
        /// </summary>
        public AsynchronousAction TickAction;

        public GameLauncher(string[] args)
        {
            InitializeComponent();
            Parameter.Initialize(args);

            TickAction = new AsynchronousAction();

            nicknameTextBox.Text = Parameter.GameClientSettingsInformation.SavedNickname;
            versionLabel.Text = Parameter.GameClientSettingsInformation.ClientVersionHistory.PatchVersionName;

            //Disable Login Button
            SetEnableTextBox(true);
            btnLogin.Enabled = false;
        }

        public bool CheckFiles()
        {
            return false;

            PatchHistoryFetchLoadingScreen lhfls = new PatchHistoryFetchLoadingScreen();

            switch (lhfls.ShowDialog())
            {
                // Updated sucessfully. Launcher needs to close.
                case DialogResult.OK:
                    Close(DialogResult = DialogResult.OK);
                    return true;
                // No need to update.
                case DialogResult.No:
                    DialogResult = DialogResult.No;
                    return false;
                // Application failed to update. Launcher needs to close.
                case DialogResult.Cancel:
                    SetEnableTextBox(false);
                    return false;
            }

            return false;
        }

        public LauncherInformation OpenDialog()
        {
            //If the launcher should close
            if (CheckFiles())
                return new LauncherInformation(LauncherOperationStatus.Closed, null, null);

            DialogResult dr = ShowDialog();

            if (dr == DialogResult.OK)
                return new LauncherInformation(LauncherOperationStatus.AuthConfirmed,
                    Parameter.GameClientSettingsInformation, Parameter.Player);
            else
                return new LauncherInformation(LauncherOperationStatus.Closed, null, null);
        }

        #region Element Actions
        private void GameLauncher_Load(object sender, EventArgs e) { }

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
                button1.Enabled = button3.Enabled = button4.Enabled = isEnabled;
        }

        public void SetEnableTextBox(bool isEnabled)
        {
            btnLogin.Enabled = passwordTextBox.Enabled = nicknameTextBox.Enabled = isEnabled;
        }

        private void LoginTextbox_TextChanged(object sender, EventArgs e)
        {
            btnLogin.Enabled = nicknameTextBox.Text.Length > 2 && passwordTextBox.Text.Length > 2;
        }
        #endregion

        #region Screen Manupulation
        public void Close(DialogResult dialogResult = DialogResult.OK)
        {
            DialogResult = dialogResult;
            base.Close();
        }
        #endregion

        #region Button Actions
        private void BtnGameSettings_Click(object sender, EventArgs e)
        {
            GameSettings gs = new GameSettings();
            gs.ShowDialog();
        }

        private void BtnSignup_Click(object sender, EventArgs e)
        {
            new SignUpForm().ShowDialog();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {            
            LoginLoadingScreen lls = new LoginLoadingScreen(nicknameTextBox.Text, passwordTextBox.Text);

            SetEnableTextBox(false);
            SetEnableInterfaceButtons(false);

            if (lls.ShowDialog() == DialogResult.OK)
                Close();

            SetEnableTextBox(true);
            SetEnableInterfaceButtons(true);
        }
        #endregion
    }
}
