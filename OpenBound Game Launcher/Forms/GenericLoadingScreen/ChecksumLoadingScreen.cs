using OpenBound_Game_Launcher.Common;
using OpenBound_Game_Launcher.Helper;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Language = OpenBound_Game_Launcher.Common.Language;

namespace OpenBound_Game_Launcher.Forms.GenericLoadingScreen
{
    public class ChecksumLoadingScreen : LoadingMenu
    {
        private readonly string[] forbiddenExtensions = new string[] { ".json", ".txt", ".log" };

        public ChecksumLoadingScreen()
        {
            Thread t = new Thread(() =>
            {
                FileList fileList = Manifest.GetMissingInvalidAndOutdatedFiles(
                    Parameter.GameClientSettingsInformation.ClientVersionHistory.FileList.Checksum,
                    Directory.GetCurrentDirectory());

                // If the patching process was somehow incomplete
                if (fileList.ToBeDownloaded.Count > 0)
                {
                    timer1InvokeAndDestroyTickAction += OnFailToCheckFiles;
                    return;
                }

                // Delete all extra (unused) files
                try
                {
                    foreach (string filePath in fileList.ToBeDeleted)
                    {
                        if (!containsForbiddenExtensionList(filePath))
                            File.Delete(filePath);
                    }
                }
                catch (Exception)
                {
                    timer1InvokeAndDestroyTickAction += OnFailToRemoveUnusedFiles;
                    return;
                }

                timer1InvokeAndDestroyTickAction += () => { Close(DialogResult.OK); };
            });
        }

        private bool containsForbiddenExtensionList(string word)
        {
            foreach(string ext in forbiddenExtensions)
            {
                if (word.Contains(ext))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnFailToCheckFiles()
        {
            Hide();
            Feedback.CreateWarningMessageBox(Language.FailToCheckFileManifest);
            Close(DialogResult.Cancel);
        }

        private void OnFailToRemoveUnusedFiles()
        {
            Hide();
            Close(DialogResult.Cancel);
        }
    }
}
