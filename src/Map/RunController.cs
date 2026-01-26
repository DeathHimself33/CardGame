
namespace CardGame;
public sealed class RunController
{
    private readonly CardLibrary _cardLibrary;
    private readonly RelicLibrary _relicLibrary;
    private readonly Random _seed;

    public Player Player {get;}
    enum RoomType
    {
        Combat,
        Elite,
        Rest,
        Shop,
        Boss
    }
    public int Floor {get; private set;} = 1;
    public int MaxFloors {get;}

    public RunController(Player player, CardLibrary cardLibrary, RelicLibrary relicLibrary, Random seed, int maxFloors = 10)
    {
        Player = player;
        _cardLibrary = cardLibrary;
        _relicLibrary = relicLibrary;
        _seed = seed;
        MaxFloors = maxFloors;
    }

    public void Run()
    {
        for(Floor = 1; Floor <= MaxFloors; Floor++)
        {
            var room = CreateRoom(Floor);
            switch (room)
            {
                case RoomType.Boss:
                    var boss = CreateBoss(Floor);
                    CombatController combat = new CombatController(Player, boss, _cardLibrary);
                    Console.WriteLine("Starting boss!");
                    RunCombatLoop(combat, boss);
                    if (combat.PlayerWon)
                    {
                        DoCardReward(RewardProfiles.BossCard);
                        DoGoldReward(1.5);
                        DoRelicReward(combat, RewardProfiles.BossRelic);
                        Player.BetweenFloorsReset();
                    }
                    else  
                    {
                        Console.WriteLine("You Lost!");
                        return;
                    }
                    break;
                case RoomType.Combat:
                    var enemies = CreateCombat(Floor);
                    combat = new CombatController(Player, enemies, _cardLibrary);
                    Console.WriteLine("Starting normal combat!");
                    RunCombatLoop(combat,enemies);
                    if (combat.PlayerWon)
                    {
                        DoCardReward(RewardProfiles.NormalCard);
                        DoGoldReward(1);
                        Player.BetweenFloorsReset();
                    }
                    else
                    {
                        Console.WriteLine("You Lost!");
                        return;
                    }
                    break;
                case RoomType.Rest:
                    CreateRest();
                    break;
                case RoomType.Shop:
                    CreateShop();
                    break;
                case RoomType.Elite:
                    enemies = CreateElite(Floor);
                    combat = new CombatController(Player, enemies, _cardLibrary);
                    Console.WriteLine("Starting elite combat!");
                    RunCombatLoop(combat,enemies);
                    if (combat.PlayerWon)
                    {
                        DoCardReward(RewardProfiles.EliteCard);
                        DoRelicReward(combat, RewardProfiles.EliteRelic);
                        DoGoldReward(1.3);
                        Player.BetweenFloorsReset();
                    }
                    else
                    {
                        Console.WriteLine("You Lost!");
                        return;
                    }
                    break;
            }
        }

        Console.WriteLine("Run Complete!");
    }

    private RoomType CreateRoom(int floor)
    {
        if(floor == MaxFloors)
        {
            return RoomType.Boss;
        }
        var r = _seed.NextDouble();
        if(r < 0.65)
            return RoomType.Combat;
        else if(r < 0.80)
            return RoomType.Rest;
        else if(r < 0.93)
            return RoomType.Shop;
        else
            return RoomType.Elite;
    }

