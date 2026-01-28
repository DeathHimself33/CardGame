#nullable enable
using System.Collections.Generic;
using System;

namespace CardGame
{

    public class CombatController
    {
        private readonly CardLibrary _cardLibrary;
        public Random seed = new Random();

        public Character Player { get; set; }
        public List<Enemy> Enemies { get; set; }

        public CombatContext Context { get; private set; }

        public CombatController(Character player, List<Enemy> enemies, CardLibrary cardLibrary)
        {
            Player = player;
            Enemies = enemies;
            Context = new CombatContext(player, enemies, OnCharacterDamaged);
            _cardLibrary = cardLibrary;
        }

        public enum State
        {
            CombatStart,
            PlayerTurnStart,
            PlayerWaitingAction,
            PlayerTurnEnd,
            EnemyTurnStart,
            EnemyActing,
            EnemyTurnEnd,
            CombatEnd
        }

        private enum CombatResult { None, Victory, Defeat }

        public abstract record PlayerAction;
        public sealed record EndTurnAction : PlayerAction;
        public sealed record PlayCardAction(int cardIndex, Character? Target) : PlayerAction;

        public State CurrentState { get; private set; }
        private CombatResult combatResult { get; set; } = CombatResult.None;

        public bool PlayerWon => combatResult == CombatResult.Victory;

        public void TransitionToState(State newState) => CurrentState = newState;

        public void OnCharacterDamaged(Character damagedCharacter)
        {
            if (damagedCharacter.HP > 0)
                return;

            // defeat first
            if (ReferenceEquals(damagedCharacter, Player))
            {
                combatResult = CombatResult.Defeat;
                TransitionToState(State.CombatEnd);
                return;
            }

            // prune and check victory
            Enemies.RemoveAll(e => e.HP <= 0);
            if (Enemies.Count == 0)
            {
                combatResult = CombatResult.Victory;
                TransitionToState(State.CombatEnd);
            }
        }

        public void SubmitPlayerAction(PlayerAction action)
        {
            if (CurrentState != State.PlayerWaitingAction)
                throw new InvalidOperationException("Its not the players turn to act.");

            switch (action)
            {
                case EndTurnAction:
                    TransitionToState(State.PlayerTurnEnd);
                    break;

                case PlayCardAction playCardAction:
                    if (playCardAction.cardIndex < 0 || playCardAction.cardIndex >= Player.Hand.Count)
                        throw new ArgumentOutOfRangeException(nameof(playCardAction.cardIndex), "Invalid card index.");

                    bool success = Player.TryPlayCard(Context, Player.Hand[playCardAction.cardIndex], playCardAction.Target);
                    if (!success)
                        throw new InvalidOperationException("Cannot play the selected card.");

                    break;

                default:
                    throw new ArgumentException("Unknown player action.");
            }
        }

        public void AdvanceState()
        {
            switch (CurrentState)
            {
                case State.CombatStart:
                    Context.TriggerRelics(Player, TriggerEvent.CombatStart, Context, null);
                    TransitionToState(State.PlayerTurnStart);
                    break;

                case State.PlayerTurnStart:
                    Player.StartTurn(Context, 3);

                    // Plan intents here so player always sees them (locked for the turn)
                    foreach (var enemy in Enemies)
                        if (enemy.HP > 0)
                            enemy.PlanNextAction();

                    TransitionToState(State.PlayerWaitingAction);
                    break;

                case State.PlayerWaitingAction:
                    // UI drives via SubmitPlayerAction
                    break;

                case State.PlayerTurnEnd:
                    Player.EndTurn(Context);
                    TransitionToState(State.EnemyTurnStart);
                    break;

                case State.EnemyTurnStart:
                    foreach (var enemy in Enemies)
                        enemy.StartTurn(Context, 3);

                    TransitionToState(State.EnemyActing);
                    break;

                case State.EnemyActing:
                    foreach (var enemy in Enemies)
                    {
                        if (enemy.HP <= 0) continue;
                        if (Player is Player player)
                            enemy.ExecutePlannedAction(Context, player);
                    }
                    TransitionToState(State.EnemyTurnEnd);
                    break;

                case State.EnemyTurnEnd:
                    foreach (var enemy in Enemies)
                        enemy.EndTurn(Context);

                    TransitionToState(State.PlayerTurnStart);
                    break;

                case State.CombatEnd:
                    Context.TriggerRelics(Player, TriggerEvent.CombatEnd, Context, Context);
                    break;
            }
        }
    }
}
