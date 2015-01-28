using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Jayce
{
    internal class Mechanics
    {
        private static readonly Random Random = new Random();

        public static void ProcessCombo(Obj_AI_Hero player, Obj_AI_Base target)
        {
            if (!target.IsValidTarget())
            {
                return;
            }

            var spells = SpellManager.GetSpellManager(player);
            var useQ = Jayce.Menu.Item("l33t.jayce.combo._q" + ((spells.IsRanged) ? "1" : "0")).GetValue<bool>();
            var useW = Jayce.Menu.Item("l33t.jayce.combo._w" + ((spells.IsRanged) ? "1" : "0")).GetValue<bool>();
            var useE = Jayce.Menu.Item("l33t.jayce.combo._e" + ((spells.IsRanged) ? "1" : "0")).GetValue<bool>();
            var useR = Jayce.Menu.Item("l33t.jayce.combo._r" + ((spells.IsRanged) ? "1" : "0")).GetValue<bool>();

            if (spells.IsRanged)
            {
                #region Q Casting

                if (useQ && spells.Spells[SpellSlot.Q][0].Spell.IsReady() &&
                    player.Distance(target.ServerPosition) > player.AttackRange)
                {
                    if (useE && spells.Spells[SpellSlot.E][0].Spell.IsReady() &&
                        player.Distance(target.ServerPosition) >
                        player.AttackRange + (spells.Spells[SpellSlot.E][0].Spell.Range / 3))
                    {
                        if (player.ManaPercentage() >=
                            Jayce.Menu.Item("l33t.jayce.combo.c_qeMana").GetValue<Slider>().Value)
                        {
                            var ePred = spells.Spells[SpellSlot.Q][1].Spell.GetPrediction(target);
                            if (ePred.CollisionObjects.Count == 0)
                            {
                                var castingPos = player.ServerPosition.Extend(
                                    target.ServerPosition,
                                    (spells.Spells[SpellSlot.E][0].Spell.Range / 3) +
                                    Random.NextFloat(
                                        0f,
                                        spells.Spells[SpellSlot.E][0].Spell.Range -
                                        (spells.Spells[SpellSlot.E][0].Spell.Range / 3)));

                                spells.Spells[SpellSlot.E][0].Spell.Cast(castingPos);
                                spells.Spells[SpellSlot.Q][1].Spell.Cast(target.ServerPosition);
                                return;
                            }
                        }
                    }

                    if (player.ManaPercentage() >= Jayce.Menu.Item("l33t.jayce.combo.c_qMana").GetValue<Slider>().Value)
                    {
                        var pred = spells.Spells[SpellSlot.Q][0].Spell.GetPrediction(target);
                        if (pred.CollisionObjects.Count == 0)
                        {
                            spells.Spells[SpellSlot.Q][0].Spell.Cast(target.ServerPosition);
                        }
                    }
                }

                #endregion

                #region W Casting

                if (useW && spells.Spells[SpellSlot.W][0].Spell.IsReady() &&
                    player.ManaPercentage() >= Jayce.Menu.Item("l33t.jayce.combo.c_wMana").GetValue<Slider>().Value)
                {
                    var pred = Prediction.GetPrediction(target, 0.25f, 20f, target.MoveSpeed);
                    if (pred.UnitPosition.Distance(player.ServerPosition) < player.AttackRange)
                    {
                        spells.Spells[SpellSlot.W][0].Spell.Cast();
                    }
                }

                #endregion

                #region E Casting

                if (useE && spells.Spells[SpellSlot.E][0].Spell.IsReady())
                {
                    var ePred = spells.Spells[SpellSlot.Q][1].Spell.GetPrediction(target);
                    if (ePred.CollisionObjects.Count > 1)
                    {
                        var castingPos = player.Position.Extend(
                            target.ServerPosition,
                            player.BoundingRadius + 125f +
                            Random.NextFloat(
                                0f, spells.Spells[SpellSlot.E][0].Spell.Range - (player.BoundingRadius + 125f)));
                        spells.Spells[SpellSlot.E][0].Spell.Cast(castingPos);
                    }
                }

                #endregion

                #region R Casting

                if (useR && spells.Spells[SpellSlot.R][0].Spell.IsReady())
                {
                    var pred = Prediction.GetPrediction(target, 0.25f, 20f, target.MoveSpeed);
                    if (pred.UnitPosition.Distance(player.ServerPosition) < 125)
                    {
                        if (useW && spells.Spells[SpellSlot.W][0].Spell.IsReady() &&
                            player.ManaPercentage() >=
                            Jayce.Menu.Item("l33t.jayce.combo.c_wMana").GetValue<Slider>().Value)
                        {
                            spells.Spells[SpellSlot.W][0].Spell.Cast();
                            spells.Spells[SpellSlot.R][0].Spell.Cast();
                        }
                        return;
                    }
                    if (pred.UnitPosition.Distance(player.ServerPosition) < player.AttackRange)
                    {
                        if (!player.HasBuff("jaycehypercharge"))
                        {
                            spells.Spells[SpellSlot.R][0].Spell.Cast();
                            return;
                        }
                        var ePred = spells.Spells[SpellSlot.Q][1].Spell.GetPrediction(target);
                        var pPred = spells.Spells[SpellSlot.Q][0].Spell.GetPrediction(target);
                        if ((ePred.CollisionObjects.Count > 0 || pPred.CollisionObjects.Count > 0) ||
                            !spells.Spells[SpellSlot.Q][0].Spell.IsReady())
                        {
                            spells.Spells[SpellSlot.R][0].Spell.Cast();
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region Q Casting

                if (useQ && spells.Spells[SpellSlot.Q][0].Spell.IsReady() &&
                    player.Distance(target.ServerPosition) < spells.Spells[SpellSlot.Q][0].Spell.Range)
                {
                    if (player.Distance(target.ServerPosition) >=
                        Jayce.Menu.Item("l33t.jayce.combo.h_qDistance").GetValue<Slider>().Value)
                    {
                        if (Jayce.Menu.Item("l33t.jayce.combo.h_bTowers").GetValue<bool>() ||
                            (!target.Position.UnderTurret(true) ||
                             (target.Position.UnderTurret(true) && player.Position.UnderTurret(true))))
                        {
                            spells.Spells[SpellSlot.Q][0].Spell.Cast(target);
                        }
                    }
                }

                #endregion

                #region W Casting

                if (useW && spells.Spells[SpellSlot.W][0].Spell.IsReady())
                {
                    if (player.CountEnemiesInRange(spells.Spells[SpellSlot.W][0].Spell.Range) >=
                        Jayce.Menu.Item("l33t.jayce.combo.h_wEnemies").GetValue<Slider>().Value)
                    {
                        spells.Spells[SpellSlot.W][0].Spell.Cast();
                    }
                }

                #endregion

                #region E Casting

                if (useE && spells.Spells[SpellSlot.E][0].Spell.IsReady())
                {
                    var pred = player.ServerPosition.Extend(
                        target.ServerPosition,
                        player.Distance(target.ServerPosition) + spells.Spells[SpellSlot.E][0].Spell.Range);
                    if (pred.IsWall())
                    {
                        spells.Spells[SpellSlot.E][0].Spell.Cast(target);
                    }
                }

                #endregion

                #region R Casting

                if (useR && spells.Spells[SpellSlot.R][0].Spell.IsReady())
                {
                    if (player.Distance(target.ServerPosition) > spells.Spells[SpellSlot.Q][0].Spell.Range / 1.5f)
                    {
                        spells.Spells[SpellSlot.R][0].Spell.Cast();
                    }
                }

                #endregion
            }
        }
    }
}