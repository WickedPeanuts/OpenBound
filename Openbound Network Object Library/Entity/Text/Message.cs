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
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.Models;
using System.Collections.Generic;

namespace OpenBound_Network_Object_Library.Entity.Text
{
    #region Message Types
    /// <summary>
    /// This kind of messages are always interpreted by the game client in the following pattern:
    /// [<see cref="Player.Nickname"/>]: Text. All text colors are placed automatically,
    /// the nickname color is set by <see cref="Message.TextToColor"/>. If the PlayerTeam is set
    /// the text is painted as accordingly. If not, the default color is white.
    /// </summary>
    public class PlayerMessage
    {
        public Player Player;
        public string Text;
        public PlayerTeam? PlayerTeam;
    }

    /// <summary>
    /// There is no default interpretation for this kind of message. Each line MUST be composed
    /// by a list of custom messages. This element does not support line breaks and all texts
    /// must be preciselly calculated since it overflows the textbox. Check
    /// <see cref="Message.RoomWelcomeMessage"/> for a multi-line example with combined characters
    /// of font-awesome tokens and consolas texts.
    /// </summary>
    public class CustomMessage
    {
        public string Text;
        public uint Token;
        public uint TextColor, TextBorderColor;
        public FontTextType FontTextType;

        public void AppendTokenToText() { Text += (char)Token; }
    }
    #endregion

    public class Message
    {
        #region TextBuilders
        public static string BuildGameServerChatGameList(int id) => NetworkObjectParameters.GameServerChatGameListIdentifier + id.ToString();
        public static string BuildGameServerChatGameRoom(int id) => NetworkObjectParameters.GameServerChatGameRoomIdentifier + id.ToString();

        private static CustomMessage CreateFontAwesomeText(uint token) => new CustomMessage() { Token = token, TextColor = NetworkObjectParameters.ServerMessageColor, TextBorderColor = NetworkObjectParameters.ServerMessageBorderColor, FontTextType = FontTextType.FontAwesome10 };
        private static CustomMessage CreateConsolasServerMessageText(string text) => new CustomMessage() { Text = text, TextColor = NetworkObjectParameters.ServerMessageColor, TextBorderColor = NetworkObjectParameters.ServerMessageBorderColor, FontTextType = FontTextType.Consolas10 };
        private static CustomMessage CreateConsolasGoldBonusMessageText(string text) => new CustomMessage() { Text = text, TextColor = NetworkObjectParameters.GoldBonusMessageColor, TextBorderColor = NetworkObjectParameters.ServerMessageBorderColor, FontTextType = FontTextType.Consolas10 };
        private static CustomMessage CreateConsolasGoldDecreaseMessageText(string text) => new CustomMessage() { Text = text, TextColor = NetworkObjectParameters.GoldDecreaseMessageColor, TextBorderColor = NetworkObjectParameters.ServerMessageBorderColor, FontTextType = FontTextType.Consolas10 };
        private static CustomMessage CreatePlayerColoredText(string text) => new CustomMessage() { Text = text, TextColor = TextToColor(text).PackedValue, TextBorderColor = Color.Black.PackedValue, FontTextType = FontTextType.Consolas10 };
        #endregion

        #region Font Awesome Texts (Tokens)
        private static CustomMessage FAComputerToken = CreateFontAwesomeText(FontAwesomeIconIndex.Desktop);
        private static CustomMessage FAServerToken = CreateFontAwesomeText(FontAwesomeIconIndex.Server);
        private static CustomMessage FAGamepadToken = CreateFontAwesomeText(FontAwesomeIconIndex.Gamepad);
        private static CustomMessage FASadCry = CreateFontAwesomeText(FontAwesomeIconIndex.SadCry);
        private static CustomMessage FAHeartBroken = CreateFontAwesomeText(FontAwesomeIconIndex.Heart_Broken);
        private static CustomMessage FACoins = CreateFontAwesomeText(FontAwesomeIconIndex.Coins);
        #endregion

        #region Consolas Texts
        private static CustomMessage CSpace = CreateConsolasServerMessageText(" ");

        //Welcome
        //- Channel
        private static CustomMessage CChannelWelcome1 = CreateConsolasServerMessageText(Language.ChannelWelcomeMessage1);
        private static CustomMessage CChannelWelcome2 = CreateConsolasServerMessageText(" " + NetworkObjectParameters.GameServerInformation?.ServerName + Language.ChannelWelcomeMessage2);

        //- Room
        private static CustomMessage CRoomWelcome1  = CreateConsolasServerMessageText(Language.RoomWelcomeMessage1);
        private static CustomMessage CRoomWelcome1E = CreateConsolasServerMessageText(").");
        private static CustomMessage CRoomWelcome2  = CreateConsolasServerMessageText(Language.RoomWelcomeMessage2);

