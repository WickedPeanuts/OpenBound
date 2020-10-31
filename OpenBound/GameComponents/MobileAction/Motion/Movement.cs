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
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Sync;
using System;
using System.Linq;

namespace OpenBound.GameComponents.MobileAction.Motion
{
    /// <summary>
    /// The method collisions are now calculated must be re-created to mimic.
    /// original GB movement. Right now im out of ideas.
    /// </summary>
    public abstract class Movement
    {
        //Falling Control Variable
        public bool IsFalling;

        // Collision Variables
        public float CollisionOffset { get; set; }
        public bool IsAbleToMove { get; set; }
        public bool IsMoving;

        //Angulation Variables
        public float BufferRotationInDegrees;

        //Steps Per Turn
        public int RemainingStepsThisTurn;
        public int MaximumStepsPerTurn;

        //Movement Delays
        protected float sidewaysDelayTimer;
        protected float gravityDelayTimer;

        //Wind-pushing force
        private float windForceAccumulator;

        //Reference
        protected Mobile Mobile;
        protected float GravitySpeed;

        public Movement(Mobile mobile)
        {
            Mobile = mobile;
            IsFalling = true;
            IsAbleToMove = false;
            IsMoving = false;
        }

        public void MoveSideways(Facing Facing)
        {
            //Sound Effect
            Mobile.PlayMovementSE();

            //Copy parameter tank speed
            float TankSpeed = Parameter.TankMovementSpeed;

            if (Facing == Facing.Right)
                TankSpeed *= -1;

            //Start calculating the next possible position
            int[] relPos = Topography.GetRelativePosition(Mobile.Position);
            int i = (int)Parameter.TankMovementMaxYStepping;

            //Flag to determine if the movement is possible
            bool isValid = false;

            //Get the possible adjacent block coordinate and save it on newX & newY
            for (; i > -Parameter.TankMovementMinYStepping; i--)
            {
                //If the collision flag of the adjacent block is 0

                //Save as a possible adjacent solution

                if (relPos[0] - TankSpeed < 0 || relPos[0] - TankSpeed >= Topography.MapHeight)
                {
                    //Keep the height
                    i = 0;
                    isValid = true;
                    break;
                }

                isValid = isValid || !Topography.CollidableForegroundMatrix[relPos[1] - i][(int)(relPos[0] - TankSpeed)];

                //if has already a valid movement and a wall is detected
                if (isValid && Topography.CollidableForegroundMatrix[relPos[1] - i][(int)(relPos[0] - TankSpeed)])
                    break;
            }

            if (isValid)
            {
                if (!IsMoving)
                {
                    IsMoving = true;
                    sidewaysDelayTimer = 0;
                }

                sidewaysDelayTimer += Parameter.ProjectileMovementFixedElapedTime;

                if (sidewaysDelayTimer > Parameter.TankMovementSidewaysDelay)
                {
                    // Add new movement based on tank speed
                    RemainingStepsThisTurn--;
                    Mobile.Position += new Vector2(-(int)TankSpeed, -i);
                    Mobile.ChangeFlipbookState(ActorFlipbookState.Moving, true);
                    if (Mobile.IsPlayable) LevelScene.HUD.MovementBar.PerformStep();
                }
            }
            else
            {
                InvalidateMovementAttempt();
            }

            IsAbleToMove = RemainingStepsThisTurn > 0;
        }

        public virtual void InvalidateMovementAttempt()
        {
            //Sync
            if (Mobile.MobileFlipbook.State != ActorFlipbookState.UnableToMove)
                Mobile.ForceSynchronize = true;

            Mobile.ChangeFlipbookState(ActorFlipbookState.UnableToMove, true);
            Mobile.PlayUnableToMoveSE();
        }

