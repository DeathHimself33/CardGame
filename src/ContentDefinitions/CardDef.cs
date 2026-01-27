namespace CardGame
{


    public readonly record struct UpgradedStats(
            bool isUpgraded,
            int upgradedDamage,
            int upgradedCost,
            string upgradedDescription
        );
    public sealed class CardDef
    {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public int Cost { get; set; }
        public string Description { get; set; } = "";
        public TargetType TargetType { get; set; }
        public CardRarity Rarity { get; set; }
        public CardType Type { get; set; }
        public List<EffectOpDef> Ops { get; set; } = new();

        public CardDef? Upgraded { get; set; }
    }
}