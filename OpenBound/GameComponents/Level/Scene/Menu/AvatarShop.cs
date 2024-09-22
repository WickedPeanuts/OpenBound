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
using OpenBound.GameComponents.Asset;
using OpenBound.GameComponents.Interface.Builder;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Interactive.AvatarShop;
using OpenBound.GameComponents.Interface.Popup;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Pawn;
using OpenBound.ServerCommunication;
using OpenBound.ServerCommunication.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Language = OpenBound.Common.Language;

namespace OpenBound.GameComponents.Level.Scene.Menu
{
    /// <summary>
    /// Object meant for controlling the avatar shop filtering routines
    /// </summary>
    public class AvatarSearchFilter
    {
        public AvatarCategory AvatarCategory;
        public string AvatarName;
        public int CurrentPage;
        public int LastPage;
        public bool IsRenderingInventory;

        public AvatarSearchFilter() { AvatarName = ""; }
    }

    public class AvatarShop : GameScene
    {
        // Interface animated buttons
        private List<AnimatedButton> animatedButtonList;
        private AnimatedButton cashChargeButton;

        // Sprites
        private List<Sprite> spriteList;
        private Sprite foreground1, foreground2;

        // List of avatar buttons
        public List<AvatarButton> avatarButtonList;

        // List of regular buttons (currently only the tab buttons)
        private List<Button> buttonList;

        private TabButtonList tabButtonList;

        // All texts that are being used on the scene, except the ones
        // wrapped on other objects
        private List<SpriteText> spriteTextList;
        private SpriteText avatarPreviewSpriteText;
        private SpriteText inGamePreviewSpriteText;
        private SpriteText filterCurrentPage, filterLastPage, filterDivider;

        //Preview
        private Rider shopRiderPreview, inventoryRiderPreview;

        //In-Game Preview
        private InGamePreview shopInGamePreview, inventoryInGamePreview;

        private AttributeMenu attributeMenu;
        public AvatarMetadata selectedAvatarMetadata;

        // Filter variables
        private TextField searchTextField;

        private List<AnimatedButton> filterButtonList;
        private AnimatedButton filterHatButton, filterBodyButton, filterGogglesButton, filterFlagButton;
        private AnimatedButton filterExItemButton, filterPetButton, filterMiscButton, filterExtraButton;

        // Footer buttons
        private AnimatedButton tryButton, buyButton, giftButton;

        // Filtering options
        private AvatarSearchFilter searchFilter;
        private AnimatedButton filterLeftButton, filterRightButton;

        // Popups
        private PopupBuyAvatar popupBuyAvatar;

        // Padlocks for asynchronous operations controls
        private object asyncPadlock;

        // Object references
        private readonly Player player;

        public AvatarShop()
        {
            asyncPadlock = new object();
            animatedButtonList = new List<AnimatedButton>();
            filterButtonList = new List<AnimatedButton>();
            spriteList = new List<Sprite>();
            avatarButtonList = new List<AvatarButton>();
            spriteTextList = new List<SpriteText>();
            buttonList = new List<Button>();

            player = GameInformation.Instance.PlayerInformation;

            //Background / Foreground
            Background = new Sprite(@"Interface/InGame/Scene/AvatarShop/Background",
                position: Parameter.ScreenCenter,
                layerDepth: DepthParameter.Foreground,
                shouldCopyAsset: false);

            foreground1 = new Sprite("Interface/InGame/Scene/AvatarShop/Foreground1", Vector2.Zero, DepthParameter.Foreground);
            foreground2 = new Sprite("Interface/InGame/Scene/AvatarShop/Foreground2", Vector2.Zero, DepthParameter.Foreground);
            foreground2.Color = Color.Transparent;
            foreground1.Pivot = foreground2.Pivot = Vector2.Zero;
            foreground1.Position = foreground2.Position = Parameter.ScreenCenter - Background.Pivot;

            spriteList.Add(foreground1);
            spriteList.Add(foreground2);

            //Interface components
            AddBottomAnimatedButtonsToScene();
            AddFilterAnimatedButtonsToScene();
            AddSearchTextToScene();
            AddPreviewsToScene();
            AddFilteringToScene();

            //Tab Index
            tabButtonList = new TabButtonList(new Vector2(-110, -285),
                new List<TabButtonParameter>() {
                    new TabButtonParameter(Language.AvatarShopTabShop, ShopTabButtonAction),
                    new TabButtonParameter(Language.AvatarShopTabInventory, InventoryTabButtonAction),
                });

            //AttributeButton
            Vector2 positionOffset = new Vector2(Background.SourceRectangle.Width, Background.SourceRectangle.Height) / 2;
            attributeMenu = new AttributeMenu(Parameter.ScreenCenter - positionOffset, player);

            //Buttons standard preset
            tryButton.Disable(true);
            buyButton.Disable(true);
            giftButton.Disable(true);
            cashChargeButton.Disable(true);

            //Popup configuration
            PopupHandler.PopupGameOptions.OnClose = OptionsCloseAction;

            //Feedbacks / Callbacks
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerAvatarShopBuyAvatarGold, GoldPurchaseConfirmationAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerAvatarShopBuyAvatarCash, CashPurchaseConfirmationAsyncCallback);
        }

