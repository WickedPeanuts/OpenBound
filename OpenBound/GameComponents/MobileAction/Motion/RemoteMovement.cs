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
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Sync;
using System.Collections.Generic;

namespace OpenBound.GameComponents.MobileAction.Motion
{
    public class RemoteMovement : Movement
    {
        public Vector2 DesiredPosition;
        public Queue<Vector2> DesiredPositionQueue;

        public bool IsReadyToShoot => DesiredPositionQueue.Count == 0 && Mobile.Position == DesiredPosition;
        public bool IsReadyToDequeue => DesiredPositionQueue.Count > 0 && DesiredPosition.X == Mobile.Position.X;

        public RemoteMovement(Mobile mobile) : base(mobile)
        {
            DesiredPositionQueue = new Queue<Vector2>();
        }

        public override void Update()
        {
            ApplyGravity();

            if (Mobile.IsAlive && Mobile.SyncMobile != null)
                Move();

            //Must be called before Crosshair.Update();
            UpdateAngle();
        }

        public void EnqueuePosition(Vector2 position)
        {
            DesiredPositionQueue.Enqueue(position.ToIntegerDomain());

            if (IsReadyToDequeue)
                DesiredPosition = DesiredPositionQueue.Dequeue();
        }

        public void Move()
        {
            //If is falling, dont move
            if (IsFalling) return;

            Vector2 movVector = Mobile.Position - DesiredPosition;

            //and move to the left
            if (movVector.X > 0)
            {
                if (Mobile.Facing != Facing.Left)
                    Mobile.Flip();

                MoveSideways(Mobile.Facing);
            }
            else if (movVector.X < 0)
            {
                if (Mobile.Facing != Facing.Right)
                    Mobile.Flip();

                MoveSideways(Mobile.Facing);
            }
            else
            {
                if (Mobile.Facing != Mobile.SyncMobile.Facing)
                {
                    Mobile.Flip();
                    //Mobile.Facing = Mobile.SyncMobile.Facing;
                }
            }

            Vector2 movVector2 = Mobile.Position - DesiredPosition;

            if (movVector.X == movVector2.X)
            {
                if (IsMoving && sidewaysDelayTimer > Parameter.TankMovementSidewaysDelay)
                {
                    IsMoving = false;

                    Mobile.Position = DesiredPosition;
                }

                if (Mobile.SyncMobile.ContainsAction(SynchronizableAction.UnableToMove))
                {
                    Mobile.PlayUnableToMoveSE();
                    Mobile.ChangeFlipbookState(ActorFlipbookState.UnableToMove, true);
                }
                else
                {
                    Mobile.ChangeFlipbookState(ActorFlipbookState.Stand);
                }

                if (IsReadyToDequeue)
                    DesiredPosition = DesiredPositionQueue.Dequeue();
            }
        }
    }
}
