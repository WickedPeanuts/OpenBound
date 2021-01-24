using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OpenBound_Management_Tools.Helper
{
    public class UpdatePatchEntry
    {
        public string PatchHistoryFile;
        public List<string> PatchFiles;

        public UpdatePatchEntry()
        {
            PatchHistoryFile = "";
            PatchFiles = new List<string>();
        }
    }

    class PipelineHelper
    {
        public static IEnumerable<string> GenerateTemplateFiles(string templateFolder, string baseOutputPath, Dictionary<string, string> fileFields, string searchPattern = "*")
        {
            string[] files = Directory.GetFiles(templateFolder, searchPattern);

            foreach (string file in files)
            {
                yield return GenerateTemplateFile(file, baseOutputPath, fileFields);
            }
        }

        private static string GenerateTemplateFile(string templateFilePath, string fileOutputPath, Dictionary<string, string> fileFields)
        {
            string text = File.ReadAllText(templateFilePath);
            string filename = templateFilePath.Split('\\').Last();

            fileFields.ForEach((pair) => text = text.Replace(pair.Key, pair.Value));

            List<string> directories = fileOutputPath.Split('\\').ToList();
            directories.Remove(directories.Last());

            Directory.CreateDirectory(fileOutputPath);

            string newFilePath = $@"{fileOutputPath}\{filename}".Replace(".Template", "");
            File.WriteAllText(newFilePath, text);

            return newFilePath;
        }

        /// <summary>
        /// Source: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.redirectstandardoutput?redirectedfrom=MSDN&view=net-5.0#System_Diagnostics_ProcessStartInfo_RedirectStandardOutput
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string ExecuteShellCommand(string command)
        {
            Console.WriteLine($"Executing command: {command}");
            string output = "";

            using (Process process = new Process())
            {
                process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                process.StartInfo.Arguments = $"/C {command}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                process.Start();

                Console.WriteLine(output = process.StandardOutput.ReadToEnd());
                Console.ResetColor();

                process.WaitForExit();
            }

            return output;
        }

        public static void CopyFolder(string sourcePath, string destinationPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
            {
                if (!(newPath.Contains("/bin/") || newPath.Contains("/obj/") || newPath.Contains("/.vs/") || newPath.Contains("/.git/") ||
                    newPath.Contains("\\bin\\") || newPath.Contains("\\obj\\") || newPath.Contains("\\.vs\\") || newPath.Contains("\\.git\\")))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }
            }
        }

        public static string SelectFolder()
        {
            string folderPath = "";

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Selecting folder...");
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowDialog();

                folderPath = dialog.SelectedPath;
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return folderPath;
        }

        public static string SelectSLNFile()
        {
            string folderPath = "";

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Selecting SLN file...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = $"Visual Studio Solution File (.sln)|*.sln;";
                dialog.ShowDialog();

                folderPath = dialog.FileName;
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return folderPath;
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
    }
}