        #region Networking / Callbacks
        public void CashPurchaseConfirmationAsyncCallback(object param)
        {
            lock (asyncPadlock)
            {
                AvatarMetadata avatarMetadata = (AvatarMetadata)param;
                player.Cash -= selectedAvatarMetadata.CashPrice;
                PurchaseConfirmationAsyncCallback(avatarMetadata);
            }
        }

        public void GoldPurchaseConfirmationAsyncCallback(object param)
        {
            lock (asyncPadlock)
            {
                AvatarMetadata avatarMetadata = (AvatarMetadata)param;
                player.Gold -= selectedAvatarMetadata.GoldPrice;
                PurchaseConfirmationAsyncCallback(avatarMetadata);
            }
        }

        /// <summary>
        /// Created a popup informing the player about the status of the previously
        /// requested purchase. If the parameter is null, the purchase has failed.
        /// </summary>
        public void PurchaseConfirmationAsyncCallback(AvatarMetadata avatarMetadata)
        {
            PopupAlertMessageBuilder.BuildPopupAlertMessage(
                avatarMetadata != null ?
                PopupAlertMessageType.AvatarPurchaseSuccessful :
                PopupAlertMessageType.AvatarPurchaseFailure, avatarMetadata);

            attributeMenu.RefreshCurrencyValues();

            EnableInterfaceElements();

            // If the player is still selecting the purchased avatar
            if (avatarMetadata != null && selectedAvatarMetadata.ID == avatarMetadata.ID)
            {
                buyButton.Disable(true);
                tryButton.Disable(true);
            }

            if (avatarMetadata == null) return;

            player.OwnedAvatar[avatarMetadata.AvatarCategory].Add(avatarMetadata.ID);
            UpdateFilter(searchFilter.AvatarCategory, searchFilter.CurrentPage);
        }
        #endregion

        #region Add/Instance Components
        public void AddAvatarButtonList(List<AvatarMetadata> metadataList)
        {
            avatarButtonList.Clear();

            const int offsetX = 108;
            const int offsetY = 86;

            for (int i = 0; i < 25 && i < metadataList.Count(); i++)
            {
                AvatarButton button = new AvatarButton(
                    new Vector2(-95, -175) + new Vector2(offsetX * (i % 5), offsetY * (i / 5)),
                    metadataList[i], AvatarButtonAction, DepthParameter.InterfaceButton);

                button.HideEquippedIndicator();

                if (searchFilter.IsRenderingInventory)
                {
                    if (inventoryRiderPreview.GetEquippedAvatarID(metadataList[i].AvatarCategory) == metadataList[i].ID)
                        button.ShowEquippedIndicator();
                }
                else
                {
                    if (shopRiderPreview.GetEquippedAvatarID(metadataList[i].AvatarCategory) == metadataList[i].ID)
                        button.ShowEquippedIndicator();
                }

                avatarButtonList.Add(button);
            }
        }

