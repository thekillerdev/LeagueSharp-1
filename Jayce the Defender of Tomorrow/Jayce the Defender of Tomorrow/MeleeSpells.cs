using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jayce
{
    internal class MeleeSpells : ISpellManager
    {
        public MeleeSpells()
        {
            Spells = new Dictionary<SpellSlot, Spell[]>
            {
                { SpellSlot.Q, new[] { new Spell(SpellSlot.Q, 600f) } },
                { SpellSlot.W, new[] { new Spell(SpellSlot.W, 285f, TargetSelector.DamageType.Magical) } },
                { SpellSlot.E, new[] { new Spell(SpellSlot.E, 240f) } },
                { SpellSlot.R, new[] { new Spell(SpellSlot.R) } }
            };
        }

        public Dictionary<SpellSlot, Spell[]> Spells { get; set; }
        public bool IsRanged { get; set; }
    }
}