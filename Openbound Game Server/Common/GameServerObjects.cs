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

using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.TCP.ServiceProvider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace OpenBound_Game_Server.Common
{
    public class GameServerObjects
    {
        private static GameServerObjects instance { get; set; }
        public static GameServerObjects Instance
        {
            get
            {
                if (instance == null) instance = new GameServerObjects();
                return instance;
            }
        }

        public static ServerServiceProvider serverServiceProvider;
        public static ClientServiceProvider lobbyServerCSP;

        public Hashtable PlayerHashtable { get; set; }
        public SortedList<int, RoomMetadata> RoomMetadataSortedList { get; set; }

        /// <summary>
        /// Stores all the chat channels of a game server. The Key is the channel id starting from 1 to <see cref="NetworkObjectParameters.GameServerChatChannelsMaximumNumber"/>.
        /// The value is a HashSet of all connected players. The key is a string composed by a leading character indicating which kind of chat the user is connected to, and the integer id
        /// which could be associated with the channel ID or the room ID.
        /// For instance: Game List Channel 7 - "G7", Game Room ID 172 - "R172".
        /// In order to access such HashSet, one must use: ChatDictionary['G'][7].
        /// </summary>
        public Dictionary<char, Dictionary<int, HashSet<Player>>> ChatDictionary { get; set; }

        private GameServerObjects()
        {
            PlayerHashtable = new Hashtable();
            RoomMetadataSortedList = new SortedList<int, RoomMetadata>();
            ChatDictionary = new Dictionary<char, Dictionary<int, HashSet<Player>>>()
            {
                { NetworkObjectParameters.GameServerChatGameListIdentifier, new Dictionary<int, HashSet<Player>>() },
                { NetworkObjectParameters.GameServerChatGameRoomIdentifier, new Dictionary<int, HashSet<Player>>() }
            };

            for(int i = 1; i <= NetworkObjectParameters.GameServerChatChannelsMaximumNumber; i++)
                ChatDictionary[NetworkObjectParameters.GameServerChatGameListIdentifier][i] = new HashSet<Player>();
        }

        public void CreateRoom(RoomMetadata room)
        {
            //Insert the room to the match metadata list;
            RoomMetadataSortedList.Add(room.ID, room);

            //Create and connect on the chat for the room
            ChatDictionary[NetworkObjectParameters.GameServerChatGameRoomIdentifier][room.ID] = new HashSet<Player>();
        }
    }
}
