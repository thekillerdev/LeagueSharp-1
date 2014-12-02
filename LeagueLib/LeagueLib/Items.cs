using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueLib
{
    public class Items
    {
        public static enum Tier
        {
            Basic,
            Advanced,
            Legendary,
            Mythical,
            None
        }

        private static Item[] pre_items = new Item[]
        {
            new Item(1052, "Amplifying Tome", Tier.Basic, 435, 305, 0f, new List<int> {}),
            new Item(3113, "Ather Wisp", Tier.Advanced, 515, 665, 0f, new List<int> {1052}),
        };
        private static IEnumerable<Item> IEpre_items = pre_items;
        private static List<Item> items = new List<Item>(IEpre_items);

        public static Item GetItemByName(string name)
        {
            foreach (var item in items)
                if (item.GetName().Equals(name))
                    return item;
            return null;
        }

        private class Item
        {
            private readonly int itemID;
            private readonly string itemName;
            private readonly Tier itemTier;
            private readonly int priceValue;
            private readonly int sellValue;
            private readonly float itemRange;
            private readonly List<int> builtFrom;

            public Item(int itemID, string itemName, Tier itemTier, int priceValue, int sellValue, float itemRange, List<int> builtFrom)
            {
                this.itemID = itemID;
                this.itemName = itemName;
                this.itemTier = itemTier;
                this.priceValue = priceValue;
                this.sellValue = sellValue;
                this.itemRange = itemRange;
                this.builtFrom = builtFrom;
            }

            public int GetID()
            {
                return this.itemID;
            }

            public string GetName()
            {
                return this.itemName;
            }

            public Tier GetTier()
            {
                return this.itemTier;
            }

            public int GetPriceValue()
            {
                int price = this.priceValue;
                foreach(var i in builtFrom)
                {
                    foreach(var i2 in items)
                    {
                        if(i2.GetID() == i)
                        {
                            price += i2.GetPriceValue();
                        }
                    }
                }
                return price;
            }

            public int GetSellValue()
            {
                return this.sellValue;
            }

            public float GetRange()
            {
                return this.itemRange;
            }
        }
    }
}
