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

using OpenBound_Network_Object_Library.Entity;
using System.Collections.Concurrent;
using OpenBound_Network_Object_Library.Models;
using Newtonsoft.Json;

namespace OpenBound_Game_Server.Server
{
    public class PlayerSession
    {
        public Player Player;
        public RoomMetadata RoomMetadata;
        public MatchManager MatchManager;
        public ConcurrentQueue<byte[]> ProviderQueue;
        public string CurrentConnectedChat
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool IsChatConnected => !string.IsNullOrEmpty(CurrentConnectedChat);

        public (char, int) GetCurrentConnectChatAsTuple(string param = null)
        {
            string text = (param == null) ? CurrentConnectedChat : param;
            char identifier = text[0];
            int channelID = int.Parse(text.Substring(1, text.Length - 1));
            return (identifier, channelID);
        }
    }
}
