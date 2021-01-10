using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OpenBound_Management_Tools.Helper
{
    public class ProcessHelper
    {
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
