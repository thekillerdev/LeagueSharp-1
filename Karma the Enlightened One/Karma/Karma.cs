using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Karma
{
    /// <summary>
    ///     Karma Class, contains main loading requirements.
    /// </summary>
    internal class Karma
    {
        public Karma(Obj_AI_Hero hero)
        {
            // If champion name does not equals Karma (class name)
            if (!hero.ChampionName.Equals(GetType().Name))
            {
                return;
            }

            // Save Player Instance
            Instances.Player = hero;

            // Add Spells
            Instances.Spells = new Dictionary<SpellSlot, Spell>
            {
                { SpellSlot.Q, new Spell(SpellSlot.Q, 1050f) },
                { SpellSlot.W, new Spell(SpellSlot.W, 675f) },
                { SpellSlot.E, new Spell(SpellSlot.E, 800f) },
                { SpellSlot.R, new Spell(SpellSlot.R, 950f) }
            };

            Instances.Spells[SpellSlot.Q].SetSkillshot(.25f, 65f, 1700f, true, SkillshotType.SkillshotLine);
            Instances.Spells[SpellSlot.R].SetSkillshot(.25f, 80f, 1700f, true, SkillshotType.SkillshotLine); // R+Q only

            // Add event listener onto OnGameUpdate delegate
            Game.OnGameUpdate += Game_OnGameUpdate;

            Game.PrintChat(
                "<font color=\"#be4cb7\"><b>L33T</b></font> | <font color=\"#1762a1\">Karma</font> the Enlightened One, loaded.");
        }

        /// <summary>
        ///     On Game Update event
        /// </summary>
        /// <param name="args">Unused System.EventArgs</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Instances.Orbwalker == null || Instances.Menu == null)
            {
                return;
            }

            switch (Instances.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Mechanics.ProcessCombo(Instances.Target);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mechanics.ProcessHarass(Instances.Target);
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Mechanics.ProcessLastHit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Mechanics.ProcessLaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Instances.Menu.Item("l33t.karma.flee.active").GetValue<bool>() &&
                        Instances.Menu.Item("l33t.karma.flee.activekey").GetValue<KeyBind>().Active)
                    {
                        Mechanics.ProcessFlee();
                    }
                    if (Instances.Menu.Item("l33t.karma.killsteal.active").GetValue<bool>())
                    {
                        Mechanics.ProcessKillsteal(Instances.Target);
                    }
                    break;
            }
        }

        /// <summary>
        ///     Menu Constructor
        /// </summary>
        /// <returns>Constructed Menu</returns>
        public Menu Menu()
        {
            var menu = new Menu("Karma", "l33t.karma", true);

            #region Target Selector

            var targetSelector = new Menu("Target Selector", "l33t.karma.ts");
            TargetSelector.AddToMenu(targetSelector);
            menu.AddSubMenu(targetSelector);

            #endregion

            #region Orbwalker

            var orbwalker = new Menu("Orbwalker", "l33t.karma.orbwalker");
            Instances.Orbwalker = new Orbwalking.Orbwalker(orbwalker);
            menu.AddSubMenu(orbwalker);

            #endregion

            #region Combo

            var combo = new Menu("Combo Settings", "l33t.combo");

            combo.AddItem(new MenuItem("l33t.karma.combo.q", "Use Inner Flame (Q)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.karma.combo.rq", "Use Soulflare (R + Q)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.karma.combo.w", "Use Focused Resolve (W)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.karma.combo.rw", "Use Renewal (R + W)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.karma.combo.e", "Use Inspire (E)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.karma.combo.re", "Use Defiance (R + E)")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.karma.combo.spacer0", ""));
            combo.AddItem(new MenuItem("l33t.karma.combo.useaoe", "Use Soulflare (Q) - Area of Effect")).SetValue(true);
            combo.AddItem(new MenuItem("l33t.karma.combo.minhpforrw", "Min % of Health for Renewal (R + W)"))
                .SetValue(new Slider(35));
            combo.AddItem(new MenuItem("l33t.karma.combo.minhpforre", "Min % of Health for Defiance (R + E)"))
                .SetValue(new Slider(35));
            combo.AddItem(new MenuItem("l33t.karma.combo.minalliesforre", "Min of Allies for Defiance (R + E)"))
                .SetValue(new Slider(3, 1, 5));
            combo.AddItem(new MenuItem("l33t.karma.combo.minenemiesforre", "Min of Enemies for Defiance (R + E)"))
                .SetValue(new Slider(3, 1, 5));
            combo.AddItem(new MenuItem("l33t.karma.combo.minmpforq", "Min % of Mana for Inner Flame/Soulflare"))
                .SetValue(new Slider(35));
            combo.AddItem(new MenuItem("l33t.karma.combo.minmpforw", "Min % of Mana for Focused Resolve/Renewal"))
                .SetValue(new Slider(35));
            combo.AddItem(new MenuItem("l33t.karma.combo.minmpfore", "Min % of Mana for Inspire/Defiance"))
                .SetValue(new Slider(35));

            menu.AddSubMenu(combo);

            #endregion

            #region Harass

            var harass = new Menu("Harass Settings", "l33t.karma.harass");

            harass.AddItem(new MenuItem("l33t.karma.harass.q", "Use Inner Flame (Q)")).SetValue(true);
            harass.AddItem(new MenuItem("l33t.karma.harass.rq", "Use Soulflare (R + Q)")).SetValue(true);
            harass.AddItem(new MenuItem("l33t.karma.harass.w", "Use Focused Resolve (W)")).SetValue(true);
            harass.AddItem(new MenuItem("l33t.karma.harass.rw", "Use Renewal (R + W)")).SetValue(true);
            harass.AddItem(new MenuItem("l33t.karma.harass.e", "Use Inspire (E)")).SetValue(true);
            harass.AddItem(new MenuItem("l33t.karma.harass.spacer0", ""));
            harass.AddItem(new MenuItem("l33t.karma.harass.minhprw", "Min % of Health for Renewal"))
                .SetValue(new Slider(55));
            harass.AddItem(new MenuItem("l33t.karma.harass.minmpw", "Min % of Mana for Renewal/Focued Resolve"))
                .SetValue(new Slider(35));
            harass.AddItem(new MenuItem("l33t.karma.harass.minmpq", "Min % of Mana for Inner Flame/Soulflare"))
                .SetValue(new Slider(35));

            menu.AddSubMenu(harass);

            #endregion

            #region Farming

            var farming = new Menu("Farming Settings", "l33t.karma.farming");

            farming.AddItem(new MenuItem("l33t.karma.farming.lhq", "[Last Hit] Use Inner Flame (Q)")).SetValue(true);
            farming.AddItem(new MenuItem("l33t.karma.farming.lcq", "[Lane Clear] Use Inner Flame (Q)")).SetValue(true);
            farming.AddItem(new MenuItem("l33t.karma.farming.lcrq", "[Lane Clear] Use Soulflare (R + Q)"))
                .SetValue(true);
            farming.AddItem(new MenuItem("l33t.karma.farming.spacer0", ""));
            farming.AddItem(new MenuItem("l33t.karma.farming.lhminmpq", "[Last Hit] Min % of Mana for Inner Flame"))
                .SetValue(new Slider(50));
            farming.AddItem(
                new MenuItem("l33t.karma.farming.lcminmpq", "[Lane Clear] Min % of Mana for Inner Flame/Soulflare"))
                .SetValue(new Slider(50));
            farming.AddItem(
                new MenuItem("l33t.karma.farming.lcminminions", "[Lane Clear] Min number of Minions for Soulflare"))
                .SetValue(new Slider(4, 1, 8));

            menu.AddSubMenu(farming);

            #endregion

            #region Killsteal

            var ks = new Menu("Killsteal Settings", "l33t.karma.killsteal");

            ks.AddItem(new MenuItem("l33t.karma.killsteal.enable", "Use Killsteal")).SetValue(true);
            ks.AddItem(new MenuItem("l33t.karma.killsteal.q", "Use Inner Flame (Q)")).SetValue(true);
            ks.AddItem(new MenuItem("l33t.karma.killsteal.rq", "Use Soulflare (R + Q)")).SetValue(true);
            ks.AddItem(new MenuItem("l33t.karma.killsteal.spacer0", ""));
            ks.AddItem(new MenuItem("l33t.karma.killsteal.useaoe", "Use Soulflare (Q) - Area of Effect")).SetValue(true);
            ks.AddItem(new MenuItem("l33t.karma.killsteal.minmanaq", "Min % of Mana for Inner Flame / Soulflare"))
                .SetValue(new Slider(55));

            menu.AddSubMenu(ks);

            #endregion

            #region Flee

            var flee = new Menu("Flee Settings", "l33t.karma.flee");

            flee.AddItem(new MenuItem("l33t.karma.flee.active", "Use Flee")).SetValue(true);
            flee.AddItem(new MenuItem("l33t.karma.flee.activekey", "Flee Key"))
                .SetValue(new KeyBind('Z', KeyBindType.Press));
            flee.AddItem(new MenuItem("l33t.karma.flee.spacer0", ""));
            flee.AddItem(new MenuItem("l33t.karma.flee.e", "Use Inspire (E)")).SetValue(true);
            flee.AddItem(new MenuItem("l33t.karma.flee.re", "Use Defiance (R + E)")).SetValue(true);
            flee.AddItem(new MenuItem("l33t.karma.flee.minalliesforre", "Minimum Allies for Defiance (R + E)"))
                .SetValue(new Slider(3, 1, 5));

            menu.AddSubMenu(flee);

            #endregion

            menu.AddItem(new MenuItem("l33t.karma.spacer0", ""));
            menu.AddItem(new MenuItem("l33t.karma.title", "Karma the Enlightened One"));

            return menu;
        }
    }
}