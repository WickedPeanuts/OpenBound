using OpenBound_Game_Launcher.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Helper
{
    public class Feedback
    {
        public static DialogResult CreateWarningMessageBox(string message, 
            string title = Language.PopupTitleWarning, MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(message, title, messageBoxButtons, MessageBoxIcon.Warning);
        }

        public static DialogResult CreateInformationMessageBox(string message, string title = Language.PopupTitleInformation)
        {
            return MessageBox.Show(message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
