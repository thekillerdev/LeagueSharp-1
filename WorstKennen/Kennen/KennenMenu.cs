using LeagueSharp.Common;

namespace WorstKennen
{
    internal class KennenMenu
    {
        private const string RootName = "worstping_kennen";
        public static readonly string ComboQ = "_combo_useq";
        public static readonly string ComboW = "_combo_usew";
        public static readonly string ComboE = "_combo_usee";
        public static readonly string ComboR = "_combo_user";
        public static readonly string ComboRChampInRange = "_combo_minuser";
        public static readonly string ComboEMode = "_combo_emode";
        public static readonly string HarassQ = "_harass_useq";
        public static readonly string MiscPackets = "_misc_packets";
        private readonly Menu _menu;
        private readonly Orbwalking.Orbwalker _orbwalker;

        public KennenMenu()
        {
            _menu = new Menu("WorstPing | Kennen", RootName, true);
            SimpleTs.AddToMenu(_menu.AddSubMenu(new Menu("Target Selector", RootName + "_ts")));
            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalker", RootName + "_orb")));

            var combo = _menu.AddSubMenu(new Menu("Combo", RootName + "_combo"));
            combo.AddItem(new MenuItem(RootName + ComboQ, "Use Q")).SetValue(true);

            var harass = _menu.AddSubMenu(new Menu("Harass", RootName + "_harass"));
            harass.AddItem(new MenuItem(RootName + HarassQ, "Use Q")).SetValue(true);

            var farming = _menu.AddSubMenu(new Menu("Farming", RootName + "_farming"));

            var miscellaneous = _menu.AddSubMenu(new Menu("Miscellaneous", RootName + "_misc"));
            miscellaneous.AddItem(new MenuItem(RootName + MiscPackets, "Use Packets")).SetValue(true);

            _menu.AddItem(new MenuItem(RootName + "_spacer_desc", "Kennen - the Heart of the Tempest"));
            _menu.AddToMainMenu();
        }

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
    }
}