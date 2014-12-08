using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueLib
{
    public class Shop
    {
        private Hashtable shopItems = new Hashtable();
        private readonly int MAX_SHOP_ITEMS = 7;

        public Shop() { }

        public void Add(ShopItem shopItem)
        {
            int i = GetIndex();
            if(i != -1)
                shopItems.Add(i, shopItem);
        }

        public void Remove(ShopItem shopItem)
        {
            for (int i = 0; i < MAX_SHOP_ITEMS; ++i)
                if (shopItems[i].Equals(shopItem))
                    shopItems.Remove(i);
        }

        private int GetIndex()
        {
            for(int i = 0; i < MAX_SHOP_ITEMS; ++i)
            {
                if(!shopItems.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public class ShopItem
    {
        private readonly Item item;
        
        public ShopItem(Item item)
        {
            this.item = item;
        }

        public Item GetItem()
        {
            return this.item;
        }

        public bool BuyItem()
        {
            return false;
        }

        public bool SellItem()
        {
            return false;
        }
    }
}