        //In Game Messages
        //- Gold-Related
        private static CustomMessage IGGBMultipleKill     = CreateConsolasGoldBonusMessageText(Language.GoldRewardMultipleKill);
        private static CustomMessage IGGBEnemyKill        = CreateConsolasGoldBonusMessageText(Language.GoldRewardSingleKill);
        private static CustomMessage IGGBBungeeKill       = CreateConsolasGoldBonusMessageText(Language.GoldRewardBungeeKill);
        private static CustomMessage IGGBDExcelentShot    = CreateConsolasGoldBonusMessageText(Language.GoldRewardD500Damage);
        private static CustomMessage IGGBDGoodShot        = CreateConsolasGoldBonusMessageText(Language.GoldRewardD250Damage);
        private static CustomMessage IGGBDSimpleShot      = CreateConsolasGoldBonusMessageText(Language.GoldRewardD150Damage);
        private static CustomMessage IGGBTornadoShot      = CreateConsolasGoldBonusMessageText(Language.GoldRewardTornadoShot);
        private static CustomMessage IGGBMirrorShot       = CreateConsolasGoldBonusMessageText(Language.GoldRewardMirrorShot);
        private static CustomMessage IGGBHighAngle        = CreateConsolasGoldBonusMessageText(Language.GoldRewardHighAngle);
        private static CustomMessage IGGBUHighAngle       = CreateConsolasGoldBonusMessageText(Language.GoldRewardUHighAngle);
        private static CustomMessage IGGBGoldBoomerShot   = CreateConsolasGoldBonusMessageText(Language.GoldRewardGoldBoomerShot);
        private static CustomMessage IGGBBackShot         = CreateConsolasGoldBonusMessageText(Language.GoldRewardBackShot);
        private static CustomMessage IGGB3000Damage       = CreateConsolasGoldBonusMessageText(Language.GoldReward3000Damage);
        private static CustomMessage IGGB2000Damage       = CreateConsolasGoldBonusMessageText(Language.GoldReward2000Damage);
        private static CustomMessage IGGB1000Damage       = CreateConsolasGoldBonusMessageText(Language.GoldReward1000Damage);
        private static CustomMessage IGGDDecreaseAllyKill = CreateConsolasGoldDecreaseMessageText(Language.GoldDecreaseAllyKill);
        private static CustomMessage IGGDSuicide          = CreateConsolasGoldDecreaseMessageText(Language.GoldDecreaseSuicide);
        private static CustomMessage IGGDFriendlyFire     = CreateConsolasGoldDecreaseMessageText(Language.GoldDecreaseFriendlyFire);

        private static CustomMessage IGGBWin4v4Match      = CreateConsolasGoldBonusMessageText(Language.GoldRewardWin4v4Match);
        private static CustomMessage IGGBWin3v3Match      = CreateConsolasGoldBonusMessageText(Language.GoldRewardWin3v3Match);
        private static CustomMessage IGGBWin2v2Match      = CreateConsolasGoldBonusMessageText(Language.GoldRewardWin2v2Match);
        private static CustomMessage IGGBWin1v1Match      = CreateConsolasGoldBonusMessageText(Language.GoldRewardWin1v1Match);

        private static CustomMessage IGGBLose4v4Match     = CreateConsolasGoldBonusMessageText(Language.GoldRewardLose4v4Match);
        private static CustomMessage IGGBLose3v3Match     = CreateConsolasGoldBonusMessageText(Language.GoldRewardLose3v3Match);
        private static CustomMessage IGGBLose2v2Match     = CreateConsolasGoldBonusMessageText(Language.GoldRewardLose2v2Match);
        private static CustomMessage IGGBLose1v1Match     = CreateConsolasGoldBonusMessageText(Language.GoldRewardLose1v1Match);

        private static CustomMessage IGGBPopularity       = CreateConsolasGoldBonusMessageText(Language.GoldRewardPopularity);
        private static CustomMessage IGGDPopularity       = CreateConsolasGoldDecreaseMessageText(Language.GoldDecreasePopularity);


        //- Death
        private static CustomMessage IGMDeath1 = CreateConsolasServerMessageText(Language.DeathMessage1);
        #endregion

        #region 'Pre-baked' Messages
        //WelcomeMessage
        //- Channel
        private static List<CustomMessage> ChannelWelcomeMessage = new List<CustomMessage>() { FAComputerToken, CChannelWelcome1, FAServerToken, CChannelWelcome2 };

        //- Room
        private static List<List<CustomMessage>> RoomWelcomeMessage = new List<List<CustomMessage>>() {
            new List<CustomMessage>() { FAComputerToken, CRoomWelcome1, FAGamepadToken, CSpace, CRoomWelcome1E },
            new List<CustomMessage>() { FAComputerToken, CRoomWelcome2 } };

        //In Game Messages
        //- Death
        private static List<CustomMessage> DeathMessage = new List<CustomMessage>() { FASadCry, CSpace, IGMDeath1, FAHeartBroken };
        #endregion

