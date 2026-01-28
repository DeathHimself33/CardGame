using System.Collections.Generic;

namespace CardGame
{
    public sealed class EnemyView
    {
        public int Index { get; }
        public string Name { get; }
        public int HP { get; }
        public int Block { get; }
        public string Intent { get; }

        public EnemyView(int index, string name, int hp, int block, string intent)
        {
            Index = index;
            Name = name;
            HP = hp;
            Block = block;
            Intent = intent;
        }
    }

    public sealed class CardView
    {
        public int Index { get; }
        public string Name { get; }
        public int Cost { get; }
        public TargetType TargetType { get; }

        public CardView(int index, string name, int cost, TargetType targetType)
        {
            Index = index;
            Name = name;
            Cost = cost;
            TargetType = targetType;
        }
    }

    public sealed class CombatSnapshot
    {
        public int Floor { get; }
        public int MaxFloors { get; }
        public int PlayerHP { get; }
        public int PlayerMaxHP { get; }
        public int PlayerBlock { get; }
        public int PlayerEnergy { get; }
        public int PlayerGold { get; }
        public List<EnemyView> Enemies { get; }
        public List<CardView> Hand { get; }

        public CombatSnapshot(
            int floor,
            int maxFloors,
            int playerHP,
            int playerMaxHP,
            int playerBlock,
            int playerEnergy,
            int playerGold,
            List<EnemyView> enemies,
            List<CardView> hand)
        {
            Floor = floor;
            MaxFloors = maxFloors;
            PlayerHP = playerHP;
            PlayerMaxHP = playerMaxHP;
            PlayerBlock = playerBlock;
            PlayerEnergy = playerEnergy;
            PlayerGold = playerGold;
            Enemies = enemies;
            Hand = hand;
        }
    }

    public sealed class ShopSnapshot
    {
        public int PlayerGold { get; }
        public List<CardView> CardOffers { get; }
        public List<string> RelicOffers { get; }

        public ShopSnapshot(
            int playerGold,
            List<CardView> cardOffers,
            List<string> relicOffers)
        {
            PlayerGold = playerGold;
            CardOffers = cardOffers;
            RelicOffers = relicOffers;
        }
    }

    public sealed class RestSnapshot
    {
        public int HealAmount { get; }
        public List<CardView> Deck { get; }

        public RestSnapshot(int healAmount, List<CardView> deck)
        {
            HealAmount = healAmount;
            Deck = deck;
        }
    }
}
