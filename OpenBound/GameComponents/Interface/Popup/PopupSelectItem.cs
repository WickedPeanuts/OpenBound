using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Level.Scene;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Language = OpenBound.Common.Language;

namespace OpenBound.GameComponents.Interface.Popup
{
    public class ItemFilter
    {
        public int Slots;
        public ItemFilter(int slots)
        {
            Slots = slots;
        }
    }

    public class PopupSelectItem : PopupMenu
    {
        List<Button> itemButtonList;

        ItemFilter itemFilter;

        SpriteText itemName, itemCost, itemDelayCost, itemDescription;

        //Tab Indexing
        TabButtonList tabButtonList;

        EquippedButtonBar equippedItemButtonBar;

        public PopupSelectItem() : base(true)
        {
            Background = new Sprite("Interface/Popup/Blue/SelectItem/Background", layerDepth: DepthParameter.InterfacePopupBackground);

            itemButtonList = new List<Button>();

            itemName        = new SpriteText(FontTextType.Consolas11, Language.InGameItemName,      Color.White, Alignment.Left,  DepthParameter.InterfacePopupText, default, outlineColor: Color.Black);
            itemDescription = new SpriteText(FontTextType.Consolas10Family, "",                           Color.White, Alignment.Left,  DepthParameter.InterfacePopupText, default, outlineColor: Color.Black);
            itemCost        = new SpriteText(FontTextType.Consolas11, Language.InGameItemCost,      Color.White, Alignment.Right, DepthParameter.InterfacePopupText, default, outlineColor: Color.Black);
            itemDelayCost   = new SpriteText(FontTextType.Consolas11, Language.InGameItemDelayCost, Color.White, Alignment.Left,  DepthParameter.InterfacePopupText, default, outlineColor: Color.Black);

            itemName.PositionOffset        = new Vector2(-182, 68);
            itemDescription.PositionOffset = new Vector2(-182, 88);
            itemCost.PositionOffset        = new Vector2(182, 68);
            itemDelayCost.PositionOffset   = new Vector2(60, 68);

            spriteTextList.Add(itemName);
            spriteTextList.Add(itemCost);
            spriteTextList.Add(itemDescription);
            spriteTextList.Add(itemDelayCost);

            equippedItemButtonBar = new EquippedButtonBar(new Vector2(-182f, 157f),
                onClick: RemoveButtonAction,
                onBeginHoover: OnBeginHooverAction);

            //Item tabs
            tabButtonList = new TabButtonList(new Vector2(-145, -148),
                new List<TabButtonParameter>()
                {
                    new TabButtonParameter(Language.InGameItemTab1, TabList2SlotsAction),
                    new TabButtonParameter(Language.InGameItemTab2, TabList1SlotAction),
                });

            //Item tab filtering
            itemFilter = new ItemFilter(2);
            CreateItemList(2);

            buttonList.Add(
                new Button(
                    ButtonType.Accept, 
                    DepthParameter.InterfacePopupButtons, 
                    CloseAction, 
                    buttonOffset: new Vector2(175, 178)));

            PopupHandler.Add(this);

            ShouldRender = false;
        }

        public void RemoveButtonAction(Button sender, ItemButtonPreset preset)
        {
            equippedItemButtonBar.RemoveButton(sender, preset);
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Remove(preset.ItemPreset.ItemType);
            equippedItemButtonBar.RealocateEquippedItems(positionOffset);
        }

        public void TabList2SlotsAction(object sender)
        {
            CreateItemList(2);
        }

        public void TabList1SlotAction(object sender)
        {
            CreateItemList(1);
        }

        public void CreateItemList(int slots)
        {
            Vector2 basePosition;
            Vector2 positionOffset;
            int elementsPerLine;

            itemFilter.Slots = slots;
            
            if (slots == 2)
            {
                basePosition = background.PositionOffset + new Vector2(-79f, -78);
                positionOffset = new Vector2(85, 40);
                elementsPerLine = 4;
            }
            else
            {
                basePosition = background.PositionOffset + new Vector2(-40f * 3 - 18f, -78);
                positionOffset = new Vector2(40, 40);
                elementsPerLine = 9;
            }

            itemButtonList.ForEach((x) => buttonList.Remove(x));
            itemButtonList.Clear();

            List<ItemButtonPreset> filteredPopupMenuItems =
                EquippedButtonBar.ItemButtonPresets.Where((x) => x.ItemPreset.Slots == itemFilter.Slots).ToList();

            for (int i = 0; i < filteredPopupMenuItems.Count(); i++)
            {
                ItemButtonPreset preset = filteredPopupMenuItems[i];

                itemButtonList.Add(
                    BuildButton(
                        basePosition - positionOffset + positionOffset * new Vector2(i % elementsPerLine, i / elementsPerLine),
                        preset,
                        (o) =>
                        {
                            ((Button)o).Disable();
                            ((Button)o).Enable();
                            equippedItemButtonBar.AddItemIntoBarAction(preset);
                            equippedItemButtonBar.RealocateEquippedItems(this.positionOffset);
                        }));

                itemButtonList[i].Update();
            }

            buttonList.AddRange(itemButtonList);
        }

        public Button BuildButton(Vector2 position, ItemButtonPreset preset, Action<object> action)
        {
            Button b = new Button(preset.ButtonType, DepthParameter.InterfacePopupButtons, action, buttonOffset: position);
            b.OnBeginHoover = (x) =>{ OnBeginHooverAction(preset); };
            b.ButtonSprite.Pivot = Vector2.Zero;
            return b;
        }

        public void OnBeginHooverAction(ItemButtonPreset preset)
        {
            itemName.Text = preset.ItemName;
            itemDescription.Text = preset.ItemDescription;
            itemCost.Text = preset.ItemPreset.ItemCost + "";

            switch (preset.ItemPreset.ItemCategory)
            {
                case ItemCategory.Red:
                    itemName.Color = itemName.BaseColor = Parameter.TextColorItemRed; break;
                case ItemCategory.Blue:
                    itemName.Color = itemName.BaseColor = Parameter.TextColorItemBlue; break;
                case ItemCategory.Green:
                    itemName.Color = itemName.BaseColor = Parameter.TextColorItemGreen; break;
                case ItemCategory.Purple:
                    itemName.Color = itemName.BaseColor = Parameter.TextColorItemPurple; break;
            }
        }

        public override void RealocateElements(Vector2 delta)
        {
            delta = delta.ToIntegerDomain();

            base.RealocateElements(delta);
            tabButtonList.RealocateElements(delta);

            UpdateAttatchmentPosition();
            equippedItemButtonBar.RealocateElements(delta);
        }

        protected override void UpdateAttatchmentPosition()
        {
            base.UpdateAttatchmentPosition();

            tabButtonList.UpdateAttatchedPosition();
            equippedItemButtonBar.UpdateAttatchMentPosition();
        }

        public override void Update(GameTime gameTime)
        {
            if (!ShouldRender) return;

            base.Update(gameTime);

            tabButtonList.Update();
            UpdateAttatchmentPosition();

            equippedItemButtonBar.Update();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!ShouldRender) return;

            base.Draw(gameTime, spriteBatch);
            tabButtonList.Draw(gameTime, spriteBatch);
            equippedItemButtonBar.Draw(gameTime, spriteBatch);
        }
    }
}
