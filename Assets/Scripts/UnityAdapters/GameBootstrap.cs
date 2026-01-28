using System.IO;
using UnityEngine;
using CardGame;
public class GameBootstrap : MonoBehaviour
{
    public RunController RunController { get; private set; }
    public CardLibrary CardLibrary { get; private set; }
    public RelicLibrary RelicLibrary { get; private set; }
    public Player Player { get; private set; }

    private System.Random rng;

    void Awake()
    {
        var cardsFolder = Path.Combine(Application.streamingAssetsPath, "Cards");
        var relicsFolder = Path.Combine(Application.streamingAssetsPath, "Relics");

        CardLibrary = new CardLibrary(GameDataLoader.LoadCards(cardsFolder));
        RelicLibrary = new RelicLibrary(GameDataLoader.LoadRelics(relicsFolder));

        rng = new System.Random();
        Player = new Player(30);
        for (int i = 0; i < 3; i++)
        {
            Player.Deck.Add(CardLibrary.Create("strike"));
            Player.Deck.Add(CardLibrary.Create("block"));
        }
        Player.ShuffleDeck();
        RunController = new RunController(Player, CardLibrary, RelicLibrary, rng, maxFloors: 10);

        Debug.Log("IT WORKS");
    }
}
