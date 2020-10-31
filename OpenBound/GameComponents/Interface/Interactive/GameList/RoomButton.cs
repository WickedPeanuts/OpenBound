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
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Entity.Text;
using SharpDX.WIC;

namespace OpenBound.GameComponents.Interface.Interactive.GameList
{
    class RoomButton : Button
    {
        protected static Dictionary<PlayerStatus, Dictionary<GameMode, ButtonPreset>> RoomButtonPresets;

        private static void Initialize()
        {
            int assetWidth = 253;
            int assetHeight = 64;

            RoomButtonPresets = new Dictionary<PlayerStatus, Dictionary<GameMode, ButtonPreset>>();

            for (int userStatus = 0; userStatus < 4; userStatus++)
            {
                PlayerStatus roS = (PlayerStatus)userStatus;
                Dictionary<GameMode, ButtonPreset> buttonPreset = new Dictionary<GameMode, ButtonPreset>();
                RoomButtonPresets.Add(roS, buttonPreset);

                for (int gameMode = 0; gameMode < 4; gameMode++)
                {
                    Dictionary<ButtonAnimationState, Rectangle> statePreset = new Dictionary<ButtonAnimationState, Rectangle>();

                    for (int buttonAState = 0; buttonAState < 3; buttonAState++)
                    {
                        ButtonAnimationState bAS = (ButtonAnimationState)buttonAState;

                        Rectangle rect = new Rectangle(assetWidth * (3 * gameMode + buttonAState), assetHeight * userStatus, assetWidth, assetHeight);
                        statePreset.Add(bAS, rect);
                    }

                    buttonPreset.Add((GameMode)gameMode, new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Background",
                        StatePreset = statePreset
                    });
                }
            }
        }

        public static RoomButton CreateRoomButton(RoomMetadata roomMetadata, Vector2 buttonPosition, Action<object> action)
        {
            if (RoomButtonPresets == null)
                Initialize();

            return new RoomButton(roomMetadata, buttonPosition, action, RoomButtonPresets[roomMetadata.RoomOwner.PlayerStatus][roomMetadata.GameMode]);
        }

        CompositeSpriteText compositeSpriteText;
        List<Sprite> spriteList;
        List<NumericSpriteFont> spriteFontList;

        private RoomButton(RoomMetadata roomMetadata, Vector2 buttonPosition, Action<object> action, ButtonPreset buttonPreset)
            : base(ButtonType.RoomButton, DepthParameter.InterfaceButton, action, buttonPosition, buttonPreset)
        {
            spriteList = new List<Sprite>();
            spriteFontList = new List<NumericSpriteFont>();

            //Since the button elements dont update, the screencenter
            //must be added in order to create the right position
            //on the elements
            buttonPosition += Parameter.ScreenCenter;

            //MAP
            Vector2 mapCenter = buttonPosition + new Vector2(28, 11f);
            Sprite map = new Sprite($"Graphics/Maps/{roomMetadata.Map.GameMap}/GameListThumb{roomMetadata.Map.GameMapType}", mapCenter, DepthParameter.InterfaceButtonIcon);

            spriteList.Add(map);

            //RoomStatus
            int rectangleOffset = roomMetadata.IsPlaying ? 0 :
                (roomMetadata.IsFull ? 2 * 91 : 91);

            Sprite roomStatus = new Sprite("Interface/StaticButtons/GameList/Status",
                mapCenter - new Vector2(100, 0), DepthParameter.InterfaceButtonIcon,
                new Rectangle(rectangleOffset, 0, 91, 27))
            { Pivot = new Vector2(91 / 2, 27 / 2) };

            spriteList.Add(roomStatus);

            //RoomName
            SpriteText[] roomName = new SpriteText[2] {
                new SpriteText(FontTextType.Consolas11, string.Format("{0,3}", roomMetadata.ID) + " ",
                    Color.White, Alignment.Left, DepthParameter.InterfaceButtonText),
                new SpriteText(FontTextType.Consolas11, roomMetadata.Name,
                    Color.White, Alignment.Left, DepthParameter.InterfaceButtonText)
            };

            compositeSpriteText = CompositeSpriteText.CreateCompositeSpriteText(roomName.ToList(), Orientation.Horizontal, Alignment.Left, buttonPosition - new Vector2(115, 25), 0);

            //SpriteFont
            NumericSpriteFont nsf = new NumericSpriteFont(FontType.GameListPlayerCounter,
                1, DepthParameter.InterfaceButtonText,
                Position: buttonPosition + new Vector2(91, -24),
                StartingValue: roomMetadata.NumberOfPlayers,
                attachToCamera: false);
            spriteFontList.Add(nsf);

            NumericSpriteFont nsf2 = new NumericSpriteFont(FontType.GameListPlayerCounter,
                1, DepthParameter.InterfaceButtonText,
                Position: buttonPosition + new Vector2(105, -24),
                StartingValue: (int)roomMetadata.Size,
                attachToCamera: false);
            spriteFontList.Add(nsf2);

            //IsPasswordProtected
            if (roomMetadata.HasPassword)
            {
                Sprite passwordLock = new Sprite("Interface/StaticButtons/GameList/PasswordProtected",
                    mapCenter, DepthParameter.InterfaceButtonAnimatedIcon);
                spriteList.Add(passwordLock);
            }

            //HasBuddyInRoom
            if (HasBuddyInside(roomMetadata))
            {
                Sprite buddy = new Sprite("Interface/StaticButtons/GameList/BuddyInside",
                    mapCenter + new Vector2(32, 0), DepthParameter.InterfaceButtonAnimatedIcon);
                spriteList.Add(buddy);
            }
        }

        private bool HasBuddyInside(RoomMetadata matchMetadata)
        {
            List<Player> inGamePlayers = matchMetadata.TeamASafe.Union(matchMetadata.TeamBSafe).ToList();
            #warning needs refactoring due new friend list structure
            //Player match = GameInformation.Instance.PlayerInformation.BefriendedFrom?.First((x) => inGamePlayers.Contains(x));
            //return match != null;
            return false;
        }

        public override void Update()
        {
            base.Update();

            spriteFontList.ForEach((x) => x.Update(null));
        }

        public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            base.Draw(GameTime, SpriteBatch);

            spriteFontList.ForEach((x) => x.Draw(GameTime, SpriteBatch));
            compositeSpriteText.Draw(SpriteBatch);
            spriteList.ForEach((x) => x.Draw(GameTime, SpriteBatch));
        }
    }
}
