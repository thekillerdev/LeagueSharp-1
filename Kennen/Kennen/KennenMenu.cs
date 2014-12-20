using LeagueSharp.Common;

namespace Kennen
{
    internal class KennenMenu
    {
        public KennenMenu()
        {
            _menu = new Menu("WorstPing | Kennen", RootName, true);
            SimpleTs.AddToMenu(_menu.AddSubMenu(new Menu("Target Selector", RootName + "_ts")));
            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalker", RootName + "_orb")));

            var combo = _menu.AddSubMenu(new Menu("Combo", RootName + "_combo"));
            combo.AddItem(new MenuItem(RootName + ComboQ, "Use Q")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboW, "Use W")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboE, "Use E")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboR, "Use R")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + "_harass_spacer", ""));
            combo.AddItem(new MenuItem(RootName + ComboWMode, "Min. Stacks to W. Enemy"))
                .SetValue(new StringList(new[] {"None", "> 1 stack", "> 2 stacks", "Stunnable only"}));
            combo.AddItem(new MenuItem(RootName + ComboWChampInRange, "Min. Enemy to W")).SetValue(new Slider(1, 1, 5));
            combo.AddItem(new MenuItem(RootName + ComboEChampsInRange, "Max. Enemy to E Gap Closer")).SetValue(new Slider(1, 1, 5));
            combo.AddItem(new MenuItem(RootName + ComboRChampsInRange, "Min. Enemy to R")).SetValue(new Slider(1, 1, 5));

            var harass = _menu.AddSubMenu(new Menu("Harass", RootName + "_harass"));
            harass.AddItem(new MenuItem(RootName + HarassQ, "Use Q")).SetValue(true);
            harass.AddItem(new MenuItem(RootName + HarassW, "Use W")).SetValue(true);
            harass.AddItem(new MenuItem(RootName + HarassE, "Use E")).SetValue(false);
            harass.AddItem(new MenuItem(RootName + HarassR, "Use R")).SetValue(false);
            harass.AddItem(new MenuItem(RootName + "_harass_spacer", ""));
            harass.AddItem(new MenuItem(RootName + HarassWMode, "Min. Stacks to W. Enemy"))
                .SetValue(new StringList(new[] {"None", "> 1 stack", "> 2 stacks", "Stunnable only"}, 3));
            harass.AddItem(new MenuItem(RootName + HarassWChampInRange, "Min. Enemy to W"))
                .SetValue(new Slider(1, 1, 5));

            var farming = _menu.AddSubMenu(new Menu("Farming", RootName + "_farming"));

            var drawing = _menu.AddSubMenu(new Menu("Drawing", RootName + "_drawing"));

            var miscellaneous = _menu.AddSubMenu(new Menu("Miscellaneous", RootName + "_misc"));
            miscellaneous.AddItem(new MenuItem(RootName + MiscPackets, "Use Packets")).SetValue(true);
            miscellaneous.AddItem(new MenuItem(RootName + MiscIgnoreSpellShields, "Ignore Spell Shields")).SetValue(true);
            miscellaneous.AddItem(new MenuItem(RootName + MiscHighChance, "Hit Chance")).SetValue(new StringList(new [] {"Low", "Medium", "High", "Very High"}, 2));

            _menu.AddItem(new MenuItem(RootName + "_spacer0", ""));
            _menu.AddItem(new MenuItem(RootName + RootMode, "Mode")).SetValue(new StringList(new [] {"Full Combo", "Reguler"}, 1));
            _menu.AddItem(new MenuItem(RootName + RootModeKey, "Mode Switch")).SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press));
            _menu.AddItem(new MenuItem(RootName + "_spacer1", ""));

            _menu.AddItem(new MenuItem(RootName + "_spacer_desc", "Kennen - the Heart of the Tempest"));
            _menu.AddToMainMenu();
        }

        #region Vars

        private const string RootName = "worstping_kennen";
        public static readonly string RootMode = "_mode";
        public static readonly string RootModeKey = "_modekey";

        public static readonly string ComboQ = "_combo_useq";
        public static readonly string ComboW = "_combo_usew";
        public static readonly string ComboWMode = "_combo_wmode";
        public static readonly string ComboWChampInRange = "_harass_wmode";
        public static readonly string ComboE = "_combo_usee";
        public static readonly string ComboEChampsInRange = "_combo_minusee";
        public static readonly string ComboR = "_combo_user";
        public static readonly string ComboRChampsInRange = "_combo_minuser";

        public static readonly string HarassQ = "_harass_useq";
        public static readonly string HarassW = "_harass_usew";
        public static readonly string HarassWMode = "_harass_wmode";
        public static readonly string HarassWChampInRange = "_harass_wmode";
        public static readonly string HarassE = "_harass_usee";
        public static readonly string HarassR = "_harass_user";
        public static readonly string HarassRChampInRange = "_harass_minuser";

        public static readonly string MiscPackets = "_misc_packets";
        public static readonly string MiscIgnoreSpellShields = "_misc_ignorespellshields";
        public static readonly string MiscHighChance = "_misc_hitchance";

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

        #endregion
    }
}