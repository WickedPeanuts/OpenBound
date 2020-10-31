using Microsoft.Xna.Framework;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.MobileAction;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.MobileAction
{
    public class HelicoidalTrace : Trace
    {
        static AnimationInstance traceLeadAnimationInstance = new AnimationInstance() { StartingFrame = 0, EndingFrame = 7, TimePerFrame = 1 / 30f };

        private MobileType mobileType;
        private ShotType shotType;

        public HelicoidalTrace(MobileType mobileType, ShotType shotType, Color color, Projectile projectile)
            : base(mobileType, color, projectile)
        {
            this.mobileType = mobileType;
            leadTrace = SpawnFlipbook(mobileType, shotType, traceLeadAnimationInstance);
            leadTrace.Color = color;
            this.shotType = shotType;
        }

        public virtual void Update(Vector2 projectilePosition, Vector2 angularPositionOffset, float currentProjectileAngle, float elapsedTime, float angleOffset, float rotationFactor = 1f, float angleFactor = 1f)
        {
            rotationAngle += elapsedTime * MathHelper.TwoPi * rotationFactor;

            //Update DC
            positionRotatedOffset = Vector2.Transform(angularPositionOffset * (float)Math.Sin(rotationAngle + angleOffset) * angleFactor, Matrix.CreateRotationZ(currentProjectileAngle));
            positionOffset = projectilePosition + positionRotatedOffset;

            if (lastSpawn == null || Vector2.Distance(lastSpawn, positionOffset) > 8)
            {
                //Top
                Flipbook newFlipbook = SpawnFlipbook(mobileType, shotType);
                newFlipbook.Position = positionOffset;
                newFlipbook.SetTransparency(0);

                lastSpawn = positionOffset;

                traceList.Add(newFlipbook);

                float f = 1;
                for (int i = traceList.Count - 1; i >= 0; i--, f -= 0.03f)
                {
                    if (i - 1 >= 0)
                    {
                        var prev = traceList[i - 1];
                        var curr = traceList[i];
                        curr.Rotation = (float)Helper.AngleBetween(curr.Position, prev.Position) + MathHelper.Pi / 2;
                    }

                    traceList[i].Color = Color * f;
                }
            }

            leadTrace.Position = positionOffset;
            leadTrace.Rotation = currentProjectileAngle;

#if DEBUG
            dc0.Update(projectilePosition);
            dc1.Update(positionRotatedOffset + projectilePosition);
#endif
        }
    }
}
