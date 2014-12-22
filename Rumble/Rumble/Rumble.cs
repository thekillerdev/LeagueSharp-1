using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Design;
using Color = System.Drawing.Color;

namespace Rumble
{
    internal class Rumble
    {
        #region Spells

        private static void RumbleCombo()
        {
            var target = TargetSelector.GetTarget(1200f, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget()) return;

            // ==== [ CHECKS ] ====

            CalculateHeatFunctions(target);

            var hitChance = HitChance.Medium;
            if (Menu.GetMenu() != null && Menu.GetItem(RumbleMenu.MiscHitChance) != null)
            {
                var menuItem = Menu.GetValue<StringList>(RumbleMenu.MiscHitChance);
                Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out hitChance);
            }

            // ==== [  COMBO  ] ====

            /* Q CASTING */
            if (Menu.GetValue<bool>(RumbleMenu.ComboQ) && QSpell.IsReady() && _shouldCastQAndE &&
                target.Distance(PlayerObjAiHero.Position) < 600f && PlayerObjAiHero.IsFacing(target, 600f) &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                QSpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
            }

            /* E CASTING */
            if (Menu.GetValue<bool>(RumbleMenu.ComboE) && !PlayerObjAiHero.HasBuff("RumbleGrenade", true) &&
                ESpell.IsReady() && _shouldCastQAndE &&
                target.Distance(PlayerObjAiHero.Position) < 850f &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                ESpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                _lastETick = Environment.TickCount;
            }

            if (Menu.GetValue<bool>(RumbleMenu.ComboE) && PlayerObjAiHero.HasBuff("RumbleGrenade", true) &&
                ESpell.IsReady() && target.Distance(PlayerObjAiHero.Position) < 850f)
            {
                var c0 = Menu.GetValue<bool>(RumbleMenu.MiscEmDelay) &&
                         Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250 &&
                         target.Distance(PlayerObjAiHero.Position) >= 300f;

                var c1 = Menu.GetValue<bool>(RumbleMenu.MiscEmDelay) &&
                         Environment.TickCount - _lastETick > 1000*Menu.GetValue<Slider>(RumbleMenu.MiscEDelay).Value &&
                         target.Distance(PlayerObjAiHero.Position) < 350f;

                var c2 = !Menu.GetValue<bool>(RumbleMenu.MiscEmDelay) &&
                         Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250;

                if (!c0 && !c1 && !c2) return;
                ESpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                _lastETick = Environment.TickCount;
            }

            /* W CASTNG */
            if (Menu.GetValue<bool>(RumbleMenu.ComboW) && WSpell.IsReady() && _shouldCastW &&
                target.Distance(PlayerObjAiHero.Position) < 900f &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                // TODO
            }

            /* R CASTING */
            if (Menu.GetValue<bool>(RumbleMenu.ComboR))
            {
                var rc0 = (RSpell.IsReady() && ESpell.IsReady() && QSpell.IsReady()) &&
                          PlayerObjAiHero.GetComboDamage(target,
                              new[] {SpellSlot.Q, SpellSlot.E, SpellSlot.E, SpellSlot.R}) >
                          target.Health;

                var rc1 = (RSpell.IsReady() && QSpell.IsReady()) &&
                          PlayerObjAiHero.GetComboDamage(target, new[] {SpellSlot.R, SpellSlot.Q}) > target.Health;

                var rc2 = (RSpell.IsReady() && ESpell.IsReady()) &&
                          PlayerObjAiHero.GetComboDamage(target, new[] {SpellSlot.R, SpellSlot.E, SpellSlot.E}) >
                          target.Health;

                if (!rc0 && !rc1 && !rc2) return;

                CastR(target);
            }

            /* OVERHEAT */
            if (Menu.GetValue<bool>(RumbleMenu.ComboOverheat) && _shouldOverheat &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                if (Menu.GetValue<bool>(RumbleMenu.ComboQ) && QSpell.IsReady() &&
                    target.Distance(PlayerObjAiHero.Position) < 600f && PlayerObjAiHero.IsFacing(target, 600f))
                {
                    QSpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }

                if (Menu.GetValue<bool>(RumbleMenu.ComboE) &&
                    ESpell.IsReady() &&
                    target.Distance(PlayerObjAiHero.Position) < 850f)
                {
                    ESpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                    _lastETick = Environment.TickCount;
                }

                WSpell.Cast(Menu.GetValue<bool>(RumbleMenu.MiscPackets));
            }
        }

