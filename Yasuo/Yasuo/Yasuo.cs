using System.Collections.Generic;
using LeagueSharp;
using Yasuo.Evade;

namespace Yasuo
{
    public class Yasuo
    {
        public static Obj_AI_Hero Player;
        public static YasuoMenu Menu;
        public static bool ShouldDash;
        public static List<YasuoInterrupter> Interrupters = new List<YasuoInterrupter>();
        public static List<Skillshot> DetectedSkillShots = new List<Skillshot>();
        public static List<Skillshot> EvadeDetectedSkillshots = new List<Skillshot>();

        public static List<MenuData> MenuWallsList = new List<MenuData>();
        public static List<MenuData> MenuDashesList = new List<MenuData>();

        public struct MenuData
        {
            public string ChampionName;
            public string SpellName;
            public string SpellDisplayName;
            public string DisplayName;
            public string Slot;
            public bool IsWindwall;

            public void AddToMenu()
            {
                if (
                    Menu.GetItem(
                        ((IsWindwall) ? YasuoMenu.AutoWindWallLoc : YasuoMenu.EvadeLoc) + "." + ChampionName + "." +
                        Slot) == null)
                {
                    Menu.AddItem(
                        (IsWindwall) ? YasuoMenu.AutoWindMenu : YasuoMenu.EvadeMenu, SpellDisplayName,
                        ((IsWindwall) ? YasuoMenu.AutoWindWallLoc : YasuoMenu.EvadeLoc) + "." + ChampionName + "." +
                        Slot).SetValue(true);
                }
            }
        }
    }
}