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

using OpenBound_Network_Object_Library.Common;
using System.Globalization;

namespace OpenBound.Common
{
    public class Language
    {
        //Interface Components - Game Modes
        public const string GameModeSolo  = "Solo";
        public const string GameModeScore = "Score";
        public const string GameModeTag   = "Tag";
        public const string GameModeJewel = "Jewel";

        //Interface - Messages
        public const string HUDTextAllText  = "All";
        public const string HUDTextTeamText = "Team";

        //Interface - Weather - Weather Names
        public const string WeatherForce       = "Force";
        public const string WeatherTornado     = "Tornado";
        public const string WeatherElectricity = "Electricity";
        public const string WeatherWeakness    = "Weakness";
        public const string WeatherMirror      = "Mirror";
        public const string WeatherRandom      = "Random";

        //Popup - Create Room
        public static string PopupCreateRoomOneVersusOneText     = "Room Size: 2 players, 1 on each side.";
        public static string PopupCreateRoomTwoVersusTwoText     = "Room Size: 4 players, 2 on each side.";
        public static string PopupCreateRoomThreeVersusThreeText = "Room Size: 6 players, 3 on each side.";
        public static string PopupCreateRoomFourVersusFourText   = "Room Size: 8 players, 4 on each side.";

        public static string PopupCreateRoomScoreText = "The first team to destroy the remaining\n" +
                                                        "number of vehicles shown on the scoreboard\n" +
                                                        "wins.";
        public static string PopupCreateRoomSoloText  = "The last team standing in the arena wins.";
        public static string PopupCreateRoomTagText   = "Each player has two vehicles, the second\n" +
                                                        "mobile have half of its original HP. The\n" +
                                                        "last team standing in the arena\n" +
                                                        "wins.";
        public static string PopupCreateRoomJewelText = "Each team must destroy the Jewels, little\n" +
                                                        "mechanical creatures, in order to claim it's\n" +
                                                        "points. On the end of the match\n" +
                                                        "the team with the biggest score\n" +
                                                        "wins.";

        public static string PopupCreateRoomTitle    = "Room Title: ";
        public static string PopupCreateRoomPassword = "Room Password: ";

        //Popup - Game Options

        public const string PopupGameOptionsGameplay                = "Gameplay";
        public const string PopupGameOptionsGameplayAimingMode      = "Aiming Mode";
        public const string PopupGameOptionsGameplayAimingModeDrag  = "Drag";
        public const string PopupGameOptionsGameplayAimingModeSlice = "Slice";

        public const string PopupGameOptionsGameplayScrollSpeed = "Scroll Speed";
        public const string PopupGameOptionsGameplayMouseSpeed  = "Mouse Speed";

        public const string PopupGameOptionsSound    = "Sound";
        public const string PopupGameOptionsSoundBGM = "BGM";
        public const string PopupGameOptionsSoundSFX = "SFX";

        public const string PopupGameOptionsMisc              = "Miscelaneous";
        public const string PopupGameOptionsMiscBackground    = "Background";
        public const string PopupGameOptionsMiscBackgroundOn  = "On";
        public const string PopupGameOptionsMiscBackgroundOff = "Off";

        public const string PopupGameOptionsMiscInterface          = "Interface";
        public const string PopupGameOptionsMiscInterfaceClassic   = "Classic";
        public const string PopupGameOptionsMiscInterfaceTHBlue    = "TH Blue";
        public const string PopupGameOptionsMiscInterfaceTHWhite   = "TH White";
        public const string PopupGameOptionsMiscInterfaceOpenBound = "OpenBound";

        //Popup - AlertMessage - Leave Game

        public const string PopupAlertMessageLeaveGameTitle     = "You want to close the game?";
        public const string PopupAlertMessageLeaveGameMessage11 = "Your current leave percentage is ";
        public static string PopupAlertMessageLeaveGameMessage12 => $"{string.Format(CultureInfo.InvariantCulture, "{0:0.00}", GameInformation.Instance.PlayerInformation.LeavePercentage)}";
        public static string PopupAlertMessageLeaveGameMessage13 =  " %";
        public const string  PopupAlertMessageLeaveGameMessage21 =  "If you leave this number is going to increase.\nPlayers with higher leave percentages are less likely\nto be trusted by others as a good player and you may\nalso not be able to play in some servers.\nYou still want to proceed?";

        public static string PopupCreateRoomNamePlaceholder => $"{GameInformation.Instance.PlayerInformation.Nickname}'s Room";

