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

using Microsoft.Xna.Framework;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.GameComponents.Pawn
{
    public class ActorBuilder
    {
        public static Mobile BuildMobile(MobileType mobileType, Player player, Vector2 position, bool isInGame = true)
        {
            Mobile mobile = null;

            switch (mobileType)
            {
                case MobileType.Random:       mobile = new Random(player, position);       break;
                case MobileType.Armor:        mobile = new Armor(player, position);        break;
                case MobileType.Bigfoot:      mobile = new Bigfoot(player, position);      break;
                case MobileType.Dragon:       mobile = new Dragon(player, position);       break;
                case MobileType.Mage:         mobile = new Mage(player, position);         break;
                case MobileType.Ice:          mobile = new Ice(player, position);          break;
                case MobileType.Knight:       mobile = new Knight(player, position);       break;
                case MobileType.RaonLauncher: mobile = new RaonLauncher(player, position); break;
                case MobileType.Trico:        mobile = new Trico(player, position);        break;
                case MobileType.Turtle:       mobile = new Turtle(player, position);       break;
                case MobileType.Lightning:    mobile = new Lightning(player, position);    break;
            }

            if (isInGame)
                mobile.HideLobbyExclusiveAvatars();

            return mobile;
        }
    }
}
