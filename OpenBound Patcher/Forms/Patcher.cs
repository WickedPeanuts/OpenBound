using OpenBound_Patcher.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

            gameClientPath = args[1];
            tmpFolderPath = args[2];
            downloadedPatch = args[3];
            currentPatch = args[4];
            gameClientProcessName = args[5];

            installingLabel.Text = "";

            copyActionQueue = new Queue<Action>();

            Process process = Process.GetProcessById(int.Parse(args[0]));
            //process.WaitForExit();
        }

        private void Patcher_Load(object sender, EventArgs e)
        {
            List<string> fileList = Directory.GetFiles(tmpFolderPath, "*", SearchOption.AllDirectories).ToList();

            progressBar1.Maximum = fileList.Count;

            for(int i = 0; i < fileList.Count; i++)
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
                    catch(Exception)
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
                MessageBox.Show(
                    "One or more files couldn't be moved from the temporary folder\n" +
                    $"{tmpFolderPath}\n" +
                    "into the game's folder.\n" +
                    $"{gameClientPath}" +
                    "this error might have happened because your user does not own the rights\n" +
                    "to perform operations on this folder. You can try doing it manually or\n" +
                    "update the game again with administrator privilleges. If the error persists\n" +
                    "contact the support and send them the following file: \"PatchError.log\"\n");

                invalidFileList.Add($"{currentPatch} -> {downloadedPatch}");
                File.WriteAllLines(gameClientPath, invalidFileList);
            } else {
                try
                {
                    Directory.Delete(tmpFolderPath, true);
                }
                catch (Exception)
                {
                    MessageBox.Show("The application couldn't wipe the temporary folder\n" +
                        $"{tmpFolderPath}\n" +
                        "this will not generate any problems in your gaming experience but\n" +
                        "some unused are going to remain in the game until your next\n" +
                        "installation. Consider granting this application\n" +
                        "(OpenBound Patcher.exe) administrator privilleges");
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
                process.StartInfo.FileName = $@"{gameClientPath}\{gameClientProcessName}";
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
