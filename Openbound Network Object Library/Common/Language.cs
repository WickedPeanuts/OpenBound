using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.Common
{
    public class Language
    {
        public const string PlayerDTOPasswordErrorMessage = "The password contains invalid characters.";
        public const string PlayerDTOPasswordConfirmationName = "Password confirmation";

        public const string ChannelWelcomeMessage1 = " Welcome to OpenBound (";
        public const string ChannelWelcomeMessage2 = ") - Channel ";

        public const string RoomWelcomeMessage1 = " You have joined a room (";
        public const string RoomWelcomeMessage2 = " Kindness creates a large and healthy community!";

        public const string DeathMessage1 = " was defeated. ";

        public const string GoldRewardMultipleKill   = " [Multiple Termination]  +";
        public const string GoldRewardSingleKill     =  " [Terminate one enemy]  +";
        public const string GoldRewardBungeeKill     =   " [Bungee Termination]  +";
        public const string GoldRewardD500Damage     =           " [500 Damage]  +";
        public const string GoldRewardD250Damage     =           " [250 Damage]  +";
        public const string GoldRewardD150Damage     =           " [150 Damage]  +";

        public const string GoldRewardTornadoShot    =         " [Tornado Shot]  +";
        public const string GoldRewardMirrorShot     =          " [Mirror Shot]  +";

        public const string GoldRewardHighAngle      =           " [High Angle]  +";
        public const string GoldRewardUHighAngle     =     " [Ultra High Angle]  +";
        public const string GoldRewardGoldBoomerShot =          " [Boomer Shot]  +";
        public const string GoldRewardBackShot       =            " [Back Shot]  +";
        public const string GoldReward3000Damage     =    " [3000 Total Damage]  +";
        public const string GoldReward2000Damage     =    " [2000 Total Damage]  +";
        public const string GoldReward1000Damage     =    " [1000 Total Damage]  +";

        public const string GoldDecreaseSuicide      =     " [Self-Termination]  -";
        public const string GoldDecreaseAllyKill     =     " [Ally Termination]  -";
        public const string GoldDecreaseFriendlyFire =        " [Friendly Fire]  -";

        public const string GoldRewardWin4v4Match    =        " [Victory - 4v4]  +";
        public const string GoldRewardWin3v3Match    =        " [Victory - 3v3]  +";
        public const string GoldRewardWin2v2Match    =        " [Victory - 2v2]  +";
        public const string GoldRewardWin1v1Match    =        " [Victory - 1v1]  +";

        public const string GoldRewardLose4v4Match   =         " [Defeat - 4v4]  +";
        public const string GoldRewardLose3v3Match   =         " [Defeat - 3v3]  +";
        public const string GoldRewardLose2v2Match   =         " [Defeat - 2v2]  +";
        public const string GoldRewardLose1v1Match   =         " [Defeat - 1v1]  +";

        public const string GoldRewardPopularity     =     " [Popularity Bonus]  +";
        public const string GoldDecreasePopularity   =   " [Popularity Penalty]  -";
    }
}
