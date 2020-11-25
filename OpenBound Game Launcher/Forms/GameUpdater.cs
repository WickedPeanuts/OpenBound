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
using System.Globalization;
using System.IO;
using System.Linq;
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
        public Action RepeatedTickAction;
        
        private MenuState menuState;
        private bool shouldShowLog;

        private Label[] interfaceLabels;

        private PatchEntry currentPatchEntry;
        private List<PatchEntry> patchesToBeDownload;

        public GameUpdater()
        {
            TickAction = new AsynchronousAction();
            shouldShowLog = true;

            interfaceLabels = new Label[] { downloadLabel1, downloadLabel2, downloadLabel3 };

            InitializeComponent();

            UpdateMenuLabels(MenuState.ReadyToDownload);
        }

        public void Initialize(PatchHistory serverPatchHistory)
        {
            currentPatchEntry = NetworkObjectParameters.PatchHistory.PatchEntryList
                .OrderBy((x) => x.ReleaseDate).First();

            patchesToBeDownload = serverPatchHistory.PatchEntryList
                .Where((x) => x.ReleaseDate < currentPatchEntry.ReleaseDate)
                .OrderBy((x) => x.ReleaseDate).ToList();
        }

        private void GameUpdater_Load(object sender, EventArgs e)
        {
            //Calculate remaining versions
            downloadLabel3.Text = downloadLabel2.Text = downloadLabel1.Text = "";
            
            UpdateMenuButtons(MenuState.ReadyToDownload);
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
                    replacingTextDictionary = new Dictionary<string, string>()
                    {
                        { "currentversion", currentPatchEntry.PatchVersionName },
                        { "nextversion", patchesToBeDownload.Last().PatchVersionName },
                    };

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
                    replacingTextDictionary = new Dictionary<string, string>()
                    {
                        { "updatedat", DateTime.Now.ToString("G", CultureInfo.CurrentCulture) },
                    };

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

            if (replacingTextDictionary == null) return;

            foreach (Label label in interfaceLabels)
                foreach (KeyValuePair<string, string> kvp in replacingTextDictionary)
                    label.Text = label.Text.Replace($"%{kvp.Key}%", kvp.Value);
        }

        private string PatchDirectory => @$"{Directory.GetCurrentDirectory()}/tmp/";

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateMenuButtons(MenuState.Downloading);

            int currentFile = 0;

            Stack<Action> actStack = new Stack<Action>();

            foreach(PatchEntry pE in patchesToBeDownload)
            {
                actStack.Push(() =>
                {
                    string patchDir = PatchDirectory;
                    Directory.CreateDirectory(patchDir);

                    DateTime downloadStartTime = DateTime.Now;

                    HttpWebRequest.AsyncDownloadFile(
                        pE.PatchPath,
                        $@"{patchDir}/{pE.PatchPath}",
                        (p, r, t) => OnReceiveData(pE.PatchPath, currentFile, p, r, t, downloadStartTime),
                        () =>
                        {
                            TickAction += () =>
                            {
                                currentFile++;

                                if (actStack.Count > 0)
                                    actStack.Pop().Invoke();
                            };
                        });
                });
            }

            actStack.Pop().Invoke();

            HttpWebRequest.AsyncDownloadJsonObject<object>(
                "http://www.json-generator.com/api/json/get/cglxbuiiUO?indent=2",
                (o) =>
                {
                    MessageBox.Show(ObjectWrapper.Serialize(o));
                });
        }

        private void OnReceiveData(string filename, int downloadedFiles, float downloadPercentage, long receivedBytes, long totalBytes, DateTime downloadStartTime)
        {
            TickAction += () =>
            {
                double totalElapsedSeconds = (DateTime.Now - downloadStartTime).TotalSeconds;
                float totalReceivedMB = (receivedBytes / 1024);
                float maxSizeMB = (totalBytes / 1024);
                float remainingMB = totalReceivedMB - maxSizeMB;

                UpdateMenuLabels(MenuState.Downloading, new Dictionary<string, string>()
                {
                    { "downloadedfiles", $"{downloadedFiles}" },
                    { "totalfiles", $"{patchesToBeDownload.Count}" },
                    { "filename", filename },
                    { "totalsize", totalReceivedMB.ToString("0.00") },
                    { "downloadedsize", remainingMB.ToString("0.00") },
                    { "speed", (totalReceivedMB / totalElapsedSeconds).ToString("0.00") }
                });

                currentProgressBar.Value = (int)downloadPercentage;
            };
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
            RepeatedTickAction?.Invoke();
        }
    }
}
