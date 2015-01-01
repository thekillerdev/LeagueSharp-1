using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Yasuo
{
    public static class YasuoDamage
    {
        /// <summary>
        ///     Returns Yasuo's Steel Tempest (Q) damage
        /// </summary>
        /// <param name="base">Base object</param>
        /// <param name="target">Target object</param>
        /// <param name="forceExtraDamage">Force Extra Damage</param>
        /// <returns>Steel Tempest Damage</returns>
        public static double GetSteelTempestDamage(this Obj_AI_Base @base, Obj_AI_Base target, bool forceExtraDamage = false)
        {
            var damage = @base.GetSpellDamage(
                target, (@base.HasWhirlwind() ? YasuoSpells.QWind.Instance.Name : YasuoSpells.Q.Instance.Name));

            if ((int) (@base.Crit) == 1)
            {
                damage *= 1.625d;
                // TODO: More accurate crit damage
            }

            if ((@base.HasBuff("ItemStatikShankCharge") && !@base.HasWhirlwind()) || forceExtraDamage)
            {
                double magicResist = (target.SpellBlock * @base.PercentMagicPenetrationMod) -
                                     @base.FlatMagicPenetrationMod;
                var damageMultipiler = (magicResist >= 0)
                    ? (100 / (100 + magicResist))
                    : (2 - 100 / (100 - magicResist));
                damageMultipiler = PassivePercentMod(@base, target, damageMultipiler);
                damage += (damageMultipiler * 100 + PassiveFlatMod(@base, target));
            }

            // TODO: Add Hydra / Tiamat
            return damage;
        }

        /// <summary>
        ///     Returns Yasuo's Sweeping Blade (E) damage
        /// </summary>
        /// <param name="base">Base object</param>
        /// <param name="target">Target object</param>
        /// <param name="damageModifier">Damage Modifier (Sweeping Blade Stacks)</param>
        /// <returns>Sweeping Blade Damage</returns>
        public static double GetSweepingBladeDamage(this Obj_AI_Base @base, Obj_AI_Base target, double damageModifier)
        {
            var damage = 50 + (20 * YasuoSpells.E.Level * damageModifier + (@base.FlatMagicDamageMod * 0.6));
            return @base.CalcDamage(target, Damage.DamageType.Magical, damage);
        }

        private static double PassivePercentMod(Obj_AI_Base source, Obj_AI_Base target, double k)
        {
            var siegeMinionList = new List<string> { "Red_Minion_MechCannon", "Blue_Minion_MechCannon" };
            var normalMinionList = new List<string>
            {
                "Red_Minion_Wizard",
                "Blue_Minion_Wizard",
                "Red_Minion_Basic",
                "Blue_Minion_Basic"
            };

            //Minions and towers passives:
            if (source is Obj_AI_Turret)
            {
                //Siege minions receive 70% damage from turrets
                if (siegeMinionList.Contains(target.BaseSkinName))
                {
                    k = 0.7d * k;
                }

                //Normal minions take 114% more damage from towers.
                else if (normalMinionList.Contains(target.BaseSkinName))
                {
                    k = (1 / 0.875) * k;
                }

                // Turrets deal 105% damage to champions for the first attack.
                else if (target is Obj_AI_Hero)
                {
                    k = 1.05 * k;
                }
            }

            //Masteries:

            //Offensive masteries:
            var hero = source as Obj_AI_Hero;
            if (hero != null)
            {
                var sourceAsHero = hero;

                //Double edge sword:
                //  Melee champions: You deal 2% increase damage from all sources, but take 1% increase damage from all sources.
                //  Ranged champions: You deal and take 1.5% increased damage from all sources. 
                if (sourceAsHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
                {
                    if (sourceAsHero.CombatType == GameObjectCombatType.Melee)
                    {
                        k = k * 1.02d;
                    }
                    else
                    {
                        k = k * 1.015d;
                    }
                }

                //Havoc:
                //  Increases damage by 3%. 
                if (sourceAsHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 146 && m.Points == 1))
                {
                    k = k * 1.03d;
                }

                //Executioner
                //  Increases damage dealt to champions below 20 / 35 / 50% by 5%. 
                if (target is Obj_AI_Hero)
                {
                    var mastery =
                        (sourceAsHero).Masteries.FirstOrDefault(m => m.Page == MasteryPage.Offense && m.Id == 100);
                    if (mastery != null && mastery.Points >= 1 &&
                        target.Health / target.MaxHealth <= 0.05d + 0.15d * mastery.Points)
                    {
                        k = k * 1.05;
                    }
                }
            }


            if (!(target is Obj_AI_Hero))
            {
                return k;
            }

            var targetAsHero = (Obj_AI_Hero) target;

            //Defensive masteries:

            //Double edge sword:
            //     Melee champions: You deal 2% increase damage from all sources, but take 1% increase damage from all sources.
            //     Ranged champions: You deal and take 1.5% increased damage from all sources. 
            if (!targetAsHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
            {
                return k;
            }

            if (target.CombatType == GameObjectCombatType.Melee)
            {
                k = k * 1.01d;
            }
            else
            {
                k = k * 1.015d;
            }

            return k;
        }

        private static double PassiveFlatMod(Obj_AI_Base source, Obj_AI_Base target)
        {
            double d = 0;

            if (!(source is Obj_AI_Hero))
            {
                return d;
            }

            //Offensive masteries:

            //Butcher
            //  Basic attacks and single target abilities do 2 bonus damage to minions and monsters. 
            if (!(target is Obj_AI_Minion))
            {
                return d;
            }

            if (((Obj_AI_Hero) source).Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
            {
                d = d + 2;
            }

            return d;
        }
    }
}