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

namespace OpenBound_Network_Object_Library.Entity
{
    public class ServerInformation
    {
        public string ServerName { get; set; }
        public string ServerLocalAddress { get; set; }
        public string ServerPublicAddress { get; set; }
        public int ServerPort { get; set; }

        [JsonIgnore]
        public string ServerConsoleName => $"{ServerName}:P{ServerPublicAddress}:L{ServerLocalAddress}:{ServerPort}";
    }
}
