using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

namespace LeagueLib
{
    public class ShopAI
    {
        private readonly Hashtable shop;
        private int? shopIndex;

        public ShopAI(Hashtable shop = null, int? shopIndex = int.MaxValue)
        {
            this.shop = shop ?? new Hashtable();
            this.shopIndex = shopIndex ?? int.MaxValue;

            if (shopIndex == int.MaxValue && shop.Count < int.MaxValue)
                shopIndex = (shop.Count + 1);
        }

        public bool AddItem(ShopItem shopItem)
        {
            var index = shopIndex;
            if (shop.ContainsKey(shopIndex))
                index = GetFreeIndex();

            if(index < int.MaxValue)
            {
                shop.Add(index, shopItem);
                shopIndex = GetFreeIndex();
                return true;
            }
            return false;
        }

        public bool RemoveItem(int itemIndex, bool force = false)
        {
            if(shop.ContainsKey(itemIndex))
            {
                if (((ShopItem)shop[itemIndex]).IsProtected() && !force)
                    return false;
                shop.Remove(itemIndex);
                return true;
            }
            return false;
        }

        private int GetFreeIndex()
        {
            var fIndex = 0;
            while (fIndex != int.MaxValue)
            {
                if (!shop.ContainsKey(fIndex))
                    return fIndex;
                ++fIndex;
            }
            return int.MaxValue;
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
