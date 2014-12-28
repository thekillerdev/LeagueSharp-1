#region

using System;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace Yasuo
{
    internal class Yasuo
    {
        #region Fields

        private static readonly Obj_AI_Base Player;
        private static readonly YasuoMenu Menu;

        private static readonly Spell QSpell;
        private static readonly Spell QWindSpell;
        private static readonly Spell WSpell;
        private static readonly Spell ESpell;
        private static readonly Spell RSpell;

        private static int _knockedUpUnits;

        #endregion

        #region Loader

        private static void Main(string[] args)
        {
            if (args != null)
            {
                Game.OnGameUpdate += PreGameOnOnGameUpdate;
            }
        }

        private static void PreGameOnOnGameUpdate(EventArgs args)
        {
            if (!(Game.Time > 20f))
            {
                return;
            }
            if (!Player.BaseSkinName.Contains("Yasuo"))
            {
                return;
            }

            Game.OnGameUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObjectOnOnDelete;
            Game.PrintChat("WorstPing | Yasuo the Unforgiven, loaded.");
            Game.OnGameUpdate -= PreGameOnOnGameUpdate;
        }

        #region EventHandlers

        private static void GameOnOnGameUpdate(EventArgs args) {}

        private static void DrawingOnOnDraw(EventArgs args) {}

        private static void GameObjectOnOnCreate(GameObject sender, EventArgs args) {}

        private static void GameObjectOnOnDelete(GameObject sender, EventArgs args) {}

        #endregion

        static Yasuo()
        {
            Player = ObjectManager.Player;

            Menu = new YasuoMenu();

            QSpell = new Spell(SpellSlot.Q, 475f);
            QWindSpell = new Spell(SpellSlot.Q, 900f);
            QSpell.SetSkillshot(0.36f, 350f, 20000f, false, SkillshotType.SkillshotLine);
            QWindSpell.SetSkillshot(0.36f, 120f, 1200f, true, SkillshotType.SkillshotLine);
            WSpell = new Spell(SpellSlot.W, 400f);
            ESpell = new Spell(SpellSlot.E, 475f);
            RSpell = new Spell(SpellSlot.R, 1200f);

            _knockedUpUnits = 0;
        }

        #endregion
    }
}