using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Yasuo.Evade;

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
                Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;

                Yasuo.Menu = new YasuoMenu();
                new YasuoEvade();
                new YasuoInterrupter();

                Menu menu = null;
                var flag =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(e => e.IsEnemy)
                        .Any(e => Yasuo.Interrupters.Any(i2 => i2.GetChampion() == e.BaseSkinName));
                if (flag)
                {
                    menu = Yasuo.Menu.AddSubMenu("Interrupt Settings", ".interrupt");
                    menu.AddItem(new MenuItem(YasuoMenu.RootName + "interrupt.use", "Use Interrupter")).SetValue(true);
                    menu.AddItem(new MenuItem(YasuoMenu.RootName + "interrupt.underturret", "Interrupt under turret"))
                        .SetValue(true);
                    menu.AddItem(new MenuItem(YasuoMenu.RootName + "interrupt.specialspacer0", ""));
                }

                if (menu != null)
                {
                    foreach (
                        var ii in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(e => e.IsEnemy)
                                .SelectMany(i1 => Yasuo.Interrupters.Where(i2 => i2.GetChampion() == i1.BaseSkinName)))
                    {
                        Yasuo.Menu.AddItem(menu, ii.GetMenuDisplayString(), ii.GetMenuString()).SetValue(true);
                    }
                }

                var enemies = ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsEnemy);

                foreach (var e in enemies)
                {
                    var e1 = e;
                    foreach (var spell in SpellDatabase.Spells.Where(s => s.ChampionName == e1.BaseSkinName))
                    {
                        // => Windwall
                        if (spell.CollisionObjects.Any(e2 => e2 == CollisionObjectTypes.YasuoWall))
                        {
                            var spellActualName = spell.ChampionName;
                            var slot = "?";
                            switch (spell.Slot)
                            {
                                case SpellSlot.Q:
                                    spellActualName += " Q";
                                    slot = "Q";
                                    break;
                                case SpellSlot.W:
                                    spellActualName += " W";
                                    slot = "W";
                                    break;
                                case SpellSlot.E:
                                    spellActualName += " E";
                                    slot = "E";
                                    break;
                                case SpellSlot.R:
                                    spellActualName += " R";
                                    slot = "R";
                                    break;
                            }
                            var theSpell = new Yasuo.MenuData
                            {
                                ChampionName = spell.ChampionName,
                                SpellName = spell.SpellName,
                                SpellDisplayName = spellActualName,
                                DisplayName = spellActualName,
                                Slot = slot,
                                IsWindwall = true
                            };
                            theSpell.AddToMenu();
                            Yasuo.MenuWallsList.Add(theSpell);
                        }

                        // => Evade
                        var eVspellActualName = spell.ChampionName;
                        var eVslot = "?";
                        switch (spell.Slot)
                        {
                            case SpellSlot.Q:
                                eVspellActualName += " Q";
                                eVslot = "Q";
                                break;
                            case SpellSlot.W:
                                eVspellActualName += " W";
                                eVslot = "W";
                                break;
                            case SpellSlot.E:
                                eVspellActualName += " E";
                                eVslot = "E";
                                break;
                            case SpellSlot.R:
                                eVspellActualName += " R";
                                eVslot = "R";
                                break;
                        }
                        var eVtheSpell = new Yasuo.MenuData
                        {
                            ChampionName = spell.ChampionName,
                            SpellName = spell.SpellName,
                            SpellDisplayName = eVspellActualName,
                            DisplayName = eVspellActualName,
                            Slot = eVslot,
                            IsWindwall = false
                        };
                        eVtheSpell.AddToMenu();
                        Yasuo.MenuDashesList.Add(eVtheSpell);
                    }
                }

                Game.PrintChat("WorstPing | Yasuo the Unforgiven, loaded.");
            };
        }

        private static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Yasuo.Player.HasWhirlwind() || !(Yasuo.Player.Distance(sender.Position) <= YasuoSpells.E.Range) || !Yasuo.Menu.GetItemValue<bool>(YasuoMenu.RootName + "interrupt.use"))
            {
                return;
            }

            var interrupt =
                Yasuo.Interrupters.Any(
                    i =>
                        i.GetChampion() == sender.BaseSkinName && i.GetSpellName() == args.SData.Name &&
                        Yasuo.Menu.GetItemValue<bool>(i.GetMenuString()));
            if (!interrupt)
            {
                return;
            }

            if ((Yasuo.Player.GetDashingEnd(sender).To3D().UnderTurret(true) &&
                 Yasuo.Menu.GetItemValue<bool>(YasuoMenu.RootName + "interrupt.underturret")) ||
                !Yasuo.Player.GetDashingEnd(sender).To3D().UnderTurret(true))
            {

                YasuoSpells.E.Cast(sender);
                Utility.DelayAction.Add(
                    100, () =>
                    {
                        YasuoSpells.QWind.Cast(Yasuo.Menu.GetItemValue<bool>(YasuoMenu.MiscPacketsLoc));
                    });
            }
        }
    }
}