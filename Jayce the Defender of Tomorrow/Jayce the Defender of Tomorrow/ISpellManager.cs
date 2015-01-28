using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jayce
{
    internal interface ISpellManager
    {
        Dictionary<SpellSlot, SpellStage[]> Spells { get; set; }
        bool IsRanged { get; set; }
    }
}