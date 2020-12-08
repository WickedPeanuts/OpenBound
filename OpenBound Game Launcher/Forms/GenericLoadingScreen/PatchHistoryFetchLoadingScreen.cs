using OpenBound_Game_Launcher.Common;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using OpenBound_Network_Object_Library.WebRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Forms.GenericLoadingScreen
{
    public class PatchHistoryFetchLoadingScreen : LoadingMenu
    {
        string latestPatchHistoryPath;

        public PatchHistoryFetchLoadingScreen()
        {
            latestPatchHistoryPath = Parameter.LatestPatchHistoryPath;

            HttpWebRequest.AsyncDownloadFile(
                Parameter.BuildFetchHistoryURL(),
                latestPatchHistoryPath,
                onFinishDownload: OnFinishDownload,
                onFailToDownload: OnFailToDownload
                );
        }

        private void OnFinishDownload()
        {
            timer1InvokeAndDestroyTickAction += () => {
                OpenUpdaterDialog();
            };
        }

        private void OpenUpdaterDialog()
        {
            PatchHistory patchHistory = PatchHistory.CreatePatchHistoryInstance(latestPatchHistoryPath);
            if (patchHistory.ID != Parameter.GameClientSettingsInformation.ClientVersionHistory.ID)
            {
                Hide();

                GameUpdater gU = new GameUpdater(patchHistory);
                gU.ShowDialog();

                if (gU.DialogResult == DialogResult.OK)
                {
                    Close(DialogResult.OK);
                    return;
                }
            }

            Close(DialogResult.Cancel);
        }

        private void OnFailToDownload(Exception ex)
        {
            timer1InvokeAndDestroyTickAction += () =>
            {
                MessageBox.Show(ex.Message);
                Close(DialogResult.Cancel);
            };
        }
    }
}
