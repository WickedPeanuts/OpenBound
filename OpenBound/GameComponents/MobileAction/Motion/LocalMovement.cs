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
using Microsoft.Xna.Framework.Input;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Sync;

namespace OpenBound.GameComponents.MobileAction.Motion
{
    public class LocalMovement : Movement
    {
        public LocalMovement(Mobile mobile) : base(mobile) { }

        public override void Update()
        {
            ApplyGravity();

            if (Mobile.IsAlive)
                Move();

            //Must be called before Crosshair.Update();
            UpdateAngle();
        }

        public void Move()
        {
            //If mobile is falling or its actions are locked, dont move
            if (IsFalling || Mobile.IsActionsLocked) return;

            //if user pressed left...
            if (InputHandler.IsCKDown(Keys.Left))
            {
                //and is facing right...
                if (Mobile.Facing == Facing.Right)
                    Mobile.Flip();

                //and move to the left
                if (IsAbleToMove)// && GameInformation.Instance.IsPlayerTurn)
                {
                    MoveSideways(Mobile.Facing);
                }
                else
                {
                    Mobile.ChangeFlipbookState(ActorFlipbookState.UnableToMove, true);
                    Mobile.PlayUnableToMoveSE();
                    return;
                }
            }
            //if the user pressed right...
            else if (InputHandler.IsCKDown(Keys.Right))
            {
                //and is facing left...
                if (Mobile.Facing == Facing.Left)
                    Mobile.Flip();

                if (IsAbleToMove)// && GameInformation.Instance.IsPlayerTurn)
                {
                    MoveSideways(Mobile.Facing);
                }
                else
                {
                    Mobile.ChangeFlipbookState(ActorFlipbookState.UnableToMove, true);
                    Mobile.PlayUnableToMoveSE();
                    return;
                }
            }
            else
            {
                IsMoving = false;
            }

            if ((InputHandler.IsCKUp(Keys.Left) && InputHandler.IsCKUp(Keys.Right)) || !IsAbleToMove)
                Mobile.ChangeFlipbookState(ActorFlipbookState.Stand);
        }
    }
}
