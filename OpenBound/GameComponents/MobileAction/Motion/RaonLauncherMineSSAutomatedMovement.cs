using OpenBound.GameComponents.Pawn.Unit;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound.GameComponents.MobileAction.Motion
{
    public class RaonLauncherMineSSAutomatedMovement : AutomatedMovement
    {
        public RaonLauncherMineSSAutomatedMovement(RaonLauncherMineSS mobile)
            : base(mobile) { }

        public override void Move()
        {
            //If is falling, dont move
            if (IsFalling) return;

            MoveSideways(Mobile.Facing);

            if (!IsAbleToMove)
                OnRemaningMovementEnds?.Invoke();
        }
    }
}
