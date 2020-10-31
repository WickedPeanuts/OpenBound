using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn.UnitProjectiles;
using OpenBound_Network_Object_Library.Models;
using System;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBound.GameComponents.Collision;
using OpenBound.GameComponents.Level;
using OpenBound.GameComponents.Audio;
using OpenBound.GameComponents.MobileAction;
using OpenBound.GameComponents.MobileAction.Motion;
using OpenBound.GameComponents.WeatherEffect;

namespace OpenBound.GameComponents.Pawn.Unit
{
    public class RaonLauncherMineS2 : RaonLauncherMine
    {
        public Mobile Target;

        float freezeTime;

        private List<RaonLauncherMineS2> extraMineList;

        // If this variable is true, the mine has the turn and is moving
        private bool isMoving;

        public RaonLauncherMineS2(Mobile mobile, Vector2 position) : base(mobile, position, MobileType.RaonLauncherMineS2)
        {
            MobileFlipbook = MobileFlipbook.CreateMobileFlipbook(MobileType.RaonLauncherMineS2, position);
            MobileFlipbook.ChangeState(ActorFlipbookState.Dormant, true);

            Movement = new RaonLauncherMineS2AutomatedMovement(this);

            Movement.CollisionOffset = 10;
            Movement.MaximumStepsPerTurn = Parameter.ProjectileRaonLauncherS2MineMaximumStepsPerTurn;

            CollisionBox = new CollisionBox(this, new Rectangle(0, 0, 8, 8), new Vector2(0, 5));

            ((AutomatedMovement)Movement).OnInvalidMovemenAttempt += LoseTurn;
            ((AutomatedMovement)Movement).OnRemaningMovementEnds  += () => { Movement.RemainingStepsThisTurn = 0; };

            isMoving = false;
        }

        /// <summary>
        /// Forces this unity to move towards the closest mobile if this entity
        /// is able to move and after the freezetime is over
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (Movement.IsAbleToMove && freezeTime <= Parameter.ProjectileRaonLauncherS2MineTurnFreezetime)
            {
                freezeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }

            base.Update(gameTime);

            if (IsAlive)
                UpdateTornadoCollider();
        }

        /// <summary>
        /// Checks if a tornado is colliding with the mine
        /// </summary>
        public void UpdateTornadoCollider()
        {
            foreach (Weather w in LevelScene.WeatherHandler.WeatherList)
            {
                if (w is Tornado && w.Intersects(this))
                {
                    if (Target == null)
                        SpecialEffectBuilder.RaonLauncherProjectile2DormantTornado(Position);
                    else
                        SpecialEffectBuilder.RaonLauncherProjectile2ActiveTornado(Position);

                    Die();
                }
            }
        }

        /// <summary>
        /// Calculates the closest mobile and sets it as a target.
        /// A target is then used as a reference for the mine movement, if there is
        /// not a close mobile, the target is set to null.
        /// </summary>
        public override void UpdateCollider()
        {
            float minDist = float.MaxValue;

            //Sets the closest mobile and stores it as target
            foreach (Mobile m in LevelScene.MobileList)
            {
                float dist = (float)Helper.SquaredEuclideanDistance(m.Position, Position);

                if (dist < minDist)
                {
                    minDist = dist;
                    Target = m;
                }
            }

            //If the target is colliding with the mine
            if (Target.CollisionBox.CheckCollision(CollisionBox.Center))
            {
                OnCollide();
            }

            if (minDist < Parameter.ProjectileRaonLauncherS2MineSquaredProximityRange)
            {
                if (Position.X - Target.Position.X < 0 && Facing == Facing.Left)
                    Flip();
                else if (Position.X - Target.Position.X > 0 && Facing == Facing.Right)
                    Flip();

                if (IsStateDormant(MobileFlipbook.State))
                {
                    ChangeFlipbookState(ActorFlipbookState.Activated, true);
                }
            }
            else if (IsStateActivated(ActorFlipbookState.Activated))
            {
                Target = null;
                ChangeFlipbookState(ActorFlipbookState.Dormant, true);
            }
        }

        public override Projectile BuildMineProjectile() { return new RaonProjectile2Explosion((RaonLauncher)mobile); }

        #region Turn Management
        /// <summary>
        /// Resets the movement steps left and sets a few variables to start
        /// the mines motion routines.
        /// </summary>
        public override void GrantTurn()
        {
            //Force targetting re-checking
            UpdateCollider();

            if (Target == null)
            {
                LoseTurn();
                return;
            }

            isMoving = true;
            freezeTime = 0;

            base.GrantTurn();
        }

        /// <summary>
        /// This method is part of the 'daisy chain' method calling started by
        /// <see cref="RaonLauncher.GrantTurn"/>. This method sets extraMineList and calls
        /// <see cref="GrantTurn"/>. The next element GrantTurn is called on <see cref="LoseTurn"/>.
        /// </summary>
        /// <param name="extraMineList"></param>
        public void GrantTurn(List<RaonLauncherMineS2> extraMineList)
        {
            this.extraMineList = extraMineList;
            GrantTurn();
        }

        /// <summary>
        /// Loses a turn, sets the current turn owner to another mine existing in the
        /// <see cref="extraMineList"/>. If there are no mines left to earn the turn, calls the
        /// <see cref="RaonLauncher.GrantTurn"/> again.
        /// </summary>
        public override void LoseTurn()
        {
            isMoving = false;
            Movement.IsAbleToMove = false;
            Movement.RemainingStepsThisTurn = 0;

            if (IsStateMoving(MobileFlipbook.State))
                ChangeFlipbookState(ActorFlipbookState.Activated, true);

            //If it has died before its turn has reached
            if (extraMineList == null) return;

            extraMineList.Remove(this);

            if (extraMineList.Count > 0)
                extraMineList[0].GrantTurn(extraMineList);
            else
                mobile.GrantTurn();

            extraMineList = null;
        }
        #endregion

        public override void ChangeFlipbookState(ActorFlipbookState NewState, bool Force = false)
        {
            if (IsStateDormant(NewState) && isMoving)
                NewState = ActorFlipbookState.Activated;

            base.ChangeFlipbookState(NewState, Force);
        }

        public override void Die()
        {
            base.Die();
            LoseTurn();
        }
    }
}
