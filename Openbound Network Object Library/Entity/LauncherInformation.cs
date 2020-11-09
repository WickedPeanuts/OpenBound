using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.FileManager;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Network_Object_Library.Entity
{
    public enum LauncherOperationStatus
    {
        Closed,
        AuthInvalid,
        AuthConfirmed,
        ProccessFinished,
    }

    public struct LauncherInformation
    {
        public LauncherOperationStatus LauncherOperationStatus;
        public GameClientSettingsInformation GameClientSettingsInformation;
        public Player Player;

        public LauncherInformation(LauncherOperationStatus launcherDialogResult, GameClientSettingsInformation gameClientSettingsInformation, Player player)
        {
            LauncherOperationStatus = launcherDialogResult;
            GameClientSettingsInformation = gameClientSettingsInformation;
            Player = player;
        }
    }
}
