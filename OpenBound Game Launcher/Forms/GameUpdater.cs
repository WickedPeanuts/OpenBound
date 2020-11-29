using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Properties;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using OpenBound_Network_Object_Library.WebRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        Unpacking,
        Done,
        Error,
    }

    public partial class GameUpdater : Form
    {
        public AsynchronousAction Timer1TickAction, Timer2TickAction;
        
        private MenuState menuState;
        private bool shouldShowLog;

        private Label[] interfaceLabels;

        private int currentDownloadedFile;

        private PatchEntry currentPatchEntry;
        private List<PatchEntry> patchesToBeDownload;

        public GameUpdater(PatchHistory serverPatchHistory)
        {
            InitializeComponent();

            Timer1TickAction = new AsynchronousAction();
            Timer2TickAction = new AsynchronousAction();

            shouldShowLog = true;

            interfaceLabels = new Label[] { downloadLabel1, downloadLabel2, downloadLabel3 };

            currentPatchEntry = Parameter.GameClientSettingsInformation.ClientVersionHistory.PatchEntryList
                .OrderBy((x) => x.ReleaseDate).First();

            patchesToBeDownload = serverPatchHistory.PatchEntryList
                .Where((x) => x.ReleaseDate < currentPatchEntry.ReleaseDate)
                .OrderBy((x) => x.ReleaseDate).ToList();

            UpdateMenuLabels(MenuState.ReadyToDownload);
        }

        private void GameUpdater_Load(object sender, EventArgs e) { }

        private void UpdateMenuButtons(MenuState menuState)
        {
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
                case MenuState.Unpacking:
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
            UpdateMenuButtons(menuState);

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
                case MenuState.Unpacking:
                    downloadLabel1.Text = Language.GameUpdaterLabel1Unpacking;
                    downloadLabel2.Text = Language.GameUpdaterLabel2Unpacking;
                    downloadLabel3.Text = Language.GameUpdaterLabel3Unpacking;
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

            if (replacingTextDictionary == null) return;

            foreach (Label label in interfaceLabels)
                foreach (KeyValuePair<string, string> kvp in replacingTextDictionary)
                    label.Text = label.Text.Replace($"%{kvp.Key}%", kvp.Value);
        }

        #region Download Async Callback Hell
        private string BuildPatchURLPath(string patchPath) =>
            $@"http://{NetworkObjectParameters.FetchServerInformation.ServerPublicAddress}/{NetworkObjectParameters.FetchServerVersioningFolder}/{NetworkObjectParameters.FetchServerPatchesFolder}/{patchPath}";

        private void updateButton_Click(object sender, EventArgs e)
        {
            Queue<Action> actQueue = new Queue<Action>();
            List<PatchEntry> downloadedFileList = new List<PatchEntry>();

            string patchDir = @$"{Directory.GetCurrentDirectory()}\{NetworkObjectParameters.PatchTemporaryPath}\";

            Directory.CreateDirectory(patchDir);

            foreach (PatchEntry pE in patchesToBeDownload)
            {
                actQueue.Enqueue(() =>
                {
                    DateTime downloadStartTime = DateTime.Now;

                    UpdateMenuLabels(MenuState.Downloading, new Dictionary<string, string>()
                    {
                        { "filename", pE.Path },
                        { "current", $"{currentDownloadedFile + 1}" },
                        { "total", $"{patchesToBeDownload.Count}" },

                        { "remaining", "0" },
                        { "downloaded", "0" },
                        { "speed", "0" }
                    });

                    currentProgressBar.Value = 0;

                    HttpWebRequest.AsyncDownloadFile(
                        BuildPatchURLPath(pE.Path),
                        $@"{patchDir}\{pE.Path}",
                        (percentage, receivedBytes, totalBytes) =>
                        {
                            OnReceiveData(pE.Path, currentDownloadedFile, percentage, receivedBytes, totalBytes, downloadStartTime);
                        },
                        onFinishDownload: () => { OnFinalizeDownloadingFile(actQueue); },
                        onFailToDownload: (ex) => { OnFailToDownloadPatch(ex, pE); });
                });
            }

            //Start triggeting the asynchronous callback hell
            actQueue.Dequeue().Invoke();
        }

        private void OnFailToDownloadPatch(Exception exception, PatchEntry patchEntry)
        {
            MessageBox.Show(exception.Message);
        }

        private void OnFinalizeDownloadingFile(Queue<Action> actionQueue)
        {
            Timer1TickAction += new Action(() =>
            {
                currentDownloadedFile++;

                totalProgressBar.Value = (int)Math.Ceiling(100 * (currentDownloadedFile / (float)patchesToBeDownload.Count));

                if (actionQueue.Count > 0)
                {
                    actionQueue.Dequeue().Invoke();
                }
                else
                {
                    StartUnpackingFiles();
                }
            });
        }

        private void OnReceiveData(string filename, int downloadedFiles, float downloadPercentage, 
            long receivedBytes, long totalBytes, DateTime downloadStartTime)
        {
            Timer1TickAction += () =>
            {
                double totalElapsedSeconds = Math.Max(1, (DateTime.Now - downloadStartTime).TotalSeconds);
                float totalReceivedMB = ((receivedBytes / 1024f) / 1024f);
                float maxSizeMB = ((totalBytes / 1024f) / 1024f);
                float remainingMB = maxSizeMB - totalReceivedMB;

                UpdateMenuLabels(MenuState.Downloading, new Dictionary<string, string>()
                {
                    { "filename", filename },
                    { "current", $"{downloadedFiles + 1}" },
                    { "total", $"{patchesToBeDownload.Count}" },
                    
                    { "remaining", remainingMB.ToString("0.##") },
                    { "downloaded", totalReceivedMB.ToString("0.##") },
                    { "speed", (totalReceivedMB / totalElapsedSeconds).ToString("0.##") }
                });

                currentProgressBar.Value = Math.Min((int)downloadPercentage, 100);
            };
        }
        #endregion

        private void StartUnpackingFiles()
        {
            currentProgressBar.Value = 0;
            totalProgressBar.Value = 0;

            GamePatcher.UnpackPatchList(
                Directory.GetCurrentDirectory(),
                patchesToBeDownload,
                OnStartUnpackingPatch,
                OnUnpackPatch);
        }

        private void OnStartUnpackingPatch(PatchEntry patch)
        {
            Timer1TickAction += () =>
            {
                UpdateMenuLabels(MenuState.Unpacking, new Dictionary<string, string>()
                {
                    { "patchname", $"{patch.PatchVersionName} - {patch.Path}"  }
                });
            };
        }

        private void OnUnpackPatch(PatchEntry patch, bool success)
        {
            Timer1TickAction += () =>
            {
                int current = patchesToBeDownload.IndexOf(patch);
                int total = patchesToBeDownload.Count;

                totalProgressBar.Value = currentProgressBar.Value = 100 * ((current + 1) / total);

                if (success)
                {

                }
                else
                {
                    MessageBox.Show("Deu merda capitão: " + patch.Path);
                }

                if (patchesToBeDownload.Last() == patch)
                {
                    timer2.Enabled = true;
                    Timer2TickAction += () => { CloseClientAndApplyPatchTickAction(DateTime.Now); };
                }
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

        private void CloseClientAndApplyPatchTickAction(DateTime unpackingFinalizationTime)
        {
            double timediff = (unpackingFinalizationTime - DateTime.Now).TotalSeconds;
            int secdiff = 10 - (int)timediff;

            if (secdiff > 0)
            {
                UpdateMenuLabels(
                    MenuState.Done,
                    new Dictionary<string, string>()
                    {
                        { "updatedat", DateTime.Now.ToString("G", CultureInfo.CurrentCulture) },
                        { "secondstopatch", $"{secdiff}" },
                    });
            }
            else
            {
                OpenPatcher();
            }
        }

        private void OpenPatcher()
        {
            string[] applicationParameter = new string[]{
                    $"{Process.GetCurrentProcess().Id}",
                    NetworkObjectParameters.GameClientProcessName,
                    @$"{NetworkObjectParameters.PatchTemporaryPath}\{NetworkObjectParameters.PatchUnpackPath}"
                };

            Process.Start(
                $@"{Directory.GetCurrentDirectory()}/{NetworkObjectParameters.PatcherProcessName}",
                string.Join(' ', applicationParameter));

            Process.GetCurrentProcess().Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Timer2TickAction.AsynchronousInvoke();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Timer1TickAction.AsynchronousInvokeAndDestroy();
        }
    }
}
