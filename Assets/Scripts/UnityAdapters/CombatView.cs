using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardGame;
using System.Runtime.CompilerServices;
using UnityEditor;

public class CombatView : MonoBehaviour
{
    public Transform handContainer;
    public GameObject cardButtonPrefab;
    private int? pendingCardIndex = null;

    public Transform enemiesContainer;
    public GameObject enemyPanelPrefab;

    public TMP_Text playerStatsText;
    private CombatController combat;
    private Player player;

    public void BeginCombat(Player player, List<Enemy> enemies, CardLibrary cardLibrary)
    {
        this.player = player;
        combat = new CombatController(player,enemies, cardLibrary);

        AdvanceTillPlayerTurn();
        RefreshUI();
    }
    private void AdvanceTillPlayerTurn()
    {
        while(combat.CurrentState != CombatController.State.PlayerWaitingAction
                && combat.CurrentState != CombatController.State.CombatEnd)
            {
                combat.AdvanceState();
            }
    }
    private void RefreshUI()
    {
        //Player Stats
        if(player == null)
            return;
        playerStatsText.text = $"HP: {player.HP}/{player.MaxHP} Block: {player.Block} Energy: {player.Energy} Gold: {player.Gold}";
        
        //Hand
        foreach(Transform child in handContainer)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0;i < player.Hand.Count; i++)
        {
            var card = player.Hand[i];
            var go = Instantiate(cardButtonPrefab, handContainer);
            var ui = go.GetComponent<CardButton>();
            ui.Setup(i, card.Name, card.Cost, this); 
        }

        //Enemies

        foreach(Transform child in enemiesContainer)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0;i < combat.Enemies.Count; i++)
        {
            var enemy = combat.Enemies[i];
            if(enemy.HP <= 0)
                return;

            var go  = Instantiate(enemyPanelPrefab, enemiesContainer);
            var ui = go.GetComponent<EnemyPanel>();
            ui.Setup(i,enemy,this);
        }
    }
    public void OnEndTurnButton()
    {
        if(combat == null)
            return;
        if(combat.CurrentState != CombatController.State.PlayerWaitingAction)
            return;
        combat.SubmitPlayerAction(new CombatController.EndTurnAction());
        AdvanceTillPlayerTurn();
        RefreshUI();
    }

    public void OnCardClicked(int handIndex)
    {
        if(combat == null)
            return;
        if(combat.CurrentState != CombatController.State.PlayerWaitingAction)
            return;
        var card = player.Hand[handIndex];
        Debug.Log($"OnCardCliked: {card.Name}, TargetType={card.TargetType}");

        if(card.TargetType == TargetType.SingleEnemy)
        {
            pendingCardIndex = handIndex;
            Debug.Log($"Selected card {card.Name}, now click an enemy.");
            return;
        }
        PlayCard(handIndex, null);
    }

    public void OnEnemyClicked(int enemyIndex)
    {
        Debug.Log($"OnEnemyClicked: enemyIndex = {enemyIndex}, pendingCardIndex = {pendingCardIndex}");
        if (!pendingCardIndex.HasValue)
        {
            Debug.Log("Clicked enemy but no card is pending");
            return;
        }
        
        var target = combat.Enemies[enemyIndex];
        PlayCard(pendingCardIndex.Value, target);
        pendingCardIndex = null;
    }

    private void PlayCard(int cardIndex, Character target)
    {
        Debug.Log($"PlayCard index = {cardIndex}, target = {target}");
        combat.SubmitPlayerAction(new CombatController.PlayCardAction(cardIndex, target));
        AdvanceTillPlayerTurn();
        RefreshUI();
    }
}
