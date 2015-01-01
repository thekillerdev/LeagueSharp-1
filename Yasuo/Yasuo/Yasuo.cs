using System.Collections.Generic;
using LeagueSharp;

namespace Yasuo
{
    public class Yasuo
    {
        public static Obj_AI_Hero Player;
        public static YasuoMenu Menu;
        public static bool ShouldDash;
        public static List<YasuoInterrupter> Interrupters = new List<YasuoInterrupter>();
    }
}