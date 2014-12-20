#region

using System;
using System.CodeDom;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

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

            CustomEvents.Game.OnGameLoad += args =>
            {
                Game.OnGameUpdate += OnGameUpdate;
                Game.PrintChat("WorstPing | Kennen the Heart of the Tempest, loaded.");
            };
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (Menu.GetOrbwalker().ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    KennenCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    KennenLaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    KennenHarass();
                    break;
            }
        }

        #region Combo

        private static void KennenCombo()
        {
            /* E CAST */
            if (Menu.GetValue<bool>(KennenMenu.ComboE) &&
                (Menu.GetValue<StringList>(KennenMenu.RootMode).SelectedIndex == 0) && E.IsReady() &&
                (ObjectManager.Get<Obj_AI_Base>().Count(e => e.IsValidTarget() && e.Distance(Player.Position) <= 1200f) <
                 Menu.GetValue<int>(KennenMenu.ComboEChampsInRange)))
            {
                var target = SimpleTs.GetTarget(Player, 1200f, SimpleTs.DamageType.Magical);

                if (target != null)
                {
                    var time = ((Vector3.Distance(target.Position, Player.Position)/((Player.MoveSpeed*2)/1000))/1000);
                    Console.WriteLine(time);

                    if (time <= 4)
                    {
                        // TODO: check time units (m/s/ms?)
                        E.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                    }
                }
            }

            /* W CAST */
            if (Menu.GetValue<bool>(KennenMenu.ComboW) && W.IsReady())
            {
                if (Menu.GetValue<int>(KennenMenu.ComboWMode) != 0)
                {
                    foreach (
                        var buff in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(e => e.IsValidTarget() && e.Distance(Player.Position) <= W.Range)
                                .SelectMany(
                                    enemy =>
                                        enemy.Buffs.Where(
                                            buff => /* buff.Name.Equals("") &&*/
                                                buff.Count >= Menu.GetValue<int>(KennenMenu.ComboWMode))))
                    {
                        Console.WriteLine("{0} / {1} / {2}", buff.Name, buff.DisplayName, buff.SourceName);
                        // TODO: get buff name
                    }
                }
                else
                {
                    W.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                }
            }

            /* Q CAST */
            if (Menu.GetValue<bool>(KennenMenu.ComboQ) && Q.IsReady())
            {
                var target = SimpleTs.GetTarget(Player, Q.Range, SimpleTs.DamageType.Magical);

                if (target != null && (!Menu.GetValue<bool>(KennenMenu.MiscIgnoreSpellShields) && HasBlockableBuff(target)))
                {
                    var hitChance = HitChance.High;
                    Enum.TryParse(
                        Menu.GetValue<StringList>(KennenMenu.MiscHighChance).SList[
                            Menu.GetValue<StringList>(KennenMenu.MiscHighChance).SelectedIndex], out hitChance);
                    Q.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(KennenMenu.MiscPackets));
                }
            }

            /* R CAST */
            if (Menu.GetValue<bool>(KennenMenu.ComboR) && R.IsReady() &&
                Player.CountEnemysInRange((int)R.Range) > Menu.GetValue<int>(KennenMenu.ComboRChampInRange))
            {
                var target = SimpleTs.GetTarget(Player, R.Range, SimpleTs.DamageType.Magical);
                R.CastIfWillHit(target, Menu.GetValue<int>(KennenMenu.ComboRChampInRange),
                    Menu.GetValue<bool>(KennenMenu.MiscPackets));
            }
        }

        #endregion

        #region LaneClear

        private static void KennenLaneClear()
        {
            
        }

        #endregion

        #region Harass

        private static void KennenHarass()
        {

        }

        #endregion

        #region Voids

        private static bool HasBlockableBuff(Obj_AI_Base objAiBase)
        {
            return objAiBase.HasBuff("buff") || objAiBase.HasBuff("buff2");
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