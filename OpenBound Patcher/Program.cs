using OpenBound_Patcher.Forms;
using System;
using System.Windows.Forms;

namespace OpenBound_Patcher
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //if (args.Length == 0)
            //  return;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Patcher(args));
        }
    }
}
