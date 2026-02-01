using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardGame;
public class CombatView : MonoBehaviour
{
    public RunView runView;
    public CombatController Combat => combat;

    public Transform handContainer;
    public GameObject cardButtonPrefab;
    private Card? pendingCard;

    public Transform enemiesContainer;
    public GameObject enemyPanelPrefab;

    public TMP_Text playerStatsText;
    private CombatController combat;
    private Player player;

    public void BeginCombat(Player player, List<Enemy> enemies, CardLibrary cardLibrary)
    {
        this.player = player;
        Debug.Log($"BeginCombat: deck={player.Deck.Count}, hand={player.Hand.Count}");
        combat = new CombatController(player, enemies, cardLibrary);

        AdvanceTillPlayerTurn();
        Debug.Log($"After AdvanceTillPlayerTurn: hand={player.Hand.Count}");
        RefreshUI();
    }
    private void Update()
    {
        if (combat == null) return;

        if (combat.CurrentState == CombatController.State.CombatEnd)
        {
            bool playerWon = combat.PlayerWon;
            if(runView != null)
                runView.OnCombatEnded(playerWon, combat);
            else
                Debug.Log($"Combat ended")  ;          
            combat = null;
        }
    }

    private void AdvanceTillPlayerTurn()
    {
        if(combat == null)
            return;

        int safety = 0;
        while (combat.CurrentState != CombatController.State.PlayerWaitingAction
                && combat.CurrentState != CombatController.State.CombatEnd)
        {
            combat.AdvanceState();
            safety++;
            if(safety > 1000)
            {
                Debug.LogError("AdvanceTillPlayerTurn: safety break, combat state not progressing");
                break;
            }
        }
        if(combat.CurrentState == CombatController.State.CombatEnd && runView != null)
        {
            bool playerWon = combat.PlayerWon;
            runView.OnCombatEnded(playerWon, combat);
            combat = null;
        }
    }
    private void RefreshUI()
    {

        if(combat == null)
            return;
        if(combat.CurrentState == CombatController.State.CombatEnd)
            return;
        
        //Player Stats
        if (player == null)
            return;
        playerStatsText.text = $"HP: {player.HP}/{player.MaxHP} Block: {player.Block} Energy: {player.Energy} Gold: {player.Gold}";

        //Hand
        foreach (Transform child in handContainer)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < player.Hand.Count; i++)
        {
            var card = player.Hand[i];
            var go = Instantiate(cardButtonPrefab, handContainer);
            var ui = go.GetComponent<CardButton>();
            ui.Setup(i, card.Name, card.Cost, this);
        }

        //Enemies

        foreach (Transform child in enemiesContainer)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < combat.Enemies.Count; i++)
        {
            var enemy = combat.Enemies[i];
            if (enemy.HP <= 0)
                continue;

            var go = Instantiate(enemyPanelPrefab, enemiesContainer);
            var ui = go.GetComponent<EnemyPanel>();
            ui.Setup(i, enemy, this);
        }
    }
    public void OnEndTurnButton()
    {
        if (combat == null)
            return;
        if (combat.CurrentState != CombatController.State.PlayerWaitingAction)
            return;
        combat.SubmitPlayerAction(new CombatController.EndTurnAction());
        AdvanceTillPlayerTurn();
        RefreshUI();
    }

    public void OnCardClicked(int handIndex)
    {
        if (combat == null)
            return;
        if (combat.CurrentState != CombatController.State.PlayerWaitingAction)
            return;
        var card = player.Hand[handIndex];
        Debug.Log($"OnCardClicked: {handIndex},card = {card.Name} TargetType={card.TargetType}");

        if (card.TargetType == TargetType.SingleEnemy)
        {
            pendingCard = card;
            Debug.Log($"Selected card {card.Name}, now click an enemy.");
            return;
        }
        PlayCard(card, null);
    }

    public void OnEnemyClicked(int enemyIndex)
    {
        Debug.Log($"OnEnemyClicked: enemyIndex = {enemyIndex}, pendingCard = {pendingCard}");
        if (pendingCard == null)
        {
            Debug.Log("Clicked enemy but no card is pending");
            return;
        }

        var target = combat.Enemies[enemyIndex];
        PlayCard(pendingCard, target);
        pendingCard = null;
    }

    private void PlayCard(Card card, Character target)
    {
        int cardIndex = player.Hand.IndexOf(card);
        if(cardIndex < 0)
        {
            Debug.LogWarning($"PlayCard: card {card.Name} not found in hand.");
            return;
        }
        Debug.Log($"PlayCardIndex = {cardIndex},card = {card.Name}, target = {target}");
        combat.SubmitPlayerAction(new CombatController.PlayCardAction(cardIndex, target));
        AdvanceTillPlayerTurn();
        RefreshUI();
    }
}
