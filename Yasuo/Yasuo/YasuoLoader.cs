using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

// ReSharper disable ObjectCreationAsStatement

namespace Yasuo
{
    public class YasuoLoader
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                return;
            }

            CustomEvents.Game.OnGameLoad += eventArgs =>
            {
                Yasuo.Player = ObjectManager.Player;

                if (Yasuo.Player.BaseSkinName != "Yasuo")
                {
                    return;
                }

                Game.OnGameUpdate += YasuoGame.OnGameUpdate;
                Drawing.OnDraw += YasuoDrawing.OnDraw;
                GameObject.OnCreate += GameObjectOnOnCreate;
                GameObject.OnDelete += GameObjectOnOnDelete;
                Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;

                new YasuoSpells();
                Yasuo.Menu = new YasuoMenu();

                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Caitlyn", "CaitlynAceintheHole", YasuoMenu.InterruptAceInTheHole,
                        YasuoMenu.InterruptAceInTheHoleLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Nunu", "AbsoluteZero", YasuoMenu.InterruptAbsoluteZero, YasuoMenu.InterruptAbsoluteZeroLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "FiddleSticks", "Crowstorm", YasuoMenu.InterruptCrowstorm, YasuoMenu.InterruptCrowstormLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "FiddleSticks", "DrainChannel", YasuoMenu.InterruptDrainChannel,
                        YasuoMenu.InterruptDrainChannelLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Galio", "GalioIdolOfDurand", YasuoMenu.InterruptIdolOfDurand,
                        YasuoMenu.InterruptIdolOfDurandLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Karthus", "FallenOne", YasuoMenu.InterruptFallenOne, YasuoMenu.InterruptFallenOneLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Katarina", "KatarinaR", YasuoMenu.InterruptKatarinaR, YasuoMenu.InterruptKatarinaRLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Malzahar", "AlZaharNetherGrasp", YasuoMenu.InterruptAlZaharNetherGrasp,
                        YasuoMenu.InterruptAlZaharNetherGraspLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "MissFortune", "MissFortuneBulletTime", YasuoMenu.InterruptBulletTime,
                        YasuoMenu.InterruptBulletTimeLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Pantheon", "Pantheon_GrandSkyfall_Jump", YasuoMenu.InterruptGrandSkyfall,
                        YasuoMenu.InterruptGrandSkyfallLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Shen", "ShenStandUnited", YasuoMenu.InterruptStandUnited, YasuoMenu.InterruptStandUnitedLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Urgot", "UrgotSwap2", YasuoMenu.InterruptUrgotSwap, YasuoMenu.InterruptUrgotSwapLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter(
                        "Warwick", "InfiniteDuress", YasuoMenu.InterruptInfiniteDuress,
                        YasuoMenu.InterruptInfiniteDuressLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter("Varus", "VarusQ", YasuoMenu.InterruptVarusQ, YasuoMenu.InterruptVarusQLoc));
                Yasuo.Interrupters.Add(
                    new YasuoInterrupter("Sion", "SionQ", YasuoMenu.InterruptSionQ, YasuoMenu.InterruptSionQLoc));


                var menu = Yasuo.Menu.AddSubMenu(YasuoMenu.Interrupter, YasuoMenu.InterrupterLoc);
                foreach (var i in ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsEnemy))
                {
                    var i1 = i;
                    foreach (var ii in Yasuo.Interrupters.Where(i2 => i2.GetChampion() == i1.BaseSkinName))
                    {
                        Yasuo.Menu.AddItem(menu, ii.GetMenuDisplayString(), ii.GetMenuString()).SetValue(true);
                    }
                }

                Game.PrintChat("WorstPing | Yasuo the Unforgiven, loaded.");
            };
        }

        private static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Yasuo.Player.HasWhirlwind() || !(Yasuo.Player.Distance(sender.Position) <= YasuoSpells.E.Range))
            {
                return;
            }

            foreach (var i in
                Yasuo.Interrupters.Where(i => i.GetChampion() == sender.BaseSkinName)
                    .Where(i => i.GetSpellName() == args.SData.Name && Yasuo.Menu.GetItemValue<bool>(i.GetMenuString()))
                )
            {
                YasuoSpells.E.Cast(sender);
                Utility.DelayAction.Add(
                    250, () =>
                    {
                        YasuoSpells.QWind.Cast(Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc));
                    });
            }
        }

        private static void GameObjectOnOnDelete(GameObject sender, EventArgs args) {}
        private static void GameObjectOnOnCreate(GameObject sender, EventArgs args) {}
    }
}