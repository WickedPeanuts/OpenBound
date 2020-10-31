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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Collision;
using OpenBound.GameComponents.Interface;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.GameComponents.Pawn.Unit
{
    public class Random : Mobile
    {
        public Random(Player player, Vector2 position) : base(player, position, MobileType.Random)
        {
            Movement.CollisionOffset = 25;
            Movement.MaximumStepsPerTurn = 90;

            //CollisionBox = new CollisionBox(this, new Rectangle(0, 0, 30, 40), new Vector2(0, 10));
        }
    }
}
