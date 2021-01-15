using OpenBound_Network_Object_Library.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenBound_Management_Tools.Helper
{
    class PipelineHelper
    {
        public static void GenerateTemplateFiles(string templateFolder, string baseOutputPath, Dictionary<string, string> fileFields)
        {
            string[] files = Directory.GetFiles(templateFolder, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                GenerateTemplateFile(file, baseOutputPath, fileFields);
            }
        }

        private static void GenerateTemplateFile(string templateFilePath, string fileOutputPath, Dictionary<string, string> fileFields)
        {
            string text = File.ReadAllText(templateFilePath);
            string filename = templateFilePath.Split('\\').Last();

            fileFields.ForEach((pair) => text = text.Replace(pair.Key, pair.Value));

            List<string> directories = fileOutputPath.Split('\\').ToList();
            directories.Remove(directories.Last());

            Directory.CreateDirectory(fileOutputPath);
            File.WriteAllText($@"{fileOutputPath}\{filename}", text);
        }

        /// <summary>
        /// Source: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.redirectstandardoutput?redirectedfrom=MSDN&view=net-5.0#System_Diagnostics_ProcessStartInfo_RedirectStandardOutput
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static void ExecuteShellCommand(string command)
        {
            Console.WriteLine($"Executing command: {command}");

            using (Process process = new Process())
            {
                process.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                process.StartInfo.Arguments = $"/C {command}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                process.Start();

                Console.WriteLine(process.StandardOutput.ReadToEnd());
                Console.ResetColor();

                process.WaitForExit();
            }
        }
    }
}
