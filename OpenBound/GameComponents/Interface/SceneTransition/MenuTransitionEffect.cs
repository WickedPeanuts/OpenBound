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
using OpenBound.GameComponents.Animation;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.SceneTransition
{
    public enum MenuAnimationState
    {
        Initialized, Closing, Closed, ExecutingClosedOperation, Opening, Finalized
    }

    public abstract class MenuTransitionEffect
    {
        //Animation
        protected MenuAnimationState state;

        protected float initialFreezetime, motionTime;
        protected float elapsedTime, totalElapsedTime;

        protected Action transitioningAction;

        protected List<Sprite> spriteList;

        public bool IsAnimationOver { get { return state == MenuAnimationState.Finalized; } }

        protected MenuTransitionEffect(float initialFreezetime, float motionTime, Action transitioningAction)
        {
            this.initialFreezetime = initialFreezetime;
            this.motionTime = motionTime;
            this.transitioningAction = transitioningAction;

            spriteList = new List<Sprite>();
        }

        public abstract void Update(GameTime gameTime);

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
