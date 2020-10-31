using Microsoft.Xna.Framework;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.Pawn.Unit;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound.GameComponents.MobileAction.Motion
{
    public class RaonLauncherMineS2AutomatedMovement : AutomatedMovement
    {
        public RaonLauncherMineS2AutomatedMovement(RaonLauncherMineS2 mobile)
            : base(mobile){ }

        public override void Move()
        {
            //If is falling, dont move
            if (IsFalling) return;

            Vector2 movVector = Mobile.Position - ((RaonLauncherMineS2)Mobile).Target.Position;

            if (Math.Abs(movVector.X) < 1)
                OnInvalidMovemenAttempt?.Invoke();

            bool canMove = IsAbleToMove;

            //and move to the left
            if (movVector.X > 0)
                MoveSideways(Mobile.Facing);
            else if (movVector.X < 0)
                MoveSideways(Mobile.Facing);

            if (canMove && !IsAbleToMove)
                OnRemaningMovementEnds?.Invoke();
        }
    }
}
