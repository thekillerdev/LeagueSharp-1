#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

#endregion

namespace LeagueLib
{
    public class ShopAI
    {
        private readonly Hashtable shop;
        private int shopIndex;
        private const int maxShop = 10;

        public ShopAI(Hashtable shop = null, int shopIndex = int.MaxValue)
        {
            this.shop = shop ?? new Hashtable();

            if (shop.Count > 0 && shop.Count < maxShop)
                this.shopIndex = shop.Count;
            else if (shop.Count < maxShop)
                this.shopIndex = 0;
            else
                this.shopIndex = maxShop;
        }

        public int AddItem(int itemId)
        {
            Console.WriteLine("debug: " + Items.GetItem(itemId));
            return this.AddItem(new ShopItem(Items.GetItem(itemId)));
        }

        public int AddItem(ItemId itemId)
        {
            Console.WriteLine("debug: " + Items.GetItem((int)itemId));
            return this.AddItem(new ShopItem(Items.GetItem((int)itemId)));
        }

        public int AddItem(string itemName)
        {
            Console.WriteLine("debug: " + Items.GetItemByName(itemName));
            return this.AddItem(new ShopItem(Items.GetItemByName(itemName)));
        }

        public int AddItem(ShopItem shopItem)
        {
            if (shop.ContainsKey(shopIndex) && shopIndex < maxShop)
                GetFreeIndex();

            if(shopIndex < maxShop)
            {
                shop.Add(shopIndex, shopItem);
                ++shopIndex;
                return shopIndex;
            }

            return -1;
        }

        public bool RemoveItem(ShopItem shopItem)
        {
            if (shop.ContainsValue(shopItem))
                foreach (var i in shop.Keys)
                    if (shop[i].Equals(shopItem))
                        return RemoveItem((int)i);

            return false;
        }

        public bool RemoveItem(int shopIndex)
        {
            if (shopIndex < maxShop && shop.ContainsKey(shopIndex))
            {
                shop.Remove(shopIndex);
                return true;
            }
            return false;
        }

        private int GetFreeIndex()
        {
            for(var c = 0; c < maxShop; ++c)
                if (!shop.ContainsKey(c))
                    return c;

            return maxShop;
        }
    }

    public class ShopItem
    {
        private enum Command
        {
            Buy,
            Sell,
            Idle
        }

        private readonly ItemId itemId;
        private readonly int itemFullPrice;
        private readonly int itemPrice;
        private readonly int[] itemComponents;

        private bool itemBought;
        private Command itemOrder;

        public ShopItem(Items.Item item)
        {
            itemId = (ItemId)item.GetId();
            itemFullPrice = item.GetFullPriceValue();
            itemPrice = item.GetPriceValue();
            itemComponents = item.GetComponents();
        }

        public void Buy()
        {
            
        }

        public void Sell()
        {

        }

        public bool IsProtected()
        {
            return itemBought ? true : false;
        }
    }
}
