using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jayce
{
    internal class SpellManager
    {
        private static readonly List<ISpellManager> SpellsValues = new List<ISpellManager>();
 
        static SpellManager()
        {
            SpellsValues.Add(new MeleeSpells());
            SpellsValues.Add(new RangedSpells());
        }

        public static ISpellManager GetSpellManager(Obj_AI_Hero player)
        {
            return player.HasBuff("jaycestancehammer") ? SpellsValues[0] : SpellsValues[1];
        }
    }
}