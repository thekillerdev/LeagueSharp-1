#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace LeagueLib
{
    public class ShopAI
    {
        private const int maxShop = 10;
        private int shopIndex;
        private readonly Hashtable shop;

        public ShopAI(Hashtable shop = null, int shopIndex = int.MaxValue)
        {
            this.shop = shop ?? new Hashtable();

            if (shop == null)
            {
                return;
            }

            if (shop.Count > 0 && shop.Count < maxShop)
            {
                this.shopIndex = shop.Count;
            }
            else if (shop.Count < maxShop)
            {
                this.shopIndex = 0;
            }
            else
            {
                this.shopIndex = maxShop;
            }
        }

        public int AddItem(int itemId)
        {
            Console.WriteLine("debug: " + Items.GetItem(itemId));
            return AddItem(new ShopItem(Items.GetItem(itemId)));
        }

        public int AddItem(ItemId itemId)
        {
            Console.WriteLine("debug: " + Items.GetItem((int) itemId));
            ShopItem shopitem;
            try
            {
                shopitem = new ShopItem(Items.GetItem((int) itemId));
            }
            catch
            {
                Console.WriteLine(itemId.ToString() + " failed!");
                return -1;
            }
            return AddItem(shopitem);
        }

        public int AddItem(string itemName)
        {
            Console.WriteLine("debug: " + Items.GetItemByName(itemName));
            return AddItem(new ShopItem(Items.GetItemByName(itemName)));
        }

        public int AddItem(ShopItem shopItem)
        {
            if (shop.ContainsKey(shopIndex) && shopIndex < maxShop)
            {
                GetFreeIndex();
            }

            if (shopIndex >= maxShop)
            {
                return -1;
            }

            shop.Add(shopIndex, shopItem);
            ++shopIndex;
            return shopIndex;
        }

        public void AddItemList(List<ItemId> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public bool RemoveItem(ShopItem shopItem)
        {
            return shop.ContainsValue(shopItem) &&
                   (from object i in shop.Keys where shop[i].Equals(shopItem) select RemoveItem((int) i)).FirstOrDefault
                       ();
        }

        public bool RemoveItem(int shopindex)
        {
            if (shopindex >= maxShop || !shop.ContainsKey(shopindex))
            {
                return false;
            }
            shop.Remove(shopindex);
            return true;
        }

        private int GetFreeIndex()
        {
            for (var c = 0; c < maxShop; ++c)
            {
                if (!shop.ContainsKey(c))
                {
                    return c;
                }
            }

            return maxShop;
        }

        public void Shop()
        {
            foreach (var item in shop.Values.Cast<ShopItem>().Where(item => !item.IsProtected()))
            {
                item.Buy();
            }
        }
    }

    public class ShopItem
    {
        private bool itemBought;
        private Command itemOrder;
        private readonly Item Item;
        private readonly List<ShopItem> itemComponents;
        private readonly int itemFullPrice;
        private readonly ItemId itemId;
        private readonly int itemPrice;

        public ShopItem(Item item)
        {
            Item = item;
            itemId = (ItemId) item.GetId();
            itemFullPrice = item.GetFullPriceValue();
            itemPrice = item.GetPrice();
            itemComponents = item.GetComponentsList();
        }

        public void Buy()
        {
            if (itemBought)
            {
                return;
            }

            var gold = ObjectManager.Player.GoldCurrent;

            // can afford full item
            if (gold >= itemFullPrice)
            {
                Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                Item.Buy();
                return;
            }

            // buy components
            foreach (var item in itemComponents.Where(item => ObjectManager.Player.Gold > item.itemPrice))
            {
                item.Buy();
            }
        }

        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] != Packet.S2C.BuyItemAns.Header ||
                Packet.S2C.BuyItemAns.Decoded(args.PacketData).Item.Id != Item.GetId())
            {
                return;
            }
            Console.WriteLine("bought item!");
            itemBought = true;
        }

        public void Sell() { }

        public bool IsProtected()
        {
            return itemBought;
        }

        private enum Command
        {
            Buy,
            Sell,
            Idle
        }
    }
}