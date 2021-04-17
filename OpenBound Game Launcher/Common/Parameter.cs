﻿using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using OpenBound_Network_Object_Library.FileManager;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OpenBound_Game_Launcher.Common
{
    public class Parameter
    {
        public static Player Player;
        public static GameClientSettingsInformation GameClientSettingsInformation;

        public static void Initialize(string[] args)
        {
            ConfigFileManager.CreateConfigFile(RequesterApplication.Launcher);
            ConfigFileManager.LoadConfigFile(RequesterApplication.Launcher);

            GameClientSettingsInformation = ConfigFileManager.ReadClientInformation();

            if (args.Length >= 2)
            {
                //Game has been patched
                string previousVersion = args[0];
                string currentVersion = args[1];

                GameClientSettingsInformation.ClientVersionHistory = ObjectWrapper.DeserializeFile<PatchHistory>(
                    $@"{Directory.GetCurrentDirectory()}\{NetworkObjectParameters.LatestPatchHistoryFilename}");
                GameClientSettingsInformation.ClientVersionHistory.PatchHistoryList.Clear();
            }

            ConfigFileManager.OverwriteGameServerSettings(GameClientSettingsInformation);
        }
    }
}
