using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeagueLib
{
    public class Item
    {
        // BASE STATS
        private readonly int ItemId;
        private readonly string ItemName;
        private readonly int MaxStacks;
        private readonly int Price;
        private readonly int SellValue;
        private readonly bool IsRecipe;
        private readonly IClass ItemClass;

        // RECIPE
        private readonly Item[] RecipeItems;

        // CATEGORY
        public readonly bool categoryCriticalStrike;
        public readonly bool categoryHealthRegen;
        public readonly bool categoryConsumable;
        public readonly bool categoryHealth;
        public readonly bool categoryDamage;
        public readonly bool categoryManaRegen;
        public readonly bool categorySpellBlock;
        public readonly bool categoryAttackSpeed;
        public readonly bool categoryLifeSteal;
        public readonly bool categorySpellDamage;
        public readonly bool categoryMana;
        public readonly bool categoryArmor;

        // Data
        public readonly float FlatCritDamageMod;
        public readonly float FlatPhysicalDamageMod;
        public readonly float FlatMovementSpeedMod;
        public readonly float FlatCritChanceMod;
        public readonly float FlatMagicDamageMod;
        public readonly float FlatHPRegenMod;
        public readonly float FlatHPPoolMod;
        public readonly float FlatSpellBlockMod;
        public readonly float FlatArmorMod;
        public readonly float PrecentAttackSpeedMod;
        public readonly float PercentSpellBlockMod;
        public readonly float PercentHPPoolMod;
        public readonly float PercentCritDamageMod;
        public readonly float PercentArmorMod;
        public readonly float PercentEXPBonus;
        public readonly float PercentHPRegenMod;
        public readonly float PercentMagicDamageMod;
        public readonly float PercentMovementSpeedMod;
        public readonly float PercentPhysicalDamageMod;

        public Item(int ItemId, string ItemName, int MaxStacks, int Price, bool IsRecipe, IClass ItemClass,
            bool categoryCriticalStrike, bool categoryHealthRegen, bool categoryConsumable, bool categoryHealth, bool categoryDamage, bool categoryManaRegen, bool categorySpellBlock, bool categoryAttackSpeed, bool categoryLifeSteal, bool categorySpellDamage, bool categoryMana, bool categoryArmor,
            float FlatCritDamageMod, float FlatPhysicalDamageMod, float FlatMovementSpeedMod, float FlatCritChanceMod, float FlatMagicDamageMod, float FlatHPRegenMod, float FlatHPPoolMod, float FlatSpellBlockMod, float FlatArmorMod,
            float PrecentAttackSpeedMod, float PercentSpellBlockMod, float PercentHPPoolMod, float PercentCritDamageMod, float PercentArmorMod, float PercentEXPBonus, float PercentHPRegenMod, float PercentMagicDamageMod, float PercentMovementSpeedMod, float PercentPhysicalDamageMod,
            Item[] RecipeItems = null)
        {
            // BASE STATS
            this.ItemId = ItemId;
            this.ItemName = ItemName;
            this.MaxStacks = MaxStacks;
            this.Price = Price;

            if (ItemId == 3069 || ItemId == 3092 || ItemId == 1055 || ItemId == 1054 || ItemId == 1039 || ItemId == 1062 || ItemId == 1063)
                this.SellValue = (Price * 30) / 100;
            else
                this.SellValue = (Price * 70) / 100;

            this.IsRecipe = IsRecipe;
            this.ItemClass = ItemClass;

            // RECIPE
            this.RecipeItems = RecipeItems;

            // CATEGORY
            this.categoryCriticalStrike = categoryCriticalStrike;
            this.categoryHealthRegen = categoryHealthRegen;
            this.categoryConsumable = categoryConsumable;
            this.categoryHealth = categoryHealth;
            this.categoryDamage = categoryDamage;
            this.categoryManaRegen = categoryManaRegen;
            this.categorySpellBlock = categorySpellBlock;
            this.categoryAttackSpeed = categoryAttackSpeed;
            this.categoryLifeSteal = categoryLifeSteal;
            this.categorySpellDamage = categorySpellDamage;
            this.categoryMana = categoryMana;
            this.categoryArmor = categoryArmor;

            // Data
            this.FlatCritDamageMod = FlatCritDamageMod;
            this.FlatPhysicalDamageMod = FlatPhysicalDamageMod;
            this.FlatMovementSpeedMod = FlatMovementSpeedMod;
            this.FlatCritChanceMod = FlatCritChanceMod;
            this.FlatMagicDamageMod = FlatMagicDamageMod;
            this.FlatHPRegenMod = FlatHPRegenMod;
            this.FlatHPPoolMod = FlatHPPoolMod;
            this.FlatSpellBlockMod = FlatSpellBlockMod;
            this.FlatArmorMod = FlatArmorMod;
            this.PrecentAttackSpeedMod = PrecentAttackSpeedMod;
            this.PercentSpellBlockMod = PercentSpellBlockMod;
            this.PercentHPPoolMod = PercentHPPoolMod;
            this.PercentCritDamageMod = PercentCritDamageMod;
            this.PercentArmorMod = PercentArmorMod;
            this.PercentEXPBonus = PercentEXPBonus;
            this.PercentHPRegenMod = PercentHPRegenMod;
            this.PercentMagicDamageMod = PercentMagicDamageMod;
            this.PercentMovementSpeedMod = PercentMovementSpeedMod;
            this.PercentPhysicalDamageMod = PercentPhysicalDamageMod;
        }

        public int GetId()
        {
            return this.ItemId;
        }

        public string GetName()
        {
            return this.ItemName;
        }

        public int GetMaxStacks()
        {
            return this.MaxStacks;
        }

        public int GetPrice()
        {
            return this.Price;
        }

        public int GetSellPrice()
        {
            return this.SellValue;
        }

        public bool IsRecipeComponent()
        {
            return this.IsRecipe;
        }

        public IClass GetTier()
        {
            return this.ItemClass;
        }

        public Item[] GetComponents()
        {
            return this.RecipeItems;
        }

        // Trees stuff, idk
        public int GetFullPriceValue()
        {
            return 0; // TODO
        }

        public void Buy()
        {
            ObjectManager.Player.BuyItem(GetItemId());
        }

        public LeagueSharp.ItemId GetItemId()
        {
            return (LeagueSharp.ItemId)ItemId;
        }

        public List<ShopItem> GetComponentsList()
        {
            return null; // TODO
        }
    }

    public enum IClass
    { // Tier.
        Basic,
        Advanced,
        Legendary,
        Mythical,
        Enchantment,
        Consumable,
        RengarsTrinket,
        None
    }
}