        public void ApplyGravity()
        {
            if (Topography.IsNotInsideMapYBoundaries(Mobile.Position - new Vector2(0, Mobile.MobileFlipbook.SpriteHeight)))
            {
                if (Mobile.IsAlive) Mobile.RequestDeath(CausaMortis.Bungee);
                return;
            }

            int[] relPos = Topography.GetRelativePosition(Mobile.Position);

            //Compute the where is the next collidable block bellow the pawn
            int yPosition = 0;

            for (; yPosition < Math.Max(Parameter.TankMovementMinYStepping, GravitySpeed); yPosition++)
            {
                if (relPos[1] + yPosition >= Topography.CollidableForegroundMatrix.Length)
                    continue;

                if (relPos[0] > 0 && relPos[0] < Topography.MapHeight && Topography.CollidableForegroundMatrix[relPos[1] + yPosition][relPos[0]])
                    break;
            }

            //If the unit isn't on the ground
            if (yPosition > 0)
            {
                //Update gravity values on the pawn
                if (IsFalling)
                {
                    gravityDelayTimer += Parameter.ProjectileMovementFixedElapedTime;
                    if (gravityDelayTimer > Parameter.TankMovementGravityDelay)
                    {
                        //Wind-changing accumulated offset
                        Vector2 wForce = (LevelScene.MatchMetadata != null) ? LevelScene.MatchMetadata.WindForceComponents().ToVector2() : Vector2.Zero;

                        windForceAccumulator = MathHelper.Clamp(windForceAccumulator + wForce.X / 45, -1, 1);

                        //Add interpolated movement
                        Vector2 newPosition = Mobile.Position + new Vector2((int)windForceAccumulator, Math.Min((int)GravitySpeed, yPosition));

                        //In case the mobile collides in walls the X axis stop updating in order to prevent wall clipping
                        if (Topography.IsInsideMapBoundaries(newPosition) && !Topography.CheckCollision(newPosition))
                            windForceAccumulator = 0;

                        Mobile.Position += new Vector2((int)windForceAccumulator, Math.Min((int)GravitySpeed, yPosition));

                        GravitySpeed += Parameter.TankMovementGravityFactor;

                        //Reseting the accumulator
                        if (Math.Abs(windForceAccumulator) >= 1)
                            windForceAccumulator = 0;
                    }
                }
                else
                {
                    gravityDelayTimer = 0;
                    GravitySpeed = Parameter.TankMovementInitialGravity;
                }

                //Reset the rotation
                BufferRotationInDegrees = 0;

                //Set the state to falling
                if (!IsFalling)
                {
                    Mobile.ForceSynchronize = true;
                    IsFalling = true;
                }

                IsMoving = false;
                //desiredPosition.Y = Mobile.Position.Y;

                Mobile.ChangeFlipbookState(ActorFlipbookState.Falling, true);
            }
            else
            {
                //Set the the falling state to false
                if (IsFalling == true)
                {
                    IsFalling = false;
                    windForceAccumulator = 0;
                    Mobile.ChangeFlipbookState(ActorFlipbookState.Stand, true);
                }
            }
        }

        /// <summary>
        ///Update mobile standing angle based on the projections defined by ParameterCalculationOffsetX/Y
        /// </summary>
        public void UpdateAngle()
        {
            Vector2 leftNotCollidablePosition, rightNotCollidablePosition;
            Vector2 leftCollidablePosition, rightCollidablePosition;
            Vector2 selectedLeft, selectedRight;

            leftNotCollidablePosition =
                leftCollidablePosition =
                rightNotCollidablePosition =
                rightCollidablePosition = default;

            int[] relPos = Topography.GetRelativePosition(Mobile.Position);

            int xAxis = Parameter.TankMovementRotationCalculationOffsetX;
            int yAxis = Parameter.TankMovementRotationCalculationOffsetY;

            int minXCoord = MathHelper.Clamp(relPos[0] - xAxis, 0, Topography.CollidableForegroundMatrix[0].Length - 1);
            int maxXCoord = MathHelper.Clamp(relPos[0] + xAxis, 0, Topography.CollidableForegroundMatrix[0].Length - 1);

            //Project possible left spot
            for (int h = -yAxis; h < yAxis; h++)
            {
                if (relPos[1] + h >= Topography.CollidableForegroundMatrix.Length)
                    break;

                if (!Topography.CollidableForegroundMatrix[relPos[1] + h][minXCoord])
                {
                    leftNotCollidablePosition = new Vector2(minXCoord, relPos[1] + h);
                }
                else
                {
                    if (leftNotCollidablePosition != default)
                        break;
                }
            }

            //Project possible right spot
            for (int h = -yAxis; h < yAxis; h++)
            {
                if (relPos[1] + h >= Topography.CollidableForegroundMatrix.Length)
                    break;

                if (!Topography.CollidableForegroundMatrix[relPos[1] + h][maxXCoord])
                {
                    rightNotCollidablePosition = new Vector2(maxXCoord, relPos[1] + h);
                }
                else
                {
                    if (rightNotCollidablePosition != default)
                        break;
                }
            }

            //TODO maybe fix it or maybe not

            // In case of left/right projection is located on a hole
            for (int h = -yAxis; h < yAxis; h++)
            {
                if (relPos[1] - h >= Topography.CollidableForegroundMatrix.Length)
                    break;

                if (Topography.CollidableForegroundMatrix[relPos[1] - h][minXCoord])
                {
                    leftCollidablePosition = new Vector2(
                        minXCoord,
                        relPos[1] - h);
                }

                if (Topography.CollidableForegroundMatrix[relPos[1] - h][maxXCoord])
                {
                    rightCollidablePosition = new Vector2(
                        maxXCoord,
                        relPos[1] - h);
                }
            }

            //Select best projection
            if (leftNotCollidablePosition == default)
            {
                selectedLeft = leftCollidablePosition;
            }
            else
            {
                selectedLeft = leftNotCollidablePosition;
            }

            if (rightNotCollidablePosition == default)
            {
                selectedRight = rightCollidablePosition;
            }
            else
            {
                selectedRight = rightNotCollidablePosition;
            }

            Vector2 tmp = selectedRight - selectedLeft;
            BufferRotationInDegrees = MathHelper.ToDegrees((float)Math.Atan2(tmp.Y, tmp.X));

            float newRot = MathHelper.ToRadians(BufferRotationInDegrees);

            if (IsFalling)
                newRot = 0;

            //Crosshair Angle
            if (Mobile.Crosshair != null)
                Mobile.Crosshair.UpdateCrosshairPointerAngle(Mobile.MobileFlipbook.Rotation - newRot);

            //Tank Rotation
            Mobile.MobileFlipbook.Rotation = newRot;
        }

        public abstract void Update();
    }
}
