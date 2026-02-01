#nullable disable
using System;
using System.Collections.Generic;
namespace CardGame
{
    public readonly struct UpgradedStats
    {
        public readonly bool IsUpgraded;        
        public readonly int UpgradedDamage;
        public readonly int UpgradedCost;
        public readonly string UpgradedDescription;
        public UpgradedStats(bool isUpgraded, int upgradedDamage, int upgradedCost, string upgradedDescription)
        {
            IsUpgraded = isUpgraded;
            UpgradedDamage = upgradedDamage;
            UpgradedCost = upgradedCost;
            UpgradedDescription = upgradedDescription;
        }
    }
    [Serializable]
    public sealed class CardDef
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Description { get; set; }
        public TargetType TargetType { get; set; }
        public CardRarity Rarity { get; set; }
        public CardType Type { get; set; }
        public List<EffectOpDef> Ops { get; set; } = new();
        public CardDef? Upgraded { get; set; }
    }
}