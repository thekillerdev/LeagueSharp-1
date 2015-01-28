using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jayce
{
    internal class MeleeSpells : ISpellManager
    {
        public MeleeSpells()
        {
            Spells = new Dictionary<SpellSlot, SpellStage[]>
            {
                { SpellSlot.Q, new[] { new SpellStage(new Spell(SpellSlot.Q, 600f)) } },
                {
                    SpellSlot.W, new[] { new SpellStage(new Spell(SpellSlot.W, 285f, TargetSelector.DamageType.Magical)) }
                },
                { SpellSlot.E, new[] { new SpellStage(new Spell(SpellSlot.E, 240f)) } },
                { SpellSlot.R, new[] { new SpellStage(new Spell(SpellSlot.R)) } }
            };
        }

        public Dictionary<SpellSlot, SpellStage[]> Spells { get; set; }
        public bool IsRanged { get; set; }
    }
}