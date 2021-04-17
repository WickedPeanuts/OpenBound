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
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Text;

namespace OpenBound.GameComponents.Interface.Popup
{
    public enum AlertMessageType
    {
        AcceptCancel,
        Cancel,
    }

    public class PopupAlertMessage : PopupMenu
    {
        public PopupAlertMessage(CompositeSpriteText title, CompositeSpriteText message, AlertMessageType alertMessageType = AlertMessageType.AcceptCancel) : base(true)
        {
            Background = new Sprite("Interface/Popup/Blue/Alert/Background", layerDepth: DepthParameter.InterfacePopupMessageBackground);

            compositeSpriteTextList.Add(title);
            compositeSpriteTextList.Add(message);

            title.PositionOffset = Background.Position - new Vector2(186, 50);
            message.PositionOffset = Background.Position - new Vector2(186, 50 - title.ElementDimensions.Y - 5);

            buttonList.Add(new Button(ButtonType.Cancel, DepthParameter.InterfacePopupMessageButtons, CloseAction, PositionOffset + new Vector2(160, 65)));
            
            if (alertMessageType != AlertMessageType.Cancel)
                buttonList.Add(new Button(ButtonType.Accept, DepthParameter.InterfacePopupMessageButtons, (sender) => { OnConfirm?.Invoke(sender); }, PositionOffset + new Vector2(125, 65)));

            ShouldRender = true;

            UpdateAttatchmentPosition();
        }

        protected override void CloseAction(object sender)
        {
            OnClose?.Invoke(sender);
            Destroy();
        }

        public override void RealocateElements(Vector2 delta)
        {
            delta = delta.ToIntegerDomain();

            base.RealocateElements(delta);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateAttatchmentPosition();
        }
    }
}
