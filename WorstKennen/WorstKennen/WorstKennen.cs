using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

namespace WorstKennen
{
    internal class WorstKennen
    {
        private static readonly Spell Q;
        private static readonly Spell W;
        private static readonly Spell E;
        private static readonly Spell R;
        private static readonly Obj_AI_Hero Player;
        private static readonly Menu Menu;
        private static Orbwalking.Orbwalker _orbwalker;

        static WorstKennen()
        {
            Menu = new Menu("WorstKennen", "_worstkennen", true);

            Player = ObjectManager.Player;

            R = new Spell(SpellSlot.R, 550);
            E = new Spell(SpellSlot.E, 550);
            W = new Spell(SpellSlot.W, 800);
            Q = new Spell(SpellSlot.Q, 1050);
        }

        private static void Main(string[] args)
        {
            if (!Player.ChampionName.Equals("Kennen"))
                return;

            Q.SetSkillshot(0.65f, 50f, 1700f, true, SkillshotType.SkillshotLine);

            var ts = new Menu("Target Selector", "_worstkennen_targetselector");
            SimpleTs.AddToMenu(ts);
            Menu.AddSubMenu(ts);

            Menu.AddSubMenu(new Menu("Orbwalker", "_worstkennen_orbwalker"));
            _orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("_worstkennen_orbwalker"));

            var combo = Menu.AddSubMenu(new Menu("Combo", "_worstkennen_combo"));
            combo.AddItem(new MenuItem("Use Q", "_worstkennen_combo_useq")).SetValue(true);
            combo.AddItem(new MenuItem("Use W", "_worstkennen_combo_usew")).SetValue(true);
            combo.AddItem(new MenuItem("Use E", "_worstkennen_combo_usee")).SetValue(true);
            combo.AddItem(new MenuItem("Use R", "_worstkennen_combo_user")).SetValue(true);
            combo.AddItem(new MenuItem("Min. champions to use R", "_worstkennen_combo_minuser"))
                .SetValue(new Slider(1, 1, 5));
            combo.AddItem(new MenuItem("E Mode", "_worstkennen_combo_emode"))
                .SetValue(new StringList(new[] {"None", "> 1 Stack", "> 2 Stack", "Stun only"}));

            var harass = Menu.AddSubMenu(new Menu("Harass", "_worstkennen_harass"));
            harass.AddItem(new MenuItem("Use Q", "_worstkennen_harass_useq")).SetValue(true);
            harass.AddItem(new MenuItem("Use W", "_worstkennen_harass_usew")).SetValue(false);
            harass.AddItem(new MenuItem("Use E", "_worstkennen_harass_usee")).SetValue(true);
            harass.AddItem(new MenuItem("Use with Mixed Mode", "_worstkennen_harass_usemixed")).SetValue(true);

            Menu.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    var target = _orbwalker.GetTarget();
                    if (target == null) break;

                    if (Menu.Item("_worstkennen_combo_useq").GetValue<bool>())
                    {
                        if (Q.IsReady() && target.IsValidTarget() && !target.HasBuff("BlackShield"))
                        {
                            Console.WriteLine(
                                (Collision.GetCollision(new List<Vector3> {target.Position},
                                    new PredictionInput {Delay = 0.65f, Range = 1050f, Speed = 1700f})).Count);

                            Q.Cast(target, true); // TODO
                        }
                    }
                    if (Menu.Item("_worstkennen_combo_usew").GetValue<bool>())
                    {
                        var mode = Menu.Item("_worstkennen_combo_emode").GetValue<StringList>().SelectedIndex;
                        if (W.IsReady() && Player.CountEnemysInRange((int) W.Range) > 0)
                        {
                            switch (mode)
                            {
                                case 0: // None
                                {
                                    W.Cast();
                                    break;
                                }
                                case 1: // 1 stack and above
                                {
                                    foreach (var buff in ObjectManager.Get<Obj_AI_Hero>()
                                        .Where(
                                            e =>
                                                e.IsValidTarget() &&
                                                e.Position.Distance(Player.Position) < W.Range)
                                        .SelectMany(
                                            enemy =>
                                                enemy.Buffs.Where(buff => /*buff.Name.Equals("test") &&*/ buff.Count >= 1)))
                                    {
                                        Console.WriteLine("buff = {0}, count = {1}", buff.Name, buff.Count);
                                    }
                                    break;
                                }
                                case 2: // 2 stack and above
                                {
                                    foreach (var buff in ObjectManager.Get<Obj_AI_Hero>()
                                        .Where(
                                            e =>
                                                e.IsValidTarget() &&
                                                e.Position.Distance(Player.Position) < W.Range)
                                        .SelectMany(
                                            enemy =>
                                                enemy.Buffs.Where(buff => /*buff.Name.Equals("test") &&*/ buff.Count >= 2)))
                                    {
                                        Console.WriteLine("buff = {0}, count = {1}", buff.Name, buff.Count);
                                    }
                                    break;
                                }
                                case 3: // stun only
                                {
                                    foreach (var buff in ObjectManager.Get<Obj_AI_Hero>()
                                        .Where(
                                            e =>
                                                e.IsValidTarget() &&
                                                e.Position.Distance(Player.Position) < W.Range)
                                        .SelectMany(
                                            enemy =>
                                                enemy.Buffs.Where(buff => /*buff.Name.Equals("test") &&*/ buff.Count == 3)))
                                    {
                                        Console.WriteLine("buff = {0}, count = {1}", buff.Name, buff.Count);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    break;
                }
                case Orbwalking.OrbwalkingMode.Mixed:
                {
                    break;
                }
            }
        }
    }
}