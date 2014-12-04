#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LeagueLib
{
    public class Champions
    {
        public enum Role
        {
            Mage,
            Tank,
            Support,
            Fighter,
            Assassin,
            Marksman,
            None
        }

        private static readonly Champion[] pre_champions =
        {
            new Champion(1, "Annie", "the Dark Child", Role.Mage, Role.None, new Power(2, 3, 10, 6), new Stats()),
            new Champion(2, "Olaf", "the Berserker", Role.Fighter, Role.Tank, new Power(9, 5, 3, 3), new Stats()),
            new Champion(3, "Galio", "the Sentinel's Sorrow", Role.Tank, Role.Mage, new Power(3, 7, 6, 3), new Stats()),
            new Champion(4, "Twisted Fate", "the Card Master", Role.Mage, Role.None, new Power(6, 2, 6, 9), new Stats()),
            new Champion(5, "Xin Zhao", "the Senechal of Demacia", Role.Fighter, Role.Assassin, new Power(8, 6, 3, 3), new Stats()),
            new Champion(6, "Urgot", "the Headsman's Pride", Role.Marksman, Role.Fighter, new Power(8, 5, 3, 8), new Stats()),
            new Champion(7, "LeBlanc", "the Deceiver", Role.Assassin, Role.Mage, new Power(1, 4, 10, 9), new Stats()),
            new Champion(8, "Vladimir", "the Crimson Reaper", Role.Mage, Role.Tank, new Power(2, 6, 8, 7), new Stats()),
            new Champion(9, "Fiddlesticks", "the Harbinger of Doom", Role.Mage, Role.Tank, new Power(2, 3, 9, 9), new Stats()),
            new Champion(10, "Kayle", "the Judicator", Role.Fighter, Role.Support, new Power(6, 6, 7, 7), new Stats())
        };
        private static readonly List<Champion> champions = new List<Champion>(pre_champions);

        public static Champion GetChampionByName(string name)
        {
            return champions.FirstOrDefault(i => i.GetName().Equals(name));
        }

        public static Champion GetChampion(int champID)
        {
            return champions.FirstOrDefault(i => i.GetId() == champID);
        }

        public List<Champion> GetComponents()
        {
            return pre_champions.ToList();
        }

        public class Champion
        {
            private readonly int champID;
            private readonly string champName;
            private readonly string champDesc;
            private readonly Role primaryRole;
            private readonly Role secondaryRole;
            private readonly Power champPower;
            private readonly Stats champStats;

            public Champion(int champID, string champName, string champDesc, Role primaryRole, Role secondaryRole, Power power, Stats stats)
            {
                this.champID = champID;
                this.champName = champName;
                this.champDesc = champDesc;
                this.primaryRole = primaryRole;
                this.secondaryRole = secondaryRole;
                this.champPower = power;
                this.champStats = stats;
            }

            public int GetId()
            {
                return champID;
            }

            public string GetName()
            {
                return champName;
            }

            public string GetDescription()
            {
                return champDesc;
            }

            public Role GetPrimaryRole()
            {
                return primaryRole;
            }

            public Role GetSecondaryRole()
            {
                return secondaryRole;
            }

            public Power GetPower()
            {
                return champPower;
            }

            public Stats GetStats()
            {
                return champStats;
            }

            public override string ToString()
            {
                return champName + "[" + champID + "]";
            }
        }

        public class Power
        {
            private readonly int attackPower;
            private readonly int defensePower;
            private readonly int abilityPower;
            private readonly int difficultyPower;

            public Power(int attackPower, int defensePower, int abilityPower, int difficultyPower)
            {
                this.attackPower = attackPower;
                this.defensePower = defensePower;
                this.abilityPower = abilityPower;
                this.difficultyPower = difficultyPower;
            }

            public int getAttackPower()
            {
                return attackPower;
            }

            public int getDefensePower()
            {
                return defensePower;
            }

            public int getAbilityPower()
            {
                return abilityPower;
            }

            public int getDifficultyPower()
            {
                return difficultyPower;
            }

            public override string ToString()
            {
                return "attackPower[" + attackPower + "] / defensePower[" + defensePower + "] / abilityPower[" + abilityPower + "] / difficultyPower[" + difficultyPower + "]";
            }
        }

        public class Stats
        {
            public Stats()
            {

            }
        }
    }
}