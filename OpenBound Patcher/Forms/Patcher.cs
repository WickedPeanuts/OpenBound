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
            InitializeComponent();

            foreach (string arg in args)
                MessageBox.Show(arg);
            
            gameClientPath = args[1];
            tmpFolderPath = args[2];

            copyActionQueue = new Queue<Action>();

            Process process = Process.GetProcessById(int.Parse(args[0]));
            process.WaitForExit();

        }

        private void Patcher_Load(object sender, EventArgs e)
        {
            List<string> fileList = Directory.GetFiles(tmpFolderPath, "*", SearchOption.AllDirectories).ToList();

            for(int i = 0; i < fileList.Count; i++)
            {
                string newfilepath = fileList[i].Replace($@"{gameClientPath}\", "");

                copyActionQueue.Enqueue(() =>
                {
                    progressBar1.Value = Math.Min(100 * (int)((i + 1) / (float)fileList.Count), 100);
                    Directory.Move(fileList[i], $@"{gameClientPath}\{newfilepath}");
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
