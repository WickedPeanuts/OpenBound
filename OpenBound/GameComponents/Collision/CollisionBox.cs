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
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Interface;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using System;

namespace OpenBound.GameComponents.Collision
{
    public class CollisionBox
    {
        /* The Corner Array can be represented by the following square when
        * C0-----C1   the current rotation is 0 degrees.
        *  |     |
        *  |     |
        * C3-----C2 */
        private readonly Vector2[] corner;
        private Vector2 cornerOffset;
        private Vector2 boxCenter;
        public Vector2 Center => rotatedBoxCenter + rotatedCornerOffset;

        private Vector2[] rotatedCorner;
        private Vector2 rotatedCornerOffset;
        private Vector2 rotatedBoxCenter;

        private Vector2 collisionOffset;

        //Object Reference
        private Mobile owner;

#if DEBUG
        //Debug
        private DebugRectangle debugRectangle;
        private DebugCrosshair debugCrosshair;
        private DebugCircle debugCircle;
        private DebugLine debugLine;
#endif

        /// <summary>
        /// The collision box is created based on a rectangle passed by parameter.
        /// This rectangle gives the dimension of the desired rectangle that will
        /// be converted to 4 points (Corners), every corner is a vertex of the
        /// rectangle, the CurrentRotatedCorner saves the current position of the
        /// rectangle around its owner.
        /// </summary>
        /// <param name="Owner">Owner of the box where it is going to be wrapped around.</param>
        /// <param name="Rectangle">Source Rectangle used as reference.</param>
        /// <param name="CollisionOffset">Offset of the rectangle around the owner.</param>
        public CollisionBox(Mobile Owner, Rectangle Rectangle, Vector2 CollisionOffset)
        {
            owner = Owner;
            collisionOffset = CollisionOffset;

            corner = new Vector2[4];
            corner[0] = Rectangle.Location.ToVector2();
            corner[1] = Rectangle.Location.ToVector2() + new Vector2(Rectangle.Width, 0);
            corner[2] = Rectangle.Location.ToVector2() + new Vector2(Rectangle.Width, Rectangle.Height);
            corner[3] = Rectangle.Location.ToVector2() + new Vector2(0, Rectangle.Height);

            rotatedBoxCenter = boxCenter = (corner[0] + corner[1] + corner[2] + corner[3]) / 4f;

            rotatedCorner = new Vector2[4];
            rotatedCorner[0] = Rectangle.Location.ToVector2();
            rotatedCorner[1] = Rectangle.Location.ToVector2() + new Vector2(Rectangle.Width, 0);
            rotatedCorner[2] = Rectangle.Location.ToVector2() + new Vector2(Rectangle.Width, Rectangle.Height);
            rotatedCorner[3] = Rectangle.Location.ToVector2() + new Vector2(0, Rectangle.Height);

#if DEBUG
            debugRectangle = new DebugRectangle(Color.Red);
            debugCrosshair = new DebugCrosshair(Color.Red);
            debugCircle = new DebugCircle(Color.Black, 25);
            debugLine = new DebugLine(Color.White);

            DebugHandler.Instance.Add(debugRectangle);
            DebugHandler.Instance.Add(debugCrosshair);
            DebugHandler.Instance.Add(debugCircle);
            DebugHandler.Instance.Add(debugLine);
#endif
        }

        /// <summary>
        /// Check if a point is inside the CollisionBox.
        /// </summary>
        /// <param name="Point"></param>
        /// <returns>Returns true if the point is inside the CollisionBox.</returns>
        public bool CheckCollision(Vector2 Point)
        {
            /* https://stackoverflow.com/questions/2752725/finding-whether-a-point-lies-inside-a-rectangle-or-not */

            Vector2[] newCorners = new Vector2[]
            {
                rotatedCorner[0] + rotatedCornerOffset, rotatedCorner[1] + rotatedCornerOffset,
                rotatedCorner[2] + rotatedCornerOffset, rotatedCorner[3] + rotatedCornerOffset,
            };

            Vector2 AB = newCorners[1] - newCorners[0];
            Vector2 AP = Point - newCorners[0];

            Vector2 BC = newCorners[3] - newCorners[0];
            Vector2 BP = Point - newCorners[1];

            float T1 = Vector2.Dot(AB, AP);
            float T2 = Vector2.Dot(BC, BP);

            return
                0 <= T1 && T1 <= Vector2.Dot(AB, AB) &&
                0 <= T2 && T2 <= Vector2.Dot(BC, BC);
        }

        
        public double GetDistance(Vector2 point, int radius)
        {
            return Math.Sqrt(GetSquaredDistance(point, radius));
        }

