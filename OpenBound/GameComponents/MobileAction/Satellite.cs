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
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.Physics;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.MobileAction
{
    public enum SatelliteSpecialAnimationState
    {
        Idle,
        Initializing1,
        Moving1,
        Attacking,
        Initializing2,
        Moving2,
    }

    public enum SatelliteFlipbookState
    {
        Idle,
        ShootingS1,
        ShootingS2,
    }

    public abstract class Satellite
    {
        static readonly Dictionary<MobileType, Dictionary<SatelliteFlipbookState, AnimationInstance>> SatelliteStatePresets = new Dictionary<MobileType, Dictionary<SatelliteFlipbookState, AnimationInstance>>()
        {
            {
                MobileType.Knight,
                new Dictionary<SatelliteFlipbookState, AnimationInstance>()
                {
                    { SatelliteFlipbookState.Idle,       new AnimationInstance(){ StartingFrame =  0, EndingFrame = 19, TimePerFrame = 1/20f } },
                    { SatelliteFlipbookState.ShootingS1, new AnimationInstance(){ StartingFrame = 20, EndingFrame = 39, TimePerFrame = 1/20f } },
                    { SatelliteFlipbookState.ShootingS2, new AnimationInstance(){ StartingFrame = 20, EndingFrame = 39, TimePerFrame = 1/20f } },
                }
            }
        };

        public Flipbook Flipbook;

        //ShotType
        protected ShotType ShotType;
        protected Vector2 CurrentOffset;
        public Vector2 S1OwnerOffset { get; protected set; }
        public Vector2 S2OwnerOffset { get; protected set; }

        //SS Animation
        public Vector2 SSTargetOffset { get; protected set; }
        protected SatelliteSpecialAnimationState SatelliteSpecialAnimationState;
        protected DefinedAcceleratedMovement ySSAnimationMovement;
        protected DefinedAcceleratedMovement xSSAnimationMovement;
        protected float currentSSAnimationAngle;

        //Flipbook State
        protected SatelliteFlipbookState SatelliteFlipbookState;
        protected Dictionary<SatelliteFlipbookState, AnimationInstance> StatePresets;

        //Satellite Centroid
        protected Vector2 Center;

        //Satellite Cannon
        protected Vector2 CannonPosition;
        protected float CannonPositionOffset;

        //Screen Animation
        public Renderable AttackingTarget { set; get; }

        //Oscilation
        protected double oscilationAmplitude;
        protected double oscilationCurrentAngle;
        protected double oscilationAngleFactor;

        //Movement Component
        protected DefinedAcceleratedMovement yMovementComponent;
        protected float currentStartingSpeed;
        protected float desiredFinalPosition;

        //Object References
        protected readonly Mobile mobile;

#if DEBUG
        //Crosshairs
        List<DebugElement> debugElemList = new List<DebugElement>();
#endif

        public Satellite(Mobile Owner)
        {
            mobile = Owner;
            SatelliteFlipbookState = SatelliteFlipbookState.Idle;
            StatePresets = SatelliteStatePresets[Owner.MobileType];
            yMovementComponent = new DefinedAcceleratedMovement();
            currentStartingSpeed = Parameter.SatelliteMovementStartingSpeed;

#if DEBUG
            debugElemList = new List<DebugElement>()
            {
                new DebugCrosshair(Color.Teal), new DebugCrosshair(Color.Blue),
                new DebugLine(Color.Bisque),new DebugLine(Color.Bisque)
            };

            DebugHandler.Instance.AddRange(debugElemList);
#endif

        }

        public void ChangeState(SatelliteFlipbookState NewState)
        {
            if (SatelliteFlipbookState == NewState) return;

            SatelliteFlipbookState = NewState;
            Flipbook.AppendAnimationIntoCycle(new List<AnimationInstance>() { StatePresets[SatelliteFlipbookState] }, true);
        }

        public void ChangeShotType(ShotType ShotType)
        {
            if (ShotType == this.ShotType) return;
            this.ShotType = ShotType;

            if (ShotType == ShotType.S1 || ShotType == ShotType.SS) desiredFinalPosition = S1OwnerOffset.Y;
            else if (ShotType == ShotType.S2) desiredFinalPosition = S2OwnerOffset.Y;

            if (desiredFinalPosition == CurrentOffset.Y) return;

            if (desiredFinalPosition < CurrentOffset.Y)
                currentStartingSpeed = -currentStartingSpeed;
            else
                currentStartingSpeed = Math.Abs(currentStartingSpeed);

            yMovementComponent.Preset(CurrentOffset.Y, desiredFinalPosition, currentStartingSpeed, Parameter.SatelliteSSAnimationTotalMotionTime);
        }

        private void RecalculateDebugCrosshairPosition()
        {
            //Debug Crosshair
            Vector2 newP = (AttackingTarget.Position - Flipbook.Position) / 10;

#if DEBUG
            ((DebugCrosshair)debugElemList[0]).Update(Center);
            ((DebugCrosshair)debugElemList[1]).Update(CannonPosition);

            ((DebugLine)debugElemList[2]).Update(Flipbook.Position, mobile.MobileFlipbook.Position);
            ((DebugLine)debugElemList[3]).Update(Flipbook.Position, AttackingTarget.Position);
#endif
        }

        public void StartSSAnimation()
        {
            SatelliteSpecialAnimationState = SatelliteSpecialAnimationState.Initializing1;
        }

        public void UpdateMovement(GameTime GameTime)
        {
            switch (SatelliteSpecialAnimationState)
            {
                /* When the satellite is idle, it just rotates toward the target and have the
                natural behavior of flying, it must only be changed if the satellite engages
                n the SS, wich its behavior changes depending on the attack phase. */
                case SatelliteSpecialAnimationState.Idle:
                    //Calculate oscilating movement
                    Center = mobile.Position + CurrentOffset;
                    oscilationCurrentAngle += oscilationAngleFactor * GameTime.ElapsedGameTime.TotalSeconds;
                    Vector2 currentOscilation = new Vector2(0, (float)(Math.Sin(oscilationCurrentAngle) * oscilationAmplitude));

                    //Repositioning
                    Flipbook.Position = mobile.Position + CurrentOffset + currentOscilation;

                    //Update Position Based on Motion
                    if (yMovementComponent.IsMoving)
                    {
                        yMovementComponent.RefreshCurrentPosition((float)GameTime.ElapsedGameTime.TotalSeconds);
                        CurrentOffset = new Vector2(0, yMovementComponent.CurrentPosition);
                    }

                    break;
                /* When the attack starts, the satellite must load all variables that are going
                to be necessary on the movement. First all the phys related variable changes
                to make the satellite move above the target. */
                case SatelliteSpecialAnimationState.Initializing1:
                    ySSAnimationMovement = new DefinedAcceleratedMovement();
                    xSSAnimationMovement = new DefinedAcceleratedMovement();
                    ySSAnimationMovement.Preset(Flipbook.Position.Y, AttackingTarget.Position.Y + SSTargetOffset.Y, 0, Parameter.SatelliteSSAnimationTotalMotionTime);
                    xSSAnimationMovement.Preset(Flipbook.Position.X, AttackingTarget.Position.X - Parameter.SatelliteSSAnimationMovementRange.X, 0, Parameter.SatelliteSSAnimationTotalMotionTime);
                    SatelliteSpecialAnimationState = SatelliteSpecialAnimationState.Moving1;

                    break;
                /* After changing the physics variables its time to start moving to the selected
                position. Here only the satellite position is updated, when the movement ends the
                satellite now can start performing the attack. */
                case SatelliteSpecialAnimationState.Moving1:
                    ySSAnimationMovement.RefreshCurrentPosition((float)GameTime.ElapsedGameTime.TotalSeconds);
                    xSSAnimationMovement.RefreshCurrentPosition((float)GameTime.ElapsedGameTime.TotalSeconds);
                    Flipbook.Position = new Vector2(xSSAnimationMovement.CurrentPosition, ySSAnimationMovement.CurrentPosition);
                    currentSSAnimationAngle = Parameter.SatelliteSSAnimationInitialAngle;

                    if (!ySSAnimationMovement.IsMoving)
                        SatelliteSpecialAnimationState = SatelliteSpecialAnimationState.Attacking;

                    break;
                /* While attacking, the satellite just moves around by multiplying the cos factor to
                the x position. After 360 degrees are past, the satellite should start moving back
                to its former position. */
                case SatelliteSpecialAnimationState.Attacking:
                    Flipbook.Position = new Vector2(xSSAnimationMovement.CurrentPosition, ySSAnimationMovement.CurrentPosition) - Parameter.SatelliteSSAnimationMovementRange * ((float)Math.Cos(currentSSAnimationAngle) - 1);
                    currentSSAnimationAngle += (float)Math.PI * Parameter.StelliteAttackSpeedFactor * (float)GameTime.ElapsedGameTime.TotalSeconds;

                    if (currentSSAnimationAngle > Parameter.SatelliteSSAnimationTotalAngle)
                        SatelliteSpecialAnimationState = SatelliteSpecialAnimationState.Initializing2;

                    break;
                /* In order to make the satellite move all we gotta do is just reset the phys
                variables to come back. */
                case SatelliteSpecialAnimationState.Initializing2:
                    ySSAnimationMovement.Preset(Flipbook.Position.Y, mobile.Position.Y + CurrentOffset.Y, 0, Parameter.SatelliteSSAnimationTotalMotionTime);
                    xSSAnimationMovement.Preset(Flipbook.Position.X, mobile.Position.X, 0, Parameter.SatelliteSSAnimationTotalMotionTime);
                    SatelliteSpecialAnimationState = SatelliteSpecialAnimationState.Moving2;
                    break;
                /* Then start the motion. When its finally over, it changes back to the original
                behavior.  */
                case SatelliteSpecialAnimationState.Moving2:
                    ySSAnimationMovement.RefreshCurrentPosition((float)GameTime.ElapsedGameTime.TotalSeconds);
                    xSSAnimationMovement.RefreshCurrentPosition((float)GameTime.ElapsedGameTime.TotalSeconds);
                    Flipbook.Position = new Vector2(xSSAnimationMovement.CurrentPosition, ySSAnimationMovement.CurrentPosition);

                    if (!ySSAnimationMovement.IsMoving)
                        SatelliteSpecialAnimationState = SatelliteSpecialAnimationState.Idle;

                    break;
            }
        }

        public void Update(GameTime GameTime)
        {
            UpdateMovement(GameTime);
            ReadjustAngle();

#if DEBUG
            RecalculateDebugCrosshairPosition();
#endif
        }

        public void ReadjustAngle()
        {
            //Satellite Angle
            Flipbook.Rotation = (float)Math.Atan2((AttackingTarget.Position.Y - Flipbook.Position.Y), (AttackingTarget.Position.X - Flipbook.Position.X));

            //Satellite Cannon Position
            CannonPosition = new Vector2((float)(Flipbook.Position.X + CannonPositionOffset * Math.Cos(Flipbook.Rotation)), (float)(Flipbook.Position.Y + CannonPositionOffset * Math.Sin(Flipbook.Rotation)));
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            Flipbook.Draw(GameTime, SpriteBatch);
        }
    }

    public class KnightSword : Satellite
    {
        public KnightSword(Mobile Owner) : base(Owner)
        {
            Flipbook = new Flipbook(Vector2.Zero,
                new Vector2(48 / 2, 30 / 2),
                48, 31,
                "Graphics/Tank/Knight/Satellite",
                StatePresets[SatelliteFlipbookState],
                DepthParameter.MobileSatellite);

            oscilationAmplitude = 3f;
            oscilationCurrentAngle = 0f;
            oscilationAngleFactor = 5f;

            CurrentOffset = S1OwnerOffset = new Vector2(0, Parameter.ProjectileKnightS1OwnerOffset);
            S2OwnerOffset = new Vector2(0, Parameter.ProjectileKnightS2OwnerOffset);
            SSTargetOffset = new Vector2(0, Parameter.ProjectileKnightSSOwnerOffset);

            AttackingTarget = Owner;

            CannonPositionOffset = 15;
        }
    }
}
