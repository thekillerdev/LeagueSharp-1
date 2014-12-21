using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Rumble
{
    internal class Rumble
    {
        #region Spells

        private static void RumbleCombo()
        {
        }

        private static void RumbleHarass()
        {
        }

        private static void RumbleFlee()
        {
        }

        #endregion

        #region Main

        static Rumble()
        {
            Menu = new RumbleMenu();

            PlayerObjAiHero = ObjectManager.Player;

            RSpell = new Spell(SpellSlot.R, 1700f);
            ESpell = new Spell(SpellSlot.E, 850f);
            WSpell = new Spell(SpellSlot.W);
            QSpell = new Spell(SpellSlot.Q, 600f);
        }

        private static void Main()
        {
            if (!PlayerObjAiHero.ChampionName.Equals("Rumble"))
                return;

            //ESpell.SetSkillshot();

            CustomEvents.Game.OnGameLoad += args =>
            {
                Game.OnGameUpdate += GameOnOnGameUpdate;
                Drawing.OnDraw += DrawingOnOnDraw;
                Game.PrintChat("WorstPing | Rumble the Mechanized Menace, loaded.");
            };
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            switch (Menu.GetOrbwalker().ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    RumbleCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    RumbleLaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    RumbleHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    RumbleLastHit();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    RumbleFlee();
                    break;
            }
        }

        #endregion

        #region Farming

        private static void RumbleLaneClear()
        {
        }

        private static void RumbleLastHit()
        {
        }

        #endregion

        #region Vars

        private static readonly Spell QSpell;
        private static readonly Spell WSpell;
        private static readonly Spell ESpell;
        private static readonly Spell RSpell;

        private static readonly Obj_AI_Hero PlayerObjAiHero;
        private static readonly RumbleMenu Menu;

        #endregion
    }
}