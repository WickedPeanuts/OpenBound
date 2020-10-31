using OpenBound_Game_Launcher.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Helper
{
    public class Feedback
    {
        public static void CreateWarningMessageBox(string message, string title = Language.PopupTitleWarning)
        {
            MessageBox.Show(message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void CreateInformationMessageBox(string message, string title = Language.PopupTitleInformation)
        {
            MessageBox.Show(message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