        public void AddFilteringToScene()
        {
            //Filtering

            //Filtering Text
            Vector2 dividerPosition = (filterLeftButton.Flipbook.Position + filterRightButton.Flipbook.Position) / 2 + new Vector2(0, -10);
            filterDivider = new SpriteText(FontTextType.Consolas14, "/", Color.White,
                Alignment.Center, DepthParameter.InterfaceButton,
                dividerPosition, Color.Black);
            filterCurrentPage = new SpriteText(FontTextType.Consolas14, "", Color.White,
                Alignment.Right, DepthParameter.InterfaceButton,
                dividerPosition - new Vector2(6, 0), Color.Black);
            filterLastPage = new SpriteText(FontTextType.Consolas14, "", Color.White,
                Alignment.Left, DepthParameter.InterfaceButton,
                dividerPosition + new Vector2(6, 0), Color.Black);

            spriteTextList.Add(filterCurrentPage);
            spriteTextList.Add(filterLastPage);
            spriteTextList.Add(filterDivider);

            //Since Hats is the first selected filter, start the window rendering all hats
            filterHatButton.ChangeButtonState(ButtonAnimationState.Activated, true);
            searchFilter = new AvatarSearchFilter();
            searchFilter.AvatarCategory = AvatarCategory.Hat;
            UpdateFilter(AvatarCategory.Hat, 0);
        }

        public void AddPreviewsToScene()
        {
            //Room Button Preview
            shopRiderPreview = new Rider(Facing.Right, player, Parameter.ScreenCenter + new Vector2(-280, -40));
            inventoryRiderPreview = new Rider(Facing.Right, player, Parameter.ScreenCenter + new Vector2(-280, -40));

            inventoryRiderPreview.Hide();

            avatarPreviewSpriteText = new SpriteText(FontTextType.Consolas10Family, Language.PreviewTextAvatarShop,
                Color.White, Alignment.Left, DepthParameter.InterfaceButton,
                Parameter.ScreenCenter - new Vector2(385, 110), Color.Black);

            spriteTextList.Add(avatarPreviewSpriteText);

            //In-Game (mobile) preview
            shopInGamePreview = new InGamePreview(Parameter.ScreenCenter + new Vector2(-290, 80));
            inventoryInGamePreview = new InGamePreview(Parameter.ScreenCenter + new Vector2(-290, 80));
            inventoryInGamePreview.Hide();

            inGamePreviewSpriteText = new SpriteText(FontTextType.Consolas10Family, Language.InGamePreviewTextAvatarShop,
                Color.White, Alignment.Left, DepthParameter.InterfaceButton,
                Parameter.ScreenCenter - new Vector2(385, -17), Color.Black);

            spriteTextList.Add(inGamePreviewSpriteText);
        }

        public void AddSearchTextToScene()
        {
            SpriteText spriteText = new SpriteText(FontTextType.FontAwesome10, "" + (char)0xf002, Color.White, Alignment.Left, DepthParameter.InterfaceButtonText,
                Parameter.ScreenCenter + new Vector2(242, -253), Color.Black);
            spriteTextList.Add(spriteText);

            searchTextField = new TextField(Parameter.ScreenCenter + new Vector2(260, -252), 120, 16, 16, FontTextType.Consolas10Family, Color.White, DepthParameter.InterfaceButtonText, Color.Black);
            searchTextField.ActivateElement();
            searchTextField.OnTextChange = OnFilterTextChange;
        }

