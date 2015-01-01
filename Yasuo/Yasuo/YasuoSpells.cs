using LeagueSharp;
using LeagueSharp.Common;

namespace Yasuo
{
    public class YasuoSpells
    {
        /// <summary>
        ///     Steel Tempest Spell
        /// </summary>
        public static Spell Q = new Spell(SpellSlot.Q, 475f); // Original: 475f

        /// <summary>
        ///     Steel Tempest Whirlwind Passive
        /// </summary>
        public static Spell QWind = new Spell(SpellSlot.Q, 900f); // Original: 900f 

        /// <summary>
        ///     Wind Wall Spell
        /// </summary>
        public static Spell W = new Spell(SpellSlot.W, 900f);

        /// <summary>
        ///     Sweeping Blade Spell
        /// </summary>
        public static Spell E = new Spell(SpellSlot.E, 475f);

        /// <summary>
        ///     Last Breath Spell
        /// </summary>
        public static Spell R = new Spell(SpellSlot.R, 1200f);

        /// <summary>
        ///     Ravenous Hydra Item
        /// </summary>
        public static ItemData.Item RavenousHydra = ItemData.Ravenous_Hydra_Melee_Only;

        /// <summary>
        ///     Tiamat Item
        /// </summary>
        public static ItemData.Item Tiamat = ItemData.Tiamat_Melee_Only;

        /// <summary>
        ///     Blade of the Ruined King Item
        /// </summary>
        public static ItemData.Item BladeoftheRuinedKing = ItemData.Blade_of_the_Ruined_King;

        /// <summary>
        ///     Bilgewater Cutlass Item
        /// </summary>
        public static ItemData.Item BilgewaterCutlass = ItemData.Bilgewater_Cutlass;

        static YasuoSpells()
        {
            Q.SetSkillshot(0.36f, 350f, 20000f, false, SkillshotType.SkillshotLine);
            QWind.SetSkillshot(0.36f, 120f, 1200f, true, SkillshotType.SkillshotLine);
        }
    }
}