using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.FileManager;
using OpenBound_Network_Object_Library.Models;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound_Game_Launcher.Common
{
    public class Parameter
    {
        public static Player Player;
        public static GameClientSettingsInformation GameClientSettingsInformation;

        public static void Initialize()
        {
            ConfigFileManager.CreateConfigFile(RequesterApplication.Launcher);
            ConfigFileManager.LoadConfigFile(RequesterApplication.Launcher);
            GameClientSettingsInformation = ConfigFileManager.ReadClientInformation();
        }
    }
}
