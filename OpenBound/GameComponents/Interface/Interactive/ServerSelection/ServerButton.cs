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
using OpenBound.GameComponents.Interface.Builder;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Interface.Interactive.ServerSelection
{
    public enum ServerStatus
    {
        Offline,
        LowPopulation,
        MediumPopulation,
        HighPopulation,
        Full,
    }

    public struct ServerStatusFlipbookIcon
    {
        public AnimationInstance FlipbookInstance;
        public AnimationType AnimationType;
        public string IconSubtitle;
    }

    public class ServerButton : Button
    {
        public static Dictionary<ServerStatus, ServerStatusFlipbookIcon> ServerStatusFlipbookIconPresets = new Dictionary<ServerStatus, ServerStatusFlipbookIcon>()
        {
            { ServerStatus.Offline,          new ServerStatusFlipbookIcon(){ IconSubtitle = Language.IconSubtitleOffline,          FlipbookInstance = new AnimationInstance() { StartingFrame = 0,   EndingFrame = 70,  TimePerFrame = 1/20f }, AnimationType = AnimationType.Foward } },
            { ServerStatus.LowPopulation,    new ServerStatusFlipbookIcon(){ IconSubtitle = Language.IconSubtitleLowPopulation,    FlipbookInstance = new AnimationInstance() { StartingFrame = 71,  EndingFrame = 125, TimePerFrame = 1/20f }, AnimationType = AnimationType.Cycle  } },
            { ServerStatus.MediumPopulation, new ServerStatusFlipbookIcon(){ IconSubtitle = Language.IconSubtitleMediumPopulation, FlipbookInstance = new AnimationInstance() { StartingFrame = 126, EndingFrame = 166, TimePerFrame = 1/20f }, AnimationType = AnimationType.Foward } },
            { ServerStatus.HighPopulation,   new ServerStatusFlipbookIcon(){ IconSubtitle = Language.IconSubtitleHighPopulation,   FlipbookInstance = new AnimationInstance() { StartingFrame = 167, EndingFrame = 232, TimePerFrame = 1/20f }, AnimationType = AnimationType.Foward } },
            { ServerStatus.Full,             new ServerStatusFlipbookIcon(){ IconSubtitle = Language.IconSubtitleFull,             FlipbookInstance = new AnimationInstance() { StartingFrame = 233, EndingFrame = 273, TimePerFrame = 1/20f }, AnimationType = AnimationType.Cycle  } },
        };

        List<CompositeSpriteText> compositeSpriteTextList;
        List<SpriteText> spriteTextList;
        List<Sprite> spriteList;
        List<Flipbook> flipbookList;
        public GameServerInformation ServerInformation { get; private set; }

        public ServerButton(GameServerInformation serverInformation, Vector2 buttonPosition, Action<object> action)
            : base(ButtonType.ServerListButton, DepthParameter.InterfaceButton, action, buttonPosition, default)
        {

            //Since the button elements dont update, the screencenter
            //must be added in order to create the right position
            //on the elements
            buttonPosition += Parameter.ScreenCenter;

            //Initializing Variables
            compositeSpriteTextList = new List<CompositeSpriteText>();
            spriteList = new List<Sprite>();
            flipbookList = new List<Flipbook>();
            spriteTextList = new List<SpriteText>();
            ServerInformation = serverInformation;

            //Server Background
            if (!serverInformation.IsOnline)
                ChangeButtonState(ButtonAnimationState.Disabled);

            //Server Level Indication Icons
            int lowerLevel = (int)serverInformation.LowerLevel;
            int higherLevel = (int)serverInformation.HigherLevel;

            Sprite lowerLevelIcon = IconBuilder.Instance.BuildServerIcon((ServerRankLimitationIcon)(lowerLevel * 2), DepthParameter.InterfaceButtonText);
            Sprite higherLevelIcon = IconBuilder.Instance.BuildServerIcon((ServerRankLimitationIcon)(higherLevel * 2 + 1), DepthParameter.InterfaceButtonText);
            Sprite avatarIcon = IconBuilder.Instance.BuildServerIcon((serverInformation.IsAvatarOn ? ServerRankLimitationIcon.AvatarOn : ServerRankLimitationIcon.AvatarOff), DepthParameter.InterfaceButtonText);

            lowerLevelIcon.Position = buttonPosition + new Vector2(40, -24);
            higherLevelIcon.Position = buttonPosition + new Vector2(68, -24);
            avatarIcon.Position = buttonPosition + new Vector2(96, -24);

            spriteList.Add(lowerLevelIcon);
            spriteList.Add(higherLevelIcon);
            spriteList.Add(avatarIcon);

            //Server Load Icon
            ServerStatusFlipbookIcon serverLoad = GetCurrentServerLoadIcon(serverInformation);
            Flipbook icon = new Flipbook(
                buttonPosition + new Vector2(130, 12), new Vector2(31, 37), 62, 58,
                "Interface/InGame/Scene/ServerList/ServerStatusAnimatedIcon", serverLoad.FlipbookInstance, DepthParameter.InterfaceButtonAnimatedIcon);

            flipbookList.Add(icon);

            //Server Button Texts
            SpriteText[] sprT = new SpriteText[2];
            sprT[0] = new SpriteText(Parameter.ServerButtonFont, $"{serverInformation.ServerID}. ", Color.DarkOrange,
                Alignment.Left, DepthParameter.InterfaceButtonText, outlineColor: Color.Black);

            sprT[1] = new SpriteText(Parameter.ServerButtonFont, serverInformation.ServerName, Color.White,
                Alignment.Left, DepthParameter.InterfaceButtonText, outlineColor: Color.Black);

            compositeSpriteTextList.Add(
                CompositeSpriteText.CreateCompositeSpriteText(sprT.ToList(), Orientation.Horizontal, Alignment.Left, buttonPosition + new Vector2(-155, -32), 0));

            sprT = new SpriteText[serverInformation.ServerDescription.Count()];
            for (int i = 0; i < serverInformation.ServerDescription.Count(); i++)
            {
                sprT[i] = new SpriteText(Parameter.ServerButtonFont, serverInformation.ServerDescription[i], Color.White,
                    Alignment.Left, DepthParameter.InterfaceButtonText, outlineColor: Color.Black);
            }

            compositeSpriteTextList.Add(CompositeSpriteText.CreateCompositeSpriteText(sprT.ToList(), Orientation.Vertical, Alignment.Left, buttonPosition + new Vector2(-155, -10), -1));

            if (serverInformation.IsOnline)
                spriteTextList.Add(new SpriteText(Parameter.ServerButtonFont, $"[{serverInformation.ConnectedClients}/{serverInformation.ConnectedClientCapacity}]", Color.White,
                    Alignment.Center, DepthParameter.InterfaceButtonText, buttonPosition + new Vector2(68, -10), Color.Black));

            //Server Icon Text
            spriteTextList.Add(new SpriteText(Parameter.ServerButtonFont,
                serverLoad.IconSubtitle, Color.White, Alignment.Center,
                DepthParameter.InterfaceButtonText, icon.Position + new Vector2(0, 10), Color.Black));

            OnBeingPressed = (sender) =>
            {
                spriteTextList.ForEach((x) => x.Position += new Vector2(1, 1));
                spriteList.ForEach((x) => x.Position += new Vector2(1, 1));
                flipbookList.ForEach((x) => x.Position += new Vector2(1, 1));
            };

            OnBeingReleased = (sender) =>
            {
                spriteTextList.ForEach((x) => x.Position -= new Vector2(1, 1));
                spriteList.ForEach((x) => x.Position -= new Vector2(1, 1));
                flipbookList.ForEach((x) => x.Position -= new Vector2(1, 1));
            };
        }

        public static ServerStatusFlipbookIcon GetCurrentServerLoadIcon(GameServerInformation serverInformation)
        {
            if (!serverInformation.IsOnline)
                return ServerStatusFlipbookIconPresets[ServerStatus.Offline];

            float serverLoad = serverInformation.ConnectedClients / (float)serverInformation.ConnectedClientCapacity;

            if (serverLoad < 0.2f)
                return ServerStatusFlipbookIconPresets[ServerStatus.LowPopulation];
            else if (serverLoad < 0.5f)
                return ServerStatusFlipbookIconPresets[ServerStatus.MediumPopulation];
            else if (serverLoad < 0.7f)
                return ServerStatusFlipbookIconPresets[ServerStatus.HighPopulation];
            else
                return ServerStatusFlipbookIconPresets[ServerStatus.Full];
        }

        public override void Enable()
        {
            if (ServerInformation.IsOnline)
                base.Enable();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            spriteTextList.ForEach((x) => x.Draw(spriteBatch));
            flipbookList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            compositeSpriteTextList.ForEach((x) => x.Draw(spriteBatch));
        }
    }
}
