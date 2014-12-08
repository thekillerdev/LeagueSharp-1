#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;

#endregion

namespace LeagueLib
{
    public class Item
    {
        public readonly float FlatArmorMod;
        public readonly float FlatCritChanceMod;
        // Data
        public readonly float FlatCritDamageMod;
        public readonly float FlatHPPoolMod;
        public readonly float FlatHPRegenMod;
        public readonly float FlatMagicDamageMod;
        public readonly float FlatMovementSpeedMod;
        public readonly float FlatPhysicalDamageMod;
        public readonly float FlatSpellBlockMod;
        private readonly bool IsRecipe;
        // CATEGORY
        private readonly ItemCategory ItemCategory;
        private readonly ItemClass ItemClass;
        // BASE STATS
        private readonly int ItemId;
        private readonly string ItemName;
        private readonly int MaxStacks;
        public readonly float PercentArmorMod;
        public readonly float PercentCritDamageMod;
        public readonly float PercentEXPBonus;
        public readonly float PercentHPPoolMod;
        public readonly float PercentHPRegenMod;
        public readonly float PercentMagicDamageMod;
        public readonly float PercentMovementSpeedMod;
        public readonly float PercentPhysicalDamageMod;
        public readonly float PercentSpellBlockMod;
        public readonly float PrecentAttackSpeedMod;
        private readonly int Price;
        // RECIPE
        private readonly Item[] RecipeItems;
        private readonly int SellValue;

        public Item(int ItemId,
            string ItemName,
            int MaxStacks,
            int Price,
            bool IsRecipe,
            ItemClass ItemClass,
            ItemCategory ItemCategory,
            float FlatCritDamageMod,
            float FlatPhysicalDamageMod,
            float FlatMovementSpeedMod,
            float FlatCritChanceMod,
            float FlatMagicDamageMod,
            float FlatHPRegenMod,
            float FlatHPPoolMod,
            float FlatSpellBlockMod,
            float FlatArmorMod,
            float PrecentAttackSpeedMod,
            float PercentSpellBlockMod,
            float PercentHPPoolMod,
            float PercentCritDamageMod,
            float PercentArmorMod,
            float PercentEXPBonus,
            float PercentHPRegenMod,
            float PercentMagicDamageMod,
            float PercentMovementSpeedMod,
            float PercentPhysicalDamageMod,
            Item[] RecipeItems = null)
        {
            // BASE STATS
            this.ItemId = ItemId;
            this.ItemName = ItemName;
            this.MaxStacks = MaxStacks;
            this.Price = Price;
            SellValue = IsReducedSellItem() ? ((Price * 30) / 100) : ((Price * 70) / 100);
            this.IsRecipe = IsRecipe;
            this.ItemClass = ItemClass;

            // RECIPE
            this.RecipeItems = RecipeItems;

            // CATEGORY
            this.ItemCategory = ItemCategory;

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
            return ItemId;
        }

        public string GetName()
        {
            return ItemName;
        }

        public int GetMaxStacks()
        {
            return MaxStacks;
        }

        public int GetPrice()
        {
            return Price;
        }

        public int GetSellPrice()
        {
            return SellValue;
        }

        public bool IsRecipeComponent()
        {
            return IsRecipe;
        }

        public ItemClass GetTier()
        {
            return ItemClass;
        }

        public Item[] GetComponents()
        {
            return RecipeItems;
        }

        public int GetFullPriceValue()
        {
            var components = GetComponentsList();

            if (components == null || components.Count == 0)
            {
                return GetPrice();
            }
            return components.Sum(component => component.GetFullPriceValue());
        }

        public bool Buy()
        {
            return ObjectManager.Player.BuyItem(GetItemId());
        }

        public bool Sell()
        {
            var slot = -1;
            foreach (var item in ObjectManager.Player.InventoryItems.Where(item => item.Id == GetItemId()))
            {
                slot = item.Slot;
                break;
            }
            return ObjectManager.Player.SellItem(slot);
        }

        public ItemId GetItemId()
        {
            return (ItemId) ItemId;
        }

        public List<Item> GetComponentsList()
        {
            return GetComponents().Select(item => Items.GetItem(item.ItemId)).ToList();
        }

        private bool IsReducedSellItem()
        {
            switch (ItemId)
            {
                case 3069:
                case 3092:
                case 1055:
                case 1054:
                case 1039:
                case 1062:
                case 1063:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum ItemClass
    {
        // Tier.
        Basic,
        Advanced,
        Legendary,
        Mythical,
        Enchantment,
        Consumable,
        RengarsTrinket,
        BasicTrinket,
        AdvancedTrinket,
        None
    }

    [Flags]
    public enum ItemCategory
    {
        None = 0,
        CriticalStrike = 1 << 0,
        HealthRegen = 1 << 1,
        Consumable = 1 << 2,
        Health = 1 << 3,
        Damage = 1 << 4,
        ManaRegen = 1 << 5,
        SpellBlock = 1 << 6,
        AttackSpeed = 1 << 7,
        LifeSteal = 1 << 8,
        SpellDamage = 1 << 9,
        Mana = 1 << 10,
        Armor = 1 << 11
    }
}