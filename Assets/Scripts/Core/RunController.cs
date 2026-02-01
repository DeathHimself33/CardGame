#nullable enable
using System;
using System.Collections.Generic;

namespace CardGame
{
    public enum ShopChoiceKind { BuyCard, BuyRelic, RemoveCard, Leave }
    public sealed record ShopChoice(ShopChoiceKind Kind, int Index);

    public enum RestChoiceKind { Heal, UpgradeCard, Leave }
    public sealed record RestChoice(RestChoiceKind Kind, int CardIndex);

    public sealed class RunController
    {
        private readonly CardLibrary _cardLibrary;
        private readonly RelicLibrary _relicLibrary;
        private readonly Random _seed;

        public Player Player { get; }

        public enum RoomType { Combat, Elite, Rest, Shop, Boss }

        public int Floor { get; private set; } = 1;
        public int MaxFloors { get; }
        private RoomType? _currentRoom;
        private bool _runFinished;

        public List<Card> CurrentShopCardOffers { get; private set; } = new();
        public List<Relic> CurrentShopRelicOffers { get; private set; } = new();

        private PendingRewards? _pendingRewards;

        private sealed class PendingRewards
        {
            public RoomType RoomType { get; }
            public List<Card> CardOffers { get; }
            public List<Relic> RelicOffers { get; }
            public int GoldGained { get; }

            public PendingRewards(RoomType roomType, List<Card> cardOffers, List<Relic> relicOffers, int goldGained)
            {
                RoomType = roomType;
                CardOffers = cardOffers;
                RelicOffers = relicOffers;
                GoldGained = goldGained;
            }
        }

        public RunController(Player player, CardLibrary cardLibrary, RelicLibrary relicLibrary, Random seed, int maxFloors = 10)
        {
            Player = player;
            _cardLibrary = cardLibrary;
            _relicLibrary = relicLibrary;
            _seed = seed;
            MaxFloors = maxFloors;
        }

