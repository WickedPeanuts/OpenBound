using OpenBound_Management_Tools.Common;
using OpenBound_Management_Tools.Helper;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.FileManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
            
            int containerPort     = Parameter.DEFAULT_FETCH_SERVER_CONTAINER_PORT;
            int startingLocalPort = Parameter.DEFAULT_FETCH_SERVER_STARTING_PORT;
            string containerName  = Parameter.DEFAULT_FETCH_SERVER_CONTAINER_NAME;
            string volumeName     = Parameter.DEFAULT_FETCH_SERVER_VOLUME_NAME;
            int numberOfContainers;

            Console.WriteLine("Enter the number of containers you intend to create: ");
            int.TryParse(Console.ReadLine(), out numberOfContainers);

            Console.WriteLine($"Enter the starting number of local ports you wish to use (default is {Parameter.DEFAULT_FETCH_SERVER_STARTING_PORT}): ");
            int.TryParse(Console.ReadLine(), out startingLocalPort);

            Dictionary<string, string> replacingTemplateFields
                = new Dictionary<string, string>()
                {
                    { "__versioning_folder__",   $"/{NetworkObjectParameters.FetchServerVersioningFolder}/" },
                    { "__game_patches_folder__", $"/{NetworkObjectParameters.FetchServerPatchesFolder}" },
                    { "__container_name__",      containerName },
                    { "__volume_name__",         volumeName },
                    { "__local_port__",          startingLocalPort.ToString() },
                    { "__container_port__",      containerPort.ToString() },
                    { "__context__",             Parameter.DEFAULT_FETCH_SERVER_CONTEXT },
                    { "__dockerfile_path__",     Parameter.DEFAULT_FETCH_SERVER_DOCKERFILE_PATH }
                };

            PipelineHelper.GenerateTemplateFiles(Directory.GetCurrentDirectory() + @"\DockerTemplates\FetchServer",
                Directory.GetCurrentDirectory() + @"\Docker", replacingTemplateFields);

            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f .\Docker\OpenBoundFetchServerCompose.yml build");

            for (int i = 0; i < numberOfContainers; i++)
            {
                PipelineHelper.ExecuteShellCommand(@$"docker run -d --name {containerName}-{i + 1} -p {startingLocalPort + i}:{containerPort} {containerName}");
                //PipelineHelper.ExecuteShellCommand(@$"docker-compose -p openbound-fetch-server -f .\Docker\OpenBoundFetchServerCompose.yml up -d {newContainerName}");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }

        private void dockerInstallGameServerContainerButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();
            Console.Clear();

            Console.WriteLine("---------------------");
            Console.WriteLine("Creating game server containers");
            Console.WriteLine("---------------------\n\n");

            int containerPort = Parameter.DEFAULT_GAME_SERVER_CONTAINER_PORT;
            int startingLocalPort = Parameter.DEFAULT_GAME_SERVER_STARTING_PORT;
            string containerName = Parameter.DEFAILT_GAME_SERVER_CONTAINER_NAME;
            string volumeName = Parameter.DEFAULT_GAME_SERVER_VOLUME_NAME;
            int numberOfContainers;

            Console.WriteLine("Enter the starting number of local ports you wish to use (default is 8100): ");
            int.TryParse(Console.ReadLine(), out startingLocalPort);

            Dictionary<string, string> replacingTemplateFields
                = new Dictionary<string, string>()
                {
                    { "__container_name__",      containerName },
                    { "__volume_name__",         volumeName },
                    { "__local_port__",          startingLocalPort.ToString() },
                    { "__container_port__",      containerPort.ToString() },
                    { "__context__",             Parameter.DEFAULT_GAME_SERVER_CONTEXT },
                    { "__dockerfile_path__",     Parameter.DEFAULT_GAME_SERVER_DOCKERFILE_PATH }
                };

            Console.WriteLine("Select the base project folder. This folder contains the \".sln\" file.");
            
            string folder = PipelineHelper.SelectFolder();
            string newDirectory = $"{Directory.GetCurrentDirectory()}/tmp/GameServer/OpenBound";
            Directory.CreateDirectory(newDirectory);
            PipelineHelper.CopyFolder(folder, newDirectory);

            PipelineHelper.GenerateTemplateFiles(Directory.GetCurrentDirectory() + @"/DockerTemplates/GameServer",
                newDirectory, replacingTemplateFields);

            File.Move($"{newDirectory}/GameServer.Dockerfile", $"{newDirectory}/OpenBound Game Server/GameServer.Dockerfile", true);
            File.Move($"{newDirectory}/OpenBoundGameServerCompose.yml", $"{Directory.GetParent(newDirectory).FullName}/OpenBoundGameServerCompose.yml", true);

            ConfigFileManager.CreateConfigFile(RequesterApplication.GameServer);

            

            PipelineHelper.ExecuteShellCommand(@$"cd ");
            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f .\Docker\OpenBoundFetchServerCompose.yml build");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }

        private void CreateGameUpdatePatch_Click(object sender, EventArgs e)
        {

        }

        private void UploadGameUpdatePatchesToAllContainers_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();
            Console.Clear();

            List<string> containerNames = GetCreatedContainerNames();

            UpdatePatchEntry pE = ReadMultipleFiles();

            foreach(string containerName in containerNames)
            {
                string filename = pE.PatchHistoryFile.Split('\\').Last();
                PipelineHelper.ExecuteShellCommand($"docker cp \"{pE.PatchHistoryFile}\" \"{containerName}:/{NetworkObjectParameters.FetchServerVersioningFolder}/{filename}\"");
                foreach (string patchPath in pE.PatchFiles)
                {
                    filename = patchPath.Split('\\').Last();
                    PipelineHelper.ExecuteShellCommand($"docker cp \"{pE.PatchHistoryFile}\" \"{containerName}:/{NetworkObjectParameters.FetchServerPatchesFolder}/{filename}\"");
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Program.HideConsole();
        }

        public static UpdatePatchEntry ReadMultipleFiles()
        {
            UpdatePatchEntry pE = new UpdatePatchEntry();

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Importing Files to be Uploaded...");
                string pExt = NetworkObjectParameters.GamePatchExtension;
                string phExt = NetworkObjectParameters.PatchHistoryExtension;
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = $"Openbound Patching Files ({pExt}, {phExt})|*{pExt};*{phExt};";
                dialog.ShowDialog();

                foreach (string str in dialog.FileNames)
                {
                    if (str.Contains(pExt))
                        pE.PatchFiles.Add(str);
                    else
                        pE.PatchHistoryFile = str;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return pE;
        }

        private List<string> GetCreatedContainerNames()
        {
            List<string> containerNames = new List<string>();
            string containerList = PipelineHelper.ExecuteShellCommand(@$"docker container ls");

            Regex rx = new Regex(@$"\b{Parameter.DEFAULT_FETCH_SERVER_CONTAINER_NAME}-[0-9]+\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(containerList);

            foreach (Match m in matches)
                containerNames.Add(m.Value);

            return containerNames;
        }
    }
}
