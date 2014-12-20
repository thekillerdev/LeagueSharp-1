#region

using System;
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

                    if (time <= 4f)
                    {
                        // TODO: check time units (m/s/ms?)
                        E.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                    }
                }
            }

            /* W CAST */
            if (Menu.GetValue<bool>(KennenMenu.ComboW) && W.IsReady())
            {
                if (Menu.GetValue<StringList>(KennenMenu.ComboWMode).SelectedIndex != 0)
                {
                    var hitcount =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                e => e.IsValidTarget() && e.Distance(Player.Position) < W.Range && !HasBlockableBuff(e))
                            .SelectMany(
                                enemy => /* buff.Name.Equals("") &&*/ // TODO: Check buff name
                                    enemy.Buffs.Where(buff => buff.Count >= Menu.GetValue<int>(KennenMenu.ComboWMode)))
                            .Count();
                    var count =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Count(e => e.IsValidTarget() && e.Distance(Player.Position) < W.Range);

                    if (hitcount >= Menu.GetValue<StringList>(KennenMenu.ComboWMode).SelectedIndex &&
                        ((Player.CountEnemysInRange((int) R.Range) < Menu.GetValue<int>(KennenMenu.ComboRChampsInRange)) ||
                         (count <= hitcount && !R.IsReady())))
                        W.CastIfWillHit(Player, Menu.GetValue<int>(KennenMenu.ComboWChampInRange),
                            Menu.GetValue<bool>(KennenMenu.MiscPackets));
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

                if (target != null && (!Menu.GetValue<bool>(KennenMenu.MiscIgnoreSpellShields) && !HasBlockableBuff(target, true)))
                {
                    HitChance hitChance;
                    Enum.TryParse(
                        Menu.GetValue<StringList>(KennenMenu.MiscHighChance).SList[
                            Menu.GetValue<StringList>(KennenMenu.MiscHighChance).SelectedIndex], out hitChance);
                    Q.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(KennenMenu.MiscPackets));
                }
            }

            /* R CAST */
            if (Menu.GetValue<bool>(KennenMenu.ComboR) && R.IsReady() &&
                Player.CountEnemysInRange((int)R.Range) >= Menu.GetValue<int>(KennenMenu.ComboRChampsInRange))
            {
                R.CastIfWillHit(Player, Menu.GetValue<int>(KennenMenu.ComboRChampsInRange),
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

        private static bool HasBlockableBuff(Obj_AI_Base objAiBase, bool ignoreVeil = false)
        { // TODO: Banshee's Veil, Sivir's Spell Shield, Nocturne's Shourd of Darkness
            if(ignoreVeil)
                return objAiBase.HasBuff("BlackShield") || objAiBase.HasBuff("buff2") || objAiBase.HasBuff("buff3");
            return objAiBase.HasBuff("BlackShield") || objAiBase.HasBuff("buff2") || objAiBase.HasBuff("buff3") || objAiBase.HasBuff("buff4");
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