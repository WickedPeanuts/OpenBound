﻿/* 
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
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using OpenBound_Network_Object_Library.WebRequest;
using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;
using OpenBound_Patcher.Forms;

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

        private string latestPatchHistoryPath;

        public GameLauncher()
        {
            CheckFiles();

            TickAction = new AsynchronousAction();
            launcherRequestManager = new LauncherRequestManager();

            InitializeComponent();
            Parameter.Initialize();

            txtNickname.Text = Parameter.GameClientSettingsInformation.SavedNickname;

            //Disable Login Button
            SetEnableTextBox(false);
            SetEnableInterfaceButtons(false);
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

        /*private void CheckLatestUpdateFiles()
        {
            //If this file exists, it means the application is being re-opened after an update
            //and it must delete all unused files
            if (!File.Exists(latestPatchHistoryPath))
                return;

            PatchHistory pH = ObjectWrapper.DeserializeFile<PatchHistory>(latestPatchHistoryPath);

            FileList fl = Manifest.GetMissingInvalidAndOutdatedFiles(
                pH.ApplicationManifest.CurrentVersionFileList.Checksum,
                Directory.GetCurrentDirectory());

            foreach (string toBeDeleted in fl.ToBeDeleted)
            {
                File.Delete($@"{Directory.GetCurrentDirectory()}\{toBeDeleted}");
            }

            Directory.Delete(@$"{Directory.GetCurrentDirectory()}\{NetworkObjectParameters.PatchTemporaryPath}", true);
        }*/

        public void GenerateDummyPatches()
        {
            var x = GamePatcher.GenerateUpdatePatch(
                @"C:\Users\Carlo\Desktop\OpenBound 1",
                @"C:\Users\Carlo\Desktop\OpenBound 2",
                @"C:\Users\Carlo\Desktop",
                @"C:\Users\Carlo\Desktop\PatchHistory.json",
                @"v0.1.1a");

            var y = GamePatcher.GenerateUpdatePatch(
                @"C:\Users\Carlo\Desktop\OpenBound 2",
                @"C:\Users\Carlo\Desktop\OpenBound 3",
                @"C:\Users\Carlo\Desktop",
                @"C:\Users\Carlo\Desktop\PatchHistory.json",
                @"v0.1.2a");
        }

        public void CheckFiles()
        {
            latestPatchHistoryPath = @$"{Directory.GetCurrentDirectory()}\{NetworkObjectParameters.PatchTemporaryPath}\{NetworkObjectParameters.PatchHistoryFilename}";

            HttpWebRequest.AsyncDownloadFile(
                "http://192.168.0.50/versioning/PatchHistory.json",
                latestPatchHistoryPath,
                onFinishDownload: OnFinishDownloadPatchHistory,
                onFailToDownload: OnFailToDownloadPatchHistory
                );
        }

        #region Element Actions
        private void GameLauncher_Load(object sender, EventArgs e)
        {

        }

        private void OnFailToDownloadPatchHistory(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private void OnFinishDownloadPatchHistory()
        {
            TickAction += () =>
            {
                PatchHistory patchHistory = PatchHistory.CreatePatchHistoryInstance(latestPatchHistoryPath);
                if (patchHistory.PatchEntryList.Last().ID != Parameter.GameClientSettingsInformation.ClientVersionHistory.PatchEntryList.Last().ID)
                {
                    GameUpdater gU = new GameUpdater(patchHistory);
                    gU.ShowDialog();

                    if (gU.DialogResult == DialogResult.OK)
                    {
                        DialogResult = DialogResult.Cancel;
                        base.Close();
                    }
                    else
                    {
                        SetEnableInterfaceButtons(true);
                        SetEnableTextBox(false);
                    }
                }
                else
                {
                    //Enable Login Button
                    SetEnableTextBox(true);
                    SetEnableInterfaceButtons(true);
                }
            };
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
            btnLogin.Enabled = txtPassword.Enabled = txtNickname.Enabled = isEnabled;
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
            launcherRequestManager.PrepareLoginThread(this, txtNickname.Text, txtPassword.Text);
        }
        #endregion
    }
}
