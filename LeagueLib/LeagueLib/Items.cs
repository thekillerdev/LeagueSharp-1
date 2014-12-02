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

        private static Tuple<int, string, Tier, Tuple<int, int>, float, Tuple<List<string>, List<int>>>[] items =
        {
            // Amplifying Tome
            Tuple.Create(1052, "Amplifying Tome", Tier.Basic, new Tuple<int, int>(435, 305), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+20 ability power"}, new List<int>{})),

            // Amplifying Tome -> Aether Wisp
            Tuple.Create(3113, "Aether Wisp", Tier.Advanced, new Tuple<int, int>(515, 665), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+30 ability power"}, new List<int>{1052})),

            // Amplifying Tome -> Fiendish Codex
            Tuple.Create(3108, "Fiendish Codex", Tier.Advanced, new Tuple<int, int>(385, 574), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+30 ability power"}, new List<int>{1052})),

            // Ruby Crystal
            Tuple.Create(1028, "Ruby Crystal", Tier.Basic, new Tuple<int, int>(400, 280), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+150 health"}, new List<int>{})),

            // Amplifying Tome + Ruby Crystal -> Haunting Guise
            Tuple.Create(3136, "Haunting Guise", Tier.Advanced, new Tuple<int, int>(640, 1040), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+25 ability power","+200 health"}, new List<int>{1052, 1028})),

            // Amplifying Tome + Amplifying Tome -> Hextech Revolver
            Tuple.Create(3145, "Hextech Revolver", Tier.Advanced, new Tuple<int, int>(330, 840), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+40 ability power"}, new List<int>{1052, 1052})),

            // Haunting Guise + Amplifying Tome -> Liandry's Torment
            Tuple.Create(3151, "Liandry's Torment", Tier.Legendary, new Tuple<int, int>(980, 2030), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+50 ability power","+300 health"}, new List<int>{3145, 1052})),

            // Amplifying Tome -> Mejai's Soulstealer
            Tuple.Create(3041, "Mejai's Soulstealer", Tier.Advanced, new Tuple<int, int>(965, 980), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+20 ability power"}, new List<int>{1052})),

            // Blasting Wand
            Tuple.Create(1026, "Blasting Wand", Tier.Basic, new Tuple<int, int>(860, 602), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+40 ability power"}, new List<int>{})),

            // Giant's Belt
            Tuple.Create(1011, "Giant's Belt", Tier.Basic, new Tuple<int, int>(1000, 700), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+380 health"}, new List<int>{})),

            // Amplifying Tome + Blasting Wand + Giant's Belt -> Rylai's Crystal Scepter
            Tuple.Create(3116, "Rylai's Crystal Scepter", Tier.Advanced, new Tuple<int, int>(605, 2030), 0f, new Tuple<List<string>, List<int>>(new List<string>{"+100 ability power","+400 health"}, new List<int>{1052, 1026, 1011})),

            // 
        };

        public static int GetItemByName(string name)
        {
            foreach (var item in items)
                if (item.Item2.Equals(name))
                    return item.Item1;
            return int.MinValue;
        }

        public static string GetName(int id)
        {
            foreach (var item in items)
                if (item.Item1 == id)
                    return item.Item2;
            return null;
        }

        public static Tier GetTier(int id)
        {
            foreach (var item in items)
                if (item.Item1 == id)
                    return item.Item3;
            return Tier.None;
        }

        public static int GetPriceValue(int id)
        {
            foreach (var item in items)
            {
                if (item.Item1 == id)
                {
                    int price = item.Item4.Item1;
                    foreach(var iGold in item.Item6.Item2)
                    {
                        price += GetPriceValue(iGold);
                    }
                    return price;
                }
            }
            return int.MinValue;
        }

        public static int GetSellValue(int id)
        {
            foreach (var item in items)
                if (item.Item1 == id)
                    return item.Item4.Item2;
            return int.MinValue;
        }

        public static float GetRange(int id)
        {
            foreach (var item in items)
                if (item.Item1 == id)
                    return item.Item5;
            return float.MinValue;
        }
    }
}