        //public static string GamemodeSolo = "Solo";

        //Popup - AlertMessage - Avatar Purchase Feedback
        public const string PopupAlertMessageAvatarPurchaseSuccessTitle     = "  Congratulations!  ";

        public const string PopupAlertMessageAvatarPurchaseSuccessMessage11 = "Your new aquisition [";
        public const string PopupAlertMessageAvatarPurchaseSuccessMessage12 = "]";
        public const string PopupAlertMessageAvatarPurchaseSuccessMessage21 = "Is already available at your inventory.\nIf you are unnable to see it then you should\nrefresh your search filters.";

        public const string PopupAlertMessageAvatarPurchaseFailureTitle     = "Something unexpected ocurred.";
        public const string PopupAlertMessageAvatarPurchaseFailureMessage11 = "For some odd reason the server\ncould not process your transaction\ntry again later and if the problem\npersists, contact the support.";

        //Popup - AlertMessage - Warning
        public const string PopupAlertMessageTitleWarning             = "Warning";
        public const string PopupAlertMessageTitleError               = "Unexpected Error ";
        public const string PopupAlertMessageServerDisconnection      = "You have been disconnected from the ";
        public const string PopupAlertMessageGameServerDisconnection  = "You are going to be redirected to server selection screen.";
        public const string PopupAlertMessageLobbyServerDisconnection = "You can not use features like buddy list or\nacess the server list screen";
        public const string PopupAlertMessageGameServer               = "Game Server.";
        public const string PopupAlertMessageLobbyServer              = "Lobby Server.";

        #region Button Texts
        //Animated Buttons
        //-- Main Menu - Various
        public const string ButtonTextExitText = "Exit";
        public const string ButtonTextBuddyList = "Buddy";
        public const string ButtonTextCashCharge = "Charge";
        public const string ButtonTextReportPlayer = "Report";
        public const string ButtonTextAvatarShop = "Avatar Shop";
        public const string ButtonTextOptions = "Options";
        public const string ButtonTextMyInfo = "My Info";
        public const string ButtonTextMuteList = "Mute List";

        //-- Room Selection - Room Filtering - By GameMode
        public const string ButtonTextRoomFilteringSolo = "Solo";
        public const string ButtonTextRoomFilteringTag = "Tag";
        public const string ButtonTextRoomFilteringScore = "Score";
        public const string ButtonTextRoomFilteringJewel = "Jewel";

        //-- Room Selection - Room Filtering - By Status
        public const string ButtonTextRoomFilteringViewAll = "All";
        public const string ButtonTextRoomFilteringViewWaiting = "Waiting";
        public const string ButtonTextRoomFilteringViewFriend = "Friend";

        //-- Room Selection - Room Options
        public const string ButtonTextGoTo = "Go To";
        public const string ButtonTextCreate = "Create";
        public const string ButtonTextQuickJoin = "Quick Join";

        //-- Room Selection - InRoom
        public const string ButtonTextInRoomReady = "Ready";
        public const string ButtonTextInRoomChangeMobile = "Mobile";
        public const string ButtonTextInRoomMatchSettings = "Settings";
        public const string ButtonTextInRoomChangeItem = "Item";
        public const string ButtonTextInRoomChangeTeam = "Team";

        //-- Avatar Shop - Avatar
        public const string ButtonTextAvatarShopTry = "Try";
        public const string ButtonTextAvatarShopBuy = "Buy";
        public const string ButtonTextAvatarShopGift = "Gift";

        //-- Avatar Shop - Filter
        public const string ButtonTextAvatarShopHat = "Hat";
        public const string ButtonTextAvatarShopBody = "Body";
        public const string ButtonTextAvatarShopGoggles = "Goggles";
        public const string ButtonTextAvatarShopFlag = "Flag";
        public const string ButtonTextAvatarShopExItem = "ExItem";
        public const string ButtonTextAvatarShopPet = "Pet";
        public const string ButtonTextAvatarShopNecklace = "Misc";
        public const string ButtonTextAvatarShopRing = "Extra";
        #endregion

        //--Avatar Shop - Popup - Buy
        public const string PopupTextConfirmPuchase = "Confirm puchase with";
        public const string PopupTextBalance        = "Balance";
        public const string PopupTextCurrencyGold   = "Gold";
        public const string PopupTextCurrencyCash   = "Cash";

