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

        public Patcher(string[] args)
        {
            gameClientPath = args[1];
            tmpFolderPath = args[2];

            copyActionQueue = new Queue<Action>();

            Process process = Process.GetProcessById(int.Parse(args[0]));
            process.WaitForExit();

            InitializeComponent();
        }

        private void Patcher_Load(object sender, EventArgs e)
        {
            List<string> fileList = Directory.GetFiles(
                Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories).ToList();

            for(int i = 0; i < fileList.Count; i++)
            {
                string filename = fileList[i].Split(@"\").Last();

                copyActionQueue.Enqueue(() =>
                {
                    progressBar1.Value = Math.Min(100 * (int)((i + 1) / (float)fileList.Count), 100);
                    Directory.Move(fileList[i], $@"{Directory.GetCurrentDirectory()}\{tmpFolderPath}\{filename}");
                });
            }

            copyActionQueue.Enqueue(CloseGame);
        }

        private void CloseGame()
        {
            Process.GetCurrentProcess().Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            copyActionQueue.Dequeue().Invoke();
        }
    }
}
