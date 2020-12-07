using OpenBound_Game_Launcher.Forms;
using OpenBound_Game_Launcher.Launcher;
using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if RELEASE
            if (args == null || args.Length == 0)
            {
                try
                {
                    Process process = new Process();
                    Process.Start($"{Directory.GetCurrentDirectory()}/{NetworkObjectParameters.GameClientProcessName}");
                }
                catch (Exception) { }
                return;
            }
#else
            MessageBox.Show("DEBUG MODE ON. Opening launcher...");
#endif

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameLauncher(args));
        }
    }
}
