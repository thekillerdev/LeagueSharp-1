using LeagueSharp.Common;

namespace Rumble
{
    internal class RumbleMenu
    {
        public RumbleMenu()
        {
            /* HEADER */
            _menu = new Menu("WorstPing | Rumble", RootName, true);
            SimpleTs.AddToMenu(_menu.AddSubMenu(new Menu("Target Selector", RootName + "_ts")));
            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalker", RootName + "_orb")));

            /* COMBO */
            var combo = _menu.AddSubMenu(new Menu("Combo", RootName + "_combo"));
            combo.AddItem(new MenuItem(RootName + ComboQ, "Use Q")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboW, "Use W")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboE, "Use E")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboR, "Use R")).SetValue(false);
            combo.AddItem(new MenuItem(RootName + ComboRMin, "Min. enemies for R")).SetValue(new Slider(2, 1, 5));
            combo.AddItem(new MenuItem(RootName + ComboOverheat, "Allow Overheating")).SetValue(true);

            /* HARASS */
            var harass = _menu.AddSubMenu(new Menu("Harass", RootName + "_harass"));
            harass.AddItem(new MenuItem(RootName + HarassQ, "Use Q")).SetValue(true);
            harass.AddItem(new MenuItem(RootName + HarassW, "Use W")).SetValue(true);
            harass.AddItem(new MenuItem(RootName + HarassE, "Use E")).SetValue(true);
            harass.AddItem(new MenuItem(RootName + HarassOverheat, "Allow Overheating")).SetValue(true);

            /* FARMING */
            var farm = _menu.AddSubMenu(new Menu("Farming", RootName + "_farm"));
            farm.AddItem(new MenuItem(RootName + FarmQ, "Use Q")).SetValue(true);
            farm.AddItem(new MenuItem(RootName + FarmE, "Use E")).SetValue(true);
            farm.AddItem(new MenuItem(RootName + FarmOverheat, "Allow Overheating")).SetValue(true);

            /* DRAWING */
            var drawing = _menu.AddSubMenu(new Menu("Drawing", RootName + "_drawing"));
            drawing.AddItem(new MenuItem(RootName + DrawQ, "Draw Q Range")).SetValue(true);
            drawing.AddItem(new MenuItem(RootName + DrawE, "Draw E Range")).SetValue(true);
            drawing.AddItem(new MenuItem(RootName + DrawR, "Draw R Range")).SetValue(true);
            drawing.AddItem(new MenuItem(RootName + DrawKillText, "Draw Kill Text")).SetValue(true);

            /* KILL STEAL */
            var ks = _menu.AddSubMenu(new Menu("Kill Steal", RootName + "_ks"));
            ks.AddItem(new MenuItem(RootName + KsQ, "Use Q")).SetValue(true);
            ks.AddItem(new MenuItem(RootName + KsE, "Use E")).SetValue(true);
            ks.AddItem(new MenuItem(RootName + KsR, "Use R")).SetValue(false);
            ks.AddItem(new MenuItem(RootName + KsOverheat, "Allow Overheating to KS")).SetValue(true);

            /* HEAT MANAGER */
            var hm = _menu.AddSubMenu(new Menu("Heat Manager", RootName + "_hm"));
            hm.AddItem(new MenuItem(RootName + HmQ, "Use Q")).SetValue(true);
            hm.AddItem(new MenuItem(RootName + HmW, "Use W")).SetValue(true);

            /* MISC */
            var misc = _menu.AddSubMenu(new Menu("Miscellaneous", RootName + "_misc"));
            misc.AddItem(new MenuItem(RootName + MiscMinWGapcloser, "Shield(W) On Gapclose Range"))
                .SetValue(new Slider(725, 0, 1450));
            misc.AddItem(new MenuItem(RootName + MiscAutoE, "Auto Harpoon(E)")).SetValue(true);
            misc.AddItem(new MenuItem(RootName + MiscEDelay, "Seconds delay between Harpoons (E)"))
                .SetValue(new Slider(1, 0, 3));
            misc.AddItem(new MenuItem(RootName + MiscHitChance, "Hit Chance"))
                .SetValue(new StringList(new[] {"Low", "Medium", "High", "Very High"}, 2));
            misc.AddItem(new MenuItem(RootName + MiscPackets, "Use Packets")).SetValue(true);

            /* FOOTER */
            _menu.AddItem(new MenuItem(RootName + "_spacer0", ""));
            _menu.AddItem(new MenuItem(RootName + "_spacer_desc", "Rumble - the Mechanized Menace"));
            _menu.AddToMainMenu();
        }

        #region Vars

        private const string RootName = "worstping_kennen";

        public static readonly string ComboQ = "_combo_useq";
        public static readonly string ComboW = "_combo_usew";
        public static readonly string ComboE = "_combo_usee";
        public static readonly string ComboR = "_combo_user";
        public static readonly string ComboRMin = "combo_usermin";
        public static readonly string ComboOverheat = "combo_overheat";

        public static readonly string HarassQ = "_harass_useq";
        public static readonly string HarassW = "_harass_usew";
        public static readonly string HarassE = "_harass_usee";
        public static readonly string HarassOverheat = "_harass_overheat";

        public static readonly string FarmQ = "_farm_useq";
        public static readonly string FarmE = "_farm_usee";
        public static readonly string FarmOverheat = "_farm_overheat";

        public static readonly string DrawQ = "_drawing_q";
        public static readonly string DrawE = "_drawing_e";
        public static readonly string DrawR = "_drawing_r";
        public static readonly string DrawKillText = "_drawing_kt";

        public static readonly string KsQ = "_ks_useq";
        public static readonly string KsE = "_ks_usee";
        public static readonly string KsR = "_ks_user";
        public static readonly string KsOverheat = "_ks_overheat";

        public static readonly string HmQ = "_hm_useq";
        public static readonly string HmW = "_hm_usew";

        public static readonly string MiscMinWGapcloser = "_misc_minwgapcloser";
        public static readonly string MiscAutoE = "_misc_autoe";
        public static readonly string MiscEDelay = "_misc_edelay";
        public static readonly string MiscHitChance = "_misc_hitchance";
        public static readonly string MiscPackets = "_misc_packets";

        private readonly Menu _menu;
        private readonly Orbwalking.Orbwalker _orbwalker;

        #endregion

        #region Get Methods

        public Menu GetMenu()
        {
            return _menu;
        }

        public Orbwalking.Orbwalker GetOrbwalker()
        {
            return _orbwalker;
        }

        public T GetValue<T>(string str)
        {
            return _menu.Item(RootName + str).GetValue<T>();
        }

        public MenuItem GetItem(string str)
        {
            return _menu.Item(RootName + str);
        }

        #endregion
    }
}