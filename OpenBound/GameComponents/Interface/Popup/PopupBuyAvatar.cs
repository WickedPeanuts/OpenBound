using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Interactive.AvatarShop;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Models;
using System;

namespace OpenBound.GameComponents.Interface.Popup
{
    public class PopupBuyAvatar : PopupMenu
    {
        AvatarButton avatarButton;

        public PopupBuyAvatar(AvatarMetadata avatarMetadata, Action<object> onClose, Action<object> onBuyCash, Action<object> onBuyGold) : base(false)
        {
            Background = new Sprite("Interface/Popup/Blue/Avatar/Buy", layerDepth: DepthParameter.InterfacePopupBackground);

            avatarButton = new AvatarButton(new Vector2(0, -70), avatarMetadata, (o) => { avatarButton.Disable(); avatarButton.Enable(); }, DepthParameter.InterfacePopupButtons);

            onClose += (o) => { Destroy(); };
            buttonList.Add(new Button(ButtonType.Cancel, DepthParameter.InterfacePopupMessageButtons, onClose, PositionOffset + new Vector2(95, 125)));

            spriteTextList.Add(
                new SpriteText(
                    FontTextType.Consolas10, Language.PopupTextConfirmPuchase,
                    Color.White, Alignment.Center, DepthParameter.InterfacePopupText,
                    default, outlineColor: Color.Black)
                {
                    PositionOffset = new Vector2(0, -25)
                });

            //Before
            spriteTextList.Add(
                new SpriteText(
                    FontTextType.Consolas10, Language.PopupTextBalance,
                    Color.White, Alignment.Center, DepthParameter.InterfacePopupText,
                    default, outlineColor: Color.Black)
                {
                    PositionOffset = new Vector2(0, 15)
                });

            int startingX = (avatarMetadata.GoldPrice > 0 && avatarMetadata.CashPrice > 0) ? 55 : 0;

            if (avatarMetadata.GoldPrice > 0) { 
                //Gold
                spriteTextList.Add(
                    new SpriteText(
                        FontTextType.Consolas10, Language.PopupTextCurrencyGold,
                        Color.White, Alignment.Center, DepthParameter.InterfacePopupText,
                        default, outlineColor: Color.Black)
                    {
                        PositionOffset = new Vector2(-startingX, -05)
                    });


                //Gold Value
                spriteTextList.Add(
                    new SpriteText(FontTextType.Consolas10,
                    string.Format("{0:N0}", GameInformation.Instance.PlayerInformation.Gold),
                    Parameter.InterfaceAvatarShopButtonGoldColor, Alignment.Center, DepthParameter.InterfacePopupText,
                    default, outlineColor: Parameter.InterfaceAvatarShopButtonGoldOutlineColor)
                    {
                        PositionOffset = new Vector2(-startingX, 30)
                    });

                //Arrow
                spriteTextList.Add(
                    new SpriteText(
                        FontTextType.FontAwesome10, "" + (char)0xf063,
                        Color.White, Alignment.Center, DepthParameter.InterfacePopupText,
                        default, outlineColor: Color.Black)
                    {
                        PositionOffset = new Vector2(-startingX, 45)
                    });

                //Gold Value 2
                spriteTextList.Add(
                    new SpriteText(FontTextType.Consolas10,
                    string.Format("{0:N0}", GameInformation.Instance.PlayerInformation.Gold - avatarMetadata.GoldPrice),
                    Parameter.InterfaceAvatarShopButtonGoldColor, Alignment.Center, DepthParameter.InterfacePopupText,
                    default, outlineColor: Parameter.InterfaceAvatarShopButtonGoldOutlineColor)
                    {
                        PositionOffset = new Vector2(-startingX, 62)
                    });

                Button goldButton = new Button(ButtonType.AvatarBuyGold,
                    DepthParameter.InterfacePopupButtons,
                    onBuyGold, new Vector2(-startingX, 92));

                buttonList.Add(goldButton);

                if (GameInformation.Instance.PlayerInformation.Gold - avatarMetadata.GoldPrice < 0)
                    goldButton.Disable();
            }

            if (avatarMetadata.CashPrice > 0)
            {
                //Cash
                spriteTextList.Add(
                    new SpriteText(
                        FontTextType.Consolas10, Language.PopupTextCurrencyCash,
                        Color.White, Alignment.Center, DepthParameter.InterfacePopupText,
                        default, outlineColor: Color.Black)
                    {
                        PositionOffset = new Vector2(startingX, -05)
                    });

                //Cash Value
                spriteTextList.Add(
                    new SpriteText(FontTextType.Consolas10,
                    string.Format("{0:N0}", GameInformation.Instance.PlayerInformation.Cash),
                    Parameter.InterfaceAvatarShopButtonCashColor, Alignment.Center, DepthParameter.InterfacePopupText,
                    default, outlineColor: Parameter.InterfaceAvatarShopButtonCashOutlineColor)
                    {
                        PositionOffset = new Vector2(startingX, 30)
                    });

                //Arrow
                spriteTextList.Add(
                    new SpriteText(
                        FontTextType.FontAwesome10, "" + (char)0xf063,
                        Color.White, Alignment.Center, DepthParameter.InterfacePopupText,
                        default, outlineColor: Color.Black)
                    {
                        PositionOffset = new Vector2(startingX, 45)
                    });

                //Cash Value 2
                spriteTextList.Add(
                    new SpriteText(FontTextType.Consolas10,
                    string.Format("{0:N0}", GameInformation.Instance.PlayerInformation.Cash - avatarMetadata.CashPrice),
                    Parameter.InterfaceAvatarShopButtonCashColor, Alignment.Center, DepthParameter.InterfacePopupText,
                    default, outlineColor: Parameter.InterfaceAvatarShopButtonCashOutlineColor)
                    {
                        PositionOffset = new Vector2(startingX, 62)
                    });

                Button cashButton = new Button(ButtonType.AvatarBuyCash,
                    DepthParameter.InterfacePopupButtons, onBuyCash,
                    new Vector2(startingX, 92));

                buttonList.Add(cashButton);

                if (GameInformation.Instance.PlayerInformation.Cash - avatarMetadata.CashPrice < 0)
                    cashButton.Disable();
            }            

            ShouldRender = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            avatarButton.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            avatarButton.Draw(gameTime, spriteBatch);
        }
    }
}
