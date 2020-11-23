using OpenBound_Game_Launcher.Properties;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using OpenBound_Network_Object_Library.WebRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Language = OpenBound_Game_Launcher.Common.Language;

namespace OpenBound_Game_Launcher.Forms
{
    public enum MenuState
    {
        ReadyToDownload,
        Downloading,
        Installing,
        Done,
        Error,
    }

    public partial class GameUpdater : Form
    {
        public AsynchronousAction TickAction;
        
        private MenuState menuState;
        private bool shouldShowLog;

        private PatchHistory patchHistory;
        private Label[] interfaceLabels;

        public GameUpdater()
        {
            TickAction = new AsynchronousAction();
            shouldShowLog = true;

            interfaceLabels = new Label[] { downloadLabel1, downloadLabel2, downloadLabel3 };

            InitializeComponent();
        }

        public void Initialize(PatchHistory patchHistory)
        {
            this.patchHistory = patchHistory;
        }

        private void GameUpdater_Load(object sender, EventArgs e)
        {
            downloadLabel3.Text = downloadLabel2.Text = downloadLabel1.Text = "";

            //Calculate remaining versions
            

            UpdateMenuButtons(MenuState.ReadyToDownload);
            //UpdateMenuLabels()
        }

        private void UpdateMenuButtons(MenuState menuState)
        {
            if (this.menuState == menuState)
                return;

            switch (this.menuState = menuState)
            {
                case MenuState.ReadyToDownload:
                    abortButton.Enabled = false;
                    updateButton.Enabled = true;
                    break;
                case MenuState.Downloading:
                    abortButton.Enabled = true;
                    updateButton.Enabled = false;
                    break;
                case MenuState.Installing:
                    abortButton.Enabled = false;
                    updateButton.Enabled = false;
                    break;
                case MenuState.Done:
                    abortButton.Enabled = false;
                    updateButton.Enabled = false;
                    break;
            }
        }

        private void UpdateMenuLabels(MenuState menuState, Dictionary<string, string> replacingTextDictionary = null)
        {
            switch (menuState)
            {
                case MenuState.ReadyToDownload:
                    downloadLabel1.Text = Language.GameUpdaterLabel1ReadyToDownload;
                    downloadLabel2.Text = Language.GameUpdaterLabel2ReadyToDownload;
                    downloadLabel3.Text = Language.GameUpdaterLabel3ReadyToDownload;
                    break;
                case MenuState.Downloading:
                    downloadLabel1.Text = Language.GameUpdaterLabel1Downloading;
                    downloadLabel2.Text = Language.GameUpdaterLabel2Downloading;
                    downloadLabel3.Text = Language.GameUpdaterLabel3Downloading;
                    break;
                case MenuState.Installing:
                    downloadLabel1.Text = Language.GameUpdaterLabel1Installing;
                    downloadLabel2.Text = Language.GameUpdaterLabel2Installing;
                    downloadLabel3.Text = Language.GameUpdaterLabel3Installing;
                    break;
                case MenuState.Done:
                    downloadLabel1.Text = Language.GameUpdaterLabel1Done;
                    downloadLabel2.Text = Language.GameUpdaterLabel2Done;
                    downloadLabel3.Text = Language.GameUpdaterLabel3Done;
                    break;
                case MenuState.Error:
                    downloadLabel1.Text = Language.GameUpdaterLabel1Error;
                    downloadLabel2.Text = Language.GameUpdaterLabel2Error;
                    downloadLabel3.Text = Language.GameUpdaterLabel3Error;
                    break;
            }

            foreach (Label label in interfaceLabels)
                foreach (KeyValuePair<string, string> kvp in replacingTextDictionary)
                    label.Text = label.Text.Replace($"%{kvp.Key}%", kvp.Value);
        }

        private void calculateRemaining

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateMenuButtons(MenuState.Downloading);
            UpdateMenuLabels(MenuState.Downloading, new Dictionary<string, string>()
            {

            });

            HttpWebRequest.AsyncDownloadJsonObject<object>(
                "http://www.json-generator.com/api/json/get/cglxbuiiUO?indent=2",
                (o) =>
                {
                    MessageBox.Show(ObjectWrapper.Serialize(o));
                });
        }

        private void toggleLogButton_Click(object sender, EventArgs e)
        {
            int offset = logGroupBox.Height;

            if (shouldShowLog)
            {
                logGroupBox.Hide();
                offset *= -1;
                toggleLogButton.Text = "Show Log";
            }
            else
            {
                logGroupBox.Show();
                toggleLogButton.Text = "Hide Log";
            }

            toggleLogButton.Location = new Point(toggleLogButton.Location.X, toggleLogButton.Location.Y + offset);
            updateButton.Location = new Point(updateButton.Location.X, updateButton.Location.Y + offset);
            abortButton.Location = new Point(abortButton.Location.X, abortButton.Location.Y + offset);
            closeButton.Location = new Point(closeButton.Location.X, closeButton.Location.Y + offset);

            Size = new Size(Size.Width, Size.Height + offset);

            shouldShowLog = !shouldShowLog;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TickAction.AsynchronousInvokeAndDestroy();
        }
    }
}
