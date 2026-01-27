namespace CardGame
{

    public readonly struct RewardProfile
    {
        public double Common { get; }
        public double Uncommon { get; }
        public double Rare { get; }

        public RewardProfile(double common, double uncommon, double rare)
        {
            Common = common;
            Uncommon = uncommon;
            Rare = rare;
        }
    }

    public static class RewardProfiles
    {
        public static readonly RewardProfile NormalCard = new RewardProfile(0.70, 0.25, 0.05);
        public static readonly RewardProfile EliteCard = new RewardProfile(0.55, 0.30, 0.15);
        public static readonly RewardProfile BossCard = new RewardProfile(0.40, 0.35, 0.25);

        public static readonly RewardProfile NormalRelic = new RewardProfile(0.50, 0.33, 0.17);
        public static readonly RewardProfile EliteRelic = new RewardProfile(0.60, 0.30, 0.10);
        public static readonly RewardProfile BossRelic = new RewardProfile(0.45, 0.35, 0.20);

        public static CardRarity RollCard(RewardProfile rewardProfile, Random seed)
        {
            var r = seed.NextDouble();
            if (r < rewardProfile.Common)
                return CardRarity.Common;
            if (r < rewardProfile.Common + rewardProfile.Uncommon)
                return CardRarity.Uncommon;
            else
                return CardRarity.Rare;
        }

        public static RelicRarity RollRelic(RewardProfile rewardProfile, Random seed)
        {
            var r = seed.NextDouble();
            if (r < rewardProfile.Common)
                return RelicRarity.Common;
            if (r < rewardProfile.Common + rewardProfile.Uncommon)
                return RelicRarity.Uncommon;
            else
                return RelicRarity.Rare;
        }
    }

}