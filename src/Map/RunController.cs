namespace CardGame;
public sealed class RunController
{
    private readonly CardLibrary _cardLibrary;
    private readonly RelicLibrary _relicLibrary;
    private readonly Random _seed;

    public Player Player {get;}

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
            var enemies = CreateEncounter(Floor);
            var combat = new CombatController(Player, enemies, _cardLibrary);
            combat.TransitionToState(CombatController.State.CombatStart);

            RunCombatLoop(combat,enemies);

            if(!combat.PlayerWon || Player.HP <= 0)
            {
                Console.WriteLine("You Lost!");
                return;
            }

            DoRelicReward(combat);
            DoCardReward();

            BetweenFloors(combat);
        }

        Console.WriteLine("Run Complete!");
    }

    private List<Enemy> CreateEncounter(int floor)
    {
        var enemies = new List<Enemy>();

        for(int i = 0;i < 2; i++)
        {
            enemies.Add(new Enemy(10 + (floor - 1) * 5));
        }

        return enemies;
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

    private void DoRelicReward(CombatController combat)
    {
        List<Relic> rewards = _relicLibrary.CreateRewardOptions(3,_seed,Player);
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
    private void DoCardReward()
    {
        List<Card> rewards = _cardLibrary.CreateRewardOptions(3,_seed);

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

    private void BetweenFloors(CombatController combat)
    {
        //Later make this campfire/shop/encounter or whatever
        Player.EndTurn(combat.Context);
        Player.ShuffleDeck();
        Player.Heal(combat.Context, Player.MaxHP);
    }
}