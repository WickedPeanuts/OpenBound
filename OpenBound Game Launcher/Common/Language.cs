using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Game_Launcher.Common
{
    public class Language
    {
        public const string PopupTitleWarning = "Warning";
        public const string PopupTitleInformation = "Information";
        public const string FailToEstabilishConnection = "Failed to estabilish connection to the server.\nTry again later.";

        //----------------
        // Game Launcher

        public const string PlayerNotFound = "Player not found.\n\nMake sure you have entered the\nright credentials before trying again";
        public const string PlayerFound    = "Player found.";

        //----------------
        // Sign Up Form

        public const string RegisterSuccess = "Registration was successful.";
        public const string RegisterFailureMessage  = "There was an error on processing your registration attempt.\n";
        public const string RegisterFailureNickname = "- The selected nickname is already in use.\n";
        public const string RegisterFailureEmail    = "- The selected email is already in use.\n";

        public const string RegisterFailureInvalidCredentials = "One or more fields were not filled correctly:\n\n";

        //----------------
        // Config Form

        public const string NoAspectRatioFound = "Custom Ratio";
        public const string BadResolutionSelection = "Bad resolution parameters were entered.\n\nIt is recommended that In-Game resolutions should be lesser or equal to windowed resolution. Preferably both resolutions should use the same aspect ratio.";
    }
}
