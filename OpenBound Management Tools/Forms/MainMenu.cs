using OpenBound_Management_Tools.Helper;
using OpenBound_Network_Object_Library.Common;
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

        //PipelineHelper.ExecuteShellCommand(@$"docker-compose -f .\Docker\OpenBoundFetchServerCompose.yml down");

        private void DockerInstallFetchServerContainersButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();

            Console.Clear();

            Console.WriteLine("---------------------");
            Console.WriteLine("Creating fetch server containers");
            Console.WriteLine("---------------------\n\n");

            Console.WriteLine("Enter the number of containers you intend to create: ");
            int numberOfContainers = int.Parse(Console.ReadLine());

            int startingLocalPort = 8100;
            Console.WriteLine("Enter the starting number of local ports you wish to use (default is 8100): ");
            try
            {
                startingLocalPort = int.Parse(Console.ReadLine());
            }
            catch (Exception) { }

            for (int i = 0; i < numberOfContainers; i++)
            {
                int containerID = 1 + i;
                string newContainerName = $"openbound-fetch-server-{containerID}";
                Dictionary<string, string> replacingTemplateFields
                    = new Dictionary<string, string>()
                    {
                        { "%versioning_folder%", $"/{NetworkObjectParameters.FetchServerVersioningFolder}/" },
                        { "%game_patches_folder%", $"/{NetworkObjectParameters.FetchServerPatchesFolder}" },
                        { "%container_name%", $"openbound-fetch-server-{containerID}" },
                        { "%local_port%", (startingLocalPort + i).ToString() },
                        { "%container_port%", "8000" }
                    };

                PipelineHelper.GenerateTemplateFiles(Directory.GetCurrentDirectory() + @"\DockerTemplates\FetchServer",
                Directory.GetCurrentDirectory() + @"\Docker", replacingTemplateFields);

                PipelineHelper.ExecuteShellCommand(@$"docker-compose -f .\Docker\OpenBoundFetchServerCompose.yml build");
                PipelineHelper.ExecuteShellCommand(@$"docker-compose -p openbound-fetch-server -f .\Docker\OpenBoundFetchServerCompose.yml up -d {newContainerName}");
                PipelineHelper.ExecuteShellCommand(@$"docker volume rm openbound-fetch-server-{containerID}");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }
    }
}