        private static void RumbleHarass()
        {
            var target = TargetSelector.GetTarget(1200f, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget()) return;

            // ==== [ CHECKS ] ====

            CalculateHeatFunctions(target);

            var hitChance = HitChance.Medium;
            if (Menu.GetMenu() != null && Menu.GetItem(RumbleMenu.MiscHitChance) != null)
            {
                var menuItem = Menu.GetValue<StringList>(RumbleMenu.MiscHitChance);
                Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out hitChance);
            }

            // ==== [  HARASS  ] ====
            /* Q CASTING */
            if (Menu.GetValue<bool>(RumbleMenu.HarassQ) && QSpell.IsReady() && _shouldCastQAndE &&
                target.Distance(PlayerObjAiHero.Position) < 600f && PlayerObjAiHero.IsFacing(target, 600f) &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                QSpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
            }

            /* E CASTING */
            if (Menu.GetValue<bool>(RumbleMenu.HarassE) && ESpell.IsReady() && _shouldCastQAndE &&
                target.Distance(PlayerObjAiHero.Position) < 850f &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                ESpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                _lastETick = Environment.TickCount;
            }

            /* W CASTING */
            if (Menu.GetValue<bool>(RumbleMenu.HarassW) && WSpell.IsReady() && _shouldCastW &&
                target.Distance(PlayerObjAiHero.Position) < 900f &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                // TODO
            }

            /* OVERHEAT */
            if (Menu.GetValue<bool>(RumbleMenu.HarassOverheat) && _shouldOverheat &&
                Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
            {
                if (Menu.GetValue<bool>(RumbleMenu.HarassQ) && QSpell.IsReady() &&
                    target.Distance(PlayerObjAiHero.Position) < 600f && PlayerObjAiHero.IsFacing(target, 600f))
                {
                    QSpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }

                if (Menu.GetValue<bool>(RumbleMenu.HarassE) &&
                    ESpell.IsReady() &&
                    target.Distance(PlayerObjAiHero.Position) < 850f)
                {
                    ESpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                    _lastETick = Environment.TickCount;
                }

                WSpell.Cast(Menu.GetValue<bool>(RumbleMenu.MiscPackets));
            }
        }

        private static void RumbleFlee()
        {
            var target =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(e => e.IsValidTarget() && e.Distance(PlayerObjAiHero.Direction) < 850f)
                    .OrderBy(e => e.Distance(PlayerObjAiHero.Position))
                    .FirstOrDefault();

            if (ESpell.IsReady())
            {
                var hitChance = HitChance.Medium;
                if (Menu.GetMenu() != null && Menu.GetItem(RumbleMenu.MiscHitChance) != null)
                {
                    var menuItem = Menu.GetValue<StringList>(RumbleMenu.MiscHitChance);
                    Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out hitChance);
                }
                ESpell.CastIfHitchanceEquals(target, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
            }

            if (WSpell.IsReady())
            {
                WSpell.Cast(Menu.GetValue<bool>(RumbleMenu.MiscPackets));
            }
        }

