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
        #region Combo

        private static void KennenCombo()
        {
            var tfMode = (Menu.GetValue<StringList>(KennenMenu.RootMode).SelectedIndex == 0);
            var target = SimpleTs.GetTarget(Player, 1200f, SimpleTs.DamageType.Magical);

            /* E Logic / Casting */
            if (E.IsReady())
            {
                var enemies = Player.CountEnemysInRange(1200);

                if (((!tfMode && enemies <= Menu.GetValue<int>(KennenMenu.ComboEChampsInRange)) || tfMode) &&
                    target != null)
                {
                    var time = ((Vector3.Distance(target.Position, Player.Position)/((Player.MoveSpeed*2)/1000))/1000);
                    Console.WriteLine(time); // TODO: Remove DEBUG

                    if (!(time <= 3.5f)) return;

                    // TODO: check time units (m/s/ms?)
                    E.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                    Menu.GetOrbwalker().SetAttack(false);
                    Utility.DelayAction.Add(4000, () => Menu.GetOrbwalker().SetAttack(true));
                }
            }

            /* W Logic / Casting */
            if (W.IsReady())
            {
                var enemies =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(e => e.IsValidTarget() && e.Distance(Player.Position) < W.Range)
                        .SelectMany(
                            e =>
                                e.Buffs.Where(
                                    buff =>
                                        buff.Name.Equals("markofthestorm") && // TODO: buff name?
                                        buff.Count >=
                                        (Menu.GetValue<StringList>(KennenMenu.ComboWMode).SelectedIndex + 1)))
                        .Count();

                if (((!tfMode && enemies <= Menu.GetValue<int>(KennenMenu.ComboWChampsInRange)) ||
                     (tfMode && (Player.CountEnemysInRange((int) W.Range)/2) >= enemies)) &&
                    target != null)
                {
                    W.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                }
            }

            if (Q.IsReady())
            {
                
            }

            if (R.IsReady())
            {
                
            }
        }

        #endregion

        #region Harass

        private static void KennenHarass()
        {
        }

        #endregion

        #region Voids

        private static bool HasBlockableBuff(Obj_AI_Base objAiBase, bool ignoreVeil = false)
        {
            // TODO: Banshee's Veil, Sivir's Spell Shield, Nocturne's Shourd of Darkness
            if (ignoreVeil)
                return objAiBase.HasBuff("BlackShield") || objAiBase.HasBuff("buff2") || objAiBase.HasBuff("buff3");
            return objAiBase.HasBuff("BlackShield") || objAiBase.HasBuff("buff2") || objAiBase.HasBuff("buff3") ||
                   objAiBase.HasBuff("buff4");
        }

        #endregion

        #region Main

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
                case Orbwalking.OrbwalkingMode.LastHit:
                    KennenLastHit();
                    break;
            }
        }

        #endregion

        #region Farming

        private static void KennenLaneClear()
        {
            if (!Menu.GetValue<bool>(KennenMenu.LaneClear)) return;

            var chargedCount =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(m => m.IsValidTarget() && m.Distance(Player.Position) <= (Player.AttackRange*1.5))
                    .SelectMany(
                        m =>
                            m.Buffs.Where(
                                buffs => buffs.Name.Equals("marofthestorm") && // TODO
                                         buffs.Count >=
                                         (Menu.GetValue<StringList>(KennenMenu.LaneClearWStacks).SelectedIndex + 1)))
                    .Count();

            if (Player.CountEnemysInRange((int) (Player.AttackRange*2)) == 0)
            {
                if (Menu.GetValue<bool>(KennenMenu.LaneClearE) &&
                    chargedCount < Menu.GetValue<int>(KennenMenu.LaneClearEMin) && E.IsReady())
                {
                    E.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                }
            }
            if (Menu.GetValue<bool>(KennenMenu.LaneClearW) &&
                chargedCount >= Menu.GetValue<int>(KennenMenu.LaneClearWMin) && W.IsReady())
            {
                W.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
            }
        }

        private static void KennenLastHit()
        {
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