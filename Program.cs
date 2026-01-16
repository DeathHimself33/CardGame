#nullable enable
namespace CardGame
{
    class Program
    {
        public static void Main(string[] args)
        {
            SimpleCombatTest();
        }
        public static void SimpleCombatTest()
        {
            List<Card> PlayerDeck = new List<Card>();
            for(int i = 0;i < 3; i++)
            {
                PlayerDeck.Add(new Strike{});
                PlayerDeck.Add(new Block{});
                PlayerDeck.Add(new Heal{});
                PlayerDeck.Add(new Cleave{});
            }
            Player Player = new Player(30);
            Player.Deck.AddRange(PlayerDeck);
            Player.ShuffleDeck();

            Enemy Enemy = new Enemy(10);
            Enemy.Deck.AddRange(PlayerDeck);
            Enemy.ShuffleDeck();

            CombatController combatController = new CombatController(Player, Enemy);
            for(int y = 0;y < 3; y++)
            {
                combatController.TransitionToState(CombatController.State.CombatStart);
                while(combatController.CurrentState != CombatController.State.CombatEnd)
                {
                    combatController.AdvanceState();
                    if(combatController.CurrentState == CombatController.State.PlayerWaitingAction)
                    {
                        for (int i = 0; i < Player.Hand.Count; i++)
                        {
                            Console.WriteLine($"{i}: {Player.Hand[i].Name} (Cost: {Player.Hand[i].Cost})");
                        }
                        
                        Console.WriteLine("Enter the index of the card to play or 'end' to end your turn:");
                        string? input = Console.ReadLine();
                        
                        if (input?.ToLower() == "end")
                        {
                            Player.EndTurn(combatController.Context);
                            break;
                        }

                        if (int.TryParse(input, out int cardIndex) && cardIndex >= 0 && cardIndex < Player.Hand.Count)
                        {
                            Card selectedCard = Player.Hand[cardIndex];
                            Character? target;
                            if(selectedCard.TargetType == TargetType.SingleEnemy)
                            {
                                target = Enemy;
                            }
                            else
                            {
                                target = null;
                            }
                            bool success = Player.TryPlayCard(combatController.Context, selectedCard, target);
                            if (!success)
                            {
                                Console.WriteLine("Cannot play that card.");
                            }
                            else
                            {
                                Console.WriteLine($"Played {selectedCard.Name}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input.");
                        }
                    }
                }
                Player.EndTurn(combatController.Context);
                if (combatController.PlayerWon)
                {
                    Random rng = new Random();
                    CardLibrary library = new CardLibrary();

                    List<Card> rewards = library.CreateRewardOptions(3, rng);

                    Card reward = rewards[0];

                    Player.Deck.Add(reward);
                    Console.WriteLine($"You won! You received a {reward.Name} card as a reward.");
                    Console.WriteLine("Deck size is now: " + Player.Deck.Count);
                }
            }        
        }
    }
}