        public void AddFilterAnimatedButtonsToScene()
        {
            Vector2 position = Parameter.ScreenCenter - new Vector2(130, 250);
            Vector2 offset = new Vector2(45, 0);
            int i = 0;
            filterButtonList.Add(filterHatButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Hat,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.Hat); }));
            filterButtonList.Add(filterBodyButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Body,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.Body); }));
            filterButtonList.Add(filterGogglesButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Goggles,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.Goggles); }));
            filterButtonList.Add(filterFlagButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Flag,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.Flag); }));
            filterButtonList.Add(filterExItemButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ExItem,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.ExItem); }));
            filterButtonList.Add(filterPetButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Pet,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.Pet); }));
            filterButtonList.Add(filterMiscButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Necklace,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.Misc); }));
            filterButtonList.Add(filterExtraButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Ring,
                position + offset * i++, (o) => { FilterButtonAction(o, AvatarCategory.Extra); }));
        }

        public void AddBottomAnimatedButtonsToScene()
        {
            //Left Side
            int i = 0;

            Vector2 basePosition = Parameter.ScreenCenter - new Vector2(360, -265);
            Vector2 offset = new Vector2(72, 0);

            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ExitDoor, basePosition + offset * i++, ExitDoorAction));
            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Buddy,    basePosition + offset * i++, ExitDoorAction));
            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Options,  basePosition + offset * i++, OptionsAction));

            /*
            basePosition += new Vector2(0, -55);
            i = 0;

            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ExitDoor, basePosition + offset * i++, ExitDoorAction));
            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ExitDoor, basePosition + offset * i++, ExitDoorAction));
            animatedButtonList.Add(AnimatedButtonBuilder.BuildButton(AnimatedButtonType.ExitDoor, basePosition + offset * i++, ExitDoorAction));*/

            //Right Side - Left
            basePosition = Parameter.ScreenCenter - new Vector2(129, -265);
            animatedButtonList.Add(cashChargeButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.CashCharge, basePosition, ExitDoorAction));

            //Right Side - Right
            offset = new Vector2(62, 0);
            basePosition = Parameter.ScreenCenter - new Vector2(-370, -265) - 2 * offset;
            i = 0;

            animatedButtonList.Add(tryButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Try, basePosition + offset * i++, TryButtonAction));
            animatedButtonList.Add(buyButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Buy, basePosition + offset * i++, BuyButtonAction));
            animatedButtonList.Add(giftButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.Gift, basePosition + offset * i++, ExitDoorAction));

            animatedButtonList.Add(filterLeftButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.LeftArrow,
                Parameter.ScreenCenter - new Vector2(-141 + 40 + 30, -245), (o) => { UpdateFilter(searchFilter.AvatarCategory, searchFilter.CurrentPage - 1); }));
            animatedButtonList.Add(filterRightButton = AnimatedButtonBuilder.BuildButton(AnimatedButtonType.RightArrow,
                Parameter.ScreenCenter - new Vector2(-141 - 40, -245), (o) => { UpdateFilter(searchFilter.AvatarCategory, searchFilter.CurrentPage + 1); }));
        }
        #endregion

        #region Button Actions
        private void OptionsCloseAction(object sender)
        {
            //roomButtonList.ForEach((x) => x.ShouldUpdate = true);
            animatedButtonList.ForEach((x) => x.ShouldUpdate = true);
            ((Button)sender).Enable();

            EnableInterfaceElements();
        }

        private void OptionsAction(object sender)
        {
            //roomButtonList.ForEach((x) => x.ShouldUpdate = false);
            animatedButtonList.ForEach((x) => x.ShouldUpdate = false);
            PopupHandler.PopupGameOptions.ShouldRender = true;
            ((AnimatedButton)sender).Disable(true);

            DisableInterfaceElements();
        }

        private void ExitDoorAction(object sender)
        {
            //Sends the server information about the attributes and equipped avatars
            ServerInformationHandler.AvatarShopUpdatePlayerData();
            SceneHandler.Instance.RequestSceneChange(SceneType.GameList, TransitionEffectType.RotatingRectangles);
            ((AnimatedButton)sender).Disable(true);
        }

        /// <summary>
        /// This action is meant for avatar inventory and shop buttons.
        /// </summary>
        private void AvatarButtonAction(object sender)
        {
            AvatarButton avatarButton = (AvatarButton)sender;
            selectedAvatarMetadata = avatarButton.AvatarMetadata;

            // If it is a inventory button
            if (searchFilter.IsRenderingInventory)
            {
                // Equip
                player.EquipAvatar(selectedAvatarMetadata.AvatarCategory, selectedAvatarMetadata.ID);

                // Update Equipped
                avatarButtonList.ForEach((x) => x.HideEquippedIndicator());
                avatarButton.ShowEquippedIndicator();

                inventoryRiderPreview.ReplaceAvatar(selectedAvatarMetadata);
                inventoryInGamePreview.ReplaceAvatar(selectedAvatarMetadata);

                tryButton.Disable(true);
                buyButton.Disable(true);

                return;
            }

            // If it isn't an inventory button
            avatarButtonList.ForEach((x) => { x.Disable(); x.Enable(); });
            avatarButton.ChangeButtonState(ButtonAnimationState.Activated, true);

            if (shopRiderPreview.GetEquippedAvatarID(selectedAvatarMetadata.AvatarCategory) != selectedAvatarMetadata.ID)
                tryButton.Enable();
            else
                tryButton.Disable(true);

            if (player.Gold >= selectedAvatarMetadata.GoldPrice ||
                player.Cash >= selectedAvatarMetadata.CashPrice)
            {
                if (!player.OwnedAvatar[selectedAvatarMetadata.AvatarCategory].Contains(selectedAvatarMetadata.ID))
                    buyButton.Enable();
                else
                    buyButton.Disable(true);
            }
        }

        /// <summary>
        /// Equip the selected avatar on the ShopPreview and then disable the button.
        /// </summary>
        /// <param name="sender"></param>
        private void TryButtonAction(object sender)
        {
            if (selectedAvatarMetadata != null)
            {
                shopRiderPreview.ReplaceAvatar(selectedAvatarMetadata);
                shopInGamePreview.ReplaceAvatar(selectedAvatarMetadata);

                foreach (AvatarButton button in avatarButtonList)
                {
                    button.HideEquippedIndicator();

                    if (button.AvatarMetadata.ID == shopRiderPreview.GetEquippedAvatarID(button.AvatarMetadata.AvatarCategory))
                        button.ShowEquippedIndicator();
                }
            }

            tryButton.Disable(true);
        }

        /// <summary>
        /// Creates a popup for avatar purchase and disable the try/buy button.
        /// While the popup is open the interface buttons are disabled.
        /// </summary>
        private void BuyButtonAction(object sender)
        {
            popupBuyAvatar = new PopupBuyAvatar(selectedAvatarMetadata,
                OnCloseBuyAvatarPopupDialog,
                OnBuyCash,
                OnBuyGold);

            PopupHandler.Add(popupBuyAvatar);

            buyButton.Disable(true);
            tryButton.Disable(true);

            DisableInterfaceElements();
        }

        /// <summary>
        /// Changes the foreground, the search filters and the rendered rider
        /// preview when clicked.
        /// </summary>
        private void ShopTabButtonAction(object sender)
        {
            foreground1.SetTransparency(1);
            foreground2.SetTransparency(0);

            shopRiderPreview.Show();
            inventoryRiderPreview.Hide();

            shopInGamePreview.Show();
            inventoryInGamePreview.Hide();

            avatarPreviewSpriteText.Text = Language.PreviewTextAvatarShop;
            inGamePreviewSpriteText.Text = Language.InGamePreviewTextAvatarShop;

            searchFilter.IsRenderingInventory = false;
            UpdateFilter(searchFilter.AvatarCategory, 0);
        }

        /// <summary>
        /// Changes the foreground, the search filters and the rendered rider
        /// preview when clicked.
        /// </summary>
        private void InventoryTabButtonAction(object sender)
        {
            foreground1.SetTransparency(0);
            foreground2.SetTransparency(1);

            shopRiderPreview.Hide();
            inventoryRiderPreview.Show();

            shopInGamePreview.Hide();
            inventoryInGamePreview.Show();

            avatarPreviewSpriteText.Text = Language.PreviewTextAvatarShopEquipped;
            inGamePreviewSpriteText.Text = Language.InGamePreviewTextAvatarShopEquipped;

            searchFilter.IsRenderingInventory = true;
            UpdateFilter(searchFilter.AvatarCategory, 0);

            buyButton.Disable(true);
            tryButton.Disable(true);
        }

        /// <summary>
        /// Action being called on each avatar shop filter.
        /// This action changes the filtering options and refresh the
        /// AvatarButtonList.
        /// </summary>
        public void FilterButtonAction(object sender, AvatarCategory category)
        {
            filterButtonList.ForEach((x) =>
            {
                if (x != sender) x.Enable();
            });

            ((AnimatedButton)sender).ChangeButtonState(ButtonAnimationState.Activated);

            UpdateFilter(category, 0);
        }

        public void OnCloseBuyAvatarPopupDialog(object sender)
        {
            EnableInterfaceElements();

            buyButton.Enable();
            tryButton.Enable();
        }

        /// <summary>
        /// Sends the server a request for purchasing the selected avatar 
        /// in cash, closes <see cref="popupBuyAvatar">. The confirmation
        /// occurs after the server process the request and a popup will show up.
        /// With the confirmation status.
        /// </summary>
        public void OnBuyCash(object sender)
        {
            //send request
            ServerInformationHandler.AvatarShopBuyAvatarCash(selectedAvatarMetadata);
            PopupHandler.Remove(popupBuyAvatar);

            buyButton.Disable(true);
            tryButton.Disable(true);
        }

        /// <summary>
        /// Sends the server a request for purchasing the selected avatar 
        /// in gold, closes <see cref="popupBuyAvatar">. The confirmation
        /// occurs after the server process the request and a popup will show up.
        /// With the confirmation status.
        /// </summary>
        public void OnBuyGold(object sender)
        {
            //send request
            ServerInformationHandler.AvatarShopBuyAvatarGold(selectedAvatarMetadata);
            PopupHandler.Remove(popupBuyAvatar);

            buyButton.Disable(true);
            tryButton.Disable(true);
        }

        /// <summary>
        /// Disables all interface elements. Including buttons and text fields.
        /// </summary>
        private void DisableInterfaceElements()
        {
            animatedButtonList.ForEach((x) => x.Disable(true));
            filterButtonList.ForEach((x) => x.Disable(true));
            avatarButtonList.ForEach((x) => x.ShouldUpdate = false);
            searchTextField.Disable();
            searchTextField.DeactivateElement();

            tabButtonList.Disable();
            attributeMenu.Disable();
        }

        /// <summary>
        /// Enables all interface elements. Including buttons and text fields.
        /// The search button states are preserved.
        /// </summary>
        private void EnableInterfaceElements()
        {
            animatedButtonList.ForEach((x) => x.Enable());
            filterButtonList.ForEach((x) => x.Enable());
            avatarButtonList.ForEach((x) => x.ShouldUpdate = true);

            tabButtonList.Enable();

            AnimatedButton button;

            switch (searchFilter.AvatarCategory)
            {
                case AvatarCategory.Hat: button = filterHatButton; break;
                case AvatarCategory.Body: button = filterBodyButton; break;
                case AvatarCategory.Goggles: button = filterGogglesButton; break;
                case AvatarCategory.Flag: button = filterFlagButton; break;
                case AvatarCategory.ExItem: button = filterExItemButton; break;
                case AvatarCategory.Pet: button = filterPetButton; break;
                case AvatarCategory.Misc: button = filterMiscButton; break;
                default: button = filterExtraButton; break;
            }

            button.ChangeButtonState(ButtonAnimationState.Activated);

            giftButton.Disable(true);
            cashChargeButton.Disable(true);

            //Update filter buttons
            if (searchFilter.CurrentPage == 0) filterLeftButton.Disable(true);
            else if (filterLeftButton.IsDisabled) filterLeftButton.Enable();

            if (searchFilter.CurrentPage + 1 == searchFilter.LastPage) filterRightButton.Disable(true);
            else if (filterRightButton.IsDisabled) filterRightButton.Enable();

            //Search text field
            searchTextField.Enable();
            searchTextField.ActivateElement();
        }
        #endregion

        #region Action handlers
        /// <summary>
        /// This method is called whenever a letter is typed on the textfield.
        /// </summary>
        public void OnFilterTextChange(string newText)
        {
            searchFilter.AvatarName = newText;
            UpdateFilter(searchFilter.AvatarCategory, searchFilter.CurrentPage);
        }
        #endregion

        #region Filtering
        /// <summary>
        /// This method updates <see cref="searchFilter"/> and refreshes the
        /// AvatarButtonList.
        /// </summary>
        public void UpdateFilter(AvatarCategory category, int currentPage)
        {
            searchFilter.AvatarCategory = category;
            searchFilter.CurrentPage = currentPage;

            IEnumerable<AvatarMetadata> metadataList = MetadataManager.AvatarMetadataDictionary[player.Gender][searchFilter.AvatarCategory].Values;

            //Text filter
            metadataList = metadataList.Where((x) => x.Name.ToLower().Contains(searchFilter.AvatarName.ToLower()));

            if (searchFilter.IsRenderingInventory)
                metadataList = metadataList.Where((x) => player.OwnedAvatar[category].Contains(x.ID));

            searchFilter.LastPage = Math.Max((int)Math.Ceiling(metadataList.Count() / 25f), 1);

            metadataList = metadataList.Skip(searchFilter.CurrentPage * 25).Take(searchFilter.CurrentPage * 25 + 25).ToList();

            AddAvatarButtonList(metadataList.ToList());

            filterCurrentPage.Text = (searchFilter.CurrentPage + 1).ToString();
            filterLastPage.Text = searchFilter.LastPage.ToString();

            //Update filter buttons
            if (currentPage == 0) filterLeftButton.Disable(true);
            else if (filterLeftButton.IsDisabled) filterLeftButton.Enable();

            if (currentPage + 1 == searchFilter.LastPage) filterRightButton.Disable(true);
            else if (filterRightButton.IsDisabled) filterRightButton.Enable();
        }
        #endregion

        public override void OnDropGameServerConnection(Exception ex)
        {
            base.OnDropGameServerConnection(ex);
            attributeMenu.Disable();
        }

        public override void Update(GameTime gameTime)
        {
            lock (asyncPadlock)
            {
                base.Update(gameTime);
                buttonList.ForEach((x) => x.Update());
                tabButtonList.Update();
                animatedButtonList.ForEach((x) => x.Update());
                filterButtonList.ForEach((x) => x.Update());
                avatarButtonList.ForEach((x) => x.Update(gameTime));
                searchTextField.Update(gameTime);
                attributeMenu.Update(gameTime);

                shopInGamePreview.Update();
                inventoryInGamePreview.Update();
            }

            if (Input.InputHandler.IsBeingPressed(Microsoft.Xna.Framework.Input.Keys.D7))
            {
                SceneHandler.Instance.RequestSceneChange(SceneType.AvatarShop, TransitionEffectType.RotatingRectangles);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            lock (asyncPadlock)
            {
                base.Draw(gameTime);
                spriteList.ForEach((x) => x.Draw(null, spriteBatch));
                animatedButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
                filterButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
                avatarButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
                spriteTextList.ForEach((x) => x.Draw(spriteBatch));
                searchTextField.Draw(spriteBatch);
                buttonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
                shopRiderPreview.Draw(gameTime, spriteBatch);
                inventoryRiderPreview.Draw(gameTime, spriteBatch);

                attributeMenu.Draw(gameTime, spriteBatch);

                shopInGamePreview.Draw(gameTime, spriteBatch);
                inventoryInGamePreview.Draw(gameTime, spriteBatch);
                tabButtonList.Draw(gameTime, spriteBatch);
            }
        }
    }
}
