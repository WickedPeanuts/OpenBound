using OpenBound_Management_Tools.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Management_Tools.Forms
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void DockerInstallFetchServerContainersButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();

            Console.Clear();

            ProcessHelper.ExecuteShellCommand(@"docker-compose -f .\Docker\OpenBoundFetchServerCompose.yml down");
            ProcessHelper.ExecuteShellCommand(@"docker volume rm openbound-fetch-server");
            ProcessHelper.ExecuteShellCommand(@"docker-compose -f .\Docker\OpenBoundFetchServerCompose.yml build");
            ProcessHelper.ExecuteShellCommand(@"docker-compose -f .\Docker\OpenBoundFetchServerCompose.yml up -d --force-recreate openbound-fetch-server");
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }
    }
}
