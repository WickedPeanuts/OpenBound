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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Audio;

namespace OpenBound.GameComponents.Level.Scene.Menu
{
    public class GameLogo : GameScene
    {
        public float sceneTimespan;
        public bool hasRequestedNextScene;

        public GameLogo()
        {
            //Camera.CameraOffset = new Vector2(Parameter.ScreenResolutionWidth / 2, Parameter.ScreenResolutionHeight / 2);
            Background = new Sprite(@"Interface/InGame/Scene/SplashScreen/SplashScreen",
                position: Parameter.ScreenCenter,
                layerDepth: DepthParameter.Background,
                shouldCopyAsset: false);

            sceneTimespan = 1f;
            hasRequestedNextScene = false;
        }

        public override void OnSceneIsActive()
        {
            base.OnSceneIsActive();

            AudioHandler.PlaySoundEffect(SoundEffectParameter.GameLogo);
        }

        public override void Update(GameTime GameTime)
        {
            base.Update(GameTime);

            sceneTimespan -= (float)GameTime.ElapsedGameTime.TotalSeconds;

            if (!hasRequestedNextScene && sceneTimespan < 0)
            {
                hasRequestedNextScene = true;
                SceneHandler.Instance.RequestSceneChange(SceneType.ServerSelection, TransitionEffectType.RotatingRectangles);
            }
        }
    }
}
