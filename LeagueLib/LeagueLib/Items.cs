using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueLib
{
    public class Items
    {
        private static Tuple<int, string, int, int, int, List<string>, List<int>>[] items =
        {
            // BASIC
            Tuple.Create(1052, "Amplifying Tome", 0, 435, 305, new List<string> {"+20 ability power"}, new List<int>{}),
            Tuple.Create(3301, "Ancient Coin", 0, 365, 146, new List<string> {"+25% Mana Regen per 5 seconds"}, new List<int>{}),
            Tuple.Create(1038, "B. F. Sword", 0, 1550, 1085, new List<string> {"+50 attack damage"}, new List<int>{}),
            Tuple.Create(1026, "Blasting Wand", 0, 860, 602, new List<string> {"+40 ability power"}, new List<int>{}),
            Tuple.Create(1001, "Boots of Speed", 0, 325, 227, new List<string> {}, new List<int>{}),
            Tuple.Create(1051, "Brawler's Gloves", 0, 400, 280, new List<string> {"+8% critical strike chance"}, new List<int>{}),
        };

        #region GetItem
        public static int GetItem(string itemName)
        {
            for(int i = 0; i < items.Length; ++i)
                if (items[i].Item2.Equals(itemName))
                    return i;

            return -1;
        }

        public static int GetItem(int itemID)
        {
            for (int i = 0; i < items.Length; ++i)
                if (items[i].Item1 == itemID)
                    return i;

            return -1;
        }
        #endregion

        #region GetData
        public static void GetData(int iDataID, out int itemID, out string itemName, out int type, out int price, out int sell, out List<string> stats, out List<int> buildsOutOf)
        {
            if (iDataID < items.Length)
            {
                itemID = items[iDataID].Item1;
                itemName = items[iDataID].Item2;
                type = items[iDataID].Item3;
                price = items[iDataID].Item4;
                sell = items[iDataID].Item5;
                stats = items[iDataID].Item6;
                buildsOutOf = items[iDataID].Item7;
                return;
            }
            itemID = -1;
            itemName = null;
            type = -1;
            price = -1;
            sell = -1;
            stats = null;
            buildsOutOf = null;
        }

        public static List<object> GetData(int iDataID)
        {
            if(iDataID < items.Length)
            {
                List<object> list = new List<object>();
                list.Add(items[iDataID].Item1);
                list.Add(items[iDataID].Item2);
                list.Add(items[iDataID].Item3);
                list.Add(items[iDataID].Item4);
                list.Add(items[iDataID].Item5);
                list.Add(items[iDataID].Item6);
                list.Add(items[iDataID].Item7);
                return list;
            }
            return null;
        }

        public static int GetItemID(int iDataID)
        {
            if (iDataID < items.Length)
                return items[iDataID].Item1;
            return -1;
        }

        public static string GetItemName(int iDataID)
        {
            if (iDataID < items.Length)
                return items[iDataID].Item2;
            return null;
        }

        public static int GetItemType(int iDataID)
        {
            if (iDataID < items.Length)
                return items[iDataID].Item3;
            return -1;
        }

        public static int GetItemPrice(int iDataID)
        {
            if (iDataID < items.Length)
                return items[iDataID].Item4;
            return -1;
        }

        public static int GetItemSellValue(int iDataID)
        {
            if (iDataID < items.Length)
                return items[iDataID].Item5;
            return -1;
        }

        public static List<string> GetItemStats(int iDataID)
        {
            if (iDataID < items.Length)
                return items[iDataID].Item6;
            return null;
        }

        public static List<int> GetItemBuildsOutOf(int iDataID)
        {
            if (iDataID < items.Length)
                return items[iDataID].Item7;
            return null;
        }
        #endregion
    }
}
