#region

using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

#endregion

namespace LeagueLib
{
    public class Items
    {
        public static Item GetItemByName(string name)
        {
            return items.FirstOrDefault(i => i.GetName().Equals(name));
        }

        public static Item GetItem(int itemID)
        {
            return items.FirstOrDefault(i => i.GetId() == itemID);
        }

        internal static readonly Item[] pre_items =
        {
            new Item(1038, "B. F. Sword", 0, 1550, false, ItemClass.None, (ItemCategory.Damage), 0f, 50f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new Item[] {  }),
            new Item(3031, "Infinity Edge", 0, 645, true, ItemClass.None, (ItemCategory.CriticalStrike & ItemCategory.Damage), 0f, 80f, 0f, 0.25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new Item[] { GetItem(1038), GetItem(1037), GetItem(1018) }),
            new Item(2003, "Health Potion", 5, 35, false, ItemClass.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new Item[] {  }),
            new Item(2041, "Crystalline Flask", 0, 345, false, ItemClass.None, (ItemCategory.HealthRegen & ItemCategory.Consumable & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new Item[] {  }),
            new Item(3301, "Ancient Coin", 0, 365, false, ItemClass.None, (ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new Item[] {  }),

        };
        private static readonly List<Item> items = new List<Item>(pre_items);
    }
}