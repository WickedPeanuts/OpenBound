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

        //PipelineHelper.ExecuteShellCommand(@$"docker-compose -f .\Docker\OpenBoundFetchServer.yml down");

        private static void PrintHeader(string title)
        {
            Console.WriteLine("\n\n---------------------");
            Console.WriteLine(title);
            Console.WriteLine("---------------------\n\n");
        }

        private void dockerInstallDatabaseContainerButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();
            Console.Clear();

            PrintHeader("Creating database container");

            int localPort = Parameter.DEFAULT_DATABASE_SERVER_STARTING_PORT;
            Console.WriteLine($"Enter the starting number of local ports you wish to use (default is {Parameter.DEFAULT_DATABASE_SERVER_STARTING_PORT}): ");
            int.TryParse(Console.ReadLine(), out localPort);

            Dictionary<string, string> replacingTemplateFields
                = new Dictionary<string, string>()
                {
                    { "__versioning_folder__",   $"/{NetworkObjectParameters.FetchServerVersioningFolder}/" },
                    { "__game_patches_folder__", $"/{NetworkObjectParameters.FetchServerPatchesFolder}" },
                    { "__container_name__",      Parameter.DEFAULT_DATABASE_SERVER_CONTAINER_NAME },
                    { "__volume_name__",         Parameter.DEFAULT_DATABASE_SERVER_VOLUME_NAME },
                    { "__local_port__",          localPort.ToString() },
                    { "__container_port__",      Parameter.DEFAULT_DATABASE_SERVER_CONTAINER_PORT.ToString() },
                    { "__context__",             Parameter.DEFAULT_DATABASE_SERVER_CONTEXT },
                    { "__dockerfile_path__",     Parameter.DEFAULT_DATABASE_SERVER_DOCKERFILE_PATH }
                };

            string templatePath = Directory.GetCurrentDirectory() + @"\DockerTemplates\OpenBound Database";

            List<string> files = PipelineHelper.GenerateTemplateFiles(templatePath, templatePath, replacingTemplateFields, "*.Template.Dockerfile").ToList();
            files.AddRange(PipelineHelper.GenerateTemplateFiles(templatePath, templatePath, replacingTemplateFields, "*.Template.yml").ToList());

            PipelineHelper.ExecuteShellCommand($"docker-compose -p=openbound -f \".\\DockerTemplates\\OpenBound Database\\OpenBoundDatabase.yml\" up -d {Parameter.DEFAULT_DATABASE_SERVER_CONTAINER_NAME}");

            PipelineHelper.ExecuteShellCommand($"docker exec -it {Parameter.DEFAULT_DATABASE_SERVER_CONTAINER_NAME} /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P \"password\" ");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }

        private void DockerInstallFetchServerContainersButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();
            Console.Clear();

            PrintHeader("Creating fetch server container");
            
            int containerPort     = Parameter.DEFAULT_FETCH_SERVER_CONTAINER_PORT;
            int localPort         = Parameter.DEFAULT_FETCH_SERVER_STARTING_PORT;
            string containerName  = Parameter.DEFAULT_FETCH_SERVER_CONTAINER_NAME;
            string volumeName     = Parameter.DEFAULT_FETCH_SERVER_VOLUME_NAME;

            Console.WriteLine($"Enter the starting number of local ports you wish to use (default is {localPort}): ");
            int.TryParse(Console.ReadLine(), out localPort);

            Dictionary<string, string> replacingTemplateFields
                = new Dictionary<string, string>()
                {
                    { "__versioning_folder__",   $"/{NetworkObjectParameters.FetchServerVersioningFolder}/" },
                    { "__game_patches_folder__", $"/{NetworkObjectParameters.FetchServerPatchesFolder}" },
                    { "__container_name__",      containerName },
                    { "__volume_name__",         volumeName },
                    { "__local_port__",          localPort.ToString() },
                    { "__container_port__",      containerPort.ToString() },
                    { "__context__",             Parameter.DEFAULT_FETCH_SERVER_CONTEXT },
                    { "__dockerfile_path__",     Parameter.DEFAULT_FETCH_SERVER_DOCKERFILE_PATH }
                };

            string templatePath = Directory.GetCurrentDirectory() + @"\DockerTemplates\FetchServer";

            List<string> files = PipelineHelper.GenerateTemplateFiles(templatePath, templatePath, replacingTemplateFields, "*.Template.Dockerfile").ToList();
            files.AddRange(PipelineHelper.GenerateTemplateFiles(templatePath, templatePath, replacingTemplateFields, "*.Template.yml").ToList());
            files.AddRange(PipelineHelper.GenerateTemplateFiles(templatePath, templatePath, replacingTemplateFields, "*.Template.conf").ToList());

            PipelineHelper.ExecuteShellCommand(@$"docker-compose -p=openbound -f .\DockerTemplates\FetchServer\OpenBoundFetchServer.yml build");
            PipelineHelper.ExecuteShellCommand(@$"docker-compose -p openbound -f .\DockerTemplates\FetchServer\OpenBoundFetchServer.yml up -d {containerName}");
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }

        private void DockerInstallLoginServerContainerButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();
            Console.Clear();

            //Creating containers
            PrintHeader("Creating Login Server container");

            int containerPort = Parameter.DEFAULT_LOGIN_SERVER_CONTAINER_PORT;
            int startingLocalPort = Parameter.DEFAULT_LOGIN_SERVER_STARTING_PORT;
            string containerName = Parameter.DEFAILT_LOGIN_SERVER_CONTAINER_NAME;
            string volumeName = Parameter.DEFAULT_LOGIN_SERVER_VOLUME_NAME;

            Console.WriteLine($"Enter the starting local ports you wish to use (default is {containerPort}): ");
            int.TryParse(Console.ReadLine(), out startingLocalPort);

            Dictionary<string, string> replacingTemplateFields
                = new Dictionary<string, string>()
                {
                    { "__container_name__",      containerName },
                    { "__volume_name__",         volumeName },
                    { "__local_port__",          startingLocalPort.ToString() },
                    { "__container_port__",      containerPort.ToString() },
                    { "__context__",             Parameter.DEFAULT_LOGIN_SERVER_CONTEXT },
                    { "__dockerfile_path__",     Parameter.DEFAULT_LOGIN_SERVER_DOCKERFILE_PATH }
                };

            Console.WriteLine("Select the base project folder. This folder contains the \".sln\" file.");

            string slnDir = PipelineHelper.SelectSLNFile();
            slnDir = slnDir.Replace("\\" + slnDir.Split("\\").Last(), "");
            string lspDir = $@"{slnDir}\OpenBound Login Server";

            List<string> files = PipelineHelper.GenerateTemplateFiles(slnDir, slnDir, replacingTemplateFields, "*.Template.yml").ToList();
            files.AddRange(PipelineHelper.GenerateTemplateFiles(lspDir, lspDir, replacingTemplateFields, "*.Template.Dockerfile").ToList());

            //Building & Starting Container
            PrintHeader("Building Containers");

            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f {slnDir}\OpenBoundServerCompose.yml build");
            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f {slnDir}\OpenBoundServerCompose.yml up -d");

            //Configuring Container
            ConfigFileManager.CreateConfigFile(RequesterApplication.LoginServer, true);
            PrintHeader("Building Containers");

            Console.WriteLine("Close the notepad in order to continue the server configuration. Reminder: host.docker.internal is your ip from container's perspective.");
            Thread.Sleep(5000);

            PipelineHelper.ExecuteShellCommand($@"notepad.exe {Directory.GetCurrentDirectory()}\Config\LoginServerServerConfig.json");
            PipelineHelper.ExecuteShellCommand($@"notepad.exe {Directory.GetCurrentDirectory()}\Config\DatabaseConfig.json");

            PipelineHelper.ExecuteShellCommand($"docker cp \"{Directory.GetCurrentDirectory()}\\Config\\LoginServerServerConfig.json\" \"{containerName}:\\OpenBound Login Server\\Config\\LoginServerServerConfig.json\"");
            PipelineHelper.ExecuteShellCommand($"docker cp \"{Directory.GetCurrentDirectory()}\\Config\\DatabaseConfig.json\" \"{containerName}:\\OpenBound Login Server\\Config\\DatabaseConfig.json\"");

            foreach (string file in files)
                try { File.Delete(file); } catch { }

            PipelineHelper.ExecuteShellCommand(@$"docker restart {containerName}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }

        private void DockerInstallLobbyServerContainerButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();
            Console.Clear();

            //Creating containers
            PrintHeader("Creating Lobby Server container");

            int containerPort = Parameter.DEFAULT_LOBBY_SERVER_CONTAINER_PORT;
            int startingLocalPort = Parameter.DEFAULT_LOBBY_SERVER_STARTING_PORT;
            string containerName = Parameter.DEFAILT_LOBBY_SERVER_CONTAINER_NAME;
            string volumeName = Parameter.DEFAULT_LOBBY_SERVER_VOLUME_NAME;

            Console.WriteLine($"Enter the starting local ports you wish to use (default is {containerPort}): ");
            int.TryParse(Console.ReadLine(), out startingLocalPort);

            Dictionary<string, string> replacingTemplateFields
                = new Dictionary<string, string>()
                {
                    { "__container_name__",      containerName },
                    { "__volume_name__",         volumeName },
                    { "__local_port__",          startingLocalPort.ToString() },
                    { "__container_port__",      containerPort.ToString() },
                    { "__context__",             Parameter.DEFAULT_LOBBY_SERVER_CONTEXT },
                    { "__dockerfile_path__",     Parameter.DEFAULT_LOBBY_SERVER_DOCKERFILE_PATH }
                };

            Console.WriteLine("Select the base project folder. This folder contains the \".sln\" file.");

            string slnDir = PipelineHelper.SelectSLNFile();
            slnDir = slnDir.Replace("\\" + slnDir.Split("\\").Last(), "");
            string lspDir = $@"{slnDir}\OpenBound Lobby Server";

            List<string> files = PipelineHelper.GenerateTemplateFiles(slnDir, slnDir, replacingTemplateFields, "*.Template.yml").ToList();
            files.AddRange(PipelineHelper.GenerateTemplateFiles(lspDir, lspDir, replacingTemplateFields, "*.Template.Dockerfile").ToList());

            //Building & Starting Container
            PrintHeader("Building Containers");

            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f {slnDir}\OpenBoundServerCompose.yml build");
            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f {slnDir}\OpenBoundServerCompose.yml up -d");

            //Configuring Container
            ConfigFileManager.CreateConfigFile(RequesterApplication.LobbyServer, true);
            PrintHeader("Building Containers");

            Console.WriteLine("Close the notepad in order to continue the server configuration. Reminder: host.docker.internal is your ip from container's perspective.");
            Thread.Sleep(5000);

            PipelineHelper.ExecuteShellCommand($@"notepad.exe {Directory.GetCurrentDirectory()}\Config\LobbyServerListPlaceholders.json");
            PipelineHelper.ExecuteShellCommand($@"notepad.exe {Directory.GetCurrentDirectory()}\Config\LobbyServerServerConfig.json");
            PipelineHelper.ExecuteShellCommand($@"notepad.exe {Directory.GetCurrentDirectory()}\Config\LobbyServerWhitelist.json");

            PipelineHelper.ExecuteShellCommand($"docker cp \"{Directory.GetCurrentDirectory()}\\Config\\LobbyServerListPlaceholders.json\" \"{containerName}:\\OpenBound Lobby Server\\Config\\LobbyServerListPlaceholders.json\"");
            PipelineHelper.ExecuteShellCommand($"docker cp \"{Directory.GetCurrentDirectory()}\\Config\\LobbyServerServerConfig.json\" \"{containerName}:\\OpenBound Lobby Server\\Config\\LobbyServerServerConfig.json\"");
            PipelineHelper.ExecuteShellCommand($"docker cp \"{Directory.GetCurrentDirectory()}\\Config\\LobbyServerWhitelist.json\" \"{containerName}:\\OpenBound Lobby Server\\Config\\LobbyServerWhitelist.json\"");

            foreach (string file in files)
                try { File.Delete(file); } catch { }

            PipelineHelper.ExecuteShellCommand(@$"docker restart {containerName}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Program.HideConsole();
        }

        private void DockerInstallGameServerContainerButton_Click(object sender, EventArgs e)
        {
            Program.ShowConsole();
            Console.Clear();

            //Creating containers
            PrintHeader("Creating Game Server container");

            int containerPort = Parameter.DEFAULT_GAME_SERVER_CONTAINER_PORT;
            int startingLocalPort = Parameter.DEFAULT_GAME_SERVER_STARTING_PORT;
            string containerName = Parameter.DEFAILT_GAME_SERVER_CONTAINER_NAME;
            string volumeName = Parameter.DEFAULT_GAME_SERVER_VOLUME_NAME;

            Console.WriteLine($"Enter the local ports you wish to use (default is {startingLocalPort}): ");
            int.TryParse(Console.ReadLine(), out startingLocalPort);

            Console.WriteLine("Enter the server ID (copy this id on ServerID txt): ");
            int containerID = 0;
            int.TryParse(Console.ReadLine(), out containerID);

            containerName += $"-{containerID}";

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
            
            string slnDir = PipelineHelper.SelectSLNFile();
            slnDir = slnDir.Replace("\\" + slnDir.Split("\\").Last(), "");
            string gspDir = $@"{slnDir}\OpenBound Game Server";

            List<string> files = PipelineHelper.GenerateTemplateFiles(slnDir, slnDir, replacingTemplateFields, "*.Template.yml").ToList();
            files.AddRange(PipelineHelper.GenerateTemplateFiles(gspDir, gspDir, replacingTemplateFields, "*.Template.Dockerfile").ToList());

            //Building & Starting Container
            PrintHeader("Building Containers");

            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f {slnDir}\OpenBoundServerCompose.yml build");
            PipelineHelper.ExecuteShellCommand(@$"docker-compose -f {slnDir}\OpenBoundServerCompose.yml up -d");

            //Configuring Container
            ConfigFileManager.CreateConfigFile(RequesterApplication.GameServer, true);
            PrintHeader("Building Containers");
            
            Console.WriteLine("Close the notepad in order to continue the server configuration. Reminder: host.docker.internal is your ip from container's perspective.");
            Thread.Sleep(5000);

            PipelineHelper.ExecuteShellCommand($@"notepad.exe {Directory.GetCurrentDirectory()}\Config\DatabaseConfig.json");
            PipelineHelper.ExecuteShellCommand($@"notepad.exe {Directory.GetCurrentDirectory()}\Config\GameServerServerConfig.json");

            PipelineHelper.ExecuteShellCommand($"docker cp \"{Directory.GetCurrentDirectory()}\\Config\\DatabaseConfig.json\" \"{containerName}:\\OpenBound Game Server\\Config\\DatabaseConfig.json\"");
            PipelineHelper.ExecuteShellCommand($"docker cp \"{Directory.GetCurrentDirectory()}\\Config\\GameServerServerConfig.json\" \"{containerName}:\\OpenBound Game Server\\Config\\GameServerServerConfig.json\"");

            foreach (string file in files)
                try { File.Delete(file); } catch { }

            PipelineHelper.ExecuteShellCommand(@$"docker restart {containerName}");

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
