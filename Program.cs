#nullable enable
using System;
using System.Collections.Generic;

namespace CardGame;

class Program
{
    private static readonly CardLibrary CardLibrary = new CardLibrary();
    public static void Main(string[] args) => SimpleCombatTest();

    public static void SimpleCombatTest()
    {
        Random random = new Random();
        List<Card> playerDeck = new();
        for (int i = 0; i < 3; i++)
        {
            playerDeck.Add(new Strike());
            playerDeck.Add(new Block());
            playerDeck.Add(new Heal());
            playerDeck.Add(new Cleave());
        }

        var player = new Player(30);
        player.Deck.AddRange(playerDeck);
        player.ShuffleDeck();

        // 10 floors
        for(int y = 0; y < 10; y++)
        {
            
            List<Enemy> enemies = new();
            for (int i = 0; i < 2; i++)
            {
                var enemy = new Enemy(10 + y * 5);
                enemy.Deck.AddRange(playerDeck);
                enemy.ShuffleDeck();
                enemies.Add(enemy);
            }

            var combatController = new CombatController(player, enemies);
            combatController.TransitionToState(CombatController.State.CombatStart);

            while (combatController.CurrentState != CombatController.State.CombatEnd)
            {
                combatController.AdvanceState();

                if (combatController.CurrentState != CombatController.State.PlayerWaitingAction)
                    continue;

                // multiple actions per turn: keep prompting until "end" or CombatEnd
                while (combatController.CurrentState == CombatController.State.PlayerWaitingAction &&
                    combatController.CurrentState != CombatController.State.CombatEnd)
                {
                    if(player.HP <= 0)
                    {
                        combatController.TransitionToState(CombatController.State.CombatEnd);
                        break;
                    }
                    Console.WriteLine();
                    Console.WriteLine($"Your HP: {player.HP} | Block: {player.Block} | Energy: {player.Energy}");

                    Console.WriteLine("Enemies:");
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        var e = enemies[i];
                        if (e.HP <= 0) continue;

                        string intentText = e.plannedAction is null
                            ? ""
                            : $" | Intent: {e.plannedAction.Intent} {e.plannedAction.Amount}";

                        Console.WriteLine($"{i}: Enemy | HP: {e.HP} | Block: {e.Block}{intentText}");
                    }

                    Console.WriteLine("Hand:");
                    for (int i = 0; i < player.Hand.Count; i++)
                        Console.WriteLine($"{i}: {player.Hand[i].Name} (Cost: {player.Hand[i].Cost})");

                    Console.WriteLine("Enter card index to play or 'end':");
                    string? input = Console.ReadLine();

                    if (input?.ToLower() == "end")
                    {
                        combatController.SubmitPlayerAction(new CombatController.EndTurnAction());

                        // advance until it's player waiting again or combat ends
                        while (combatController.CurrentState != CombatController.State.PlayerWaitingAction &&
                            combatController.CurrentState != CombatController.State.CombatEnd)
                        {
                            combatController.AdvanceState();
                        }
                        break;
                    }

                    if (!int.TryParse(input, out int cardIndex) || cardIndex < 0 || cardIndex >= player.Hand.Count)
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }

                    Card selectedCard = player.Hand[cardIndex];
                    Character? target = null;

                    if (selectedCard.TargetType == TargetType.SingleEnemy)
                    {
                        Console.WriteLine("Select target enemy index:");
                        string? targetInput = Console.ReadLine();

                        if (!int.TryParse(targetInput, out int targetIndex) || targetIndex < 0 || targetIndex >= enemies.Count)
                        {
                            Console.WriteLine("Invalid target index.");
                            continue;
                        }
                        if (enemies[targetIndex].HP <= 0)
                        {
                            Console.WriteLine("Target is dead.");
                            continue;
                        }

                        target = enemies[targetIndex];
                    }

                    combatController.SubmitPlayerAction(new CombatController.PlayCardAction(cardIndex, target));

                    if (combatController.CurrentState == CombatController.State.CombatEnd)
                        break;
                }
            }

            Console.WriteLine();
            if (combatController.PlayerWon)
            {
                List<Card> rewards = CardLibrary.CreateRewardOptions(3,random);

                Console.WriteLine("Choose a reward (0-2) or type 'skip': ");
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
                        player.Deck.Add(rewards[_choice]);
                        Console.WriteLine($"Added {rewards[_choice].Name} to your deck.");
                        break;
                    } 
                    Console.WriteLine("Invalid choice. Enter 0-2 or 'skip'.");
                }
                player.EndTurn(combatController.Context);
                player.ShuffleDeck();
                player.Heal(combatController.Context,30);
            }
            else
            {
                Console.WriteLine("You lost!");
                return;
            }
        }
    }
}
