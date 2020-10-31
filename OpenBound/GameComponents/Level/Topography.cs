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
using System.Linq;

namespace OpenBound.GameComponents.Level
{
    public class Topography
    {
        public static Vector2 MapSize { get; protected set; }
        public static bool[][] CollidableForegroundMatrix { get; protected set; }
        private static Color[] StageArray;

        public static int MapHeight => CollidableForegroundMatrix.Length;
        public static int MapWidth => CollidableForegroundMatrix[0].Length;
        public static int FirstCollidableBlockY = int.MinValue;

        //Object Reference
        private static Sprite foreground;

        /// <summary>
        /// This method initializes the topography class loading all informations about the terrain
        /// collidable spots. Must be initialized on a scene that contains the foreground sprite
        /// (playable map area).
        /// </summary>
        /// <param name="Foreground"></param>
        public static void Initialize(Sprite Foreground)
        {
            foreground = Foreground;
            ExtractTerrainInformation();
        }

        /// <summary>
        /// Erodes the scenario on a determined circular position with a determined radius.
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Radius"></param>
        /// <returns>The number of removed pixels</returns>
        public static int CreateErosion(Vector2 Position, float Radius)
        {
            int[] pos = GetRelativePosition(Position);

            float blastRadius = Radius + Parameter.BlastBlackmaskRadius;

            int startingIndexX = MathHelper.Clamp(pos[0] - (int)blastRadius, 0, CollidableForegroundMatrix[0].Length);
            int endingIndexX = MathHelper.Clamp(pos[0] + (int)blastRadius + 1, 0, CollidableForegroundMatrix[0].Length);

            int startingIndexY = MathHelper.Clamp(pos[1] - (int)blastRadius, 0, CollidableForegroundMatrix.Length);
            int endingIndexY = MathHelper.Clamp(pos[1] + (int)blastRadius + 1, 0, CollidableForegroundMatrix.Length);

            int numberOfRemovedPixels = 0;

            for (int h = startingIndexY; h < endingIndexY; h++)
            {
                for(int w = startingIndexX; w < endingIndexX; w++)
                {
                    //Using Squared euclidean to prevent SQRT method
                    float calculatedDistance = Helper.SquaredEuclideanDistance(pos[0], pos[1], w, h);

                    //If it the explosion is able to generate black mask
                    if (calculatedDistance <= blastRadius * blastRadius)
                    {
                        int i = h * CollidableForegroundMatrix.Length + w;
                        //If the explosion is able to open holes
                        if (calculatedDistance <= Radius * Radius)
                        {
                            if (StageArray[i] != Color.Transparent)
                            {
                                numberOfRemovedPixels++;
                                StageArray[i] = Color.Transparent;
                                CollidableForegroundMatrix[h][w] = false;
                            }
                        }
                        else
                        {
                            StageArray[i] = StageArray[i].MultiplyByFactor(Parameter.BlastBlackmaskExplosionRadiusColorFactor);
                        }
                    }
                }
            }

            //Change the scenario to a 1D matrix and set the texture data. Ideally there should be no
            //2D matrix at all, but since this method runs pretty quick i see no problem in leaving it like that
            //fror now
            //TODO: Fix This
            foreground.Texture2D.SetData(StageArray);

            return numberOfRemovedPixels;
        }

        /// <summary>
        /// Extracts the terrain information and initializes all static variables.
        /// </summary>
        private static void ExtractTerrainInformation()
        {
            Color[] color1D = new Color[foreground.Texture2D.Width * foreground.Texture2D.Height];
            foreground.Texture2D.GetData(color1D);

            StageArray = color1D;
            CollidableForegroundMatrix = new bool[foreground.Texture2D.Height][];

            for (int h = 0, index1D = 0; h < foreground.Texture2D.Height; h++)
            {
                CollidableForegroundMatrix[h] = new bool[foreground.Texture2D.Width];
                for (int w = 0; w < foreground.Texture2D.Width; w++)
                {
                    Color tmp = color1D[index1D];
                    index1D++;
                    CollidableForegroundMatrix[h][w] = tmp.A > 0;
                }
            }

            //Extract first collidable block y
            for (int h = 0; h < CollidableForegroundMatrix.Length; h++)
            {
                if (CollidableForegroundMatrix[h].ToList().Any((x) => x)) { 
                    FirstCollidableBlockY = GetTransformedPosition(new Vector2(0, h))[1];
                    break;
                }
            }

            MapSize = new Vector2(MapWidth, MapHeight);
        }

        /// <summary>
        /// Converts a normalized float array into relative position
        /// </summary>
        /// <param name="normalizedWorldPosition">Numbers in this array must be normalzized between 0 and 1.</param>
        public static Vector2 FromNormalizedPositionToRelativePosition(float[] normalizedWorldPosition)
        {
            Vector2 newPos = (normalizedWorldPosition.ToVector2() - new Vector2(0.5f, 0.5f)) * MapSize;
            return new Vector2((int)newPos.X, (int)newPos.Y);
        }

        public static int[] GetRelativePosition(Vector2 Position)
        {
            return new int[] { (int)Position.X + CollidableForegroundMatrix[0].Length / 2, (int)Position.Y + CollidableForegroundMatrix.Length / 2 };
        }

        public static int[] GetTransformedPosition(Vector2 Position)
        {
            return new int[] { (int)Position.X - CollidableForegroundMatrix[0].Length / 2, (int)Position.Y - CollidableForegroundMatrix.Length / 2 };
        }

        public static bool IsInsideMapBoundaries(Vector2 Position) => !IsNotInsideMapBoundaries(Position);

        public static bool IsNotInsideMapBoundaries(Vector2 Position)
        {
            //The map boundaires allows the projectiles to exceed its Y position on values
            //lower than the limit 0.

            int[] relPos = GetRelativePosition(Position);
            return relPos[1] >= CollidableForegroundMatrix.Length || relPos[1] < Parameter.ProjectilePlayableMapAreaYLimit || relPos[0] < 0 || relPos[0] >= CollidableForegroundMatrix[0].Length;
        }

        public static bool IsNotInsideMapYBoundaries(Vector2 Position)
        {
            int[] relPos = GetRelativePosition(Position);
            return relPos[1] >= CollidableForegroundMatrix.Length || relPos[1] < Parameter.ProjectilePlayableMapAreaYLimit;
        }

        public static bool CheckCollision(Vector2 Position)
        {
            int[] relPos = GetRelativePosition(Position);

            //if the position is above the stage limit, its collision must not be tested
            //in order to avoid detect collision in a index out of bounds
            if (relPos[1] < 0) return false;

            return CollidableForegroundMatrix[relPos[1]][relPos[0]];
        }
    }
}