    private void CreateShop()
    {
        var cardOffers = _cardLibrary.CreateRewardOptions(3, _seed, RewardProfiles.NormalCard);
        var relicOffers = _relicLibrary.CreateRewardOptions(3, _seed, Player, RewardProfiles.NormalRelic);
        while (true)
        {
            Console.WriteLine("Welcome to the shop! ");
            Console.WriteLine($"Player gold: {Player.Gold}");
            Console.WriteLine("Would you like to buy a card (30 gold, 'card'), buy a relic (120 gold, 'relic') or remove a card (75 gold, 'remove') or leave ('leave')");
            string? choice = Console.ReadLine();
            if(choice?.ToLower() == "card")
            {
                while (true)
                {
                    Console.WriteLine("Choose a card to buy (index) or type 'back' to go back:");
                    for(int i = 0;i < cardOffers.Count;i++)
                    {
                        Console.WriteLine($"{i}: {cardOffers[i].Name}");
                    }
                    string? input = Console.ReadLine().ToLower();
                    if(input == "back")
                    {
                        break;
                    }
                    if(int.TryParse(input, out int cardChoice) && cardChoice >= 0 && cardChoice < cardOffers.Count && Player.Gold >= 30)
                    {
                        var bought = cardOffers[cardChoice];
                        Player.Deck.Add(bought);
                        Player.Gold -= 30;
                        cardOffers.Remove(bought);
                        Console.WriteLine($"Bought: {bought}");
                    }
                    else if (Player.Gold < 30)
                    {
                        Console.WriteLine("Not Enough gold!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                }
            }
            else if(choice?.ToLower() == "relic")
            {
                while (true)
                {
                    Console.WriteLine("Choose a relic to buy (index) or type 'back' to go back:");
                    for(int i = 0;i < relicOffers.Count;i++)
                    {
                        Console.WriteLine($"{i}: {relicOffers[i].Name}");
                    }
                    string? input = Console.ReadLine().ToLower();
                    if(input == "back")
                    {
                        break;
                    }
                    if(int.TryParse(input, out int relicChoice) && relicChoice >= 0 && relicChoice < relicOffers.Count && Player.Gold >= 120)
                    {
                        var bought = relicOffers[relicChoice];
                        bought.PickupOutOfCombat(Player);
                        Player.Gold -= 120;
                        relicOffers.Remove(bought);
                        Console.WriteLine($"Bought: {bought}");
                    }
                    else if (Player.Gold < 120)
                    {
                        Console.WriteLine("Not Enough gold!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                }
            }
            else if(choice?.ToLower() == "remove")
            {
                while (true)
                {
                    Console.WriteLine("Choose a card to remove (index) or type 'back' to go back:");
                    for(int i = 0;i < Player.Deck.Count;i++)
                    {
                        Console.WriteLine($"{i}: {Player.Deck[i].Name}");
                    }
                    string? input = Console.ReadLine().ToLower();
                    if(input == "back")
                    {
                        break;
                    }
                    if(int.TryParse(input, out int cardChoice) && cardChoice >= 0 && cardChoice < Player.Deck.Count && Player.Gold >= 75)
                    {
                        Player.Deck.RemoveAt(cardChoice);
                        Player.Gold -= 75;
                        Console.WriteLine($"Removed: {Player.Deck[cardChoice]}");
                    }
                    else if(Player.Gold < 75)
                    {
                        Console.WriteLine("Not enough gold!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                }
            }
            else if(choice?.ToLower() == "leave")
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }
    private void CreateRest()
    {

        int healAmount = (int)Math.Ceiling(Player.MaxHP * 0.3);
        Console.Clear();

        while (true)
        {
            Console.WriteLine("Welcome to a rest location!");
            Console.WriteLine($"Would you like to heal 30% ({healAmount}) of your hp ({Player.HP}/{Player.MaxHP}) or remove a card");
            Console.WriteLine("Write 'heal' for healing or 'upgrade' for upgrading a card or write 'leave' to leave");
            string? choice = Console.ReadLine();
            if(choice?.ToLower() == "heal")
            {
                Player.HealRaw(healAmount);
                return;
            }
            else if(choice?.ToLower() == "upgrade")
            {
                while (true)
                {
                    Console.WriteLine("Choose a card to upgrade (index) or type 'back' to go back:");
                    for(int i = 0;i < Player.Deck.Count;i++)
                    {
                        Console.WriteLine($"{i}: {Player.Deck[i].Name} (Amount: {Player.Deck[i]})");
                    }
                    string? input = Console.ReadLine().ToLower();
                    if(input == "back")
                    {
                        break;
                    }
                    if(int.TryParse(input, out int cardChoice) && cardChoice >= 0 && cardChoice < Player.Deck.Count)
                    {
                        Card card = Player.Deck[cardChoice];
                        if(card is DataCard dataCard)
                        {
                            dataCard.Upgrade();
                        }
                        Console.WriteLine($"Upgraded: {card.Name}");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                }
            }
            else if(choice?.ToLower() == "leave")
            {
                return;
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }
    private List<Enemy> CreateCombat(int floor)
    {
        var enemies = new List<Enemy>();

        for(int i = 0;i < 2; i++)
        {
            enemies.Add(new Enemy(10 + (floor - 1) * 5));
        }

        return enemies;
    }
    private List<Enemy> CreateElite(int floor)
    {
        var enemies = new List<Enemy>();
        for(int i = 0;i < 2; i++)
        {
            enemies.Add(new Enemy(30 + (floor - 1) * 4));
        }
        return enemies;
    }

    private List<Enemy> CreateBoss(int floor)
    {
        //Later actually make this a boss!
        var boss = new List<Enemy>();
        boss.Add(new Enemy(50 + (floor - 1) * 5));
        return boss;
    }
    private void RunCombatLoop(CombatController combat, List<Enemy> enemies)
    {
        while(combat.CurrentState != CombatController.State.CombatEnd)
        {
            combat.AdvanceState();
            if(combat.CurrentState != CombatController.State.PlayerWaitingAction)
                continue;

            while(combat.CurrentState == CombatController.State.PlayerWaitingAction && combat.CurrentState != CombatController.State.CombatEnd)
            {
                if(Player.HP <= 0)
                {
                    combat.TransitionToState(CombatController.State.CombatEnd);
                    break;
                }

                PrintCombatUI(enemies);

                var input = Console.ReadLine();
                if(input?.ToLower() == "end")
                {
                    combat.SubmitPlayerAction(new CombatController.EndTurnAction());
                    while(combat.CurrentState != CombatController.State.PlayerWaitingAction && combat.CurrentState != CombatController.State.CombatEnd)
                    {
                        combat.AdvanceState();
                    }
                    break;
                }

                if(!int.TryParse(input,out int cardIndex) || cardIndex < 0 || cardIndex >= Player.Hand.Count)
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                var selectedCard = Player.Hand[cardIndex];
                Character? target = null;

                if(selectedCard.TargetType == TargetType.SingleEnemy)
                {
                    Console.WriteLine("Select target index:");
                    var targetInput = Console.ReadLine();

                    if(!int.TryParse(targetInput, out int targetIndex) || targetIndex < 0 || targetIndex >= enemies.Count)
                    {
                        Console.WriteLine("Invalid target index.");
                        continue;
                    }
                    if(enemies[targetIndex].HP <= 0)
                    {
                        Console.WriteLine("Target is dead.");
                        continue;
                    }

                    target = enemies[targetIndex];
                }

                combat.SubmitPlayerAction(new CombatController.PlayCardAction(cardIndex, target));

                if(combat.CurrentState == CombatController.State.CombatEnd)
                    break;
            }
        }
    }

    private void PrintCombatUI(List<Enemy> enemies)
    {
        Console.WriteLine();
        Console.WriteLine($"Floor {Floor}/{MaxFloors}");
        Console.WriteLine($"Your HP: {Player.HP} | Block: {Player.Block} | Energy: {Player.Energy}");

        Console.WriteLine("Enemies:");
        for (int i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if (e.HP <= 0) continue;

            var intentText = e.plannedAction is null ? "" : $" | Intent: {e.plannedAction.Intent} {e.plannedAction.Amount}";
            Console.WriteLine($"{i}: Enemy | HP: {e.HP} | Block: {e.Block}{intentText}");
        }

        Console.WriteLine("Hand:");
        for (int i = 0; i < Player.Hand.Count; i++)
            Console.WriteLine($"{i}: {Player.Hand[i].Name} (Cost: {Player.Hand[i].Cost})");

        Console.WriteLine("Enter card index to play or 'end':");
    }

    private void DoRelicReward(CombatController combat, RewardProfile rewardProfile)
    {
        List<Relic> rewards = _relicLibrary.CreateRewardOptions(3,_seed,Player, rewardProfile);
        Console.WriteLine("Choose a relic reward (0-2) or type 'skip': ");
        for(int i = 0;i < rewards.Count; i++)
        {
            Console.WriteLine($"{i}: {rewards[i].Name}");
        }
        while (true)
        {
            string? choice = Console.ReadLine();
            if(choice?.ToLower() == "skip")
                break;
            if(int.TryParse(choice, out int _choice) && _choice >= 0 && _choice < rewards.Count)
            {
                rewards[_choice].Pickup(Player, combat.Context);
                Console.WriteLine($"Added {rewards[_choice].Name} to your inventory.");
                break;
            }
            Console.WriteLine("Invalid choice. Enter 0-2 or 'skip'.");
        }
    }
    private void DoCardReward(RewardProfile rewardProfile)
    {
        List<Card> rewards = _cardLibrary.CreateRewardOptions(3,_seed, rewardProfile);

        Console.WriteLine("Choose a card reward (0-2) or type 'skip': ");
        for(int i = 0;i < rewards.Count; i++)
        {
            Console.WriteLine($"{i}: {rewards[i].Name} (Cost: {rewards[i].Cost})");
        }
        while (true)
        {
            string? choice = Console.ReadLine();

            if(choice?.ToLower() == "skip")
                break;
            if(int.TryParse(choice, out int _choice) && _choice >= 0 && _choice < rewards.Count)
            {
                Player.Deck.Add(rewards[_choice]);
                Console.WriteLine($"Added {rewards[_choice].Name} to your deck.");
                break;
            } 
            Console.WriteLine("Invalid choice. Enter 0-2 or 'skip'.");
        }
    }

    private void DoGoldReward(double multiplier)
    {
        int gold = (int)Math.Ceiling(_seed.Next(10,21) * multiplier);
        Player.Gold += gold;
    }
}