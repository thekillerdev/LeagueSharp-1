#region

using System;
using System.Collections.Generic;

#endregion

namespace LeagueLib
{
    public class Champions
    {
        #region champsData

        private static readonly Tuple<int, string, string, string, string, Tuple<int, int, int, int>>[] champions =
        {
            Tuple.Create(1, "Annie", "the Dark Child", "Mage", "N/A", Tuple.Create(2, 3, 10, 6)),
            Tuple.Create(2, "Olaf", "the Berserker", "Figther", "Tank", Tuple.Create(9, 5, 3, 3)),
            Tuple.Create(3, "Galio", "the Sentinel's Sorrow", "Tank", "Mage", Tuple.Create(3, 7, 6, 3)),
            Tuple.Create(4, "Twisted Fate", "the Card Master", "Mage", "N/A", Tuple.Create(6, 2, 6, 9)),
            Tuple.Create(5, "Xin Zhao", "the Senechal of Demacia", "Fighter", "Assassin", Tuple.Create(8, 6, 3, 3)),
            Tuple.Create(6, "Urgot", "the Headsman's Pride", "Marksman", "Figther", Tuple.Create(8, 5, 3, 8)),
            Tuple.Create(7, "LeBlanc", "the Deceiver", "Assassin", "Mage", Tuple.Create(1, 4, 10, 9)),
            Tuple.Create(8, "Vladimir", "the Crimson Reaper", "Mage", "Tank", Tuple.Create(2, 6, 8, 7)),
            Tuple.Create(9, "Fiddlesticks", "the Harbinger of Doom", "Mage", "Support", Tuple.Create(2, 3, 9, 9)),
            Tuple.Create(10, "Kayle", "the Judicator", "Fighter", "Support", Tuple.Create(6, 6, 7, 7)),
            Tuple.Create(266, "Aatrox", "the Darkin Blade", "Fighter", "Tank", Tuple.Create(8, 4, 3, 4)),
            Tuple.Create(103, "Ahri", "the Nine-Tailed Fox", "Mage", "Assassin", Tuple.Create(3, 4, 8, 5))
        };

        #endregion

        #region GetChampion

        public static int GetChampion(string championName)
        {
            for (var i = 0; i < champions.Length; ++i)
            {
                if (champions[i].Item2.Equals(championName))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int GetChampion(int championID)
        {
            for (var i = 0; i < champions.Length; ++i)
            {
                if (champions[i].Item1 == championID)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region GetData

        public static void GetData(int cDataID,
            out int champID,
            out string champName,
            out string champDesc,
            out string roleA,
            out string roleB,
            out int attackPower,
            out int defensePower,
            out int abilityPower,
            out int difficultyPower)
        {
            if (cDataID < champions.Length)
            {
                champID = champions[cDataID].Item1;
                champName = champions[cDataID].Item2;
                champDesc = champions[cDataID].Item3;
                roleA = champions[cDataID].Item4;
                roleB = champions[cDataID].Item5;
                attackPower = champions[cDataID].Item6.Item1;
                defensePower = champions[cDataID].Item6.Item2;
                abilityPower = champions[cDataID].Item6.Item3;
                difficultyPower = champions[cDataID].Item6.Item4;
                return;
            }
            champID = -1;
            champName = null;
            champDesc = null;
            roleA = null;
            roleB = null;
            attackPower = -1;
            defensePower = -1;
            abilityPower = -1;
            difficultyPower = -1;
        }

        public static List<object> GetData(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                var list = new List<object>();
                list.Add(champions[cDataID].Item1);
                list.Add(champions[cDataID].Item2);
                list.Add(champions[cDataID].Item3);
                list.Add(champions[cDataID].Item4);
                list.Add(champions[cDataID].Item5);
                list.Add(champions[cDataID].Item6.Item1);
                list.Add(champions[cDataID].Item6.Item2);
                list.Add(champions[cDataID].Item6.Item3);
                list.Add(champions[cDataID].Item6.Item4);
                return list;
            }
            return null;
        }

        public static int GetChampionID(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item1;
            }
            return -1;
        }

        public static string GetChampionName(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item2;
            }
            return null;
        }

        public static string GetChampionDesc(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item3;
            }
            return null;
        }

        public static string GetChampionRole(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item4;
            }
            return null;
        }

        public static string GetChampionSecondaryRole(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item5;
            }
            return null;
        }

        public static void GetChampionStats(int cDataID,
            out int attackPower,
            out int defensePower,
            out int abilityPower,
            out int difficultyPower)
        {
            if (cDataID < champions.Length)
            {
                attackPower = champions[cDataID].Item6.Item1;
                defensePower = champions[cDataID].Item6.Item2;
                abilityPower = champions[cDataID].Item6.Item3;
                difficultyPower = champions[cDataID].Item6.Item4;
                return;
            }
            attackPower = -1;
            defensePower = -1;
            abilityPower = -1;
            difficultyPower = -1;
        }

        public static List<int> GetChampionStats(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                var list = new List<int>();
                list.Add(champions[cDataID].Item6.Item1);
                list.Add(champions[cDataID].Item6.Item2);
                list.Add(champions[cDataID].Item6.Item3);
                list.Add(champions[cDataID].Item6.Item4);
                return list;
            }
            return null;
        }

        public static int GetChampionAttackPower(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item6.Item1;
            }
            return -1;
        }

        public static int GetChampionDefensePower(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item6.Item2;
            }
            return -1;
        }

        public static int GetChampionAbilityPower(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item6.Item3;
            }
            return -1;
        }

        public static int GetChampionDifficultyPower(int cDataID)
        {
            if (cDataID < champions.Length)
            {
                return champions[cDataID].Item6.Item4;
            }
            return -1;
        }

        #endregion
    }
}