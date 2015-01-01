using LeagueSharp.Common;

namespace Yasuo
{
    public class YasuoAutoLevel
    {
        private readonly AutoLevel autoLevel;
        private readonly int[] playerLevel = { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };

        public YasuoAutoLevel(bool enabled)
        {
            autoLevel = new AutoLevel(playerLevel);
            AutoLevel.Enabled(enabled);
        }

        public void Enabled(bool flag)
        {
            AutoLevel.Enabled(flag);
        }

        public AutoLevel GetAutoLevel()
        {
            return autoLevel;
        }
    }
}