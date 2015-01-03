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
    }
}