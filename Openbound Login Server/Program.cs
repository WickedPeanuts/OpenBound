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

using OpenBound_Login_Server.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.FileOutput;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.TCP.ServiceProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Database.Controller;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.TCP.Entity;

namespace OpenBound_Login_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigFileManager.CreateConfigFile(RequesterApplication.LoginServer);
            ConfigFileManager.LoadConfigFile(RequesterApplication.LoginServer);

            Console.WriteLine("OpenBound Login Server");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine($"Login Server ({NetworkObjectParameters.LoginServerInformation.ServerConsoleName}) has started listening for players.");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine($"The server must be to connected to Lobby Server ({NetworkObjectParameters.LobbyServerInformation.ServerConsoleName}) for exchanging UID for each user that tries to login.");
            Console.WriteLine("The server won't work when the Lobby Server is offline.");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine("Server Responsabilities:");
            Console.WriteLine("Lobby Server  - Handshake and request UID to all players that requests login");
            Console.WriteLine("Game Launcher - Create and retrive players from database, grants access to login requests");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine("Operation Log:");
            Console.WriteLine("----------------------\n");

            ServerServiceProvider serverServiceProvider = new ServerServiceProvider(
                NetworkObjectParameters.LoginServerInformation.ServerPort,
                NetworkObjectParameters.LoginServerBufferSize,
                LoginServiceHUB);

            serverServiceProvider.StartOperation();

            CreateDatabaseIfNecessary();
        }

        public static void LoginServiceHUB(ExtendedConcurrentQueue<byte[]> provider, string[] request, Dictionary<int, object> paramDictionary)
        {
            int service = int.Parse(request[0]);
            object answer = null;

            switch (service)
            {
                case NetworkObjectParameters.LoginServerLoginAttemptRequest:
                    PlayerHandler.StartListeningPlayerLoginAttempt(provider, request[1]);
                    return;
                case NetworkObjectParameters.LoginServerAccountCreationRequest:
                    answer = PlayerHandler.RegistrationAttempt(request[1]); 
                    break;
            }

            provider.Enqueue(service, answer);
        }

        public static void CreateDatabaseIfNecessary()
        {
            //When any table on the database is accessed the DBContext will create the tables and the database if there is no scheme on the given address
            //Meaning that searching for a random player basically does nothing but creates the db scheme/tables.
            try
            {
                new PlayerController().LoginPlayer(new Player() { Nickname = "OpenBound", Password = "Rocks" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database could not be created. Check the configuration files or the inner exception for more details about what went wrong.");
                Console.WriteLine($"Message: {ex.Message}");
            }
        }
    }
}
