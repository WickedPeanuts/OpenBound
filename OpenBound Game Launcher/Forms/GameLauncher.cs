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
using OpenBound_Game_Launcher.Properties;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.WebRequest;
using System;
using System.Windows.Forms;

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
            CheckFiles();

            InitializeComponent();
            Parameter.Initialize();

            TickAction = new AsynchronousAction();
            launcherRequestManager = new LauncherRequestManager();

            txtNickname.Text = Parameter.GameClientSettingsInformation.SavedNickname;

            //CheckFiles();
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

        public void CheckFiles()
        {
            //var x = GamePatcher.GenerateUpdatePatch(@"C:\Users\Carlo\Desktop\OpenBound 1", @"C:\Users\Carlo\Desktop\OpenBound 2", @"C:\Users\Carlo\Desktop");
            //var y = GamePatcher.GenerateUpdatePatch(@"C:\Users\Carlo\Desktop\OpenBound 2", @"C:\Users\Carlo\Desktop\OpenBound 3", @"C:\Users\Carlo\Desktop");
            HttpWebRequest.AsyncDownloadFile(
                "https://mirrors.edge.kernel.org/tails/stable/tails-amd64-4.12/tails-amd64-4.12.img",
                @"C:\Users\Carlo\Desktop\Tails.iso",
                (f) =>
                {
                    System.Diagnostics.Debug.WriteLine(f);
                });
            //WebRequestManager wrM = new WebRequestManager(RegionEndpoint.SAEast1, "arn:aws:s3:sa-east-1:414350350235:accesspoint/openbound-ap1", "OpenBound v1.1b/Castle.Core.dll");
            //GamePatcher.MergeUpdatePatch(@"C:\Users\Carlo\Desktop\Patch-14-11-2020-5fb2e263-f66d-456b-8474-59b34fecd86b.obup", @"C:\Users\Carlo\Desktop\Patch-14-11-2020-5b6c6bb7-b0d2-411a-a0cc-4184ce079822.obup", @"C:\Users\Carlo\Desktop");
            //GamePatcher.ApplyUpdatePatch(@"C:\Users\Carlo\Desktop\OpenBound PATCHED", @"C:\Users\Carlo\Desktop\Patch.obup");
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
