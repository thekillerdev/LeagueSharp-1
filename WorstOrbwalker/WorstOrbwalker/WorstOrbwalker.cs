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
        private Obj_AI_Base _forcedTarget;
        private Vector3 _orbwalkingPoint;
        private Obj_AI_Minion _prevMinion;
        private bool srFix = false;

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
            _config.AddSubMenu(ts);

            /* Drawings menu */
            var drawings = new Menu("Drawings", "_worstorbwalker_drawings");
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_aacircle", "Attack Range")).SetValue(new Circle(true, Color.FromArgb(255, 255, 0, 255)));
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_aaenemycircle", "Enemy Attack Range")).SetValue(new Circle(true, Color.FromArgb(255, 255, 0, 255)));
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_selectedtarget", "Selected Target")).SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255)));
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_focusedminion", "Focused Minion")).SetValue(new Circle(true, Color.FromArgb(255, 255, 0, 0)));
            drawings.AddItem(new MenuItem("_worstorbwalker_drawings_focusedminionsize", "Focused Minion Circles")).SetValue(new Slider(1, 1, 3));
            _config.AddSubMenu(drawings);

            /* Misc menu */
            var misc = new Menu("Miscellaneous", "_worstorbwalker_miscellaneous");
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_priorizefarm", "Priorize farm over harass").SetValue(true));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_holdposradius", "Hold Position Radius")).SetValue(new Slider(0, 150, 0));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_humanizerdelay", "Humanizer Delay")).SetValue(new Slider(50, 100, 0));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_extrawinduptime", "Extra Windup Time")).SetValue(new Slider(80, 200, 0));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_selectedtargetfocus", "Enable selected target focus")).SetValue(true);
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_harassturret", "Harass under turret")).SetValue(false);
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_mixedlaneclear", "Mixed lane clear")).SetValue(false);
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_farmdelay", "Farm Delay")).SetValue(new Slider(0, 200, 0));
            misc.AddItem(new MenuItem("_worstorbwalker_miscellaneous_towerwait", "Tower Last Hit")).SetValue(false);
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
            if (target == null && CanAttack())
            {
                if (ActiveMode != MODE.COMBO && ActiveMode != MODE.FLEE)
                {
                    foreach (var building in ObjectManager.Get<Obj_Building>().Where(b => IsValidBuilding(b) && InAutoAttackRangeBuildings(b) && (b.Name.StartsWith("Barracks_") || IsValidBuilding(b) && InAutoAttackRangeBuildings(b) && b.Name.StartsWith("HQ_"))))
                    {
                        Player.IssueOrder(GameObjectOrder.AttackUnit, building);

                        if (_lastTarget.IsValid && target.IsValid && _lastTarget.NetworkId != target.NetworkId)
                            LastAATick = Environment.TickCount + Game.Ping / 2;

                        _lastTarget = target;
                        return;
                    }
                }
            }
            else if (target != null && CanAttack())
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

            var target = GetTarget();
            Orbwalk(target, (_orbwalkingPoint.To2D().IsValid()) ? _orbwalkingPoint : Game.CursorPos, _config.Item("_worstorbwalker_miscellaneous_extrawinduptime").GetValue<Slider>().Value, _config.Item("_worstorbwalker_miscellaneous_holdposradius").GetValue<Slider>().Value);
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            if (_config.Item("_worstorbwalker_drawings_aacircle").GetValue<Circle>().Active)
                Utility.DrawCircle(Player.Position, GetRealAutoAttackRange(null) + 65f, _config.Item("_worstorbwalker_drawings_aacircle").GetValue<Circle>().Color);

            if (_config.Item("_worstorbwalker_drawings_aaenemycircle").GetValue<Circle>().Active)
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget(1175f)))
                    Utility.DrawCircle(target.Position, GetRealAutoAttackRange(target) + 65f, _config.Item("_worstorbwalker_drawings_aaenemycircle").GetValue<Circle>().Color);

            if (_config.Item("_worstorbwalker_drawings_selectedtarget").GetValue<Circle>().Active)
                if (Hud.SelectedUnit != null && isValidSelectedUnit() && Hud.SelectedUnit.IsEnemy && Hud.SelectedUnit.Position.Distance(Player.Position) <= 1175f)
                    Utility.DrawCircle(Hud.SelectedUnit.Position, Hud.SelectedUnit.BoundingRadius + (10f), _config.Item("_worstorbwalker_drawings_selectedtarget").GetValue<Circle>().Color);

            if(_config.Item("_worstorbwalker_drawings_focusedminion").GetValue<Circle>().Active)
                if(ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.LAST_HIT)
                    for (int i = 0; i < _config.Item("_worstorbwalker_drawings_focusedminionsize").GetValue<Slider>().Value; ++i )
                        Utility.DrawCircle(Hud.SelectedUnit.Position, Hud.SelectedUnit.BoundingRadius + (10f + (10f * i)), _config.Item("_worstorbwalker_drawings_focusedminion").GetValue<Circle>().Color);
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

        public Obj_AI_Base GetTarget()
        {
            Obj_AI_Base result = null;
            var r = float.MaxValue;

            /* SELECTED */
            if (ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.MIXED)
                if (Hud.SelectedUnit != null)
                    if (Hud.SelectedUnit.IsValid && Hud.SelectedUnit.IsEnemy && !Hud.SelectedUnit.IsDead && InAutoAttackRange((Obj_AI_Base)Hud.SelectedUnit))
                        if (isValidSelectedUnit())
                            return (Obj_AI_Base)Hud.SelectedUnit;

            if ((ActiveMode == MODE.MIXED || ActiveMode == MODE.LANE_CLEAR) && !_config.Item("_worstorbwalker_miscellaneous_priorizefarm").GetValue<bool>())
            {
                var target = SimpleTs.GetTarget(-1, SimpleTs.DamageType.Physical);
                if (target != null)
                    return target;
            }

            /* Killable Minion */
            if (ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.MIXED || ActiveMode == MODE.LAST_HIT)
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && InAutoAttackRange(minion)))
                {
                    var t = (int)(Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)Player.Distance(minion) / (int)GetMyProjectileSpeed();
                    var predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay);

                    if (minion.Team != GameObjectTeam.Neutral)
                    {
                        if (predHealth <= 0)
                            FireOnNonKillableMinion(minion);

                        if (predHealth > 0 && predHealth <= Player.GetAutoAttackDamage(minion, true))
                            return minion;
                    }
                }
            }

            /* Forced target */
            if (_forcedTarget != null && _forcedTarget.IsValidTarget() && InAutoAttackRange(_forcedTarget))
                return _forcedTarget;

            /* Champions */
            if (ActiveMode != MODE.LAST_HIT)
            {
                var target = SimpleTs.GetTarget(-1, SimpleTs.DamageType.Physical);
                if (target != null)
                    return target;
            }

            /* Jungle minions */
            if (ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.MIXED)
            {
                foreach (var mob in ObjectManager.Get<Obj_AI_Minion>().Where(mob => mob.IsValidTarget() && InAutoAttackRange(mob) && mob.Team == GameObjectTeam.Neutral).Where(mob => mob.MaxHealth >= r || Math.Abs(r - float.MaxValue) < float.Epsilon))
                {
                    result = mob;
                    r = mob.MaxHealth;
                }
            }

            if (result != null)
                return result;

            /* Lane Clear minions */
            r = float.MaxValue;
            if (ActiveMode == MODE.LANE_CLEAR || ActiveMode == MODE.MIXED && _config.Item("_worstorbwalker_miscellaneous_mixedlaneclear").GetValue<bool>())
            {
                if (!ShouldWait())
                {
                    if (_prevMinion != null && _prevMinion.IsValidTarget() && InAutoAttackRange(_prevMinion))
                    {
                        var predHealth = HealthPrediction.LaneClearHealthPrediction(_prevMinion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay);
                        if (predHealth >= 2 * Player.GetAutoAttackDamage(_prevMinion, false) || Math.Abs(predHealth - _prevMinion.Health) < float.Epsilon)
                            return _prevMinion;
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

            /* Turrets */
            if (ActiveMode == MODE.LANE_CLEAR)
                foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && InAutoAttackRange(t)))
                {
                    if (!ShouldWaitTower() && _config.Item("_worstorbwalker_miscellaneous_towerwait").GetValue<bool>())
                        return turret;
                    else if (!_config.Item("_worstorbwalker_miscellaneous_towerwait").GetValue<bool>())
                        return turret;
                }

            return result;
        }

        private int FarmDelay
        {
            get { return _config.Item("_worstorbwalker_miscellaneous_farmdelay").GetValue<Slider>().Value; }
        }

        private bool isValidSelectedUnit()
        {
            if (Hud.SelectedUnit.Type == GameObjectType.obj_AI_Base || Hud.SelectedUnit.Type == GameObjectType.obj_AI_Turret || Hud.SelectedUnit.Type == GameObjectType.obj_AI_Minion || Hud.SelectedUnit.Type == GameObjectType.obj_AI_Hero || Hud.SelectedUnit.Type == GameObjectType.obj_BarracksDampener || Hud.SelectedUnit.Type == GameObjectType.obj_HQ)
                return true;
            return false;
        }
    }
}
