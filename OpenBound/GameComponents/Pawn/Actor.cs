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
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.MobileAction;
using OpenBound.GameComponents.MobileAction.Motion;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.GameComponents.Pawn
{
    public abstract class Actor : Renderable
    {
        public Facing Facing { get; set; }

        //Movement
        public Movement Movement;
        public Vector2 FlipbookOffset;

        //Object References
        public Player Owner;

        public Actor() : base()
        {
            Facing = Facing.Left;
            LayerDepth = DepthParameter.Mobile;
        }

        public virtual void Flip()
        {
            Facing = (Facing == Facing.Left) ? Facing.Right : Facing.Left;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void ReceiveDamage(int damage);
        public abstract void ReceiveShock(int damage);
        public virtual void PlayMovementSE(float pitch, float pan) { }
        public virtual void PlayUnableToMoveSE(float pitch, float pan) { }
    }
}
