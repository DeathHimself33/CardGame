using System.Collections.Generic;
using UnityEngine;
using CardGame;

public class CombatTestStarter : MonoBehaviour
{
    public CombatView combatView;

    void Start()
    {
        Debug.Log("CombatTestStarter.Start running");
        var bootstrap = FindObjectOfType<GameBootstrap>();
        var player = bootstrap.Player;
        var cardLibrary = bootstrap.CardLibrary;

        var enemies = new List<Enemy>
        {
            new Enemy(20),
            new Enemy(20)
        };

        combatView.BeginCombat(player, enemies, cardLibrary);
    }
}