        /// <summary>
        /// Get the distance between a circumference and the CollisionBox.
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="Radius"></param>
        public double GetSquaredDistance(Vector2 Point, int Radius)
        {
            /* http://www.migapro.com/circle-and-rotated-rectangle-collision-detection/ */

            float rotation = owner.MobileFlipbook.Rotation;

            // Rotate circle's center point back
            Vector2 rPos = Vector2.Transform(Point - rotatedCornerOffset, Matrix.CreateRotationZ(-rotation))
                + rotatedBoxCenter;

            // Closest point in the rectangle to the center of circle rotated backwards(unrotated)
            Vector2 closestPosition = Vector2.Zero;

            // Find the unrotated closest x point from center of unrotated circle
            if (rPos.X < rotatedBoxCenter.X)
                closestPosition += new Vector2(rotatedBoxCenter.X, 0);
            else if (rPos.X > rotatedBoxCenter.X + corner[2].X)
                closestPosition += new Vector2(rotatedBoxCenter.X + corner[2].X, 0);
            else
                closestPosition += new Vector2(rPos.X, 0);

            // Find the unrotated closest y point from center of unrotated circle
            if (rPos.Y < rotatedBoxCenter.Y)
                closestPosition += new Vector2(0, rotatedBoxCenter.Y);
            else if (rPos.Y > rotatedBoxCenter.Y + corner[2].Y)
                closestPosition += new Vector2(0, rotatedBoxCenter.Y + corner[2].Y);
            else
                closestPosition += new Vector2(0, rPos.Y);

#if DEBUG
            //Debug Line Update
            Vector2 destination = Vector2.Transform(closestPosition - rotatedBoxCenter, Matrix.CreateRotationZ(rotation)) + rotatedCornerOffset;
            Point += Vector2.Transform(new Vector2(-Radius, 0), Matrix.CreateRotationZ((float)Helper.AngleBetween(Point, destination)));
            debugLine.Update(Point, destination);
#endif

            // Determine collision
            return Helper.SquaredEuclideanDistance(rPos, closestPosition);// < Radius;
        }

        /// <summary>
        /// This method refreshes the currentRotatedCorner vectors based on
        /// the desired Offset and the owner rotation and position on the scene.
        /// </summary>
        public void Update()
        {
            float rotation = owner.MobileFlipbook.Rotation;

            //Corner
            cornerOffset = owner.MobileFlipbook.Position + collisionOffset - boxCenter;

            //Rotated Corner
            for (int i = 0; i < 4; i++)
                rotatedCorner[i] = Vector2.Transform(corner[i], Matrix.CreateRotationZ(rotation));

            rotatedBoxCenter = Vector2.Transform(boxCenter, Matrix.CreateRotationZ(rotation));

            rotatedCornerOffset = owner.MobileFlipbook.Position - rotatedBoxCenter
                + Vector2.Transform(collisionOffset, Matrix.CreateRotationZ(rotation));

#if DEBUG
            UpdateDebugElements();
#endif
        }

#if DEBUG
        public void UpdateDebugElements()
        {
            LevelScene.MobileList.ForEach((x) =>
            {
                double distance = GetDistance(Cursor.Instance.CurrentFlipbook.Position, 25);
                Color newColor = distance < 25 ? Color.Red : Color.Black;
                debugRectangle.SetColor(newColor);
                debugCrosshair.Sprite.Color = newColor;
            });

            Vector2[] newRotatedCorners = new Vector2[] {
                rotatedCorner[0] + rotatedCornerOffset,
                rotatedCorner[1] + rotatedCornerOffset,
                rotatedCorner[2] + rotatedCornerOffset,
                rotatedCorner[3] + rotatedCornerOffset
            };

            debugCrosshair.Update(rotatedBoxCenter + rotatedCornerOffset);
            debugCrosshair.Sprite.Rotation = owner.MobileFlipbook.Rotation;
            debugRectangle.Update(newRotatedCorners);
            debugCircle.Update(Cursor.Instance.CurrentFlipbook.Position);
        }
#endif
    }
}