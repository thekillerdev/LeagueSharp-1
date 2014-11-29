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
            AUTO_PRIORITY,
            LESS_ATTACK,
            LESS_CAST
        }

        public enum DamageType
        {
            Magical,
            Physical,
            True,
        }

        private static double lastTick;

        private static readonly string[] AP =
        {
            "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana",
            "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus", "Kassadin", "Katarina", "Kayle", "Kennen",
            "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna", "Ryze", "Sion",
            "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra",
            "Velkoz"
        };

        private static readonly string[] SUP =
        {
            "Blitzcrank", "Janna", "Karma", "Leona", "Lulu", "Nami", "Sona",
            "Soraka", "Thresh", "Zilean"
        };

        private static readonly string[] TANK =
        {
            "Amumu", "Chogath", "DrMundo", "Galio", "Hecarim", "Malphite",
            "Maokai", "Nasus", "Rammus", "Sejuani", "Shen", "Singed", "Skarner", "Volibear", "Warwick", "Yorick", "Zac",
            "Nunu", "Taric", "Alistar", "Garen", "Nautilus", "Braum"
        };

        private static readonly string[] AD =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "KogMaw",
            "MissFortune", "Quinn", "Sivir", "Talon", "Tristana", "Twitch", "Urgot", "Varus", "Vayne", "Zed", "Jinx",
            "Yasuo", "Lucian", "Kalista"
        };

        private static readonly string[] BRUISER =
        {
            "Darius", "Elise", "Evelynn", "Fiora", "Gangplank", "Gnar", "Jayce",
            "Pantheon", "Irelia", "JarvanIV", "Jax", "Khazix", "LeeSin", "Nocturne", "Olaf", "Poppy", "Renekton",
            "Rengar", "Riven", "Shyvana", "Trundle", "Tryndamere", "Udyr", "Vi", "MonkeyKing", "XinZhao", "Aatrox",
            "Rumble", "Shaco", "MasterYi"
        };

        public Obj_AI_Hero Target;
        private bool drawCircle;
        private Obj_AI_Hero mainTarget;
        private TargetingMode mode;
        private float range;
        private bool update = true;
        private static Menu _config;

        public WorstSelector(float range, TargetingMode mode)
        {
            this.range = range;
            this.mode = mode;

            Game.OnGameUpdate += onGameUpdate;
            Drawing.OnDraw += onDraw;
            Game.OnWndProc += onWndProc;
        }

        public void AddToMenu(Menu menu)
        {
            _config = menu;
            menu.AddItem(new MenuItem("FocusSelected", "Focus selected target").SetShared().SetValue(true));
            menu.AddItem(new MenuItem("SelTColor", "Selected target color").SetShared().SetValue(new Circle(true, System.Drawing.Color.Red)));
            menu.AddItem(new MenuItem("Sep", "").SetShared());
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
                            Target = mainTarget;
                        }
                        else
                        {
                            GetNormalTarget();
                        }
                    }
                }
            }
        }

        private void onDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead && drawCircle && Target != null && Target.IsVisible && !Target.IsDead)
            {
                Drawing.DrawCircle(Target.Position, 125, _config.Item("SelTColor").GetValue<Circle>().Color);
            }
        }

        private void onWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen || ObjectManager.Player.Spellbook.SelectedSpellSlot != SpellSlot.Unknown)
                return;

            if(args.WParam == 1)
            {
                if(args.Msg == 257)
                {
                    foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (hero.IsValidTarget() && SharpDX.Vector2.Distance(Game.CursorPos.To2D(), hero.ServerPosition.To2D()) < 300)
                        {
                            Target = hero;
                            mainTarget = hero;
                            Game.PrintChat("TargetSelector: New main target: " + mainTarget.ChampionName);
                        }
                    }
                }
            }
        }

        public void GetNormalTarget(int range = -1, DamageType damage = DamageType.Physical)
        {
            Obj_AI_Hero newtarget = null;
            if (mode != TargetingMode.AUTO_PRIORITY)
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(target => target.IsValidTarget() && Geometry.Distance(target) <= range))
                {
                    if (newtarget == null)
                    {
                        newtarget = target;
                    }
                    else
                    {
                        switch (mode)
                        {
                            case TargetingMode.LOW_HP:
                                if (target.Health < newtarget.Health)
                                {
                                    newtarget = target;
                                }
                                break;
                            case TargetingMode.MOST_AD:
                                if (target.BaseAttackDamage + target.FlatPhysicalDamageMod <
                                    newtarget.BaseAttackDamage + newtarget.FlatPhysicalDamageMod)
                                {
                                    newtarget = target;
                                }
                                break;
                            case TargetingMode.MOST_AP:
                                if (target.FlatMagicDamageMod < newtarget.FlatMagicDamageMod)
                                {
                                    newtarget = target;
                                }
                                break;
                            case TargetingMode.CLOSEST:
                                if (Geometry.Distance(target) < Geometry.Distance(newtarget))
                                {
                                    newtarget = target;
                                }
                                break;
                            case TargetingMode.NEAR_MOUSE:
                                if (SharpDX.Vector2.Distance(Game.CursorPos.To2D(), target.Position.To2D()) + 50 <
                                    SharpDX.Vector2.Distance(Game.CursorPos.To2D(), newtarget.Position.To2D()))
                                {
                                    newtarget = target;
                                }
                                break;

                            case TargetingMode.LESS_ATTACK:
                                if ((target.Health -
                                     ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, target.Health) <
                                     (newtarget.Health -
                                      ObjectManager.Player.CalcDamage(
                                          newtarget, Damage.DamageType.Physical, newtarget.Health))))
                                {
                                    newtarget = target;
                                }
                                break;
                            case TargetingMode.LESS_CAST:
                                if ((target.Health -
                                     ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, target.Health) <
                                     (newtarget.Health -
                                      ObjectManager.Player.CalcDamage(
                                          newtarget, Damage.DamageType.Magical, newtarget.Health))))
                                {
                                    newtarget = target;
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
            Target = newtarget;
        }

        public static bool IsInvulnerable(Obj_AI_Base target)
        {
            //TODO: add yasuo wall, spellshields, etc.
            if (target.HasBuff("Undying Rage") && target.Health >= 2f)
            {
                return true;
            }

            if (target.HasBuff("JudicatorIntervention"))
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
                if (!hero.IsValidTarget() || IsInvulnerable(hero) ||
                    ((!(range < 0) || !Orbwalking.InAutoAttackRange(hero)) && !(champion.Distance(hero) < range)))
                {
                    continue;
                }
                var damage = 0f;

                switch (damageType)
                {
                    case DamageType.Magical:
                        damage = (float)ObjectManager.Player.CalcDamage(hero, Damage.DamageType.Magical, 100);
                        break;
                    case DamageType.Physical:
                        damage = (float)ObjectManager.Player.CalcDamage(hero, Damage.DamageType.Physical, 100);
                        break;
                    case DamageType.True:
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

        public static float GetPriority(Obj_AI_Hero hero)
        {
            var p = 1;
            if (_config != null && _config.Item("SimpleTS" + hero.ChampionName + "Priority") != null)
            {
                p = _config.Item("WorstSelector" + hero.ChampionName + "Priority").GetValue<Slider>().Value;
            }

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

        private static void autoPriorityItem_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!e.GetNewValue<bool>())
            {
                return;
            }
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.Team != ObjectManager.Player.Team)
                )
            {
                _config.Item("WorstSelector" + enemy.ChampionName + "Priority")
                    .SetValue(new Slider(GetPriorityFromDb(enemy.ChampionName), 5, 1));
            }
        }

        public void SetDrawCircleOfTarget(bool draw)
        {
            drawCircle = draw;
        }

        public void OverrideTarget(Obj_AI_Hero newtarget)
        {
            Target = newtarget;
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
            return this.Target;
        }

        public override string ToString()
        {
            return "Target: " + Target.ChampionName + "Range: " + range + "Mode: " + mode;
        }
    }
}
