using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Yasuo
{
    public class YasuoGame
    {
        private static int _sweepingBladeDelay;

        public static void OnGameUpdate(EventArgs args)
        {
            switch (Yasuo.Menu.GetOrbwalker().ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Farming(true);
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Farming(false);
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Yasuo.Menu.GetItemValue<KeyBind>(YasuoMenu.FleeKeyLoc).Active)
                    {
                        Flee();
                    }
                    break;
            }

            Killsteal();

            if (Yasuo.Menu.GetOrbwalker().ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                Auto();
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1200f, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }

            var packets = Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc);

            // => Steel Tempest (Q)
            if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ComboQLoc) && Yasuo.Player.GetYasuoQState().IsReady())
            {
                if (!Yasuo.Player.HasWhirlwind() &&
                    Yasuo.Player.Distance(target.Position) <=
                    Yasuo.Menu.GetItemValue<Slider>(YasuoMenu.ComboQRangeLoc).Value)
                {
                    Yasuo.Player.GetYasuoQState().Cast(target.Position, packets);
                }
                else
                {
                    if (Yasuo.Player.HasWhirlwind() &&
                        Yasuo.Player.Distance(target.Position) <= Yasuo.Player.GetYasuoQState().Range)
                    {
                        Yasuo.Player.GetYasuoQState().Cast(target.Position, packets);
                    }
                }
            }

            // => Sweeping Blade (E)
            if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ComboELoc) && YasuoSpells.E.IsReady())
            {
                var dashData = Yasuo.Player.GetDashData(target.Position, target);
                var dashPoint = (Vector3) dashData[0];
                var dashAi = (Obj_AI_Base) dashData[1];

                if (dashPoint.IsValid() && dashAi.IsValidTarget())
                {
                    if (Environment.TickCount - _sweepingBladeDelay > 300)
                    {
                        if (Game.CursorPos.Distance(target.Position) <= 500)
                        {
                            if (dashPoint.Distance(target.Position) < Yasuo.Player.Distance(target.Position) - 100 &&
                                dashPoint.Distance(target.Position) < 250)
                            {
                                YasuoSpells.E.Cast(dashAi, packets);
                                _sweepingBladeDelay = Environment.TickCount;
                            }
                        }
                    }
                }
            }

            // => Last Breath (R)
            if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ComboRLoc) && YasuoSpells.R.IsReady())
            {
                var enemies = ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsValidTarget() && e.IsKnockedUp()).ToList();
                if (enemies.Count >= Yasuo.Menu.GetItemValue<Slider>(YasuoMenu.ComboREnemiesLoc).Value)
                {
                    var health = enemies.Sum(enemy => enemy.Health / enemy.MaxHealth * 100) / enemies.Count;
                    if (health <= Yasuo.Menu.GetItemValue<Slider>(YasuoMenu.ComboREnemiesPercentLoc).Value)
                    {
                        YasuoSpells.R.Cast(packets);
                    }
                }
            }

            // => Items
            if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ComboItemsLoc))
            {
                if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.HarassItemsLoc))
                {
                    var item = (YasuoSpells.RavenousHydra.GetItem().IsOwned())
                        ? YasuoSpells.RavenousHydra
                        : YasuoSpells.Tiamat;

                    // HYDRA/TIAMAT
                    if (YasuoSpells.RavenousHydra.GetItem().IsOwned() &&
                        Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsHydraLoc) ||
                        YasuoSpells.Tiamat.GetItem().IsOwned() &&
                        Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsTiamatLoc))
                    {
                        if (item.GetItem().IsInRange(target))
                        {
                            item.GetItem().Cast();
                        }
                    }

                    // BILGEWATER/BOTRK
                    item = (YasuoSpells.BladeoftheRuinedKing.GetItem().IsOwned())
                        ? YasuoSpells.BladeoftheRuinedKing
                        : YasuoSpells.BilgewaterCutlass;
                    if (YasuoSpells.BladeoftheRuinedKing.GetItem().IsOwned() &&
                        Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsBladeLoc) ||
                        YasuoSpells.BilgewaterCutlass.GetItem().IsOwned() &&
                        Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsBilgewaterLoc))
                    {
                        if (item.GetItem().IsInRange(target))
                        {
                            item.GetItem().Cast();
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(1200f, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }

            var packets = Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc);

            // => Flee (E)
            if (Yasuo.ShouldDash && YasuoSpells.E.IsReady())
            {
                var dashData = new object[2];

                var map = Utility.Map.GetMap();
                if (map.Type == Utility.Map.MapType.SummonersRift)
                {
                    dashData =
                        Yasuo.Player.GetDashData(
                            (Yasuo.Player.Team == GameObjectTeam.Chaos)
                                ? new Vector3(14294f, 14390f, 171.9777f)
                                : new Vector3(414f, 418f, 183.0536f));
                }
                else if (map.Type == Utility.Map.MapType.CrystalScar)
                {
                    dashData =
                        Yasuo.Player.GetDashData(
                            (Yasuo.Player.Team == GameObjectTeam.Chaos)
                                ? new Vector3(13311.96f, 4161.232f, -37.36907f)
                                : new Vector3(573f, 4125f, -35.08974f));
                }

                if (dashData == null)
                {
                    Yasuo.ShouldDash = false;
                    return;
                }

                var dashPoint = (Vector3) dashData[0];
                var dashAi = (Obj_AI_Base) dashData[1];

                if (dashPoint.IsValid() && dashAi.IsValidTarget() && !dashPoint.UnderTurret(true))
                {
                    YasuoSpells.E.Cast(dashAi, packets);
                    Yasuo.ShouldDash = false;
                }
            }
            else if (Yasuo.Player.Distance(target.Position) <= YasuoSpells.Q.Range && YasuoSpells.Q.IsReady() &&
                     YasuoSpells.E.IsReady())
            {
                // => Q / E
                if (Yasuo.Player.HasWhirlwind() && Yasuo.Player.IsDashable(target) &&
                    Yasuo.Menu.GetItemValue<bool>(YasuoMenu.HarassQeComboLoc))
                {
                    YasuoSpells.E.Cast(target, packets);
                    Utility.DelayAction.Add(
                        250, () =>
                        {
                            YasuoSpells.Q.Cast(target, packets);

                            // => Items
                            if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.HarassItemsLoc))
                            {
                                var item = (YasuoSpells.RavenousHydra.GetItem().IsOwned())
                                    ? YasuoSpells.RavenousHydra
                                    : YasuoSpells.Tiamat;

                                if (YasuoSpells.RavenousHydra.GetItem().IsOwned() &&
                                    Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsHydraLoc) ||
                                    YasuoSpells.Tiamat.GetItem().IsOwned() &&
                                    Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsTiamatLoc))
                                {
                                    item.GetItem().Cast();
                                }
                            }

                            Yasuo.ShouldDash = true;
                        });
                }
                else
                {
                    YasuoSpells.Q.Cast(target, packets);

                    // => Items
                    if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.HarassItemsLoc))
                    {
                        var item = (YasuoSpells.RavenousHydra.GetItem().IsOwned())
                            ? YasuoSpells.RavenousHydra
                            : YasuoSpells.Tiamat;

                        if (YasuoSpells.RavenousHydra.GetItem().IsOwned() &&
                            Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsHydraLoc) ||
                            YasuoSpells.Tiamat.GetItem().IsOwned() &&
                            Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsTiamatLoc))
                        {
                            item.GetItem().Cast();
                        }
                    }

                    Yasuo.ShouldDash = true;
                }
            }
            else if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.HarassEGapcloserLoc) && YasuoSpells.E.IsReady())
            {
                // => Gap Closer
                var dashData = Yasuo.Player.GetDashData(target.Position, target);
                var dashPoint = (Vector3) dashData[0];
                var dashAi = (Obj_AI_Base) dashData[1];

                if (dashPoint.IsValid() && dashAi.IsValidTarget())
                {
                    if (Environment.TickCount - _sweepingBladeDelay > 300)
                    {
                        if (Game.CursorPos.Distance(target.Position) <= 400)
                        {
                            if (dashPoint.Distance(target.Position) < Yasuo.Player.Distance(target.Position) - 100 &&
                                dashPoint.Distance(target.Position) < 250)
                            {
                                YasuoSpells.E.Cast(dashAi, packets);
                                _sweepingBladeDelay = Environment.TickCount;
                            }
                        }
                    }
                }
            }
        }

        private static void Farming(bool p0)
        {
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(m => m.IsValidTarget() && m.Distance(Yasuo.Player.Position) <= 1200f);
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();

            if (!objAiMinions.Any())
            {
                return;
            }

            var lowHpMinion = objAiMinions.OrderBy(m => m.Health).FirstOrDefault();
            var firstOrDefault = Yasuo.Player.Buffs.FirstOrDefault(b => b.DisplayName.Equals("YasuoDashScalar"));
            var eStacks = 0;
            if (firstOrDefault != null)
            {
                eStacks = firstOrDefault.Count;
            }
            var stacks = 1 + 0.25 * eStacks;
            var packets = Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc);

            // => Last hit
            if (Yasuo.Player.GetYasuoQState().IsReady() && YasuoSpells.E.IsReady() && lowHpMinion != null)
            {
                if (((!Yasuo.Player.HasWhirlwind() && Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitQLoc)) ||
                     (Yasuo.Player.HasWhirlwind() && !Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitQWindLoc))) &&
                    (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitELoc)))
                {
                    var count = objAiMinions.Where(m => m.Distance(lowHpMinion.Position) < 350f);
                    if (count.Count() > 3)
                    {
                        if (objAiMinions.All(m => m.Health < Yasuo.Player.GetSteelTempestDamage(m, true)) &&
                            !Yasuo.Player.GetDashingEnd(lowHpMinion).To3D().UnderTurret(true))
                        {
                            YasuoSpells.E.Cast(lowHpMinion, packets);
                            Utility.DelayAction.Add(
                                250, () => { Yasuo.Player.GetYasuoQState().Cast(lowHpMinion.Position, packets); });
                        }
                    }
                }
            }


            if (YasuoSpells.E.IsReady() && lowHpMinion != null &&
                lowHpMinion.Health < Yasuo.Player.GetSweepingBladeDamage(lowHpMinion, stacks) &&
                !Yasuo.Player.GetDashingEnd(lowHpMinion).To3D().UnderTurret(true))
            {
                if (Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitELoc))
                {
                    YasuoSpells.E.Cast(lowHpMinion, packets);
                }
            }


            if (Yasuo.Player.GetYasuoQState().IsReady() && lowHpMinion != null &&
                lowHpMinion.Health < Yasuo.Player.GetSteelTempestDamage(lowHpMinion) &&
                lowHpMinion.Health > Yasuo.Player.GetSweepingBladeDamage(lowHpMinion, stacks))
            {
                if ((!Yasuo.Player.HasWhirlwind() && Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitQLoc)) ||
                    (Yasuo.Player.HasWhirlwind() && Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitQWindLoc)))
                {
                    Yasuo.Player.GetYasuoQState().Cast(lowHpMinion.Position, packets);
                }
            }

            // => Lane clear
            if (p0)
            {
                // => Tiamat/Hydra
                if (YasuoSpells.Tiamat.GetItem().IsOwned() ||
                    YasuoSpells.RavenousHydra.GetItem().IsOwned() &&
                    Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLaneClearItemsLoc))
                {
                    var item = (YasuoSpells.RavenousHydra.GetItem().IsOwned())
                        ? YasuoSpells.RavenousHydra
                        : YasuoSpells.Tiamat;

                    if (YasuoSpells.RavenousHydra.GetItem().IsOwned() &&
                        Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsHydraLoc) ||
                        YasuoSpells.Tiamat.GetItem().IsOwned() &&
                        Yasuo.Menu.GetItemValue<bool>(YasuoMenu.ItemsTiamatLoc))
                    {
                        var count = objAiMinions.Count(m => m.Distance(Yasuo.Player.Position) < item.Range);

                        if (count > 3)
                        {
                            item.GetItem().Cast();
                        }
                    }
                }

                if (Yasuo.Player.GetYasuoQState().IsReady())
                {
                    if ((!Yasuo.Player.HasWhirlwind() && Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitQLoc)) ||
                        (Yasuo.Player.HasWhirlwind() && Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FarmingLastHitQWindLoc)))
                    {
                        if (lowHpMinion != null)
                        {
                            Yasuo.Player.GetYasuoQState().Cast(lowHpMinion.Position, packets);
                        }
                    }
                }

                // TODO: Add jungle support
            }
        }

        private static void Flee()
        {
            var dashData = Yasuo.Player.GetDashData(
                Game.CursorPos, null, Yasuo.Menu.GetItemValue<bool>(YasuoMenu.FleeTowersLoc));
            var dashPoint = (Vector3) dashData[0];
            var dashAi = (Obj_AI_Base) dashData[1];

            if (dashAi.IsValidTarget() && dashPoint.IsValid())
            {
                YasuoSpells.E.Cast(dashAi, Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc));
            }

            Yasuo.Player.MoveTo(Game.CursorPos, Yasuo.Player.BoundingRadius * 2f);
        }

        private static void Killsteal()
        {
            var target =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(e => e.IsValidTarget() && e.Distance(Yasuo.Player.Position) < 1200f)
                    .OrderBy(e => e.Health)
                    .FirstOrDefault();

            if (target == null)
            {
                return;
            }

            var packets = Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc);
            var firstOrDefault = Yasuo.Player.Buffs.FirstOrDefault(b => b.DisplayName.Equals("YasuoDashScalar"));
            var eStacks = 0;
            if (firstOrDefault != null)
            {
                eStacks = firstOrDefault.Count;
            }
            var stacks = 1 + 0.25 * eStacks;

            if (target.Health < Yasuo.Player.GetSweepingBladeDamage(target, stacks))
            {
                YasuoSpells.E.Cast(target, packets);
            }
            else if (target.Health < Yasuo.Player.GetSteelTempestDamage(target))
            {
                Yasuo.Player.GetYasuoQState().Cast(target.Position, packets);
            }
        }

        private static void Auto()
        {
            var packets = Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc);

            // => Q
            if (Yasuo.Player.GetYasuoQState().IsReady() && Yasuo.Menu.GetItemValue<bool>(YasuoMenu.AutoQLoc))
            {
                var enemy =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            e =>
                                e.IsValidTarget() &&
                                e.Distance(Yasuo.Player.Position) < Yasuo.Player.GetYasuoQState().Range)
                        .OrderBy(e => e.Distance(Yasuo.Player.Position))
                        .FirstOrDefault();

                if (enemy != null && !enemy.UnderTurret())
                {
                    Yasuo.Player.GetYasuoQState().Cast(enemy.Position, packets);
                }
            }

            // => Anti Q-Waste
            if (Yasuo.Player.GetYasuoQState().IsReady() && Yasuo.Player.HasWhirlwind())
            {
                // TODO
            }
        }
    }
}