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
        #region Spells

        private static void KennenSpells(bool isCombo)
        {
            var isTeamFight = (Menu.GetValue<StringList>(KennenMenu.RootMode).SelectedIndex == 0);
            var target = SimpleTs.GetTarget(Player, 1200f, SimpleTs.DamageType.Magical);

            /* E CASTING */
            if (E.IsReady() && Menu.GetValue<bool>(isCombo ? KennenMenu.ComboE : KennenMenu.HarassE))
            {
                var enemies = Player.CountEnemysInRange(1200);

                if (((!isTeamFight &&
                      enemies <=
                      Menu.GetValue<int>(isCombo ? KennenMenu.ComboEChampsInRange : KennenMenu.HarassEChampsInRange)) ||
                     isTeamFight) && target != null)
                {
                    var time = ((Vector3.Distance(target.Position, Player.Position)/((Player.MoveSpeed*2)/1000))/1000);

                    if (time <= 3.5f)
                    {
                        Menu.GetOrbwalker().SetAttack(false);
                        E.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                        Utility.DelayAction.Add(4000, () => Menu.GetOrbwalker().SetAttack(true));
                    }
                }
            }

            /* W CASTING */
            if (W.IsReady() && Menu.GetValue<bool>(isCombo ? KennenMenu.ComboW : KennenMenu.HarassW))
            {
                var enemies =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            e =>
                                e.IsValidTarget() && e.Distance(Player.Position) < W.Range &&
                                HasBlockableBuff(e, isTeamFight))
                        .SelectMany(
                            e =>
                                e.Buffs.Where(
                                    buff =>
                                        buff.Name.Equals("markofthestorm") && // TODO: buff name?
                                        buff.Count >=
                                        (Menu.GetValue<StringList>(isCombo
                                            ? KennenMenu.ComboWMode
                                            : KennenMenu.HarassWMode).SelectedIndex + 1)))
                        .Count();

                if (enemies >=
                    Menu.GetValue<int>(isCombo ? KennenMenu.ComboWChampsInRange : KennenMenu.HarassWChampsInRange))
                {
                    W.Cast(Menu.GetValue<bool>(KennenMenu.MiscPackets));
                }
            }

            /* Q CASTING */
            if (Q.IsReady() && target != null && Menu.GetValue<bool>(isCombo ? KennenMenu.ComboQ : KennenMenu.HarassQ) &&
                !Q.GetPrediction(target).CollisionObjects.Any())
            {
                var hitChance = HitChance.High;
                if (Menu != null && Menu.GetItem(KennenMenu.MiscHitChance) != null)
                {
                    var menuItem = Menu.GetValue<StringList>(KennenMenu.MiscHitChance);
                    Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out hitChance);
                }
                Q.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(KennenMenu.MiscPackets));
            }

            /* R CASTING */
            if (R.IsReady() && isCombo && Menu.GetValue<bool>(KennenMenu.ComboR))
            {
                R.CastIfWillHit(Player,
                    isTeamFight
                        ? Player.CountEnemysInRange((int) R.Range)
                        : Menu.GetValue<int>(KennenMenu.ComboRChampsInRange),
                    Menu.GetValue<bool>(KennenMenu.MiscPackets));
            }
        }

        #endregion

        #region Functions

        private static bool HasBlockableBuff(Obj_AI_Base objAiBase, bool ignoreVeil = false)
        {
            var rValue = objAiBase.HasBuff("BlackShield") || objAiBase.HasBuff("buff2") || objAiBase.HasBuff("buff3");
            return ignoreVeil ? rValue : rValue | objAiBase.HasBuff("buff4");
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
                Game.OnGameUpdate += GameOnOnGameUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Game.PrintChat("WorstPing | Kennen the Heart of the Tempest, loaded.");
            };
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            switch (Menu.GetOrbwalker().ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    KennenSpells(true);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    KennenLaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    KennenSpells(false);
                    break;
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