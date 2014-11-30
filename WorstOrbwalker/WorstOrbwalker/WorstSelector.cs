using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace WorstOrbwalker
{
    public class WorstSelector
    {
        public enum TargetingMode
        {
            LOW_HP,
            MOST_AD,
            MOST_AP,
            CLOSEST,
            NEAR_MOUSE,
            PRIORITY,
            LESS_ATTACK,
            LESS_CAST
        }

        public enum DamageType
        {
            MAGICAL,
            PHYSICAL,
            TRUE
        }

        public Obj_AI_Hero target;
        private Obj_AI_Hero mainTarget;

        private double lastTick;

        private float range;
        private TargetingMode mode;

        private bool update = true;

        private Menu config;

        // ==

        public WorstSelector(float range = 1135f, TargetingMode mode = TargetingMode.PRIORITY)
        {
            this.range = range;
            this.mode = mode;

            Game.OnGameUpdate += onGameUpdate;
            Game.OnWndProc += onWndProc;
        }

        public void AddToMenu(Menu menu)
        {
            this.config = menu;

            menu.AddItem(new MenuItem("", ""));
            var autoPriorityItem = new MenuItem("AutoPriority", "Auto arrange priorities").SetShared().SetValue(false);
            autoPriorityItem.ValueChanged += autoPriorityItem_ValueChanged;

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Team != ObjectManager.Player.Team))
                menu.AddItem(new MenuItem("WorstSelector" + enemy.ChampionName + "Priority", enemy.ChampionName).SetShared().SetValue(new Slider(autoPriorityItem.GetValue<bool>() ? GetPriorityFromDb(enemy.ChampionName) : 1, 5, 1)));
            menu.AddItem(autoPriorityItem);
        }

        private void onGameUpdate(EventArgs args)
        {
            if (Environment.TickCount > lastTick + 100)
            {
                lastTick = Environment.TickCount;
                if (!update)
                {
                    return;
                }
                if (mainTarget == null)
                {
                    GetNormalTarget();
                }
                else
                {
                    if (Geometry.Distance(mainTarget) > range)
                    {
                        GetNormalTarget();
                    }
                    else
                    {
                        if (mainTarget.IsValidTarget())
                        {
                            target = mainTarget;
                        }
                        else
                        {
                            GetNormalTarget();
                        }
                    }
                }
            }
        }

        private void onWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen || ObjectManager.Player.Spellbook.SelectedSpellSlot != SpellSlot.Unknown)
                return;

            if (args.WParam == 1)
            {
                if (args.Msg == 257)
                {
                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (hero.IsValidTarget() && SharpDX.Vector2.Distance(Game.CursorPos.To2D(), hero.ServerPosition.To2D()) < 300)
                        {
                            target = hero;
                            mainTarget = hero;
                            Game.PrintChat("TargetSelector: New main target: " + mainTarget.ChampionName);
                        }
                    }
                }
            }
        }

        public void GetNormalTarget(int range = -1, DamageType damage = DamageType.PHYSICAL)
        {
            Obj_AI_Hero newtarget = null;
            if (mode != TargetingMode.PRIORITY)
            {
                foreach (var _target in ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget() && Geometry.Distance(target) <= range))
                {
                    if (newtarget == null)
                    {
                        newtarget = _target;
                    }
                    else
                    {
                        switch (mode)
                        {
                            case TargetingMode.LOW_HP:
                                if (_target.Health < newtarget.Health)
                                {
                                    newtarget = _target;
                                }
                                break;
                            case TargetingMode.MOST_AD:
                                if (_target.BaseAttackDamage + _target.FlatPhysicalDamageMod <
                                    newtarget.BaseAttackDamage + newtarget.FlatPhysicalDamageMod)
                                {
                                    newtarget = _target;
                                }
                                break;
                            case TargetingMode.MOST_AP:
                                if (_target.FlatMagicDamageMod < newtarget.FlatMagicDamageMod)
                                {
                                    newtarget = _target;
                                }
                                break;
                            case TargetingMode.CLOSEST:
                                if (Geometry.Distance(_target) < Geometry.Distance(newtarget))
                                {
                                    newtarget = _target;
                                }
                                break;
                            case TargetingMode.NEAR_MOUSE:
                                if (SharpDX.Vector2.Distance(Game.CursorPos.To2D(), _target.Position.To2D()) + 50 <
                                    SharpDX.Vector2.Distance(Game.CursorPos.To2D(), newtarget.Position.To2D()))
                                {
                                    newtarget = _target;
                                }
                                break;

                            case TargetingMode.LESS_ATTACK:
                                if ((_target.Health -
                                     ObjectManager.Player.CalcDamage(_target, Damage.DamageType.Physical, _target.Health) <
                                     (newtarget.Health -
                                      ObjectManager.Player.CalcDamage(
                                          newtarget, Damage.DamageType.Physical, newtarget.Health))))
                                {
                                    newtarget = _target;
                                }
                                break;
                            case TargetingMode.LESS_CAST:
                                if ((_target.Health -
                                     ObjectManager.Player.CalcDamage(_target, Damage.DamageType.Magical, _target.Health) <
                                     (newtarget.Health -
                                      ObjectManager.Player.CalcDamage(
                                          newtarget, Damage.DamageType.Magical, newtarget.Health))))
                                {
                                    newtarget = _target;
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                newtarget = GetTarget(range, damage);
            }
            this.target = newtarget;
        }

        public static bool IsInvulnerable(Obj_AI_Base target)
        {
            if (target.HasBuff("Undying Rage") && target.Health >= 2f)
            {
                return true;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }

            if (Collision.GetCollision(new List<SharpDX.Vector3> { ObjectManager.Player.Position, target.Position }, new PredictionInput { Radius = (ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius), Speed = ((int)(ObjectManager.Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)ObjectManager.Player.Distance(target) / (int)(ObjectManager.Player.IsMelee() ? float.MaxValue : ObjectManager.Player.BasicAttack.MissileSpeed)) }).Count == 0)
            {
                return true;
            }

            return false;
        }

        private Obj_AI_Hero GetTarget(float range, DamageType damageType)
        {
            return (GetTarget(ObjectManager.Player, range, damageType));
        }

        private Obj_AI_Hero GetTarget(Obj_AI_Base champion, float range, DamageType damageType)
        {
            Obj_AI_Hero bestTarget = null;
            var bestRatio = 0f;

            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                if (!hero.IsValidTarget() || IsInvulnerable(hero) || ((!(range < 0) || !Orbwalking.InAutoAttackRange(hero)) && !(champion.Distance(hero) < range)))
                {
                    continue;
                }
                var damage = 0f;

                switch (damageType)
                {
                    case DamageType.MAGICAL:
                        damage = (float)ObjectManager.Player.CalcDamage(hero, Damage.DamageType.Magical, 100);
                        break;
                    case DamageType.PHYSICAL:
                        damage = (float)ObjectManager.Player.CalcDamage(hero, Damage.DamageType.Physical, 100);
                        break;
                    case DamageType.TRUE:
                        damage = 100;
                        break;
                }

                var ratio = damage / (1 + hero.Health) * GetPriority(hero);

                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    bestTarget = hero;
                }
            }

            return bestTarget;
        }

        private static int GetPriorityFromDb(string championName)
        {
            string[] p1 =
            {
                "Alistar", "Amumu", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Soraka", "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

            string[] p2 =
            {
                "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", "Gragas", "Irelia", "Jax",
                "Lee Sin", "Maokai", "Morgana", "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze", "Swain",
                "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao"
            };

            string[] p3 =
            {
                "Akali", "Diana", "Fiddlesticks", "Fiora", "Fizz", "Heimerdinger", "Jayce", "Kassadin",
                "Kayle", "Kha'Zix", "Lissandra", "Mordekaiser", "Nidalee", "Riven", "Shaco", "Vladimir", "Yasuo",
                "Zilean"
            };

            string[] p4 =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "LeBlanc", "Lucian",
                "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon", "Teemo",
                "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath", "Zed",
                "Ziggs"
            };

            if (p1.Contains(championName))
            {
                return 1;
            }
            if (p2.Contains(championName))
            {
                return 2;
            }
            if (p3.Contains(championName))
            {
                return 3;
            }
            return p4.Contains(championName) ? 4 : 1;
        }

        public float GetPriority(Obj_AI_Hero hero)
        {
            var p = 1;
            if (config != null && config.Item("SimpleTS" + hero.ChampionName + "Priority") != null)
                p = config.Item("WorstSelector" + hero.ChampionName + "Priority").GetValue<Slider>().Value;

            switch (p)
            {
                case 2:
                    return 1.5f;
                case 3:
                    return 1.75f;
                case 4:
                    return 2f;
                case 5:
                    return 2.5f;
                default:
                    return 1f;
            }
        }

        private void autoPriorityItem_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!e.GetNewValue<bool>())
                return;
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Team != ObjectManager.Player.Team))
                config.Item("WorstSelector" + enemy.ChampionName + "Priority").SetValue(new Slider(GetPriorityFromDb(enemy.ChampionName), 5, 1));
        }

        public static WorstSelector.TargetingMode IntToTargetingMode(int integer)
        {
            switch (integer)
            {
                case 0:
                    return TargetingMode.LOW_HP;
                case 1:
                    return TargetingMode.MOST_AD;
                case 2:
                    return TargetingMode.MOST_AP;
                case 3:
                    return TargetingMode.CLOSEST;
                case 4:
                    return TargetingMode.NEAR_MOUSE;
                case 5:
                    return TargetingMode.PRIORITY;
                case 6:
                    return TargetingMode.LESS_ATTACK;
                case 7:
                    return TargetingMode.LESS_CAST;
                default: return TargetingMode.PRIORITY;
            }
        }

        public void OverrideTarget(Obj_AI_Hero newtarget)
        {
            target = newtarget;
            update = false;
        }

        public void DisableTargetOverride()
        {
            update = true;
        }

        public float GetRange()
        {
            return range;
        }

        public void SetRange(float range)
        {
            this.range = range;
        }

        public TargetingMode GetTargetingMode()
        {
            return mode;
        }

        public void SetTargetingMode(TargetingMode mode)
        {
            this.mode = mode;
        }

        public Obj_AI_Hero GetSelectedTarget()
        {
            return this.target;
        }

        public override string ToString()
        {
            return "Target: " + target.ChampionName + "Range: " + range + "Mode: " + mode;
        }
    }
}
