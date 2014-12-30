#region

using LeagueSharp.Common;

#endregion

namespace Yasuo
{
    internal class YasuoMenu
    {
        #region Menu

        public YasuoMenu()
        {
            /* HEADER */
            _menu = new Menu("WorstPing | Yasuo", RootName, true);
            TargetSelector.AddToMenu(_menu.AddSubMenu(new Menu("Target Selector", RootName + "_ts")));
            _orbwalker = new Orbwalking.Orbwalker(_menu.AddSubMenu(new Menu("Orbwalker", RootName + "_orb")));

            _menu.AddSubMenu(new Menu("", RootName + ".spacer0"));

            /* COMBO */
            var combo = _menu.AddSubMenu(new Menu("Combo Settings", RootName + ".combo"));
            combo.AddItem(new MenuItem(RootName + ComboQ, "Use Steel Tempest (Q)")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboQWind, "Use Whirlwind (3rd Q)")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboE, "Use Sweeping Blade (E)")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ".combo.spacer0", ""));
            combo.AddItem(new MenuItem(RootName + ComboR, "Use Last Breath (R)")).SetValue(true);
            combo.AddItem(new MenuItem(RootName + ComboRMin, "Min. enemies for ult")).SetValue(new Slider(1, 1, 5));
            combo.AddItem(new MenuItem(RootName + ComboRPer, "% of enemies health")).SetValue(new Slider(40, 5));

            /* FARMING */
            var farming = _menu.AddSubMenu(new Menu("Farming Settings", RootName + ".farming"));
            farming.AddItem(new MenuItem(RootName + FarmingLaneClearQ, "[LaneClear] Use Steel Tempest (Q)"))
                .SetValue(true);
            farming.AddItem(new MenuItem(RootName + FarmingLaneClearE, "[LaneClear] Use Sweeping Blade (E)"))
                .SetValue(true);
            farming.AddItem(new MenuItem(RootName + ".farming.spacer0", ""));
            farming.AddItem(new MenuItem(RootName + FarmingLastHitQ, "[LastHit] Use Steel Tempest (Q)")).SetValue(true);
            farming.AddItem(new MenuItem(RootName + FarmingLastHitE, "[LastHit] Use Sweeping Blade (E)")).SetValue(true);
            farming.AddItem(new MenuItem(RootName + ".farming.spacer1", ""));
            farming.AddItem(new MenuItem(RootName + FarmingUseWind, "[Both] Use Whirlwind (3rd Q)")).SetValue(true);

            /* FLEE */
            var flee = _menu.AddSubMenu(new Menu("Flee Settings", RootName + ".flee"));
            flee.AddItem(new MenuItem(RootName + FleeMode, "Enable Flee Mode")).SetValue(true);
            flee.AddItem(new MenuItem(RootName + FleeModeKey, "Flee Key"))
                .SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press));
            flee.AddItem(new MenuItem(RootName + ".flee.spacer0", ""));
            flee.AddItem(new MenuItem(RootName + FleeE, "Use Sweeping Blade (E)")).SetValue(true);
            flee.AddItem(new MenuItem(RootName + FleeW, "Use Wind Wall (W)")).SetValue(true);

            /* EVADE */
            var evade = _menu.AddSubMenu(new Menu("Evade Settings", RootName + ".evade"));

            /* INTERRUPT */
            var interrupt = _menu.AddSubMenu(new Menu("Interrupter Settings", RootName + ".interrupter"));
            interrupt.AddItem(new MenuItem(RootName + InterruptW, "Auto Wind Wall (W)")).SetValue(true);
            interrupt.AddItem(new MenuItem(RootName + InterruptWDelay, "Auto Wind Wall Delay"))
                .SetValue(new Slider(0, 0, 5));
            interrupt.AddItem(new MenuItem(RootName + ".interrupter.space0", ""));
            interrupt.AddItem(new MenuItem(RootName + InterruptActive, "Interrupt Skills")).SetValue(true);

            /* MISC */
            var misc = _menu.AddSubMenu(new Menu("Misc Settings", RootName + ".misc"));
            misc.AddItem(new MenuItem(RootName + MiscAutoQ, "Auto Q")).SetValue(true);
            misc.AddItem(new MenuItem(RootName + MiscAutoQUnderTower, "Auto Q under tower")).SetValue(true);
            misc.AddItem(new MenuItem(RootName + MiscQRange, "Q Range")).SetValue(new Slider(475, 0, 475));
            misc.AddItem(new MenuItem(RootName + ".misc.spacer0", ""));
            misc.AddItem(new MenuItem(RootName + MiscAutoR, "Auto Last Breath (R)")).SetValue(true);
            misc.AddItem(new MenuItem(RootName + MiscRMin, "Min. enemies to Auto Last Breath (R)")).SetValue(new Slider(1, 1, 5));
            misc.AddItem(new MenuItem(RootName + ".misc.spacer1", ""));
            misc.AddItem(new MenuItem(RootName + MiscHitChance, "Hit Chance")).SetValue(new StringList(new[] { "Low", "Medium", "High", "VeryHigh" }, 1));
            misc.AddItem(new MenuItem(RootName + MiscPackets, "Use Packets")).SetValue(true);

            /* FOOTER */
            _menu.AddItem(new MenuItem(RootName + "_spacer1", ""));
            _menu.AddItem(new MenuItem(RootName + "_spacer_desc", "Yasuo - the Unforgiven"));
            _menu.AddToMainMenu();
        }

        #endregion

        #region Fields

        private const string RootName = "worstping_yasuo";

        public const string ComboQ = ".combo.useq";
        public const string ComboQWind = ".combo.useqwind";
        public const string ComboE = ".combo.usee";
        public const string ComboR = ".combo.user";
        public const string ComboRMin = ".combo.usermin";
        public const string ComboRPer = ".combo.userper";

        public const string FarmingLaneClearQ = ".farming.lcq";
        public const string FarmingLaneClearE = ".farming.lce";
        public const string FarmingLastHitQ = ".farming.lhq";
        public const string FarmingLastHitE = ".farming.lhe";
        public const string FarmingUseWind = ".farming.useq3";

        public const string FleeMode = ".flee.active";
        public const string FleeModeKey = ".flee.key";
        public const string FleeE = ".flee.usee";
        public const string FleeW = ".flee.usew";

        public const string InterruptActive = ".interrupt.active";
        public const string InterruptW = ".interrupt.usew";
        public const string InterruptWDelay = ".interrupt.usewdelay";
        public const string InterruptAceInTheHole = ".interrupt.caitlyn.aceinthehole";
        public const string InterruptCrowstorm = ".interrupt.fiddlesticks.crowstorm";
        public const string InterruptDrainChannel = ".interrupt.fiddlesticks.drawinchannel";
        public const string InterruptIdolOfDurand = ".interrupt.galio.idolofdurand";
        public const string InterruptFallenOne = ".interrupt.karthus.falleone";
        public const string InterruptKatarinaR = ".interrupt.katarina.r";
        public const string InterruptNetherGrasp = ".interrupt.malzahar.nethergrasp";
        public const string InterruptBulletTime = ".interrupt.missfortune.bullettime";
        public const string InterruptAbsoluteZero = ".interrupt.nunu.absolutezero";
        public const string InterruptGrandSkyfall = ".interrupt.pantheon.grandskyfall";
        public const string InterruptStandUnited = ".interrupt.shen.standunited";
        public const string InterruptUrgotSwap = ".interrupt.urgot.swap";
        public const string InterruptVarusQ = ".interrupt.varus.q";
        public const string InterruptIfiniteDuress = ".interrupt.warwick.infiniteduress";

        public const string MiscAutoQ = ".misc.autoq";
        public const string MiscAutoQUnderTower = ".misc.autoqundertower";
        public const string MiscQRange = ".misc.qrange";
        public const string MiscAutoR = ".misc.autor";
        public const string MiscRMin = ".misc.autormin";
        public const string MiscHitChance = ".misc.hitchance";
        public const string MiscPackets = ".misc.packets";

        private readonly Menu _menu;
        private readonly Orbwalking.Orbwalker _orbwalker;

        #endregion

        #region Functions

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