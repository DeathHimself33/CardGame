using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        public string ID;
        public string Name;
        public int Cost;
        public string Description;

        public string TargetTypeRaw;
        public string RarityRaw;
        public string TypeRaw;

        [NonSerialized] public TargetType TargetType;
        [NonSerialized] public CardRarity Rarity;
        [NonSerialized] public CardType Type;

        public List<EffectOpDef> Ops;
        public CardDef Upgraded;

        public void PostProcess()
        {
            if (!Enum.TryParse(TargetTypeRaw, true, out TargetType))
                TargetType = TargetType.None;

            if (!Enum.TryParse(RarityRaw, true, out Rarity))
                Rarity = CardRarity.Common;

            if (!Enum.TryParse(TypeRaw, true, out Type))
                Type = CardType.Attack;
        }
    }
}