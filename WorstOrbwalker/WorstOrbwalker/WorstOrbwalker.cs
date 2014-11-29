﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace WorstOrbwalker
{
    public class WorstOrbwalker
    {
        public delegate void AfterAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);
        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void OnAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);
        public delegate void OnNonKillableMinionH(Obj_AI_Base minion);
        public delegate void OnTargetChangeH(Obj_AI_Base oldTarget, Obj_AI_Base newTarget);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public static event BeforeAttackEvenH BeforeAttack;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public static event OnAttackEvenH OnAttack;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public static event AfterAttackEvenH AfterAttack;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public static event OnTargetChangeH OnTargetChange;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public static event OnNonKillableMinionH OnNonKillableMinion;

        public enum MODE
        {
            LAST_HIT,
            MIXED,
            LANE_CLEAR,
            COMBO,
            FLEE,
            NONE
        }

        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane", "lucianq",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "parley", "poppydevastatingblow", "powerfist", "renektonpreexecute", "rengarq", "shyvanadoubleattack",
            "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", "vie", "volibearq",
            "xenzhaocombotarget", "yorickspectral"
        };

        private static readonly string[] NoAttacks =
        {
            "jarvanivcataclysmattack", "monkeykingdoubleattack",
            "shyvanadoubleattack", "shyvanadoubleattackdragon", "zyragraspingplantattack", "zyragraspingplantattack2",
            "zyragraspingplantattackfire", "zyragraspingplantattack2fire"
        };

        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "lucianpassiveattack", "masteryidoublestrike", "quinnwenhanced", "renektonexecute",
            "renektonsuperexecute", "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2",
            "xenzhaothrust3"
        };

        public static int LastAATick;
        public static bool Attack = true;
        public static bool DisableNextAttack = false;
        public static bool Move = true;
        public static int LastMoveCommandT = 0;
        private static Obj_AI_Base _lastTarget;
        private static Obj_AI_Hero Player;
        private static Menu _config;
        private const float LaneClearWaitTimeMod = 2f;
        private const float _gRange = 1125f;
        private Obj_AI_Base _forcedTarget;
        private Vector3 _orbwalkingPoint;
        private Obj_AI_Minion _prevMinion;
        private bool srFix = false;
        private WorstSelector worstSelector;
        private Obj_AI_Base gTarget;

        public class BeforeAttackEventArgs
        {
            public Obj_AI_Base Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;

            public bool Process
            {
                get { return _process; }
                set
                {
                    DisableNextAttack = !value;
                    _process = value;
                }
            }
        }

        public static MODE ActiveMode
        {
            get
            {
                if (_config.Item("_worstorbwalker_keybinds_combo").GetValue<KeyBind>().Active)
                    return MODE.COMBO;
                else if (_config.Item("_worstorbwalker_keybinds_laneclear").GetValue<KeyBind>().Active)
                    return MODE.LANE_CLEAR;
                else if (_config.Item("_worstorbwalker_keybinds_mixed").GetValue<KeyBind>().Active)
                    return MODE.MIXED;
                else if (_config.Item("_worstorbwalker_keybinds_lasthit").GetValue<KeyBind>().Active)
                    return MODE.LAST_HIT;
                else if (_config.Item("_worstorbwalker_keybinds_flee").GetValue<KeyBind>().Active)
                    return MODE.FLEE;

                return MODE.NONE;
            }
        }

        public WorstOrbwalker(Menu menu)
        {
            _config = menu;

            /* TargetSelector menu */
            var ts = new Menu("Target Selector", "_worstorbwalker_targetselector");
            var tm = new Menu("Targeting Mode", "_worstorbwalker_targetselector_targetingmode");
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_lowhp", "Low HP")).SetValue(false);
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_mostad", "Most AD")).SetValue(false);
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_mostap", "Most AP")).SetValue(false);
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_closest", "Closest")).SetValue(false);
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_nearmouse", "Near Mouse")).SetValue(false);
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_autopriority", "Auto Priority")).SetValue(true);
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_lessattack", "Less Attack")).SetValue(false);
            tm.AddItem(new MenuItem("_worstorbwalker_targetselector_targetingmode_lesscast", "Less Cast")).SetValue(false);
            ts.AddSubMenu(tm);
            ts.AddItem(new MenuItem("_worstorbwalker_targetselector_spacer0", ""));
            if (tm.Item("_worstorbwalker_targetselector_targetingmode_lowhp").GetValue<bool>())
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.LOW_HP);
            else if (tm.Item("_worstorbwalker_targetselector_targetingmode_mostad").GetValue<bool>())
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.MOST_AD);
            else if (tm.Item("_worstorbwalker_targetselector_targetingmode_mostap").GetValue<bool>())
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.MOST_AP);
            else if (tm.Item("_worstorbwalker_targetselector_targetingmode_closest").GetValue<bool>())
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.CLOSEST);
            else if (tm.Item("_worstorbwalker_targetselector_targetingmode_closest").GetValue<bool>())
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.NEAR_MOUSE);
            else if (tm.Item("_worstorbwalker_targetselector_targetingmode_closest").GetValue<bool>())
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.LESS_ATTACK);
            else if (tm.Item("_worstorbwalker_targetselector_targetingmode_closest").GetValue<bool>())
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.LESS_CAST);
            else
                worstSelector = new WorstSelector(_gRange, WorstSelector.TargetingMode.AUTO_PRIORITY);
            worstSelector.AddToMenu(ts);
            _config.AddSubMenu(ts);

            /* Drawings menu */
            var drawings = new Menu("Drawings", "_worstorbwalker_drawings");
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_aacircle", "Attack Range")).SetValue(new Circle(true, Color.Purple));
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_aaenemycircle", "Enemy Attack Range")).SetValue(new Circle(true, Color.Purple));
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_selectedtarget", "Selected Target")).SetValue(new Circle(true, Color.White));
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_lasthitminion", "Last Hit Minion")).SetValue(new Circle(true, Color.FromArgb(255, 205, 92, 92)));
            _config.AddSubMenu(drawings);

            /* Misc menu */
            var misc = new Menu("Miscellaneous", "_worstorbwalker_miscellaneous");
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_priorizefarm", "Priorize farm over harass").SetValue(true));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_holdposradius", "Hold Position Radius")).SetValue(new Slider(80, 150, 0));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_humanizerdelay", "Humanizer Delay")).SetValue(new Slider(50, 100, 0));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_extrawinduptime", "Extra Windup Time")).SetValue(new Slider(80, 200, 0));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_selectedtargetfocus", "Enable selected target focus")).SetValue(true);
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_harassturret", "Harass under turret")).SetValue(false);
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_mixedlaneclear", "Mixed lane clear")).SetValue(false);
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_farmdelay", "Farm Delay")).SetValue(new Slider(0, 200, 0));
            _config.AddSubMenu(misc);

            /* Main menu */
            MenuItem[] menuItem = { (new MenuItem("_worstorbwalker_keybinds_lasthit", "Last hit").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press, false))),
                                    (new MenuItem("_worstorbwalker_keybinds_mixed", "Mixed").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press, false))),
                                    (new MenuItem("_worstorbwalker_keybinds_laneclear", "Lane clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press, false))),
                                    (new MenuItem("_worstorbwalker_keybinds_flee", "Flee").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press, false))),
                                    (new MenuItem("_worstorbwalker_keybinds_combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press, false)))};

            _config.AddItem(new MenuItem("_worstorbwalker_spacer0", ""));
            foreach (var mi in menuItem)
                if (mi != null)
                    _config.AddItem(mi);

            Player = ObjectManager.Player;
            Game.OnGameUpdate += GameOnOnGameUpdate;
        }

        private static void Obj_SpellMissile_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender is Obj_SpellMissile && sender.IsValid)
            {
                var missile = (Obj_SpellMissile)sender;
                if (missile.SpellCaster is Obj_AI_Hero && missile.SpellCaster.IsValid && IsAutoAttack(missile.SData.Name))
                    FireAfterAttack(missile.SpellCaster, _lastTarget);
            }
        }

        private static void FireBeforeAttack(Obj_AI_Base target)
        {
            if (BeforeAttack != null)
                BeforeAttack(new BeforeAttackEventArgs { Target = target });
            else
                DisableNextAttack = false;
        }

        private static void FireOnAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (OnAttack != null)
                OnAttack(unit, target);
        }

        private static void FireAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (AfterAttack != null)
                AfterAttack(unit, target);
        }

        private static void FireOnTargetSwitch(Obj_AI_Base newTarget)
        {
            if (OnTargetChange != null && (_lastTarget == null || _lastTarget.NetworkId != newTarget.NetworkId))
                OnTargetChange(_lastTarget, newTarget);
        }

        private static void FireOnNonKillableMinion(Obj_AI_Base minion)
        {
            if (OnNonKillableMinion != null)
                OnNonKillableMinion(minion);
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool IsMelee(Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) || Attacks.Contains(name.ToLower());
        }

        public static float GetRealAutoAttackRange(Obj_AI_Base target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;
            if (target != null)
                return result + target.BoundingRadius;
            return result;
        }

        public static bool InAutoAttackRange(Obj_AI_Base target)
        {
            if (target != null)
            {
                var myRange = GetRealAutoAttackRange(target);
                return Vector2.DistanceSquared(target.ServerPosition.To2D(), Player.Position.To2D()) <= myRange * myRange;
            }
            return false;
        }

        public static float GetRealAutoAttackRangeBuildings(Obj_Building target)
        {
            var result = Player.AttackRange + Player.BoundingRadius;
            if (target != null)
                return result + target.BoundingRadius;
            return result;
        }

        public static bool InAutoAttackRangeBuildings(Obj_Building target)
        {
            if (target != null)
            {
                var myRange = GetRealAutoAttackRangeBuildings(target);
                return Vector2.DistanceSquared(target.Position.To2D(), Player.Position.To2D()) <= myRange * myRange;
            }
            return false;
        }

        public static float GetMyProjectileSpeed()
        {
            return IsMelee(Player) ? float.MaxValue : Player.BasicAttack.MissileSpeed;
        }

        public static bool CanAttack()
        {
            if (LastAATick <= Environment.TickCount)
                return Environment.TickCount + Game.Ping / 2 + 25 >= LastAATick + Player.AttackDelay * 1000 && Attack;

            return false;
        }

        public static bool CanMove(float extraWindup)
        {
            if (LastAATick <= Environment.TickCount)
                return (Environment.TickCount + Game.Ping / 2 >= LastAATick + Player.AttackCastDelay * 1000 + extraWindup) && Move;

            return false;
        }

        private static void MoveTo(Vector3 position, float holdAreaRadius = 0, bool overrideTimer = false)
        {
            if ((Environment.TickCount - LastMoveCommandT < (80 + _config.Item("_worstorbwalker_miscellaneous_humanizerdelay").GetValue<Slider>().Value) && !overrideTimer))
                return;

            LastMoveCommandT = Environment.TickCount;

            if (Player.ServerPosition.Distance(position) < holdAreaRadius)
            {
                if (Player.Path.Count() > 1)
                    Player.IssueOrder(GameObjectOrder.HoldPosition, Player.ServerPosition);
                return;
            }
            
            var point = Player.ServerPosition + 400 * (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
            Player.IssueOrder(GameObjectOrder.MoveTo, point);
        }

        private static bool IsValidBuilding(Obj_Building building)
        {
            if (building.Health > 0f && building.IsEnemy && building.IsTargetable && !building.IsInvulnerable && !building.IsDead && building.IsValid)
                return true;
            return false;
        }

        public static void Orbwalk(Obj_AI_Base target, Vector3 position, float extraWindup = 90, float holdAreaRadius = 0)
        {
            if(target == null && CanAttack())
            {
                foreach (var building in ObjectManager.Get<Obj_Building>().Where(b => IsValidBuilding(b) && InAutoAttackRangeBuildings(b)))
                {
                    if (building.Name.StartsWith("Barracks_") || building.Name.StartsWith("HQ_"))
                    {
                        DisableNextAttack = false;

                        if (!DisableNextAttack)
                        {
                            Player.IssueOrder(GameObjectOrder.AttackUnit, building);
                            LastAATick = Environment.TickCount + Game.Ping / 2;
                            return;
                        }
                    }
                }
            }
            if (target != null && CanAttack())
            {
                DisableNextAttack = false;
                FireBeforeAttack(target);

                if (!DisableNextAttack)
                {
                    Player.IssueOrder(GameObjectOrder.AttackUnit, target);

                    if (_lastTarget.IsValid && target.IsValid && _lastTarget.NetworkId != target.NetworkId)
                        LastAATick = Environment.TickCount + Game.Ping / 2;

                    _lastTarget = target;
                    return;
                }
            }

            if (CanMove(extraWindup))
                MoveTo(position, holdAreaRadius);
        }

        public static void ResetAutoAttackTimer()
        {
            LastAATick = 0;
        }

        private static void OnProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData[0] != 0x34 || new GamePacket(args).ReadInteger(1) != ObjectManager.Player.NetworkId || (args.PacketData[5] != 0x11 && args.PacketData[5] != 0x91))
                return;

            ResetAutoAttackTimer();
        }

        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs Spell)
        {
            if (IsAutoAttackReset(Spell.SData.Name) && unit.IsMe)
                Utility.DelayAction.Add(250, ResetAutoAttackTimer);

            if (IsAutoAttack(Spell.SData.Name))
            {
                if (unit.IsMe)
                {
                    LastAATick = Environment.TickCount - Game.Ping / 2;
                    if (Spell.Target is Obj_AI_Base)
                    {
                        FireOnTargetSwitch((Obj_AI_Base)Spell.Target);
                        _lastTarget = (Obj_AI_Base)Spell.Target;
                    }

                    if (unit.IsMelee())
                        Utility.DelayAction.Add((int)(unit.AttackCastDelay * 1000 + 40), () => FireAfterAttack(unit, _lastTarget));
                }

                FireOnAttack(unit, _lastTarget);
            }
        }

        private void GameOnOnGameUpdate(EventArgs args)
        {
            // Fix for new summoners rift
            if ((Utility.Map.GetMap()._MapType == Utility.Map.MapType.SummonersRift && Game.Time >= 23.135f && !srFix) || (Utility.Map.GetMap()._MapType != Utility.Map.MapType.SummonersRift))
            {
                Drawing.OnDraw += DrawingOnOnDraw;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                GameObject.OnCreate += Obj_SpellMissile_OnCreate;
                Game.OnGameProcessPacket += OnProcessPacket;
                Player = ObjectManager.Player;
                srFix = true;
            }

            if (ActiveMode == MODE.NONE)
                return;

            //Prevent canceling important channeled spells like Miss Fortunes R.
            if (Player.IsChannelingImportantSpell())
                return;

            gTarget = GetTarget();
            Orbwalk(gTarget, (_orbwalkingPoint.To2D().IsValid()) ? _orbwalkingPoint : Game.CursorPos, _config.Item("_worstorbwalker_miscellaneous_extrawinduptime").GetValue<Slider>().Value, _config.Item("_worstorbwalker_miscellaneous_holdposradius").GetValue<Slider>().Value);
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (_config.Item("_worstorbwalker_drawings_aacircle").GetValue<Circle>().Active)
                Utility.DrawCircle(Player.Position, GetRealAutoAttackRange(null) + 65f, _config.Item("_worstorbwalker_drawings_aacircle").GetValue<Circle>().Color);

            if (_config.Item("_worstorbwalker_drawings_aaenemycircle").GetValue<Circle>().Active)
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(1175f)))
                    Utility.DrawCircle(target.Position, GetRealAutoAttackRange(target) + 65f, _config.Item("_worstorbwalker_drawings_aaenemycircle").GetValue<Circle>().Color);

            if (_config.Item("_worstorbwalker_drawings_selectedtarget").GetValue<Circle>().Active && _config.Item("_worstorbwalker_miscellaneous_selectedtargetfocus").GetValue<bool>())
                if (Hud.SelectedUnit != null && IsValidSelectedUnit() && Hud.SelectedUnit.IsEnemy && Hud.SelectedUnit.Position.Distance(Player.Position) <= 1175f)
                    Utility.DrawCircle(Hud.SelectedUnit.Position, Hud.SelectedUnit.BoundingRadius, _config.Item("_worstorbwalker_drawings_selectedtarget").GetValue<Circle>().Color);

            if (_config.Item("_worstorbwalker_drawings_lasthitminion").GetValue<Circle>().Active)
            {
                if (ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.LAST_HIT)
                { /*TODO => ADD ENCHANCER */
                    Utility.DrawCircle(gTarget.Position, gTarget.BoundingRadius + 10f, _config.Item("_worstorbwalker_drawings_lasthitminion").GetValue<Circle>().Color);
                }
            }
        }

        public Obj_AI_Base GetTarget()
        {
            Obj_AI_Base result = null;
            var r = float.MaxValue;

            /* SELECTED TARGET */
            if(ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.MIXED)
            {
                if(Hud.SelectedUnit != null)
                {
                    if(IsValidUnit((Obj_AI_Base)Hud.SelectedUnit) && InAutoAttackRange((Obj_AI_Base)Hud.SelectedUnit))
                    {
                        return result = (Obj_AI_Base)Hud.SelectedUnit;
                    }
                }
            }

            /* Forced target */
            if (_forcedTarget != null && _forcedTarget.IsValidTarget() && InAutoAttackRange(_forcedTarget))
                return _forcedTarget;

            /* Jungle minions */
            if (ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.MIXED)
            {
                foreach (var mob in ObjectManager.Get<Obj_AI_Minion>().Where(mob => mob.IsValidTarget() && InAutoAttackRange(mob) && mob.Team == GameObjectTeam.Neutral).Where(mob => mob.MaxHealth >= r || Math.Abs(r - float.MaxValue) < float.Epsilon))
                {
                    r = mob.MaxHealth;
                    return result = mob;
                }
            }

            /* MODES */
            if (ActiveMode == MODE.LAST_HIT || ActiveMode == MODE.MIXED || ActiveMode == MODE.LANE_CLEAR)
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && InAutoAttackRange(minion) && (minion.SkinName.Equals("SRU_ChaosMinionSiege") || minion.SkinName.Equals("SRU_OrderMinionSiege") || minion.SkinName.Equals("SRU_ChaosMinionSuper") || minion.SkinName.Equals("SRU_OrderMinionSuper"))))
                {
                    if (minion.Team != GameObjectTeam.Neutral)
                    {
                        var t = (int)(Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)Player.Distance(minion) / (int)GetMyProjectileSpeed();
                        var predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay);

                        if (predHealth > 0 && predHealth <= Player.GetAutoAttackDamage(minion, true))
                        {
                            return minion;
                        }
                    }
                }
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && InAutoAttackRange(minion)))
                {
                    var t = (int)(Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)Player.Distance(minion) / (int)GetMyProjectileSpeed();
                    var predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay);

                    if (minion.Team != GameObjectTeam.Neutral)
                    {
                        if (predHealth <= 0)
                        {
                            FireOnNonKillableMinion(minion);
                        }

                        if (predHealth > 0 && predHealth <= Player.GetAutoAttackDamage(minion, true))
                        {
                            return minion;
                        }
                    }
                }
                if ((ActiveMode == MODE.MIXED && _config.Item("_worstorbwalker_miscellaneous_mixedlaneclear").GetValue<bool>()) || ActiveMode == MODE.LANE_CLEAR)
                {
                    if (!ShouldWait())
                    {
                        if (_prevMinion != null && _prevMinion.IsValidTarget() && InAutoAttackRange(_prevMinion))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(_prevMinion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay);
                            if (predHealth >= 2 * Player.GetAutoAttackDamage(_prevMinion, false) || Math.Abs(predHealth - _prevMinion.Health) < float.Epsilon)
                            {
                                Console.WriteLine("{0} : {1} : {2}", _prevMinion.Type, _prevMinion.BaseSkinName, _prevMinion.Name);
                                return _prevMinion;
                            }
                        }

                        foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && InAutoAttackRange(minion)))
                        {
                            var predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay);
                            if (predHealth >= 2 * Player.GetAutoAttackDamage(minion, false) || Math.Abs(predHealth - minion.Health) < float.Epsilon)
                            {
                                if (minion.Health >= r || Math.Abs(r - float.MaxValue) < float.Epsilon)
                                {
                                    result = minion;
                                    r = minion.Health;
                                    _prevMinion = minion;
                                }
                            }
                        }
                    }
                }
                if (ActiveMode != MODE.LAST_HIT && !ShouldWait() && !ShouldWaitTower())
                {
                    var target = worstSelector.Target;
                    if (target != null)
                    {
                        if (TowerCheck(worstSelector.Target))
                        {
                            if (_config.Item("_worstorbwalker_miscellaneous_harassturret").GetValue<bool>())
                            {
                                return target;
                            }
                        }
                        else return target;
                    }
                }

                foreach(var tower in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                    if(!ShouldWaitTower())
                        return tower;
                foreach(var building in ObjectManager.Get<Obj_Building>().Where(b => IsValidBuilding(b) && InAutoAttackRangeBuildings(b)))
                    if (building.Name.StartsWith("Barracks_") || building.Name.StartsWith("HQ_"))
                        return null;
            }

            if (ActiveMode != MODE.LAST_HIT && ActiveMode != MODE.FLEE)
            {
                var target = worstSelector.Target;
                if (target != null)
                    return target;
            }

            return result;
        }

        private bool IsValidSelectedUnit()
        {
            if (Hud.SelectedUnit.Type == GameObjectType.obj_AI_Base || Hud.SelectedUnit.Type == GameObjectType.obj_AI_Turret || Hud.SelectedUnit.Type == GameObjectType.obj_AI_Minion || Hud.SelectedUnit.Type == GameObjectType.obj_AI_Hero || Hud.SelectedUnit.Type == GameObjectType.obj_BarracksDampener || Hud.SelectedUnit.Type == GameObjectType.obj_HQ)
                return true;
            return false;
        }

        private bool IsValidUnit(Obj_AI_Base BASE)
        {
            if(BASE != null)
            {
                if (BASE.IsValid && BASE.IsEnemy && BASE.Health > 0f && !BASE.IsDead && InAutoAttackRange(BASE))
                    return true;
            }
            return false;
        }

        private bool TowerCheck(Obj_AI_Base BASE)
        {
            if(BASE != null)
            {
                foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget()))
                {
                    var turretRange = turret.AttackRange + turret.BoundingRadius;
                    if(Vector2.DistanceSquared(Player.Position.To2D(), turret.Position.To2D()) <= (turretRange * turretRange))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SetAttack(bool b)
        {
            Attack = b;
        }

        public void SetMovement(bool b)
        {
            Move = b;
        }

        public void ForceTarget(Obj_AI_Base target)
        {
            _forcedTarget = target;
        }

        public void SetOrbwalkingPoint(Vector3 point)
        {
            _orbwalkingPoint = point;
        }

        private bool ShouldWait()
        {
            return ObjectManager.Get<Obj_AI_Minion>().Any(minion => minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral && InAutoAttackRange(minion) && HealthPrediction.LaneClearHealthPrediction(minion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay) <= Player.GetAutoAttackDamage(minion));
        }

        private bool ShouldWaitTower()
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(t => t.IsValidTarget() && t.Team != GameObjectTeam.Neutral && InAutoAttackRange(t) && HealthPrediction.GetHealthPrediction(t, ((int)(Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)Player.Distance(t) / (int)GetMyProjectileSpeed()), FarmDelay) <= Player.GetAutoAttackDamage(t));
        }

        private int FarmDelay
        {
            get { return _config.Item("_worstorbwalker_miscellaneous_farmdelay").GetValue<Slider>().Value; }
        }
    }
}
