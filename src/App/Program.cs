#nullable enable
using System;
using System.Collections.Generic;

namespace CardGame;

class Program
{
    public static void Main(string[] args) => SimpleCombatTest();

    public static void SimpleCombatTest()
    {
        var cardsFolder = Path.Combine(AppContext.BaseDirectory, "Content", "Cards");
        var relicsFolder = Path.Combine(AppContext.BaseDirectory, "Content", "Relics");

        CardLibrary cardLibrary = new CardLibrary(GameDataLoader.LoadCards(cardsFolder));
        RelicLibrary relicLibrary = new RelicLibrary(GameDataLoader.LoadRelics(relicsFolder));

        var rng = new Random();

        var player = new Player(30);
        for(int i = 0;i < 3; i++)
        {
            player.Deck.Add(cardLibrary.Create("strike"));
            player.Deck.Add(cardLibrary.Create("block"));
        }
        player.ShuffleDeck();

        var run = new RunController(player,cardLibrary,relicLibrary,rng,maxFloors: 10);
        run.Run();
    }
}
