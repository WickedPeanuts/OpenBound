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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound.GameComponents.Interface.Interactive.AvatarShop
{
    public class InGamePreview
    {
        Sprite background, foreground;
        Mobile mobile;

        public InGamePreview(Vector2 position)
        {
            background = new Sprite("Interface/InGame/Scene/AvatarShop/InGamePreview/MiramoTown/Background", position, DepthParameter.Background);
            foreground = new Sprite("Interface/InGame/Scene/AvatarShop/InGamePreview/MiramoTown/Foreground2", position, DepthParameter.BackgroundAnim);

            mobile = new RaonLauncher(GameInformation.Instance.PlayerInformation, position);
            mobile.Crosshair.HideElement();
            mobile.HideLobbyExclusiveAvatars();
            mobile.Flip();
        }

        public void Hide()
        {
            mobile.HideElement();
            mobile.Rider.Hide();
        }

        public void Show()
        {
            mobile.ShowElement();
            mobile.Rider.Show();
            mobile.HideLobbyExclusiveAvatars();
        }

        public void ReplaceAvatar(AvatarMetadata avatarMetadata)
        {
            mobile.Rider.ReplaceAvatar(avatarMetadata);
        }

        public void Update()
        {
            mobile.Rider.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            background.Draw(null, spriteBatch);
            foreground.Draw(null, spriteBatch);
            mobile.Draw(gameTime, spriteBatch);
        }
    }
}
