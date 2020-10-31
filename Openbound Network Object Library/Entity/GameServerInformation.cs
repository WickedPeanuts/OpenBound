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

namespace OpenBound_Network_Object_Library.Entity
{
    public enum ServerLevelLimitation
    {
        Chick = 0,
        WoodHammer1 = 1, WoodHammer2 = 2, StoneHammer1 = 3, StoneHammer2 = 4,
        Axe1 = 5, Axe2 = 6, SilverAxe1 = 7, SilverAxe2 = 8, GoldenAxe1 = 9, GoldenAxe2 = 10,
        DAxe1 = 11, DAxe2 = 12, DSilverAxe1 = 13, DSilverAxe2 = 14, DGoldenAxe1 = 15, DGoldenAxe2 = 16,
        Staff1 = 17, Staff2 = 18, Staff3 = 19, Staff4 = 20,
        Dragon1 = 21, Dragon2 = 22, Dragon3 = 23,
        Champion1 = 24,
        GM = 25,
    }

    public class GameServerInformation : ServerInformation
    {
        public int ServerID { get; set; }
        public int ConnectedClients { get; set; }
        public int ConnectedClientCapacity { get; set; }
        public int MaximumClientsPerChatChannel { get; set; }
        public bool IsAvatarOn { get; set; }
        public string[] ServerDescription { get; set; }
        public ServerLevelLimitation LowerLevel { get; set; }
        public ServerLevelLimitation HigherLevel { get; set; }
        public bool IsOnline { get; set; }

    }
}
