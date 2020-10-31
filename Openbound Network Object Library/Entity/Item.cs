using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.Entity
{
    public enum ItemType
    {
        //Red - 2 Slots
        Dual,
        DualPlus,
        Thunder,

        //Green - 2 Slots
        EnergyUp2,

        //Purple - 2 Slots
        TeamTeleport,
        Teleport,

        //Red - 1 Slot
        Blood,
        BungeShot,
        PowerUp,

        //Blue - 1 Slot
        ChangeWind,

        //Green - 1 Slot
        EnergyUp1,
    }

    public enum ItemCategory
    {
        Red, Green, Blue, Purple
    }

    public class Item
    {
        public static readonly List<Item> ItemPresets = new List<Item>()
        {
            //2 Slots item
            new Item(NetworkObjectParameters.InGameItemDualCost,         ItemCategory.Red,    ItemType.Dual,         2),
            new Item(NetworkObjectParameters.InGameItemDualPlusCost,     ItemCategory.Red,    ItemType.DualPlus,     2),
            new Item(NetworkObjectParameters.InGameItemThunderCost,      ItemCategory.Red,    ItemType.Thunder,      2),
            new Item(NetworkObjectParameters.InGameItemEnergyUp2Cost,    ItemCategory.Green,  ItemType.EnergyUp2,    2),
            new Item(NetworkObjectParameters.InGameItemTeamTeleportCost, ItemCategory.Purple, ItemType.TeamTeleport, 2),
            new Item(NetworkObjectParameters.InGameItemTeleportCost,     ItemCategory.Purple, ItemType.Teleport,     2),

            //1 Slot item
            new Item(NetworkObjectParameters.InGameItemBloodCost,        ItemCategory.Red,    ItemType.Blood,        1),
            new Item(NetworkObjectParameters.InGameItemBungeShotCost,    ItemCategory.Red,    ItemType.BungeShot,    1),
            new Item(NetworkObjectParameters.InGameItemPowerUpCost,      ItemCategory.Red,    ItemType.PowerUp,      1),
            new Item(NetworkObjectParameters.InGameItemChangeWindCost,   ItemCategory.Blue,   ItemType.ChangeWind,   1),
            new Item(NetworkObjectParameters.InGameItemEnergyUp1Cost,    ItemCategory.Green,  ItemType.EnergyUp1,    1),
        };

        public ItemType ItemType;
        public ItemCategory ItemCategory;
        public int ItemCost;
        public short Slots;

        public Item(int itemCost, ItemCategory itemCategory, ItemType itemType, short slots)
        {
            ItemType = itemType;
            ItemCategory = itemCategory;
            ItemCost = itemCost;
            Slots = slots;
        }
    }
}
