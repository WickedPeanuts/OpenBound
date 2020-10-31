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
using OpenBound.GameComponents.Collision;
using OpenBound.GameComponents.Pawn.UnitProjectiles;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.GameComponents.Pawn.Unit
{
    public class Dragon : Mobile
    {
        public override double TeleportationBeaconInteractionTime => 0.3d;

        public Dragon(Player player, Vector2 position) : base(player, position, MobileType.Dragon)
        {
            Movement.CollisionOffset = 25;
            Movement.MaximumStepsPerTurn = 90;

            CollisionBox = new CollisionBox(this, new Rectangle(0, 0, 36, 38), new Vector2(0, -10));
        }

        public override void Die()
        {
            base.Die();
            Movement.CollisionOffset = 10;
        }

        public override void PlayUnableToMoveSE(float pitch = 0, float pan = 0)
        {
            base.PlayUnableToMoveSE(pitch: 0.75f);
        }

        protected override void Shoot(ShotType shotType, double interactionTime = 0)
        {
            if (shotType == ShotType.S1)
                UninitializedProjectileList.Add(new DragonProjectile1(this));
            else if (shotType == ShotType.S2)
                DragonProjectileEmitter.Shot2(this);
            else if (shotType == ShotType.SS)
                UninitializedProjectileList.Add(new DragonProjectile3(this));

            base.Shoot(shotType, interactionTime);
        }
    }
}