        //--Avatar Shop
        public const string AvatarShopTabShop      = "Shop";
        public const string AvatarShopTabInventory = "Inventory";


        public const string IconSubtitleOffline          = "Offline";
        public const string IconSubtitleLowPopulation    = "Low";
        public const string IconSubtitleMediumPopulation = "Medium";
        public const string IconSubtitleHighPopulation   = "High";
        public const string IconSubtitleFull             = "Full";

        public const string LoadingScreenMinimapCommand = "Press Up/Down to control the tactical map";

        public const string PreviewTextAvatarShop         = "Room Preview";
        public const string PreviewTextAvatarShopEquipped = "Room Preview - Equipped";

        public const string InGamePreviewTextAvatarShop         = "In-Game Preview";
        public const string InGamePreviewTextAvatarShopEquipped = "In-Game Preview - Equipped";


        //--Popup - Select Item
        public const string InGameItemTab1 = "2 Slots";
        public const string InGameItemTab2 = "1 Slot";

        //--Popup - Select Item - Item Names
        public const string InGameItemDual         = "Dual";
        public const string InGameItemDualPlus     = "Dual+";
        public const string InGameItemThunder      = "Thunder";
        public const string InGameItemEnergyUp2    = "Energy Up 2";
        public const string InGameItemTeamTeleport = "Team Teleport";
        public const string InGameItemTeleport     = "Teleport";

        public const string InGameItemBlood        = "Blood";
        public const string InGameItemBungeShot    = "Bunge Shot";
        public const string InGameItemPowerUp      = "Power Up";
        public const string InGameItemChangeWind   = "Change Wind";
        public const string InGameItemEnergyUp1    = "Energy Up 1";

        //--Popup - Select Item - Description

        public const  string InGameItemName      = "No item selected";
        public const  string InGameItemDelayCost = "Delay Cost:";
        public const  string InGameItemCost      = "---";

        public const  string InGameItemDualDescription         = "A mobile shoots two of the same primary projectiles.\nYou will not be able to use SS this turn. Effects\nare dismissed when the turn is over.";
        public const  string InGameItemDualPlusDescription     = "A mobile shoots two different primary projectiles,\nthe first is always the one you have selected. You\nwill not be able to use SS this turn. Effects are\ndismissed when the turn is over.";
        public const  string InGameItemThunderDescription      = "Embodies thunder property on each fired shot. Does\nnot stack with weather's thunder. You will not be\nable to use SS this turn. Effects are dismissed\nwhen the turn is over.";
        public static string InGameItemEnergyUp2Description    = $"Recovers {NetworkObjectParameters.InGameItemEnergyUp2Value}% of the mobile's maximum energy. Does\nnot replenish shield. Bionic mobiles receive an\nextra {NetworkObjectParameters.InGameItemEnergyUp2ExtraValue}% of health at no extra cost.";
        public const  string InGameItemTeamTeleportDescription = "Switches position with the most wounded teammate\n(Remaining energy), excluding yourself. Shields are\nnot accounted for.";
        public const  string InGameItemTeleportDescription     = "Turns the projectile into a Teleportation beacon.\nThe mobile is teleported to the position where the\nbeacon has hit the floor. Effects are dismissed\nwhen the turn is over.";

        public static string InGameItemBloodDescription        = $"Exchanges {NetworkObjectParameters.InGameItemBloodExtraValue}% of mobile's energy for an extra {NetworkObjectParameters.InGameItemBloodValue}%\nprojectile power. This item can not set user's\nenergy to 0. You will not be able to use SS this\nturn. Effects are dismissed when the turn is over.";
        public static string InGameItemBungeShotDescription    = $"Increases the explosion's area of influence by {NetworkObjectParameters.InGameItemBungeShotValue}%.\nNot increasing damage but destroying more land.";
        public static string InGameItemPowerUpDescription      = $"Increases a projectile destructible power. Adds {NetworkObjectParameters.InGameItemPowerUpValue}%\nmore damage into the next projectile.";
        public const  string InGameItemChangeWindDescription   = "Instantly changes the wind to it's opposite\ndirection.";
        public static string InGameItemEnergyUp1Description    = $"Recovers {NetworkObjectParameters.InGameItemEnergyUp1Value}% of the mobile's maximum energy. Does\nnot replenish shield. Bionic mobiles receive an\nextra {NetworkObjectParameters.InGameItemEnergyUp1ExtraValue}% of health at no extra cost.";
    }
}
