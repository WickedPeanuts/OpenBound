using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Helper;
using OpenBound_Game_Launcher.Properties;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using OpenBound_Network_Object_Library.WebRequest;
using OpenBound_Patcher.Forms;
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
using System.Threading;
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
        
        private bool shouldShowLog;

        private Label[] interfaceLabels;

        private int currentDownloadedFile;

        private PatchHistory currentPatchEntry;
        private List<PatchHistory> patchesToBeDownloaded;

        public GameUpdater(PatchHistory serverPatchHistory)
        {
            InitializeComponent();

            Timer1TickAction = new AsynchronousAction();
            Timer2TickAction = new AsynchronousAction();

            shouldShowLog = true;

            interfaceLabels = new Label[] { downloadLabel1, downloadLabel2, downloadLabel3 };

            currentPatchEntry = Parameter.GameClientSettingsInformation.ClientVersionHistory;

            patchesToBeDownloaded = 
                serverPatchHistory.PatchHistoryList.Union(new List<PatchHistory> { serverPatchHistory })
                .Where((x) => x.CreationDate > currentPatchEntry.CreationDate)
                .OrderBy((x) => x.CreationDate).ToList();

            UpdateMenuLabels(MenuState.ReadyToDownload);

            InsertOnLogListBox(Language.Patching1Ready);
        }

        private void InsertOnLogListBox(string text)
        {
            Timer1TickAction += () =>
            {
                logListBox.Items.Add(text);
                logListBox.SelectedItem = text;
            };
        }

        private void GameUpdater_Load(object sender, EventArgs e) { }

        private void UpdateMenuButtons(MenuState menuState)
        {
            switch (menuState)
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
                        { "nextversion", patchesToBeDownloaded.Last().PatchVersionName },
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

        private void updateButton_Click(object sender, EventArgs e)
        {
            //If someone attempt to shut down the updater before it has finished.
            FormClosing += abortButton_Click;

            Queue<Action> actQueue = new Queue<Action>();
            List<PatchHistory> downloadedFileList = new List<PatchHistory>();

            InsertOnLogListBox(Language.Patching1Downloading);

            string patchDir = @$"{Directory.GetCurrentDirectory()}\{NetworkObjectParameters.PatchTemporaryPath}\";

            Directory.CreateDirectory(patchDir);

            foreach (PatchHistory pH in patchesToBeDownloaded)
            {
                actQueue.Enqueue(() =>
                {
                    DateTime downloadStartTime = DateTime.Now;

                    InsertOnLogListBox($"{Language.Patching2Downloading} {pH.BuildPatchPath}");

                    UpdateMenuLabels(MenuState.Downloading, new Dictionary<string, string>()
                    {
                        { "filename", pH.BuildPatchPath },
                        { "current", $"{currentDownloadedFile + 1}" },
                        { "total", $"{patchesToBeDownloaded.Count}" },

                        { "remaining", "0" },
                        { "downloaded", "0" },
                        { "speed", "0" }
                    });

                    currentProgressBar.Value = 0;

                    HttpWebRequest.AsyncDownloadFile(
                        $@"{NetworkObjectParameters.BuildGamePatchURL()}/{pH.BuildPatchPath}",
                        $@"{patchDir}\{pH.BuildPatchPath}",
                        onReceiveData: 
                            (percentage, receivedBytes, totalBytes) =>
                            {
                                OnReceiveData(pH.BuildPatchPath, currentDownloadedFile, percentage, receivedBytes, totalBytes, downloadStartTime);
                            },
                        onReceiveLastData: () => { OnFinalizeDownloadingFile(actQueue, pH); },
                        onFailToDownload: (ex) => { OnFailToDownloadPatch(ex, pH); });
                });
            }

            //Start triggeting the asynchronous callback hell
            actQueue.Dequeue().Invoke();
        }

        private void OnFailToDownloadPatch(Exception exception, PatchHistory patchHistory)
        {
            InsertOnLogListBox($"{Language.Patching4ExceptionUnpack} {patchHistory.BuildPatchPath}");
            InsertOnLogListBox($"{Language.Patching5ExceptionUnpack}");
            HaltPatchingProcess(exception);
        }

        private void OnFinalizeDownloadingFile(Queue<Action> actionQueue, PatchHistory patchHistory)
        {
            Timer1TickAction += new Action(() =>
            {
                InsertOnLogListBox($"{Language.Patching3Downloading} {patchHistory.BuildPatchPath}");

                currentDownloadedFile++;

                totalProgressBar.Value = (int)Math.Ceiling(100 * (currentDownloadedFile / (float)patchesToBeDownloaded.Count));

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
                    { "total", $"{patchesToBeDownloaded.Count}" },
                    
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

            InsertOnLogListBox(Language.Patching1Done);
            InsertOnLogListBox(Language.Patching1Unpacking);

            GamePatcher.UnpackPatchList(
                Directory.GetCurrentDirectory(),
                patchesToBeDownloaded,
                OnStartUnpackingPatch,
                OnUnpackPatch);
        }

        private void OnStartUnpackingPatch(PatchHistory patch)
        {
            Timer1TickAction += () =>
            {
                InsertOnLogListBox($"{Language.Patching2Unpacking} {patch.BuildPatchPath}");

                UpdateMenuLabels(MenuState.Unpacking, new Dictionary<string, string>()
                {
                    { "patchname", $"{patch.PatchVersionName} - {patch.BuildPatchPath}"  }
                });
            };
        }

        private void OnUnpackPatch(PatchHistory patch, bool isMD5Valid, Exception exception)
        {
            Timer1TickAction += () =>
            {
                int current = patchesToBeDownloaded.IndexOf(patch);
                int total = patchesToBeDownloaded.Count;

                totalProgressBar.Value = currentProgressBar.Value = 100 * ((current + 1) / total);

                if (exception != null)
                {
                    InsertOnLogListBox($"{Language.Patching1ExceptionUnpack} {patch.BuildPatchPath}");
                    InsertOnLogListBox($"{Language.Patching2ExceptionUnpack}");
                    HaltPatchingProcess(exception);
                } else if (!isMD5Valid)
                {
                    InsertOnLogListBox($"{Language.Patching1ExceptionUnpack} {patch.BuildPatchPath}");
                    InsertOnLogListBox($"{Language.Patching3ExceptionUnpack}");
                    HaltPatchingProcess();
                }
                else
                {
                    InsertOnLogListBox($"{Language.Patching3Unpacking} {patch.BuildPatchPath}");
                }

                if (patchesToBeDownloaded.Last() == patch)
                {
                    timer2.Enabled = true;

                    InsertOnLogListBox($"{Language.Patching2Done}");

                    DateTime date = DateTime.Now;
                    CloseClientAndApplyPatchTickAction(date);
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

            Size = new Size(Size.Width, Size.Height + offset);

            shouldShowLog = !shouldShowLog;
        }

        private void CloseClientAndApplyPatchTickAction(DateTime unpackingFinalizationTime, int lastSecDiff = 10)
        {
            double timediff = (DateTime.Now - unpackingFinalizationTime).TotalSeconds;
            int secdiff = 10 - (int)timediff;

            if (secdiff > 0)
            {
                if (secdiff != lastSecDiff)
                {
                    InsertOnLogListBox($"{Language.Patching3Done} {secdiff} {Language.Patching4Done}");

                    UpdateMenuLabels(
                        MenuState.Done,
                        new Dictionary<string, string>()
                        {
                        { "updatedat", unpackingFinalizationTime.ToString("G", CultureInfo.CurrentCulture) },
                        { "secondstopatch", $"{secdiff}" },
                        });
                }

                Timer2TickAction += () => { CloseClientAndApplyPatchTickAction(unpackingFinalizationTime, secdiff); };
            }
            else
            {
                OpenPatcher();
            }
        }

        private void HaltPatchingProcess(Exception exception = null)
        {
            Timer1TickAction.Clear();

            Timer1TickAction += () =>
            {
                InsertOnLogListBox($"{Language.GameUpdaterLabel4Error}");

                if (exception != null)
                {
                    InsertOnLogListBox($"{Language.GameUpdaterLabel5Error} {exception.Message}");

                    if (exception.InnerException != null && exception.Message != exception.InnerException.Message)
                    {
                        InsertOnLogListBox($"{Language.GameUpdaterLabel6Error} {exception.InnerException.Message}");
                    }
                }

                timer2.Enabled = false;
            };
        }

        private void OpenPatcher()
        {
            string[] applicationParameter = new string[]{
                    $"\"{Process.GetCurrentProcess().Id}\"",
                    $"\"{Directory.GetCurrentDirectory()}\"",
                    $"\"{Directory.GetCurrentDirectory()}\\{NetworkObjectParameters.PatchTemporaryPath}\\{NetworkObjectParameters.PatchUnpackPath}\"",
                    $"\"{patchesToBeDownloaded.Last()}\"",
                    $"\"{currentPatchEntry}\"",
                    $"\"{NetworkObjectParameters.GameClientProcessName}\""
                };

            /*Patcher p = new Patcher(applicationParameter);
            p.ShowDialog();*/

            Process process = new Process();
            process.StartInfo.FileName = $@"{Directory.GetCurrentDirectory()}\{NetworkObjectParameters.PatcherProcessName}";
            process.StartInfo.Arguments = string.Join(' ', applicationParameter);
            process.Start();

            FormClosing -= abortButton_Click;
            DialogResult = DialogResult.OK;
            Close();
        }


        private void abortButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            DialogResult dr = Feedback.CreateWarningMessageBox(Language.GameUpdaterAbortUpdate,
                messageBoxButtons: MessageBoxButtons.YesNo);

            if (dr == DialogResult.No)
            {
                if (e is FormClosingEventArgs)
                    ((FormClosingEventArgs)e).Cancel = true;

                timer1.Enabled = true;
            }
            else
            {
                FormClosing -= abortButton_Click;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Timer2TickAction.AsynchronousInvokeAndDestroy();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Timer1TickAction.AsynchronousInvokeAndDestroy();
        }
    }
}
