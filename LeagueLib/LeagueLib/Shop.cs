#region

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace LeagueLib
{
    public class Shop
    {
        private int failCount;
        private readonly int MAX_SHOP_ITEMS = 7;
        private readonly Hashtable shopItems = new Hashtable();

        public void AddList(List<ItemId> items)
        {
            var list = items;
            foreach (
                var i in from item in ObjectManager.Player.InventoryItems from i in items where item.Id == i select i)
            {
                list.Remove(i);
            }

            foreach (var item in list)
            {
                Add(new ShopItem(Items.GetItem(item)));
            }
        }

        public void Add(ShopItem shopItem)
        {
            var i = GetIndex();
            if (i != -1)
            {
                shopItems.Add(i, shopItem);
            }
        }

        public void Remove(ShopItem shopItem)
        {
            foreach (var pair in
                from DictionaryEntry pair in shopItems let item = pair.Value where item == shopItem select pair)
            {
                shopItems.Remove(pair.Key);
            }
        }

        private int GetIndex()
        {
            for (var i = 0; i < MAX_SHOP_ITEMS; ++i)
            {
                if (!shopItems.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Tick()
        {
            if (ObjectManager.Player.GoldCurrent < 10)
            {
                return true;
            }

            for (var i = 0; i < MAX_SHOP_ITEMS; ++i)
            {
                var item = (ShopItem) shopItems[i];
                if (item.IsBought() || ObjectManager.Player.GoldCurrent < item.GetPrice())
                {
                    continue;
                }
                item.Buy();
                failCount = 0;
                break;
            }
            failCount++;
            return failCount > 5;
        }
    }

    public class ShopItem
    {
        private bool isBought;
        private readonly List<Item> componentList;
        private readonly Item item;
        private readonly int totalPrice;

        public ShopItem(Item item)
        {
            this.item = item;
            componentList = item.GetCopmponentList();
            totalPrice = item.GetTotalPrice();
        }

        public Item GetItem()
        {
            return item;
        }

        public int GetPrice()
        {
            return item.GetPrice();
        }

        public bool IsBought()
        {
            return isBought;
        }

        public void Buy()
        {
            if (isBought)
            {
                return;
            }

            var gold = ObjectManager.Player.GoldCurrent;

            // can afford full item
            if (gold >= item.GetTotalPrice())
            {
                Game.OnGameProcessPacket += Game_OnGameProcessPacket;
                ObjectManager.Player.BuyItem(item.GetItemId());
                return;
            }

            if (componentList == null || componentList.Count == 0)
            {
                return;
            }

            // buy components
            foreach (var componentItem in componentList.Where(componentItem => ObjectManager.Player.Gold > totalPrice))
            {
                ObjectManager.Player.BuyItem(componentItem.GetItemId());
            }
        }

        private void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] != Packet.S2C.BuyItemAns.Header ||
                Packet.S2C.BuyItemAns.Decoded(args.PacketData).Item.Id != item.GetId())
            {
                return;
            }
            isBought = true;
        }

        public bool SellItem()
        {
            return false;
        }
    }
}