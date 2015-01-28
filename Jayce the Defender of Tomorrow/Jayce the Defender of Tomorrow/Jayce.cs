using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jayce
{
    internal class Jayce
    {
        public static Menu Menu;
        public Orbwalking.Orbwalker Orbwalker;
        public Obj_AI_Hero Player;
        private Obj_AI_Base _target;

        public Jayce()
        {
            if (!ObjectManager.Player.ChampionName.Equals(GetType().Name))
            {
                return;
            }

            Player = ObjectManager.Player;

            Menu = new Menu("L33T | Jayce", "l33t.jayce", true);

            var orbwalker = new Menu("Orbwalker", "l33t.jayce.orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalker);
            Menu.AddSubMenu(orbwalker);

            var ts = new Menu("Target Selector", "l33t.jayce.ts");
            TargetSelector.AddToMenu(ts);
            Menu.AddSubMenu(ts);

            var combo = new Menu("Combo Settings", "l33t.jayce.combo");
            var cannon = new Menu("Transform: Mercury Cannon", "l33t.jayce.combo.cannon");
            var hammer = new Menu("Transform: Mercury Hammer", "l33t.jayce.combo.hammer");
            hammer.AddItem(new MenuItem("l33t.jayce.combo._q0", "Use To The Skies! (Q)")).SetValue(true);
            cannon.AddItem(new MenuItem("l33t.jayce.combo._q1", "Use Shock Blast (Q)")).SetValue(true);
            hammer.AddItem(new MenuItem("l33t.jayce.combo._w0", "Use Lightning Field (W)")).SetValue(true);
            cannon.AddItem(new MenuItem("l33t.jayce.combo._w1", "Use Hyper Charge (W)")).SetValue(true);
            hammer.AddItem(new MenuItem("l33t.jayce.combo._e0", "Use Thundering Blow (E)")).SetValue(true);
            cannon.AddItem(new MenuItem("l33t.jayce.combo._e1", "Use Acceleration Gate (E)")).SetValue(true);
            hammer.AddItem(new MenuItem("l33t.jayce.combo._r0", "Use Transform: Mercury Hammer (R)")).SetValue(true);
            cannon.AddItem(new MenuItem("l33t.jayce.combo._r1", "Use Transform: Mercury Cannon (R)")).SetValue(true);

            cannon.AddItem(new MenuItem("l33t.jayce.combo.spacer.0", ""));
            cannon.AddItem(new MenuItem("l33t.jayce.combo.c_qeMana", "Minimum % of Mana for Q + E")).SetValue(new Slider(60));
            cannon.AddItem(new MenuItem("l33t.jayce.combo.c_qMana", "Minimum % of Mana for Q")).SetValue(new Slider(30));
            cannon.AddItem(new MenuItem("l33t.jayce.combo.c_wMana", "Minimum % of Mana for W")).SetValue(new Slider(30));

            hammer.AddItem(new MenuItem("l33t.jayce.combo.spacer.1", ""));
            hammer.AddItem(new MenuItem("l33t.jayce.combo.h_eqMana", "Minimum % of Mana for E + Q")).SetValue(new Slider(60));
            hammer.AddItem(new MenuItem("l33t.jayce.combo.h_wMana", "Minimum % of Mana for W")).SetValue(new Slider(60));
            hammer.AddItem(new MenuItem("l33t.jayce.combo.h_wEnemies", "Minimum % of Enemies for W")).SetValue(new Slider(1, 1, 5));
            hammer.AddItem(new MenuItem("l33t.jayce.combo.h_qDistance", "Distance for Q Gapcloser"))
                .SetValue(new Slider(125, 0, 599));
            hammer.AddItem(new MenuItem("l33t.jayce.combo.h_bTowers", "Prevent Q into Tower")).SetValue(true);

            combo.AddSubMenu(hammer);
            combo.AddSubMenu(cannon);

            Menu.AddSubMenu(combo);

            Menu.AddItem(new MenuItem("l33t.jayce.spacer.0", ""));
            Menu.AddItem(new MenuItem("l33t.jayce.spacer.1", "Jayce the Defender of Tomorrow"));
            Menu.AddToMainMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static void Main(string[] args)
        {
            if (args != null)
            {
                CustomEvents.Game.OnGameLoad += eventArgs => { var exec = new Jayce(); };
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Mechanics.ProcessCombo(Player, Target);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }
        }

        public Obj_AI_Base Target
        {
            get
            {
                if (_target.IsValidTarget(1337f))
                {
                    return _target;
                }
                return _target = TargetSelector.GetTarget(1337f, TargetSelector.DamageType.Physical);
            }
        }
    }
}