        #region Customized 'Pre-baked' Message
        //Welcome
        //- Channel
        public static List<CustomMessage> CreateChannelWelcomeMessage(int channel) =>
            new List<CustomMessage>(ChannelWelcomeMessage) { CreateConsolasServerMessageText(channel + "") };

        //- Room
        public static List<List<CustomMessage>> CreateRoomWelcomeMessage(string roomName)
        {
            List<List<CustomMessage>> cmL = new List<List<CustomMessage>>()
            {
                new List<CustomMessage>(RoomWelcomeMessage[0]),
                new List<CustomMessage>(RoomWelcomeMessage[1]),
            };

            cmL[0].Insert(4, CreateConsolasServerMessageText(roomName));
            return cmL;
        }

        //In Game Messages
        //- Gold-Related
        public static List<CustomMessage> CreateIGGBMultipleKillMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBMultipleKill,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldMultipleKill))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBEnemyKillMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBEnemyKill,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldEnemyKill))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBBungeeKillMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBBungeeKill,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldBungeeShot))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBDExcellentShotMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBDExcelentShot,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldExcellentShot))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBDGoodShotMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBDGoodShot,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldGoodShot))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBDSimpleShotBonusMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBDSimpleShot,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldSimpleShot))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBTornadoShotMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBTornadoShot,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldWeatherShot))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBMirrorShotMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBMirrorShot,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldWeatherShot))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBHighAngleMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBHighAngle,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldHighAngle))
            };

            return cmL;
        }
        
        public static List<CustomMessage> CreateIGGBUHighAngleMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBUHighAngle,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldUHighAngle))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBGoldBoomerShotMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBGoldBoomerShot,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldBoomerShot))
            };

            return cmL;
        }
       
        public static List<CustomMessage> CreateIGGBBackShotMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBBackShot,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldBackShot))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGB3000DamageMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGB3000Damage,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.Gold3000Damage))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGB2000DamageMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGB2000Damage,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.Gold2000Damage))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGB1000DamageMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGB1000Damage,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.Gold1000Damage))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGDDecreaseAllyKillMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGDDecreaseAllyKill,
                CreateConsolasGoldDecreaseMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldAllyKill * -1))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGDDSuicideMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGDSuicide,
                CreateConsolasGoldDecreaseMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldAllyKill * -1))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGDFriendlyFireMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGDFriendlyFire,
                CreateConsolasGoldDecreaseMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldFriendlyFire * -1))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBWin4v4MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBWin4v4Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldWin4v4Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBWin3v3MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBWin3v3Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldWin3v3Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBWin2v2MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBWin2v2Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldWin2v2Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBWin1v1MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBWin1v1Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldWin1v1Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBLose4v4MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBLose4v4Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldLose4v4Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBLose3v3MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBLose3v3Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldLose3v3Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBLose2v2MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBLose2v2Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldLose2v2Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGGBLose1v1MatchMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>()
            {
                CreatePlayerColoredText(owner.Nickname),
                IGGBLose1v1Match,
                CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, NetworkObjectParameters.GoldLose1v1Match))
            };

            return cmL;
        }

        public static List<CustomMessage> CreateIGPopularityMessage(Player owner, int value)
        {
            List<CustomMessage> cmL = new List<CustomMessage>() { CreatePlayerColoredText(owner.Nickname) };

            if (value > 0)
            {
                cmL.Add(IGGBPopularity);
                cmL.Add(CreateConsolasGoldBonusMessageText(string.Format(NetworkObjectParameters.GoldTextMask, value)));
            }
            else
            {
                cmL.Add(IGGDPopularity);
                cmL.Add(CreateConsolasGoldDecreaseMessageText(string.Format(NetworkObjectParameters.GoldTextMask, value * -1)));
            }
            
            return cmL;
        }

        // private static CustomMessage IGGBPestrigy         = CreateConsolasGoldBonusMessageText(Language.GoldRewardPrestigy);
        // private static CustomMessage IGGDPrestigy

        //- Death
        public static List<CustomMessage> CreateDeathMessage(Player owner)
        {
            List<CustomMessage> cmL = new List<CustomMessage>(DeathMessage);
            cmL.Insert(2, CreatePlayerColoredText(owner.Nickname));
            return cmL;
        }

        #endregion

        public static Color TextToColor(string text)
        {
            uint color = 0x0;

            for (int i = 0; i < text.Length - 1; i++)
            {
                color += text[i];
                color = color.RotateLeft(4);
            }

            //Removing the first 8 bits (alpha)
            color /= 256;

            Color c = new Color();
            c.A = 255;

            c.R = (byte)(color >> 00 & 255).Normalize(0, 255, 50, 255);
            c.G = (byte)(color >> 08 & 255).Normalize(0, 255, 50, 255);
            c.B = (byte)(color >> 16 & 255).Normalize(0, 255, 50, 255);

            return c;
        }
    }
}
