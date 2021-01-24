/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.FileManagement.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenBound_Network_Object_Library.FileManager
{
    public enum RequesterApplication
    {
        LoginServer,
        LobbyServer,
        GameServer,
        Launcher,
        EFMigration
    }

    public struct ConfigServerInformation
    {
        public List<ServerInformation> ServerInformationList;
        public List<GameServerInformation> GameServerInformationList;
    }

    public struct ConfigDatabaseInformation
    {
        public string DatabaseAddress;// = ;
        public string DatabaseName;// = "OpenBound";
        public string DatabaseLogin;// = "OpenBound_admin";
        public string DatabasePassword;// = "OpenBound";
    }

    public struct ConfigLobbyServerWhitelist
    {
        public List<string> Whitelist;
    }

    public class GameClientSettingsInformation
    {
        //launcher options
        public string SavedNickname;

        //Video
        public int WindowedWidth, WindowedHeight;
        public int MenuSupportedResolutionWidth, MenuSupportedResolutionHeight;
        public int InGameResolutionWidth, InGameResolutionHeight;

        public bool IsFullScreen;

        //Video Filters
        
        /// <summary>
        /// Applies a low-pass filter in order to mask pixelated scene artifacts SMAA x8
        /// </summary>
        public bool IsMultiSamplingEnabled;
        public int MultiSamplingCount;

        /// <summary>
        /// Synchronizes the vertical retrace (vsync)
        /// </summary>
        public bool IsVSyncOn;

        //Gameplay
        public int AimType;
        public int ScrollSpeed, MouseSpeed;

        //Sound
        public int BGM, SFX;

        //Misc
        public bool IsBackgroundOn;
        public int InterfaceType;

        public int BackgroundColorRedChannel;
        public int BackgroundColorGreenChannel;
        public int BackgroundColorBlueChannel;

        //Patching
        public PatchHistory ClientVersionHistory;
    }

    public class ConfigFileManager
    {
        const string errorMessage = " file does not exist. Open the server program again and it should appear in your server folder.";

        #region Server Headers
        private const string Separator = "//--------------------";
        private const string ConfigurationGuide = "// Configuration Guide ";
        private const string ServerNameGuide = "// - ServerName - I just left it as a identifier for anyone trying to change it. Don't worry, it is impossible for clients to read this string";
        private const string ServerLocalAddressGuide = "// - ServerLocalAddress - Your local IP (usually it is 127.0.0.1) if you have a VPN you can leave your VPN's ip";
        private const string ServerPublicAddressGuide = "// - ServerPublicAddress - Your public IP, if your modem ports are forwarded you can change this to your \"global\" IP";
        private static string ServerPortGuide = $"// - ServerPort - Game's Standard port for Login server is {NetworkObjectParameters.LoginServerDefaultPort} and for Lobby server is {NetworkObjectParameters.LobbyServerDefaultPort}. I strongly suggest you not to change it, but if you do, remember to set this config for any other service's config file that connects to those services";

        private const string ServerIDGuide = "// - Server's ID. It starts from 1 to infinity. However, the interface only supports limited number of servers, it is advisable to not to exploit it.";
        private const string GameServerNameGuide = "// - Game Server's name. This is the name shown inside any game room and on the login screen buttons. Text length is also (in theory) infinite.";
        private const string IsAvatarOnGuide = "// - Enables/Disables avatar in the server";
        private static string GameServerPortGuide = $"// - Game Server's starting port is {NetworkObjectParameters.GameServerDefaultStartingPort}, It must be unique for each Game Server instance, I suggest you to Add 1+ for each game server.";
        private const string ServerDescriptionGuide = "// - This is an array. For each element in this array a new line is inserted on server's button on server selection screen";
        private const string ConnectedClientCapacityGuide = "// - This number can be as big as you want, however the interface is going to break if it goes over 4 digits";

        private const string DeleteToRestoreGuide = "// If you saved bad configurations by accident delete this file and the program will create another one with standard presets.";
        private const string JSONCommentDisclaimer = "// JSON DOES NOT SUPPORT COMMENTS. EXCEPT THIS ONE LOL.";

        private static readonly string[] LauncherServerHeader =
        {
            Separator,
            ConfigurationGuide,
            "// This file contains information about which server is the launcher going to connect.",
            ServerNameGuide,
            ServerLocalAddressGuide,
            ServerPublicAddressGuide,
            ServerPublicAddressGuide,
            ServerPortGuide,
            DeleteToRestoreGuide,
            JSONCommentDisclaimer,
            Separator,
        };

        private static readonly string[] LoginLobbyServerHeader =
        {
            Separator,
            ConfigurationGuide,
            "//The First element of the array is the login server.",
            "// The Second element is Lobby's server address from Login's perspective. Keep in mind that after changing this element you must also apply such change in LobbyServer/LobbyServerConfig.json",
            ServerNameGuide,
            ServerLocalAddressGuide,
            ServerPublicAddressGuide,
            ServerPublicAddressGuide,
            ServerPortGuide,
            DeleteToRestoreGuide,
            JSONCommentDisclaimer,
            Separator,
        };

        private static readonly string[] GameServerHeader =
        {
            Separator,
            ConfigurationGuide,
            "// For each game server this file must be edited with similar configurations.",
            ServerIDGuide,
            GameServerNameGuide,
            ServerLocalAddressGuide,
            ServerPublicAddressGuide,
            ServerPublicAddressGuide,
            GameServerPortGuide,
            IsAvatarOnGuide,
            ServerDescriptionGuide,
            ConnectedClientCapacityGuide,
            DeleteToRestoreGuide,
            JSONCommentDisclaimer,
            Separator,
        };

        private static readonly string[] WhitelistHeader =
        {
            Separator,
            ConfigurationGuide,
            "// This document lists the IP addresses that can register their GS into Lobby server.",
            DeleteToRestoreGuide,
            JSONCommentDisclaimer,
            Separator,
        };

        private static readonly string[] DatabaseConfigFileHeader =
        {
            Separator,
            ConfigurationGuide,
            "// This file helps builds the database connection string used in entity's context.",
            "// DatabaseAddress - Database IP. For instance: 127.0.0.1, 127.0.0.1////SQLEXPRESS (yes, two backslashes), OpenBound.east.rds.amazonaws.com and so on",
            "// DatabaseName - Database's name. Default is OpenBound and I hope you keep it that way.",
            "// DatabaseLogin - Database's admin login.",
            "// DatabasePassword - Database's password.",
            "// Database Connection String is going to be built like this: ",
            "// Data Source=<DatabaseAddress>; Initial Catalog=<DatabaseName>; Persist Security Info=True; User ID=<DatabaseLogin>;Password=<DatabasePassword>;MultipleActiveResultSets=True",
            DeleteToRestoreGuide,
            JSONCommentDisclaimer,
            Separator,
        };

        private static readonly string[] GameClientConfigHeader = new string[]
        {
            Separator,
            ConfigurationGuide,
            "// Do not change any of those parameters unless you know exacly what you are doing.",
            "// Do not share this file with anyone, by doing so you are jeopardize your account safety.",
            DeleteToRestoreGuide,
            Separator,
        };
        #endregion

        #region Server TXT Placeholders
        private static GameClientSettingsInformation configClientPlaceholder
            = new GameClientSettingsInformation()
            {
                SavedNickname = "",

                WindowedWidth = 1280,
                WindowedHeight = 800,
                MenuSupportedResolutionWidth = 800,
                MenuSupportedResolutionHeight = 600,
                InGameResolutionWidth = 1280,
                InGameResolutionHeight = 800,

                IsFullScreen = false,

                IsMultiSamplingEnabled = false,
                MultiSamplingCount = 8,

                IsVSyncOn = false,

                AimType = 1,
                ScrollSpeed = 50,
                MouseSpeed = 50,

#if DEBUG
                BGM = 0,
                SFX = 20,
#else
                BGM = 30,
                SFX = 50,
#endif

                IsBackgroundOn = true,
                InterfaceType = 3,

                BackgroundColorRedChannel = 0,
                BackgroundColorGreenChannel = 0,
                BackgroundColorBlueChannel = 0,

                ClientVersionHistory = new PatchHistory()
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000000"),
                    CreationDate = DateTime.MinValue,
                    PatchVersionName = "v0.1.0",
                }
            };

        private static ServerInformation LoginServerPlaceholder
            = new ServerInformation()
            {
                ServerName = "Login Server",
                ServerLocalAddress = "127.0.0.1",
                ServerPublicAddress = "127.0.0.1",
                ServerPort = NetworkObjectParameters.LoginServerDefaultPort,
            };

        private static ServerInformation LobbyServerPlaceholder
            = new ServerInformation()
            {
                ServerName = "Lobby Server",
                ServerLocalAddress = "127.0.0.1",
                ServerPublicAddress = "127.0.0.1",
                ServerPort = NetworkObjectParameters.LobbyServerDefaultPort,
            };

        private static ServerInformation FetchServerPlaceholder
            = new ServerInformation()
            {
                ServerName = "Fetch Server",
                ServerLocalAddress = "127.0.0.1",
                ServerPublicAddress = "127.0.0.1",
                ServerPort = NetworkObjectParameters.FetchServerDefaultPort,
            };

        private static GameServerInformation GameServerPlaceholder
            = new GameServerInformation()
            {
                ServerID = 1,
                LowerLevel = ServerLevelLimitation.Chick,
                HigherLevel = ServerLevelLimitation.GM,
                IsAvatarOn = true,
                ServerPublicAddress = "127.0.0.1",
                ServerLocalAddress = "127.0.0.1",
                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort,
                ServerName = "OpenBound World 1",
                ServerDescription = new string[] {
                    "Avatar On", "Free Zone", "First Opensource Server"
                },
                ConnectedClientCapacity = 1000,
                MaximumClientsPerChatChannel = 1000 / NetworkObjectParameters.GameServerChatChannelsMaximumNumber
            };

        private static readonly string GameClientSettingsInformation =
            ObjectWrapper.Serialize(configClientPlaceholder,
                Formatting.Indented);

        private static readonly string LoginLobbyFetchServerInformation =
            ObjectWrapper.Serialize(
                new ConfigServerInformation()
                {
                    ServerInformationList = new List<ServerInformation>() { LoginServerPlaceholder, LobbyServerPlaceholder, FetchServerPlaceholder },
                }, Formatting.Indented);

        private static readonly string GameServerInformation =
            ObjectWrapper.Serialize(
                new ConfigServerInformation()
                {
                    ServerInformationList = new List<ServerInformation>() { LobbyServerPlaceholder },
                    GameServerInformationList = new List<GameServerInformation>() { GameServerPlaceholder }
                }, Formatting.Indented);

        private static readonly string DatabaseInformation =
            ObjectWrapper.Serialize(
                new ConfigDatabaseInformation()
                {
                    DatabaseAddress = "127.0.0.1",
                    DatabaseName = "OpenBound",
                    DatabaseLogin = "Admin",
                    DatabasePassword = "Admin",
                }, Formatting.Indented);

        private static readonly string LobbyServerWhitelistInformation =
            ObjectWrapper.Serialize(new ConfigLobbyServerWhitelist() { Whitelist = new List<string>() { "localhost", "127.0.0.1", "host.docker.internal"
    } }, Formatting.Indented);
#endregion

        #region Serverlist Placeholders
        private static readonly string[] LobbyServerlistPlaceholder =
            ObjectWrapper.Serialize(
                new ConfigServerInformation()
                {
                    GameServerInformationList =
                        new List<GameServerInformation>()
                        {
                            new GameServerInformation(){
                                ServerID = 1,
                                HigherLevel = ServerLevelLimitation.Dragon3,
                                LowerLevel = ServerLevelLimitation.Chick,
                                IsAvatarOn = false,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort,
                                ServerName = "OpenBound World 1",
                                ServerDescription = new string[]{
                                    "Avatar Off", "Free Zone", "First Opensource Server"
                                }
                            },

                            new GameServerInformation(){
                                ServerID = 2,
                                HigherLevel = ServerLevelLimitation.Dragon3,
                                LowerLevel = ServerLevelLimitation.Chick,
                                IsAvatarOn = true,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort + 1,
                                ServerName = "OpenBound World 2",
                                ServerDescription = new string[]{
                                    "Avatar On", "Free Zone", "Second Opensource Server"
                                }
                            },

                            new GameServerInformation(){
                                ServerID = 3,
                                HigherLevel = ServerLevelLimitation.StoneHammer2,
                                LowerLevel = ServerLevelLimitation.Chick,
                                IsAvatarOn = false,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort + 2,
                                ServerName = "OpenBound World 3",
                                ServerDescription = new string[]{
                                    "Avatar Off", "Tier 1 - Restricted Zone", "Third Opensource Server"
                                }
                            },

                            new GameServerInformation(){
                                ServerID = 4,
                                HigherLevel = ServerLevelLimitation.StoneHammer2,
                                LowerLevel = ServerLevelLimitation.Chick,
                                IsAvatarOn = true,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort + 3,
                                ServerName = "OpenBound World 4",
                                ServerDescription = new string[]{
                                    "Avatar On", "Tier 1 - Restricted Zone", "Fourth Opensource Server"
                                }
                            },

                            new GameServerInformation(){
                                ServerID = 5,
                                HigherLevel = ServerLevelLimitation.DGoldenAxe2,
                                LowerLevel = ServerLevelLimitation.Axe1,
                                IsAvatarOn = false,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort + 4,
                                ServerName = "OpenBound World 5",
                                ServerDescription = new string[]{
                                    "Avatar Off", "Tier 2 - Restricted Zone", "Fifth Opensource Server"
                                }
                            },

                            new GameServerInformation(){
                                ServerID = 6,
                                HigherLevel = ServerLevelLimitation.DGoldenAxe2,
                                LowerLevel = ServerLevelLimitation.Axe1,
                                IsAvatarOn = true,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort + 5,
                                ServerName = "OpenBound World 6",
                                ServerDescription = new string[]{
                                    "Avatar On", "Tier 2 - Restricted Zone", "Sixth Opensource Server"
                                }
                            },

                            new GameServerInformation(){
                                ServerID = 7,
                                HigherLevel = ServerLevelLimitation.Dragon3,
                                LowerLevel = ServerLevelLimitation.Staff1,
                                IsAvatarOn = false,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort + 6,
                                ServerName = "OpenBound World 7",
                                ServerDescription = new string[]{
                                    "Avatar On", "Tier 3 - Major League Players Zone", "Seventh Opensource Server"
                                }
                            },

                            new GameServerInformation(){
                                ServerID = 8,
                                HigherLevel = ServerLevelLimitation.Dragon3,
                                LowerLevel = ServerLevelLimitation.Staff1,
                                IsAvatarOn = true,
                                ServerPublicAddress = "127.0.0.1", ServerLocalAddress = "127.0.0.1",
                                ServerPort = NetworkObjectParameters.GameServerDefaultStartingPort + 7,
                                ServerName = "OpenBound World 8",
                                ServerDescription = new string[]{
                                    "Avatar On", "Tier 3 - Major League Players Zone", "Eight Opensource Server"
                                }
                            },
                        },
                },
                Formatting.Indented
            ).Split('\r');

#endregion

        private static string ServerConfigPath(RequesterApplication serverType) => $@"{Directory.GetCurrentDirectory()}/Config/{serverType}ServerConfig.json";
        private static string GameClientSettingsPath => $@"{Directory.GetCurrentDirectory()}/Config/GameClientSettings.json";
        private static string ServerlistPlaceholderPath => $@"{Directory.GetCurrentDirectory()}/Config/LobbyServerListPlaceholders.json";
        private static string DatabaseConfigPath => $@"{Directory.GetCurrentDirectory()}/Config/DatabaseConfig.json";
        private static string LobbyServerWhitelistPath => $@"{Directory.GetCurrentDirectory()}/Config/LobbyServerWhitelist.json";

        public static void CreateConfigFile(RequesterApplication serverType, bool ovewrite = false)
        {
            string path = ServerConfigPath(serverType);

            Directory.CreateDirectory($@"{Directory.GetCurrentDirectory()}/Config");

            switch (serverType)
            {
                case RequesterApplication.Launcher:
                    if (!File.Exists(path) || ovewrite)
                        File.WriteAllText(path, $"{string.Join("\n", LauncherServerHeader)}\n{LoginLobbyFetchServerInformation}");

                    if (!File.Exists(GameClientSettingsPath) || ovewrite)
                        File.WriteAllText(GameClientSettingsPath, $"{string.Join("\n", GameClientConfigHeader)}\n{GameClientSettingsInformation}");
                    break;

                case RequesterApplication.LoginServer:
                    if (!File.Exists(path) || ovewrite)
                        File.WriteAllText(path, $"{string.Join("\n", LoginLobbyServerHeader)}\n{LoginLobbyFetchServerInformation}");

                    if (!File.Exists(DatabaseConfigPath) || ovewrite)
                        File.WriteAllText(DatabaseConfigPath, $"{string.Join("\n", DatabaseConfigFileHeader)}\n{DatabaseInformation}");
                    break;

                case RequesterApplication.LobbyServer:
                    if (!File.Exists(ServerlistPlaceholderPath) || ovewrite)
                        File.WriteAllText(ServerlistPlaceholderPath, $"{string.Join("\n", LoginLobbyServerHeader)}\n{string.Join("", LobbyServerlistPlaceholder)}");

                    if (!File.Exists(LobbyServerWhitelistPath) || ovewrite)
                        File.WriteAllText(LobbyServerWhitelistPath, $"{string.Join("\n", WhitelistHeader)}\n{LobbyServerWhitelistInformation}");

                    if (!File.Exists(path) || ovewrite)
                        File.WriteAllText(path, $"{string.Join("\n", LoginLobbyServerHeader)}\n{LoginLobbyFetchServerInformation}");
                    break;

                case RequesterApplication.GameServer:
                    if (!File.Exists(path) || ovewrite)
                        File.WriteAllText(path, $"{string.Join("\n", GameServerHeader)}\n{GameServerInformation}");

                    if (!File.Exists(DatabaseConfigPath) || ovewrite)
                        File.WriteAllText(DatabaseConfigPath, $"{string.Join("\n", DatabaseConfigFileHeader)}\n{DatabaseInformation}");
                    break;
            }
        }

        public static string ReadConnectionStringForMigration()
        {
            string appStartupPath = AppDomain.CurrentDomain.BaseDirectory;
            appStartupPath = appStartupPath.Replace("OpenBound Network Object Library", "OpenBound Login Server");
            string filePath = appStartupPath + @"/Config/DatabaseConfig.json";
            return filePath;
        }

        public static void LoadConfigFile(RequesterApplication requestApplication)
        {
            string path;

            if (requestApplication == RequesterApplication.EFMigration)
                path = ReadConnectionStringForMigration();
            else
                path = ServerConfigPath(requestApplication);

            if (!File.Exists(path))
            {
                Console.WriteLine($"Configuration{errorMessage}");
                return;
            }

            ConfigServerInformation csfc = ObjectWrapper.DeserializeCommentedJSONFile<ConfigServerInformation>(path);
            ConfigDatabaseInformation cdi;

            switch (requestApplication)
            {
                case RequesterApplication.Launcher:
                    NetworkObjectParameters.LoginServerInformation = csfc.ServerInformationList[0];
                    NetworkObjectParameters.LobbyServerInformation = csfc.ServerInformationList[1];
                    NetworkObjectParameters.FetchServerInformation = csfc.ServerInformationList[2];
                    break;

                case RequesterApplication.EFMigration:
                    if (!File.Exists(path))
                    {
                        Console.WriteLine($"Database{errorMessage}");
                        return;
                    }

                    ConfigDatabaseInformation dbInfo = ObjectWrapper.DeserializeCommentedJSONFile<ConfigDatabaseInformation>(path);
                    NetworkObjectParameters.DatabaseAddress = dbInfo.DatabaseAddress;
                    NetworkObjectParameters.DatabaseName = dbInfo.DatabaseName;
                    NetworkObjectParameters.DatabaseLogin = dbInfo.DatabaseLogin;
                    NetworkObjectParameters.DatabasePassword = dbInfo.DatabasePassword;
                    break;

                case RequesterApplication.LoginServer:
                    if (!File.Exists(DatabaseConfigPath))
                    {
                        Console.WriteLine($"Database{errorMessage}");
                        return;
                    }

                    cdi = ObjectWrapper.DeserializeCommentedJSONFile<ConfigDatabaseInformation>(DatabaseConfigPath);
                    NetworkObjectParameters.DatabaseAddress = cdi.DatabaseAddress;
                    NetworkObjectParameters.DatabaseName = cdi.DatabaseName;
                    NetworkObjectParameters.DatabaseLogin = cdi.DatabaseLogin;
                    NetworkObjectParameters.DatabasePassword = cdi.DatabasePassword;
                    NetworkObjectParameters.LoginServerInformation = csfc.ServerInformationList[0];
                    NetworkObjectParameters.LobbyServerInformation = csfc.ServerInformationList[1];
                    NetworkObjectParameters.FetchServerInformation = csfc.ServerInformationList[2];
                    break;

                case RequesterApplication.LobbyServer:
                    NetworkObjectParameters.LoginServerInformation = csfc.ServerInformationList[0];
                    NetworkObjectParameters.LobbyServerInformation = csfc.ServerInformationList[1];
                    NetworkObjectParameters.FetchServerInformation = csfc.ServerInformationList[2];

                    if (!File.Exists(LobbyServerWhitelistPath))
                    {
                        Console.WriteLine($"Lobby's gameserver whitelist{errorMessage}");
                        return;
                    }

                    ConfigLobbyServerWhitelist gsrwl = ObjectWrapper.DeserializeCommentedJSONFile<ConfigLobbyServerWhitelist>(LobbyServerWhitelistPath);

                    NetworkObjectParameters.GameServerRequestIPWhitelist = gsrwl.Whitelist;
                    break;

                case RequesterApplication.GameServer:
                    NetworkObjectParameters.LobbyServerInformation = csfc.ServerInformationList[0];
                    NetworkObjectParameters.GameServerInformation = csfc.GameServerInformationList[0];

                    cdi = ObjectWrapper.DeserializeCommentedJSONFile<ConfigDatabaseInformation>(DatabaseConfigPath);
                    NetworkObjectParameters.DatabaseAddress = cdi.DatabaseAddress;
                    NetworkObjectParameters.DatabaseName = cdi.DatabaseName;
                    NetworkObjectParameters.DatabaseLogin = cdi.DatabaseLogin;
                    NetworkObjectParameters.DatabasePassword = cdi.DatabasePassword;
                    break;
            }
        }

        public static List<GameServerInformation> LoadServerlistPlaceholderFile()
        {
            return ObjectWrapper.DeserializeCommentedJSONFile<ConfigServerInformation>(ServerlistPlaceholderPath).GameServerInformationList;
        }

        public static GameClientSettingsInformation ReadClientInformation()
        {
            return ObjectWrapper.DeserializeCommentedJSONFile<GameClientSettingsInformation>(GameClientSettingsPath);
        }

        public static void OverwriteGameServerSettings(GameClientSettingsInformation gcsi)
        {
            File.WriteAllText(
                GameClientSettingsPath,
                $"{string.Join("\n", GameServerHeader)}\n{ObjectWrapper.Serialize(gcsi, Formatting.Indented)}");
        }

        public static void OverwriteLauncherServerSettings(List<ServerInformation> serverInformationList)
        {
            File.WriteAllText(
                ServerConfigPath(RequesterApplication.Launcher),
                $"{string.Join("\n", LauncherServerHeader)}\n{ObjectWrapper.Serialize(new ConfigServerInformation() { ServerInformationList = serverInformationList }, Formatting.Indented)}");
        }
    }
}