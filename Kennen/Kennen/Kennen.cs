#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Kennen
{
    internal class Kennen
    {
        static Kennen()
        {
            Menu = new KennenMenu();

            Player = ObjectManager.Player;

            R = new Spell(SpellSlot.R, 550f);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W, 800f);
            Q = new Spell(SpellSlot.Q, 1050f);
        }

        private static void Main()
        {
            if (!Player.ChampionName.Equals("Kennen"))
                return;

            Q.SetSkillshot(0.65f, 50f, 1700f, true, SkillshotType.SkillshotLine);

            Game.OnGameUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (Menu.GetOrbwalker().ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    KennenCombo(SimpleTs.GetTarget(Player, Q.Range, SimpleTs.DamageType.Magical));
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
            }
        }

        #region Combo

        private static void KennenCombo(Obj_AI_Base objAiBase)
        {
            if (objAiBase == null) return;

            if (Menu.GetValue<bool>(KennenMenu.ComboQ) && objAiBase.IsValidTarget() && Q.IsReady() &&
                objAiBase.HasBuff("BlackShield"))
            {
                Q.CastIfHitchanceEquals(objAiBase, HitChance.High, Menu.GetValue<bool>(KennenMenu.MiscPackets));
            }

            if (Menu.GetValue<bool>(KennenMenu.ComboW) && W.IsReady())
            {
                if (Menu.GetValue<int>(KennenMenu.ComboEMode) != 0)
                {
                    foreach (
                        var buff in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(e => e.IsValidTarget() && e.Distance(Player.Position) <= W.Range)
                                .SelectMany(
                                    enemy =>
                                        enemy.Buffs.Where(
                                            buff => /* buff.Name.Equals("") &&*/
                                                buff.Count >= Menu.GetValue<int>(KennenMenu.ComboEMode))))
                    {
                        Console.WriteLine("{0} / {1} / {2}", buff.Name, buff.DisplayName, buff.SourceName);
                        // TODO
                    }
                }
                else
                {
                    W.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                }
            }

            if (Menu.GetValue<bool>(KennenMenu.ComboE) && E.IsReady() && objAiBase.Distance(Player.Position) > R.Range)
            {
                // TODO
            }

            if (Menu.GetValue<bool>(KennenMenu.ComboR) && R.IsReady() &&
                Player.CountEnemysInRange((int) R.Range) > Menu.GetValue<int>(KennenMenu.ComboRChampInRange))
            {
                R.CastIfWillHit(objAiBase, Menu.GetValue<int>(KennenMenu.ComboRChampInRange),
                    Menu.GetValue<bool>(KennenMenu.MiscPackets));
            }
        }

        #endregion

        #region Vars

        private static readonly Spell Q;
        private static readonly Spell W;
        private static readonly Spell E;
        private static readonly Spell R;
        private static readonly Obj_AI_Hero Player;
        private static readonly KennenMenu Menu;

        #endregion
    }
}