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

using OpenBound.Common;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.ServerCommunication;
using OpenBound_Game_Launcher.Forms;
using OpenBound_Game_Launcher.Launcher;
using OpenBound_Network_Object_Library.Entity;
using System;

namespace OpenBound
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new string[] { "" };

            GameLauncher gameLauncher = new GameLauncher(args);

#if !DEBUGSCENE
            LauncherInformation li = gameLauncher.OpenDialog();

            if (li.LauncherOperationStatus == LauncherOperationStatus.AuthConfirmed)
                Parameter.Initialize(li);
            else
                return;
#endif

#if DEBUGSCENE
            DebugScene.InitializeObjects();
#endif

            using (var game = new Game1())
            {
                game.Run();
            }

            ServerInformationBroker.Instance.StopServices();
        }
    }
}