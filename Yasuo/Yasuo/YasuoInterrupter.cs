namespace Yasuo
{
    public class YasuoInterrupter
    {
        private readonly string champion;
        private readonly string menuString;
        private readonly string spellName;
        private readonly string menuStringDisplay;

        public YasuoInterrupter(string champion, string spellName, string menuString, string menuStringDisplay)
        {
            this.champion = champion;
            this.spellName = spellName;
            this.menuString = menuString;
            this.menuStringDisplay = menuStringDisplay;
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
            return menuStringDisplay;
        }

        public string GetMenuDisplayString()
        {
            return menuString;
        }
    }
}