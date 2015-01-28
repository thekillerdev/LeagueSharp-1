using LeagueSharp.Common;

namespace Jayce
{
    internal struct SpellStage
    {
        public Spell Spell;

        public SpellStage(Spell spell)
        {
            Spell = spell;
        }
    }
}