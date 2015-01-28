using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jayce
{
    internal class RangedSpells : ISpellManager
    {
        public RangedSpells()
        {
            Spells = new Dictionary<SpellSlot, SpellStage[]>
            {
                {
                    SpellSlot.Q,
                    new[]
                    {
                        new SpellStage(new Spell(SpellSlot.Q, 1050f, TargetSelector.DamageType.Magical)),
                        new SpellStage(new Spell(SpellSlot.Q, 1470f, TargetSelector.DamageType.Magical))
                    }
                },
                { SpellSlot.W, new[] { new SpellStage(new Spell(SpellSlot.W)) } },
                { SpellSlot.E, new[] { new SpellStage(new Spell(SpellSlot.E, 650f)) } },
                { SpellSlot.R, new[] { new SpellStage(new Spell(SpellSlot.R)) } }
            };

            var q = Spells[SpellSlot.Q];
            q[0].Spell.SetSkillshot(0.25f, 70f, 1450f, true, SkillshotType.SkillshotLine);
            q[1].Spell.SetSkillshot(0.25f, 70f, 2350f, true, SkillshotType.SkillshotLine);

            IsRanged = true;
        }

        public Dictionary<SpellSlot, SpellStage[]> Spells { get; set; }
        public bool IsRanged { get; set; }
    }
}