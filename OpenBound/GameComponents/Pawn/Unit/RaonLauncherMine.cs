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

namespace OpenBound.GameComponents.Pawn.Unit
{
    public abstract class RaonLauncherMine : Mobile
    {
        protected readonly Mobile mobile;

        public RaonLauncherMine(Mobile mobile, Vector2 position, MobileType mobileType) 
            : base(mobile.Owner, position, mobileType, IsSummon: true)
        {
            this.mobile = mobile;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateOutOfBounds();
            UpdateCollider();
        }

        public override void GrantTurn()
        {
            Movement.RemainingStepsThisTurn = Movement.MaximumStepsPerTurn;
            Movement.IsAbleToMove = true;
            GameScene.Camera.TrackObject(this);
        }


        //Necessary overrides to reuse mines as a mobile

        /// <summary>
        /// Builds a instance of a projectile according to each child element
        /// </summary>
        public abstract Projectile BuildMineProjectile();

        /// <summary>
        /// Updates the collider for each child element, exploding this entity on contact.
        /// </summary>
        public abstract void UpdateCollider();
        
        //Since this mobile does not update its sync state, do nothing.
        public override void UpdateSyncMobileToServer() { return; }


        #region Flipbook & Animation state change
        protected bool IsStateDormant(ActorFlipbookState state) => state == ActorFlipbookState.Dormant;
        protected bool IsStateActivated(ActorFlipbookState state) => state == ActorFlipbookState.Activated;

        public override void ChangeFlipbookState(ActorFlipbookState NewState, bool Force = false)
        {
            if (NewState == MobileFlipbook.State || NewState == ActorFlipbookState.Stand) return;
            MobileFlipbook.ChangeState(NewState, Force);
        }
        #endregion

        #region Sound Effect
        public override void PlayMovementSE(float pitch = 0, float pan = 0)
        {
            AudioHandler.PlayUniqueSoundEffect(movingSE, () => Movement.IsAbleToMove && !Movement.IsFalling, pitch: pitch, pan: pan);
        }
        #endregion

        #region Damage & Destruction
        public void UpdateOutOfBounds() { if (Topography.IsNotInsideMapBoundaries(Position)) Die(); }

        public override void ReceiveDamage(int damage) => OnCollide();

        public override void ReceiveShock(int damage) => OnCollide();
        
        /// <summary>
        /// Explodes the mine, dies, creates a projectile defined by <see cref="BuildMineProjectile"/>
        /// overrride, sets it's position to mine's position and then explodes it
        /// </summary>
        public virtual void OnCollide()
        {
            Die();

            Projectile p = BuildMineProjectile();
            p.Position = CollisionBox.Center;
            p.Explode();
        }

        // This is called when the mine is outside the map boundaries
        // or explodes on contact with another mobile
        public override void Die()
        {
            IsAlive = false;

            //Change the state to force the SE thread to stop
            Movement.IsAbleToMove = false;

            //Grants turn to the next mine or mobile
            LevelScene.ToBeRemovedMineList.Add(this);
        }
        #endregion
    }
}
