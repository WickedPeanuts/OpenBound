using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Interface
{
    public enum IconType 
    {
        Attack = 0, Defense      = 2, AttackDelay = 4, Dig        = 6,
        Health = 1, Regeneration = 3, ItemDelay   = 5, Popuparity = 7
    }

    public class AttributeStatusBarItem
    {
        public Sprite Sprite;
        public NumericSpriteFont NumericSpriteFont;
        public Vector2? DesiredPosition;

        public AttributeStatusBarItem(Sprite sprite, NumericSpriteFont numericSpriteFont)
        {
            Sprite = sprite;
            NumericSpriteFont = numericSpriteFont;
        }
    }
    public class AttributeStatusBar
    {
        List<NumericSpriteFont> numericSpriteFontList;
        List<Sprite> spriteList;

        Mobile mobile;

        Sprite attack, defense,      attackDelay, dig;
        Sprite health, regeneration, itemDelay,   popularity;

        int[] attributes;
        int[] attDifference;

        static Vector2 elementOffsetX;
        static Vector2 elementOffsetY;

        List<AttributeStatusBarItem> attributeStatusBarItemList;
        List<List<AttributeStatusBarItem>> attributeStatusBarItemMatrix;

        public float elapsedAnimationTime;

        public AttributeStatusBar(Mobile mobile)
        {
            this.mobile = mobile;

            //Perks icon
            attack = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 1, 17 * 2, 26, 17));
            health = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 5, 17 * 2, 26, 17));

            defense = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 3, 17 * 2, 26, 17));
            regeneration = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 5, 17 * 3, 26, 17));

            attackDelay = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 3, 17 * 1, 26, 17));
            itemDelay = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 1, 17 * 3, 26, 17));

            dig = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 3, 17 * 3, 26, 17));
            popularity = new Sprite("Interface/StaticButtons/AvatarShop/AvatarButton/ButtonIcons", default, DepthParameter.InterfaceButton, new Rectangle(26 * 5, 17 * 1, 26, 17));

            spriteList = new List<Sprite>() { attack, health, defense, regeneration, attackDelay, itemDelay, dig, popularity };

            attributeStatusBarItemList = new List<AttributeStatusBarItem>();
            attributeStatusBarItemMatrix = new List<List<AttributeStatusBarItem>>();

            numericSpriteFontList = new List<NumericSpriteFont>();

            attributes = (int[])mobile.Owner.Attribute.Clone();
            attDifference = new int[mobile.Owner.Attribute.Length];

            for (int i = 0; i < 8; i++)
            {
                numericSpriteFontList.Add(
                    new NumericSpriteFont(FontType.HUDBlueStatusBar, 2, DepthParameter.HealthBar,
                    attachToCamera: false,
                    forceRendingAllNumbers: true));

                numericSpriteFontList[i].HideElement();
                spriteList[i].HideElement();
            }

            elementOffsetX = new Vector2(36, 0);
            elementOffsetY = new Vector2(0, 15);
        }

        /// <summary>
        /// Returns a rectangle with the position of each icon
        /// </summary>
        private Rectangle GetAttributeIcon(bool isPositive, IconType iconType) =>
            (isPositive, iconType) switch
            {
                (true,  IconType.Attack)       => new Rectangle(26 * 1, 17 * 2, 26, 17),
                (false, IconType.Attack)       => new Rectangle(26 * 0, 17 * 2, 26, 17),

                (true,  IconType.Health)       => new Rectangle(26 * 5, 17 * 2, 26, 17),
                (false, IconType.Health)       => new Rectangle(26 * 4, 17 * 2, 26, 17),

                (true,  IconType.Defense)      => new Rectangle(26 * 3, 17 * 2, 26, 17),
                (false, IconType.Defense)      => new Rectangle(26 * 2, 17 * 2, 26, 17),

                (true,  IconType.Regeneration) => new Rectangle(26 * 5, 17 * 3, 26, 17),
                (false, IconType.Regeneration) => new Rectangle(26 * 4, 17 * 3, 26, 17),

                (true,  IconType.AttackDelay)  => new Rectangle(26 * 3, 17 * 1, 26, 17),
                (false, IconType.AttackDelay)  => new Rectangle(26 * 2, 17 * 1, 26, 17),

                (true,  IconType.ItemDelay)    => new Rectangle(26 * 1, 17 * 3, 26, 17),
                (false, IconType.ItemDelay)    => new Rectangle(26 * 0, 17 * 3, 26, 17),

                (true,  IconType.Dig)          => new Rectangle(26 * 3, 17 * 3, 26, 17),
                (false, IconType.Dig)          => new Rectangle(26 * 2, 17 * 3, 26, 17),

                (true,  IconType.Popuparity)   => new Rectangle(26 * 5, 17 * 1, 26, 17),
                (false, IconType.Popuparity)   => new Rectangle(26 * 4, 17 * 1, 26, 17),

                (_, _) => default,
            };

        public void UpdateAttributeList(bool isShowingFullStatus = false)
        {
            attributeStatusBarItemList.Clear();

            for (int i = 0; i < 8; i++)
            {
                if (isShowingFullStatus)
                    attDifference[i] = mobile.Owner.Attribute[i];
                else
                    attDifference[i] = mobile.Owner.Attribute[i] - attributes[i];

                numericSpriteFontList[i].UpdateValue(Math.Abs(attDifference[i]));

                if (attDifference[i] != 0 || isShowingFullStatus)
                {
                    attributeStatusBarItemList.Add(new AttributeStatusBarItem(spriteList[i], numericSpriteFontList[i]));
                }

                if (attDifference[i] == 0 && !isShowingFullStatus)
                {
                    numericSpriteFontList[i].HideElement();
                    spriteList[i].HideElement();
                }
                else
                {
                    numericSpriteFontList[i].ShowElement();
                    spriteList[i].ShowElement();
                }

                spriteList[i].SourceRectangle = GetAttributeIcon(attDifference[i] >= 0, (IconType)i);
            }

            attributeStatusBarItemMatrix.Clear();
            attributeStatusBarItemMatrix.Add(attributeStatusBarItemList.Skip(0).Take(4).ToList());
            attributeStatusBarItemMatrix.Add(attributeStatusBarItemList.Skip(4).Take(4).ToList());
            
            for (int i = 0; i < attributeStatusBarItemMatrix.Count; i++)
                if (attributeStatusBarItemMatrix[i].Count < 4)
                    attributeStatusBarItemMatrix[i].Reverse();

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            // All this logic was created in order to render the elements from right to left
            // in this order: attack, defense,      attackDelay, dig;
            //                health, regeneration, itemDelay,   popularity;

            Vector2 startingPos = mobile.MobileFlipbook.Position + new Vector2(20, 118);

            for (int i = 0; i < attributeStatusBarItemMatrix.Count; i++)
            {
                for (int j = 0; j < attributeStatusBarItemMatrix[i].Count; j++)
                {
                    //Reverse Updating Draw
                    attributeStatusBarItemMatrix[i][j].Sprite.Position = startingPos
                        + ((attributeStatusBarItemMatrix[i].Count < 4) ? (3 - (j % 4)) : (j % 4)) * elementOffsetX
                        + i * elementOffsetY;

                    attributeStatusBarItemMatrix[i][j].NumericSpriteFont.Position = attributeStatusBarItemMatrix[i][j].Sprite.Position
                        + new Vector2(-93, -49);

                    attributeStatusBarItemMatrix[i][j].NumericSpriteFont.Update();
                }
            }
        }

        public void Update()
        {
            if (LevelScene.HUD.IsChatOpen) return;

            if (InputHandler.IsBeingHeldDown(Keys.Z))
            {
                UpdateAttributeList(true);
                return;
            }
            else if (InputHandler.IsBeingReleased(Keys.Z))
            {
                UpdateAttributeList(false);
                return;
            }

            UpdatePosition();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i < 8; i++)
            {
                numericSpriteFontList[i].Draw(null, spriteBatch);
                spriteList[i].Draw(null, spriteBatch);
            }
        }
    }
}
