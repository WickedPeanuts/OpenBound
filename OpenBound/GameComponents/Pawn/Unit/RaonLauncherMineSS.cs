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

namespace OpenBound.GameComponents.Pawn.Unit
{
    public class RaonLauncherMineSS : RaonLauncherMine
    {
        Action onMineIsDestroyed;

        public RaonLauncherMineSS(Mobile mobile, Vector2 position, Action onMineIsDestroyed)
            : base(mobile, position, MobileType.RaonLauncherMineSS)
        {
            MobileFlipbook = MobileFlipbook.CreateMobileFlipbook(MobileType.RaonLauncherMineSS, position);
            MobileFlipbook.ChangeState(ActorFlipbookState.Stand, true);

            this.onMineIsDestroyed = onMineIsDestroyed;

            Movement = new RaonLauncherMineSSAutomatedMovement(this);

            Movement.CollisionOffset = 10;
            Movement.MaximumStepsPerTurn = Parameter.ProjectileRaonLauncherSSMineMaximumStepsPerTurn;

            if (mobile.Facing == Facing.Right)
                Flip();

            CollisionBox = new CollisionBox(this, new Rectangle(0, 0, 8, 8), new Vector2(0, 5));

            ((AutomatedMovement)Movement).OnInvalidMovemenAttempt += Flip;
            ((AutomatedMovement)Movement).OnRemaningMovementEnds  += OnCollide;

            GrantTurn();
        }

        public override void PlayMovementSE(float pitch = -1f, float pan = 0)
        {
            AudioHandler.PlayUniqueSoundEffect(movingSE, () => IsAlive, pitch: pitch, pan: pan);
        }

        /// <summary>
        /// Forces the mine to keep moving forward until it reaches a wall, when it does, <see cref="AutomatedMovement"/>
        /// calls <see cref="AutomatedMovement.OnInvalidMovemenAttempt"/>, which calls <see cref="Mobile.Flip"/>. Making
        /// it move to the other side.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void UpdateCollider()
        {
            foreach (Mobile m in LevelScene.MobileList)
            {
                if (m.CollisionBox.CheckCollision(CollisionBox.Center))
                    OnCollide();
            }
        }

        public override Projectile BuildMineProjectile()
        {
            return new RaonProjectile3Explosion((RaonLauncher)mobile);
        }

        public override void Die()
        {
            // If the mine dies for some reason (like out of bounds), it is removed and then the
            // next turn request routine is fired
            base.Die();
            onMineIsDestroyed?.Invoke();
        }
    }
}
