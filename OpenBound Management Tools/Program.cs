using OpenBound_Management_Tools.Forms;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace OpenBound_Management_Tools
{
    class Program
    {
        // http://msdn.microsoft.com/en-us/library/ms681944(VS.85).aspx
        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        /// <returns>nonzero if the function succeeds; otherwise, zero.</returns>
        /// <remarks>
        /// A process can be associated with only one console,
        /// so the function fails if the calling process already has a console.
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int AllocConsole();

        // http://msdn.microsoft.com/en-us/library/ms683150(VS.85).aspx
        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        /// <returns>nonzero if the function succeeds; otherwise, zero.</returns>
        /// <remarks>
        /// If the calling process is not already attached to a console,
        /// the error code returned is ERROR_INVALID_PARAMETER (87).
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private static IntPtr consolePointer;
        public static AsynchronousAction AsynchronousAction;

        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AsynchronousAction = new AsynchronousAction();

            Thread t = new Thread(() =>
            {
                Application.Run(new MainMenu());
            });

            t.IsBackground = true;
            t.Start();

            AllocConsole();
            consolePointer = GetConsoleWindow();
            HideConsole();

            while (t.IsAlive)
            {
                Thread.Sleep(100);
                AsynchronousAction.AsynchronousInvokeAndDestroy();
            }
        }

        public static void ShowConsole() => ShowWindow(consolePointer, SW_SHOW);

        public static void HideConsole() => ShowWindow(consolePointer, SW_HIDE);
    }
}