        private static void RumbleHeatManager()
        {
            if (PlayerObjAiHero.HasBuff("Recall", true))
                return;

            if (Menu.GetValue<bool>(RumbleMenu.HmStayInDanger) && PlayerObjAiHero.CountEnemysInRange(1000) < 2)
            {
                if (WSpell.IsReady() && Menu.GetValue<bool>(RumbleMenu.HmW) && PlayerObjAiHero.Mana < 35 &&
                    Environment.TickCount - _lastSavedTick > 750)
                {
                    _lastSavedTick = Environment.TickCount;
                    WSpell.Cast(Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }
                else if (QSpell.IsReady() && Menu.GetValue<bool>(RumbleMenu.HmQ))
                {
                    QSpell.Cast(Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }
            }
            else if (Menu.GetValue<bool>(RumbleMenu.HmStayInDanger))
            {
                if (!WSpell.IsReady() || !Menu.GetValue<bool>(RumbleMenu.HmW) || !(PlayerObjAiHero.Mana < 35) ||
                    !(Environment.TickCount - _lastSavedTick > 750)) return;
                _lastSavedTick = Environment.TickCount;
                WSpell.Cast(Menu.GetValue<bool>(RumbleMenu.HmW));
            }
        }

        private static void KillSteal()
        {
            var hitChance = HitChance.Medium;
            if (Menu.GetMenu() != null && Menu.GetItem(RumbleMenu.MiscHitChance) != null)
            {
                var menuItem = Menu.GetValue<StringList>(RumbleMenu.MiscHitChance);
                Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out hitChance);
            }

            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(e => e.IsValidTarget() && e.Distance(PlayerObjAiHero.Position) < RSpell.Range && !IsInvulnerable(e, TargetSelector.DamageType.Magical)))
            {
                if (enemy.Distance(PlayerObjAiHero.Position) < QSpell.Range && QSpell.IsReady() &&
                    Menu.GetValue<bool>(RumbleMenu.KsQ) &&
                    Menu.GetValue<bool>(RumbleMenu.KsOverheat) && _shouldCastQAndE &&
                    PlayerObjAiHero.IsFacing(enemy, 600f) &&
                    Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
                {
                    if (PlayerObjAiHero.GetSpellDamage(enemy, SpellSlot.Q)/2 > enemy.Health)
                    {
                        QSpell.CastIfHitchanceEquals(enemy, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                    }
                }

                if (enemy.Distance(PlayerObjAiHero.Position) < ESpell.Range && ESpell.IsReady() &&
                    Menu.GetValue<bool>(RumbleMenu.KsE) &&
                    Menu.GetValue<bool>(RumbleMenu.KsOverheat) && _shouldCastQAndE &&
                    Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
                {
                    if (PlayerObjAiHero.GetSpellDamage(enemy, SpellSlot.E) > enemy.Health)
                    {
                        ESpell.CastIfHitchanceEquals(enemy, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                    }
                }

                if (enemy.Distance(PlayerObjAiHero.Position) < RSpell.Range && RSpell.IsReady() &&
                    Menu.GetValue<bool>(RumbleMenu.KsR) &&
                    Environment.TickCount - PlayerObjAiHero.LastCastedSpellT() > 250)
                {
                    if (PlayerObjAiHero.GetSpellDamage(enemy, SpellSlot.R)*1.5 > enemy.Health)
                    {
                        CastR(enemy);
                    }
                }
            }
        }

        private static void CastR(Obj_AI_Base targetObjAiBase)
        {
            if (PlayerObjAiHero.CountEnemysInRange(1000) > 1)
            {
                Vector2 fromVector2;
                Vector2 toVector2;
                int enemyHit;
                RLogic(targetObjAiBase, out fromVector2, out toVector2, out enemyHit);

                if (enemyHit > 0)
                {
                    Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, RSpell.Slot, -1, fromVector2.X, fromVector2.Y,
                        toVector2.X, toVector2.Y)).Send();
                }
            }
            else
            {
                
            }
        }

        private static void RLogic(Obj_AI_Base targetObjAiBase, out Vector2 fromVector2, out Vector2 toVector2, out int enemyHit)
        {
            var cWaypoints = targetObjAiBase.GetWaypoints();

            Vector3 bestCastPosition0Vector3 = new Vector3(-1f, -1f, -1f);
            Vector3 bestCastPosition1Vector3 = new Vector3(-1f, -1f, -1f);
            var bestHits = 0;

            var spellPredictionPos = Prediction.GetPrediction(targetObjAiBase, 0.250f, 0f, RSpell.Speed);
            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(e => e.IsValidTarget() && e.Distance(PlayerObjAiHero.Position) < 1000))
            {
                var predictedPos = Prediction.GetPrediction(enemy, 0.250f);
                if (Vector3.Distance(predictedPos.UnitPosition, spellPredictionPos.CastPosition) < RSpell.Width) // TODO?
                {
                    var midPos = GetMidPoint(predictedPos.UnitPosition, spellPredictionPos.CastPosition);
                    var midVector = predictedPos.UnitPosition - spellPredictionPos.CastPosition;
                    midVector.Normalize();

                    var pos0 = midPos + midVector*(RSpell.Width/2);
                    var pos1 = midPos + midVector*(RSpell.Width/2);
                    var currentHit = CheckHitCount(pos0, pos1, targetObjAiBase);

                    if (currentHit > bestHits)
                    {
                        bestHits = currentHit;
                        bestCastPosition0Vector3 = pos0;
                        bestCastPosition1Vector3 = pos1;
                    }
                }
            }

            if (bestHits >= 2 && !bestCastPosition0Vector3.Equals(new Vector3(-1f, -1f, -1f)) && !bestCastPosition1Vector3.Equals(new Vector3(-1f, -1f, -1f)))
            {
                if (!CheckWall(bestCastPosition0Vector3, bestCastPosition1Vector3))
                {
                    fromVector2 = new Vector2(bestCastPosition0Vector3.X, bestCastPosition0Vector3.Y);
                    toVector2 = new Vector2(bestCastPosition1Vector3.X, bestCastPosition1Vector3.Y);
                    enemyHit = bestHits;
                    return;
                }
            }

            fromVector2 = new Vector2(-1f, -1f);
            toVector2 = new Vector2(-1f, -1f);
            enemyHit = -1;
        }

        private static bool CheckWall(Vector3 posVector, Vector3 posVector3)
        {
            var eiVector = (posVector - posVector3);
            eiVector.Normalize();
            var wallCount = 0;
            for (var i = 1; i < 20; ++i)
            {
                var currentM = 60*i;
                var cVector = posVector + eiVector*currentM;
                if (cVector.IsWall())
                    ++wallCount;
            }

            return (wallCount >= 8);
        }

        private static int CheckHitCount(Vector3 posVector, Vector3 posVector3, Obj_AI_Base objAiBase)
        {
            if (!objAiBase.IsValidTarget()) return 0;

            var posMid = new Vector3((posVector.X + posVector3.X)/2, 0, (posVector.Z + posVector3.Z)/2);
            var eiVector = (posVector - posVector3);
            eiVector.Normalize();
            var extensionAmount = RSpell.Width/2; // TODO ?
            var extpos0 = posMid + eiVector*extensionAmount;
            var extpos1 = posMid - eiVector*extensionAmount;

            var hit = 0;

            foreach (var predictedPos in ObjectManager.Get<Obj_AI_Hero>()
                .Where(e => e.IsValidTarget() && e.Distance(PlayerObjAiHero.Position) < 1000f).Select(enemy => Prediction.GetPrediction(enemy, 0.250f)))
            {
                bool isInsideBox;
                Vector3 boxVector3;
                Vector3 lineVector3;

                VectorBoxCalculation(out boxVector3, out lineVector3, out isInsideBox, extpos0, extpos1, predictedPos.UnitPosition);
                if (Vector3.Distance(boxVector3, lineVector3) < RSpell.Width + 60)
                {
                    ++hit;
                }
            }

            return hit;
        }

        private static void VectorBoxCalculation(out Vector3 boxVector3, out Vector3 lineVector3, out bool insideBox,
            Vector3 v0Vector3, Vector3 v1Vector3, Vector3 v2Vector3)
        {
            var cx = v2Vector3.X;
            var cy = v2Vector3.Y;
            var ax = v0Vector3.X;
            var ay = v0Vector3.Y;
            var bx = v1Vector3.X;
            var by = v1Vector3.Y;

            var stack = ((cx - ax)*(bx - ax) + (cy - ay)*(by - ay))/((bx - ax)*(bx - ax) + (by - ay)*(by - ay));
            lineVector3 = new Vector3(ax + stack*(bx - ax), ay + stack*(by - ay), 0f);
            var stack2 = (stack < 0) ? 0 : (stack > 1 ? 1 : stack);
            insideBox = (stack.Equals(stack2));
            boxVector3 = insideBox ? lineVector3 : new Vector3(ax + stack2*(bx - ax), ay + stack2*(by - ay), 0f);
        }

        private static Vector3 GetMidPoint(Vector3 vector3, Vector3 otherVector3)
        {
            return new Vector3((vector3.X + vector3.X) / 2, 0f, (vector3.Z + otherVector3.Z)/2);
        }

        private static void SubRLogic(out Vector2 fromVector2, out Vector2 toVector2)
        {
            fromVector2 = new Vector2(-1f, -1f);
            toVector2 = new Vector2(-1f, -1f);
        }

        #endregion

        #region Main

        static Rumble()
        {
            Menu = new RumbleMenu();

            PlayerObjAiHero = ObjectManager.Player;

            RSpell = new Spell(SpellSlot.R, 1700f);
            ESpell = new Spell(SpellSlot.E, 850f);
            WSpell = new Spell(SpellSlot.W);
            QSpell = new Spell(SpellSlot.Q, 600f);
        }

        private static void Main()
        {
            if (!PlayerObjAiHero.ChampionName.Equals("Rumble"))
                return;

            ESpell.SetSkillshot(0.25f, 70, 2000, true, SkillshotType.SkillshotLine);
            RSpell.SetSkillshot(1700, 120, 1400, false, SkillshotType.SkillshotLine);

            CustomEvents.Game.OnGameLoad += args =>
            {
                Game.OnGameUpdate += GameOnOnGameUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Game.PrintChat("WorstPing | Rumble the Mechanized Menace, loaded.");
            };
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (Menu.GetValue<bool>(RumbleMenu.DrawQ))
            {
                Drawing.DrawCircle(PlayerObjAiHero.Position, QSpell.Range, Color.Red);
            }
            if (Menu.GetValue<bool>(RumbleMenu.DrawE))
            {
                Drawing.DrawCircle(PlayerObjAiHero.Position, ESpell.Range, Color.Green);
            }
            if (Menu.GetValue<bool>(RumbleMenu.DrawR))
            {
                Drawing.DrawCircle(PlayerObjAiHero.Position, ESpell.Range, Color.Yellow);
            }
            if (Menu.GetValue<bool>(RumbleMenu.DrawKillText))
            {
                var target = TargetSelector.GetTarget(1200f, TargetSelector.DamageType.Magical, true);
                if (!target.IsValidTarget()) return;

                if (PlayerObjAiHero.GetSpellDamage(target, SpellSlot.Q) / 3 > target.Health)
                {
                    Drawing.DrawText(target.Position.X, target.Position.Y, Color.White, "Short Q");
                }
                else if (PlayerObjAiHero.GetSpellDamage(target, SpellSlot.Q) > target.Health)
                {
                    Drawing.DrawText(target.Position.X, target.Position.Y, Color.White, "Q");
                }
                else if (PlayerObjAiHero.GetSpellDamage(target, SpellSlot.R) > target.Health)
                {
                    Drawing.DrawText(target.Position.X, target.Position.Y, Color.White, "R");
                }
                else if (PlayerObjAiHero.GetSpellDamage(target, SpellSlot.E) > target.Health)
                {
                    Drawing.DrawText(target.Position.X, target.Position.Y, Color.White, "E");
                }
                else if (PlayerObjAiHero.GetComboDamage(target, new[] {SpellSlot.Q, SpellSlot.E, SpellSlot.E}) > target.Health)
                {
                    Drawing.DrawText(target.Position.X, target.Position.Y, Color.White, "Q + Twice E");
                }
                else if (PlayerObjAiHero.GetComboDamage(target, new[] { SpellSlot.Q, SpellSlot.E, SpellSlot.E, SpellSlot.R}) > target.Health)
                {
                    Drawing.DrawText(target.Position.X, target.Position.Y, Color.White, "Full Combo");
                }
                else
                {
                    Drawing.DrawText(target.Position.X, target.Position.Y, Color.White, "Not Killable.");
                }
            }
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            switch (Menu.GetOrbwalker().ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    RumbleCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    RumbleLaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    RumbleHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    RumbleLastHit();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    RumbleFlee();
                    break;
            }
            KillSteal();
            RumbleHeatManager();
        }

        #endregion

        #region Farming

        private static void RumbleLaneClear()
        {
            if (Menu.GetValue<bool>(RumbleMenu.FarmQ) && QSpell.IsReady())
            {
                var hitQ = ObjectManager.Get<Obj_AI_Minion>().Count(m => m.IsValidTarget() && m.Distance(PlayerObjAiHero.Position) < QSpell.Range && PlayerObjAiHero.IsFacing(m, 600f));
                if (hitQ >= Menu.GetValue<Slider>(RumbleMenu.FarmMinQ).Value && (PlayerObjAiHero.Mana < 80 && !Menu.GetValue<bool>(RumbleMenu.FarmOverheat)))
                {
                    QSpell.Cast(Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }
                else
                {
                    QSpell.Cast(Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }
            }

            if (Menu.GetValue<bool>(RumbleMenu.FarmE) && ESpell.IsReady())
            {
                var minion =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            m =>
                                m.IsValidTarget() && m.Distance(PlayerObjAiHero.Position) < ESpell.Range &&
                                !ESpell.GetPrediction(m).CollisionObjects.Any())
                        .OrderBy(m => m.Health)
                        .FirstOrDefault();

                var hitChance = HitChance.Medium;
                if (Menu.GetMenu() != null && Menu.GetItem(RumbleMenu.MiscHitChance) != null)
                {
                    var menuItem = Menu.GetValue<StringList>(RumbleMenu.MiscHitChance);
                    Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out hitChance);
                }

                if (PlayerObjAiHero.Mana < 80 && !Menu.GetValue<bool>(RumbleMenu.FarmOverheat))
                {
                    ESpell.CastIfHitchanceEquals(minion, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }
                else
                {
                    ESpell.CastIfHitchanceEquals(minion, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
                }
            }
        }

        private static void RumbleLastHit()
        {
            if (!ESpell.IsReady()) return;

            var hitChance = HitChance.Medium;
            if (Menu.GetMenu() != null && Menu.GetItem(RumbleMenu.MiscHitChance) != null)
            {
                var menuItem = Menu.GetValue<StringList>(RumbleMenu.MiscHitChance);
                Enum.TryParse(menuItem.SList[menuItem.SelectedIndex], out hitChance);
            }

            var minion =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        m =>
                            m.IsValidTarget() && m.Distance(PlayerObjAiHero.Position) < ESpell.Range &&
                            !ESpell.GetPrediction(m).CollisionObjects.Any() && PlayerObjAiHero.GetSpellDamage(m, SpellSlot.E) > m.Health)
                    .OrderBy(m => m.Health)
                    .FirstOrDefault();

            ESpell.CastIfHitchanceEquals(minion, hitChance, Menu.GetValue<bool>(RumbleMenu.MiscPackets));
        }

        #endregion

        #region Vars

        private static readonly Spell QSpell;
        private static readonly Spell WSpell;
        private static readonly Spell ESpell;
        private static readonly Spell RSpell;

        private static readonly Obj_AI_Hero PlayerObjAiHero;
        private static readonly RumbleMenu Menu;

        private static double _lastSavedTick;
        private static double _lastETick;

        private static bool _shouldOverheat;
        private static bool _shouldCastQAndE;
        private static bool _shouldCastW;

        #endregion

        #region Functions

        private static bool ShouldOverheat(Obj_AI_Base targetObjAiBase)
        {
            return ((PlayerObjAiHero.Mana > 80 && !QSpell.IsReady() && !ESpell.IsReady() &&
                     targetObjAiBase.Distance(PlayerObjAiHero.Position) < 350f) &&
                    ((PlayerObjAiHero.GetAutoAttackDamage(targetObjAiBase, true))*3D > targetObjAiBase.Health));
        }

        private static void CalculateHeatFunctions(Obj_AI_Base targetObjAiBase)
        {
            if (!targetObjAiBase.IsValidTarget()) return;

            if (PlayerObjAiHero.Mana > 80 && ShouldOverheat(targetObjAiBase))
            {
                _shouldOverheat = true;
            }
            else if (PlayerObjAiHero.Mana > 60)
            {
                _shouldCastQAndE = true;
            }
            else if (PlayerObjAiHero.Mana > 0)
            {
                _shouldCastQAndE = true;
                _shouldCastW = true;
            }
        }

        private static bool IsInvulnerable(Obj_AI_Base @base, TargetSelector.DamageType damageType)
        {
            return TargetSelector.IsInvulnerable(@base, damageType, false, true);
        }

        #endregion
    }
}