        public RunStepResult AdvanceRun()
        {
            if (_runFinished)
                return new RunEndedResult(Player.HP > 0);

            if (_pendingRewards != null)
                return new RewardsNeededResult(_pendingRewards.CardOffers, _pendingRewards.RelicOffers, _pendingRewards.GoldGained);

            if (_currentRoom == null)
            {
                if (Floor > MaxFloors)
                {
                    _runFinished = true;
                    return new RunEndedResult(Player.HP > 0);
                }

                _currentRoom = CreateRoom(Floor);
                return new EnterRoomResult(_currentRoom.Value, Floor);
            }

            switch (_currentRoom.Value)
            {
                case RoomType.Combat:
                {
                    var enemies = CreateCombat(Floor);
                    _currentRoom = null;
                    return new CombatNeededResult(enemies, RoomType.Combat);
                }

                case RoomType.Elite:
                {
                    var enemies = CreateElite(Floor);
                    _currentRoom = null;
                    return new CombatNeededResult(enemies, RoomType.Elite);
                }

                case RoomType.Boss:
                {
                    var enemies = CreateBoss(Floor);
                    _currentRoom = null;
                    return new CombatNeededResult(enemies, RoomType.Boss);
                }

                case RoomType.Shop:
                {
                    var shop = CreateShopSnapshot();
                    return new ShopNeededResult(shop.CardOffers, shop.RelicOffers);
                }

                case RoomType.Rest:
                {
                    var rest = CreateRestSnapshot();
                    return new RestNeededResult(rest.HealAmount);
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ApplyRewardsChoice(int cardIndex, int relicIndex)
        {
            if (_pendingRewards == null)
                return;

            if (cardIndex >= 0 && cardIndex < _pendingRewards.CardOffers.Count)
                Player.Deck.Add(_pendingRewards.CardOffers[cardIndex]);

            if (relicIndex >= 0 && relicIndex < _pendingRewards.RelicOffers.Count)
                _pendingRewards.RelicOffers[relicIndex].PickupOutOfCombat(Player);

            _pendingRewards = null;
        }

        private ShopSnapshot CreateShopSnapshot()
        {
            CurrentShopCardOffers = _cardLibrary.CreateRewardOptions(3, _seed, RewardProfiles.NormalCard);
            CurrentShopRelicOffers = _relicLibrary.CreateRewardOptions(3, _seed, Player, RewardProfiles.NormalRelic);
            return new ShopSnapshot(CurrentShopCardOffers, CurrentShopRelicOffers);
        }

        public void ApplyShopChoice(ShopChoice choice)
        {
            switch (choice.Kind)
            {
                case ShopChoiceKind.BuyCard:
                {
                    if (choice.Index < 0 || choice.Index >= CurrentShopCardOffers.Count) break;
                    if (Player.Gold < 30) break;

                    var cardToBuy = CurrentShopCardOffers[choice.Index];
                    Player.Gold -= 30;
                    Player.Deck.Add(cardToBuy);
                    CurrentShopCardOffers.RemoveAt(choice.Index);
                    break;
                }

                case ShopChoiceKind.BuyRelic:
                {
                    if (choice.Index < 0 || choice.Index >= CurrentShopRelicOffers.Count) break;
                    if (Player.Gold < 120) break;

                    var relicToBuy = CurrentShopRelicOffers[choice.Index];
                    Player.Gold -= 120;
                    relicToBuy.PickupOutOfCombat(Player);
                    CurrentShopRelicOffers.RemoveAt(choice.Index);
                    break;
                }

                case ShopChoiceKind.RemoveCard:
                {
                    if (choice.Index < 0 || choice.Index >= Player.Deck.Count) break;
                    if (Player.Gold < 75) break;

                    Player.Gold -= 75;
                    Player.Deck.RemoveAt(choice.Index);
                    break;
                }

                case ShopChoiceKind.Leave:
                {
                    _currentRoom = null;
                    Floor++;
                    Player.BetweenFloorsReset();
                    break;
                }
            }
        }

        private RestSnapshot CreateRestSnapshot()
        {
            int healAmount = (int)Math.Ceiling(Player.MaxHP * 0.3);
            return new RestSnapshot(healAmount);
        }

        public void ApplyRestChoice(RestChoice choice)
        {
            switch (choice.Kind)
            {
                case RestChoiceKind.Heal:
                    Player.HealRaw(CreateRestSnapshot().HealAmount);
                    _currentRoom = null;
                    Floor++;
                    Player.BetweenFloorsReset();
                    break;

                case RestChoiceKind.UpgradeCard:
                {
                    if (choice.CardIndex < 0 || choice.CardIndex >= Player.Deck.Count) break;
                    if (Player.Deck[choice.CardIndex] is DataCard dataCard) dataCard.Upgrade();
                    _currentRoom = null;
                    Floor++;
                    Player.BetweenFloorsReset();
                    break;
                }

                case RestChoiceKind.Leave:
                    _currentRoom = null;
                    Floor++;
                    Player.BetweenFloorsReset();
                    break;
            }
        }

        public void OnCombatFinished(bool playerWon, RoomType roomType, CombatController combat)
        {
            if (!playerWon)
            {
                _runFinished = true;
                return;
            }

            int gold = RollGold(roomType);
            Player.Gold += gold;

            var cardProfile = roomType switch
            {
                RoomType.Combat => RewardProfiles.NormalCard,
                RoomType.Elite => RewardProfiles.EliteCard,
                RoomType.Boss => RewardProfiles.BossCard,
                _ => RewardProfiles.NormalCard
            };

            var relicProfile = roomType switch
            {
                RoomType.Elite => RewardProfiles.EliteRelic,
                RoomType.Boss => RewardProfiles.BossRelic,
                _ => default
            };

            var cardOffers = _cardLibrary.CreateRewardOptions(3, _seed, cardProfile);
            var relicOffers = (roomType == RoomType.Elite || roomType == RoomType.Boss)
                ? _relicLibrary.CreateRewardOptions(3, _seed, Player, relicProfile)
                : new List<Relic>();

            _pendingRewards = new PendingRewards(roomType, cardOffers, relicOffers, gold);

            Player.BetweenFloorsReset();
            Floor++;
            _currentRoom = null;
        }

        private int RollGold(RoomType roomType)
        {
            double mult = roomType switch
            {
                RoomType.Combat => 1.0,
                RoomType.Elite => 1.3,
                RoomType.Boss => 1.5,
                _ => 1.0
            };

            return (int)Math.Ceiling(_seed.Next(10, 21) * mult);
        }

        private RoomType CreateRoom(int floor)
        {
            if (floor == MaxFloors) return RoomType.Boss;

            var r = _seed.NextDouble();
            if (r < 0.65) return RoomType.Combat;
            if (r < 0.80) return RoomType.Rest;
            if (r < 0.93) return RoomType.Shop;
            return RoomType.Elite;
        }

        private List<Enemy> CreateCombat(int floor)
        {
            var enemies = new List<Enemy>();
            for (int i = 0; i < 2; i++) enemies.Add(new Enemy(10 + (floor - 1) * 5));
            return enemies;
        }

        private List<Enemy> CreateElite(int floor)
        {
            var enemies = new List<Enemy>();
            for (int i = 0; i < 2; i++) enemies.Add(new Enemy(30 + (floor - 1) * 4));
            return enemies;
        }

        private List<Enemy> CreateBoss(int floor)
        {
            var enemies = new List<Enemy>();
            enemies.Add(new Enemy(50 + (floor - 1) * 5));
            return enemies;
        }
    }
}
