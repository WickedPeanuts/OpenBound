using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Level.Scene;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.Interface.Interactive
{
    public struct ItemButtonPreset
    {
        public string ItemName, ItemDescription;
        public ButtonType ButtonType;
        public Item ItemPreset;

        public ItemButtonPreset(string itemName, string itemDescription, ButtonType buttonType, Item item)
        {
            ItemName = itemName;
            ItemDescription = itemDescription;
            ButtonType = buttonType;
            ItemPreset = item;
        }
    }

    public class EquippedButtonBar
    {
        public static List<ItemButtonPreset> ItemButtonPresets = new List<ItemButtonPreset>()
        {
            //2 Slots item
            new ItemButtonPreset(Language.InGameItemDual,         Language.InGameItemDualDescription,         ButtonType.SelectItemDual,         Item.ItemPresets[0]),
            new ItemButtonPreset(Language.InGameItemDualPlus,     Language.InGameItemDualPlusDescription,     ButtonType.SelectItemDualPlus,     Item.ItemPresets[1]),
            new ItemButtonPreset(Language.InGameItemThunder,      Language.InGameItemThunderDescription,      ButtonType.SelectItemThunder,      Item.ItemPresets[2]),
            new ItemButtonPreset(Language.InGameItemEnergyUp2,    Language.InGameItemEnergyUp2Description,    ButtonType.SelectItemEnergyUp2,    Item.ItemPresets[3]),
            new ItemButtonPreset(Language.InGameItemTeamTeleport, Language.InGameItemTeamTeleportDescription, ButtonType.SelectItemTeamTeleport, Item.ItemPresets[4]),
            new ItemButtonPreset(Language.InGameItemTeleport,     Language.InGameItemTeleportDescription,     ButtonType.SelectItemTeleport,     Item.ItemPresets[5]),

            //1 Slot item
            new ItemButtonPreset(Language.InGameItemBlood,        Language.InGameItemBloodDescription,        ButtonType.SelectItemBlood,        Item.ItemPresets[6]),
            new ItemButtonPreset(Language.InGameItemBungeShot,    Language.InGameItemBungeShotDescription,    ButtonType.SelectItemBungeShot,    Item.ItemPresets[7]),
            new ItemButtonPreset(Language.InGameItemPowerUp,      Language.InGameItemPowerUpDescription,      ButtonType.SelectItemPowerUp,      Item.ItemPresets[8]),
            new ItemButtonPreset(Language.InGameItemChangeWind,   Language.InGameItemChangeWindDescription,   ButtonType.SelectItemChangeWind,   Item.ItemPresets[9]),
            new ItemButtonPreset(Language.InGameItemEnergyUp1,    Language.InGameItemEnergyUp1Description,    ButtonType.SelectItemEnergyUp1,    Item.ItemPresets[10]),
        };

        Vector2 basePosition;

        public List<Button> ButtonList;
        List<Button> unusedEquippedButtonList;

        Action<ItemButtonPreset> onBeginHoover;
        Action<Button, ItemButtonPreset> onClick;

        int currentUsedSlots;
        float defaultLayerIndex;

        public EquippedButtonBar(Vector2 position, float defaultLayerIndex = DepthParameter.InterfacePopupButtonIcon, Action<Button, ItemButtonPreset> onClick = default, Action<ItemButtonPreset> onBeginHoover = default)
        {
            basePosition = position;

            this.defaultLayerIndex = defaultLayerIndex;
            this.onClick = onClick;
            this.onBeginHoover = onBeginHoover;

            ButtonList = new List<Button>();
            unusedEquippedButtonList = new List<Button>();

            List<ItemType> iList = GameInformation.Instance.PlayerInformation.SelectedItemTypeList;
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList = new List<ItemType>();

            foreach (ItemType it in iList)
            {
                AddItemIntoBarAction(ItemButtonPresets.Find((x) => x.ItemPreset.ItemType == it));
            }
        }

        public void Enable()
        {
            ButtonList.ForEach((x) => x.Enable());
        }

        public void Disable()
        {
            ButtonList.ForEach((x) => x.Disable());
        }

        public void AddItemIntoBarAction(ItemButtonPreset preset)
        {
            if (currentUsedSlots + preset.ItemPreset.Slots > 6)
                return;

            Button button = BuildButton(default, preset, (o) => onClick((Button)o, preset));
            ButtonList.Add(button);
            currentUsedSlots += preset.ItemPreset.Slots;
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Add(preset.ItemPreset.ItemType);
            RealocateEquippedItems();

            if (onBeginHoover != null)
                button.OnBeginHoover = (o) => { onBeginHoover(preset); };
        }

        public Button BuildButton(Vector2 position, ItemButtonPreset preset, Action<object> action)
        {
            Button b = new Button(preset.ButtonType, defaultLayerIndex, action, buttonOffset: position);
            b.ButtonSprite.Pivot = Vector2.Zero;
            return b;
        }

        public void RemoveButton(Button button, ItemButtonPreset preset)
        {
            unusedEquippedButtonList.Add(button);
            currentUsedSlots -= preset.ItemPreset.Slots;
        }

        public void RealocateEquippedItems(Vector2 parentOffset = default)
        {
            List<Button> bList = ButtonList.Except(unusedEquippedButtonList).ToList();

            if (bList.Count == 0) return;

            bList[0].ButtonOffset = (parentOffset + basePosition).ToIntegerDomain();

            for (int i = 1; i < bList.Count; i++)
                bList[i].ButtonOffset = (bList[i - 1].ButtonOffset + new Vector2(bList[i - 1].ButtonSprite.SourceRectangle.Width - 1, 0)).ToIntegerDomain();
        }

        public void RealocateElements(Vector2 delta)
        {
            ButtonList.ForEach((x) => x.ButtonOffset += delta);
        }

        public void UpdateAttatchMentPosition()
        {
            ButtonList.ForEach((x) => x.UpdateAttatchedPosition());
        }

        public void Update()
        {
            ButtonList.ForEach((x) => x.Update());
            unusedEquippedButtonList.ForEach((x) => ButtonList.Remove(x));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
