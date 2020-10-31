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
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Pawn
{
    public class Avatar
    {
        static Dictionary<AvatarCategory, float> depthDictioary = new Dictionary<AvatarCategory, float>()
        {
            { AvatarCategory.Hat,     DepthParameter.AvatarHead },
            { AvatarCategory.Body,    DepthParameter.AvatarBody },
            { AvatarCategory.Goggles, DepthParameter.AvatarGoggles },
            { AvatarCategory.Flag,    DepthParameter.AvatarFlag },
            { AvatarCategory.Pet,     DepthParameter.AvatarPet },
            { AvatarCategory.Misc,    DepthParameter.AvatarMisc },
            { AvatarCategory.Extra,   DepthParameter.AvatarExtra },
        };

        static Dictionary<AvatarCategory, Dictionary<AvatarState, AnimationInstance>> avatarState =
            new Dictionary<AvatarCategory, Dictionary<AvatarState, AnimationInstance>>()
            {
                {
                    AvatarCategory.Hat,
                    new Dictionary<AvatarState, AnimationInstance>()
                    {
                        { AvatarState.Normal, new AnimationInstance() { StartingFrame = 11, EndingFrame = 21, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                        { AvatarState.Staring, new AnimationInstance() { StartingFrame = 00, EndingFrame = 10, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                    }
                },
                {
                    AvatarCategory.Body,
                    new Dictionary<AvatarState, AnimationInstance>()
                    {
                        { AvatarState.Normal,  new AnimationInstance() { StartingFrame = 00, EndingFrame = 10, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                    }
                },
                {
                    AvatarCategory.Pet,
                    new Dictionary<AvatarState, AnimationInstance>()
                    {
                        { AvatarState.Normal, new AnimationInstance() { StartingFrame = 11, EndingFrame = 21, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                        { AvatarState.Staring, new AnimationInstance() { StartingFrame = 00, EndingFrame = 10, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                    }
                },
                {
                    AvatarCategory.Flag,
                    new Dictionary<AvatarState, AnimationInstance>()
                    {
                        { AvatarState.Normal, new AnimationInstance() { StartingFrame = 11, EndingFrame = 21, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                        { AvatarState.Staring, new AnimationInstance() { StartingFrame = 00, EndingFrame = 10, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                    }
                },
                {
                    AvatarCategory.Goggles,
                    new Dictionary<AvatarState, AnimationInstance>()
                    {
                        { AvatarState.Normal, new AnimationInstance() { StartingFrame = 11, EndingFrame = 21, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                        { AvatarState.Staring, new AnimationInstance() { StartingFrame = 00, EndingFrame = 10, TimePerFrame = 1 / 10f, AnimationType = AnimationType.Cycle } },
                    }
                },
            };

        public AvatarMetadata Metadata { get; private set; }
        public Flipbook Flipbook { get; private set; }

        public Vector2 Position { get => Flipbook.Position; set => Flipbook.Position = value; }
        public float Rotation { get => Flipbook.Rotation; set => Flipbook.Rotation = value; }

        public Avatar(AvatarMetadata metadata, bool canUseDummy = false)
        {
            Metadata = metadata;

            string path;

            if (canUseDummy && metadata.ID == 0)
                path = "Misc/Dummy";
            else
                path = $"Graphics/Avatar/{metadata.Gender}/{metadata.AvatarCategory}/{metadata.Name}";

            //Extra animation logic
            AnimationInstance animationInstance;
            if (metadata.AvatarCategory == AvatarCategory.Extra || metadata.AvatarCategory == AvatarCategory.Misc)
                animationInstance = new AnimationInstance();
            else
                animationInstance = avatarState[metadata.AvatarCategory][AvatarState.Normal];

            Flipbook = new Flipbook(default, metadata.Pivot.ToVector2(),
                metadata.FrameDimensionX, metadata.FrameDimensionY, path,
                animationInstance, depthDictioary[metadata.AvatarCategory]);

            //Extra animation logic -> Use all frames possible on animation
            if (metadata.AvatarCategory == AvatarCategory.Extra)
                Flipbook.AppendAnimationIntoCycle(new AnimationInstance() { EndingFrame = Flipbook.Texture2D.Width / Flipbook.SpriteWidth - 1, TimePerFrame = 1 / 4f }, true);
            else if (metadata.AvatarCategory == AvatarCategory.Misc)
                Flipbook.AppendAnimationIntoCycle(new AnimationInstance() { EndingFrame = Flipbook.Texture2D.Width / Flipbook.SpriteWidth - 1, TimePerFrame = 1 / 4f, AnimationType = AnimationType.Cycle }, true);
        }

        public void Flip() => Flipbook.Flip();
        public void Hide() => Flipbook.SetTransparency(0);
        public void Show() => Flipbook.SetTransparency(1);

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => Flipbook.Draw(gameTime, spriteBatch);
    }
}
