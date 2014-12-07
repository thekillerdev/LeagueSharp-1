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
        { // WILL UPDATE TOMORROW FULL LIST, IN PROGRESS.
            //new Item();
        };
        private static readonly List<Item> items = new List<Item>(pre_items);
    }
}