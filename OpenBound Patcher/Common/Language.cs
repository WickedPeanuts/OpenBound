using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Patcher.Common
{
    public class Language
    {
        public const string InstallingLabelText = "Installing File";

        public const string Exception1Title = "Error during patching process";
        public const string Exception1Text1 = "One or more files couldn't be moved from the temporary folder\n" +
                    "into the game's folder. This error might have happened because your user\n" +
                    "does not own the rights to perform operations on this folder. Your installation\n" +
                    "might have been corrupted during the patching process. In order to fix your client\n" +
                    "you MUST copy & replace the files manually. Also remember to grant Openbound Patcher.exe and\n" +
                    "OpenBound Game Launcher.exe administration privilleges to avoid future problems\n" +
                    "If the error persists, contact the support and send them the following file: \"PatchError.log\"";
        public const string Exception1Text2 = "Temporary Folder: ";
        public const string Exception1Text3 = "Game Folder: ";

        public const string Exception2Title = "Error while trying to save log";
        public const string Exception2Text1 = "There was an unexpected error while trying to save the error log\n" +
            "in your machine. This problem is also related to the lack of permissions for this\n" +
            "application to run.";

        public const string Exception3Title = "Error while cleaning the temporary files";
        public const string Exception3Text1 = "The application could not remove the temporary files of the\n" +
            "game folder. You can manually remove them by deleting the temporary folder located\n" +
            "inside the game installation folder. This will not generate any problems in your gaming\n" +
            "experience but some unused files are going to remain in the game until your next\n" +
            "installation. Consider granting this application (OpenBound Patcher.exe) admin privilleges";
    }
}
