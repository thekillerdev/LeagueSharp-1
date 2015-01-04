namespace Yasuo
{
    public class YasuoInterrupter
    {
        private readonly string champion;
        private readonly string menuString;
        private readonly string spellName;
        private readonly string menuStringDisplay;

        public YasuoInterrupter()
        {
            Yasuo.Interrupters.Add(
                new YasuoInterrupter("Caitlyn", "CaitlynAceintheHole", "Interrupt Caitlyn's Ace in the Hole"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Nunu", "AbsoluteZero", "Interrupt Nunu's Absolute Zero"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "FiddleSticks", "Crowstorm", "Interrupt Fiddlesticks' Crowstorm"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "FiddleSticks", "DrainChannel", "Interrupt Fiddlesticks' Drain Channel"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Galio", "GalioIdolOfDurand", "Interrupt Galio's Idol of Durand"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Karthus", "FallenOne", "Interrupt Karthus' Requiem"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Katarina", "KatarinaR", "Interrupt Katarina's Death Lotus"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Malzahar", "AlZaharNetherGrasp", "Interrupt Malzahar's Nether Grasp"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "MissFortune", "MissFortuneBulletTime", "Interrupt Miss Fortune's Bullet Time"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Pantheon", "Pantheon_GrandSkyfall_Jump", "Interrupt Pantheon's Grand Skyfall"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Shen", "ShenStandUnited", "Interrupt Shen's Stand United"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Urgot", "UrgotSwap2", "Interrupt Urgot's Swap"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter(
                    "Warwick", "InfiniteDuress", "Interrupt Warwick's Infinite Duress"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter("Varus", "VarusQ", "Interrupt Varus' Piercing Arrow"));
            Yasuo.Interrupters.Add(
                new YasuoInterrupter("Sion", "SionQ", "Interrupt Sion's Decimating Smash"));
        }

        public YasuoInterrupter(string champion, string spellName, string displayName)
        {
            this.champion = champion;
            this.spellName = spellName;
            menuString = ".interrupter."+champion+"."+spellName;
            menuStringDisplay = displayName;
        }

        public string GetChampion()
        {
            return champion;
        }

        public string GetSpellName()
        {
            return spellName;
        }

        public string GetMenuString()
        {
            return menuString;
        }

        public string GetMenuDisplayString()
        {
            return menuStringDisplay;
        }
    }
}