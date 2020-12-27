using OpenBound_Patcher.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OpenBound_Patcher.Forms
{
    public partial class Patcher : Form
    {
        Queue<Action> copyActionQueue;

        string gameClientPath;
        string tmpFolderPath;
        string downloadedPatch;
        string currentPatch;
        string gameClientProcessName;

        List<string> invalidFileList;

        public Patcher(string[] args)
        {
            InitializeComponent();

            invalidFileList = new List<string>();

            Process process = Process.GetProcessById(int.Parse(args[0]));
            process.WaitForExit();

            gameClientPath = args[1];
            tmpFolderPath = args[2];
            downloadedPatch = args[3];
            currentPatch = args[4];
            gameClientProcessName = args[5];

            installingLabel.Text = "";

            copyActionQueue = new Queue<Action>();
        }

        private void Patcher_Load(object sender, EventArgs e)
        {
            List<string> fileList = Directory.GetFiles(tmpFolderPath, "*", SearchOption.AllDirectories).ToList();

            progressBar1.Maximum = fileList.Count;

            for (int i = 0; i < fileList.Count; i++)
            {
                string oldFilePath = fileList[i];
                string newfilepath = fileList[i].Replace(tmpFolderPath, gameClientPath);
                string displayPath = fileList[i].Replace($@"{tmpFolderPath}\", "");

                copyActionQueue.Enqueue(() =>
                {
                    try
                    {
                        progressBar1.PerformStep();
                        installingLabel.Text = $"{Language.InstallingLabelText}:{displayPath}";
                        File.Move(oldFilePath, newfilepath, true);
                    }
                    catch (Exception)
                    {
                        invalidFileList.Add(displayPath);
                    }
                });
            }

            copyActionQueue.Enqueue(FinalizePatching);
        }

        private void FinalizePatching()
        {
            if (invalidFileList.Count > 0)
            {
                MessageBox.Show($@"{Language.Exception1Text1}\n{Language.Exception1Text2}{tmpFolderPath}\n{Language.Exception1Text3}{gameClientPath}",
                    Language.Exception1Title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                invalidFileList.Add($"{currentPatch} -> {downloadedPatch}");

                try
                {
                    File.WriteAllLines($@"{gameClientPath}\PatchError.log", invalidFileList);
                }
                catch (Exception)
                {
                    MessageBox.Show($@"{Language.Exception2Text1}",
                        Language.Exception2Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    Directory.Delete(tmpFolderPath, true);
                }
                catch (Exception)
                {
                    MessageBox.Show($@"{Language.Exception3Text1}",
                        Language.Exception3Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            installingLabel.Text = "";

            DateTime finishedProcessTime = DateTime.Now;
            PrepareToCloseApplication(finishedProcessTime);
        }

        private void PrepareToCloseApplication(DateTime finishedProcessTime)
        {
            double timediff = (DateTime.Now - finishedProcessTime).TotalSeconds;
            int secdiff = 10 - (int)timediff;

            if (secdiff > 0)
            {
                statusLabel.Text = $"Re-opening the launcher in {secdiff} seconds...";
                copyActionQueue.Enqueue(() => PrepareToCloseApplication(finishedProcessTime));
            }
            else
            {
                CloseApplication();
            }
        }

        private void CloseApplication()
        {
            try
            {
                Process process = new Process();

                if (invalidFileList.Count == 0)
                    process.StartInfo.FileName = $@"{gameClientPath}\{gameClientProcessName}";

                process.StartInfo.Arguments = $@"{currentPatch} {downloadedPatch}";
                process.Start();
            }
            catch (Exception) { }

            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (copyActionQueue.Count() > 0)
                copyActionQueue.Dequeue().Invoke();
        }
    }
}
