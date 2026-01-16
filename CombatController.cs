#nullable enable
namespace CardGame;
public class CombatController
{
    private static readonly CardLibrary CardLibrary = new CardLibrary();
    public Random seed = new Random();
    // References to Combat Participants
    public Character Player {get;set;}
    public Character Enemy {get;set; }

    // Combat Context
    public CombatContext Context {get; private set;}

    // Constructor
    public CombatController(Character player, Character enemy)
    {
        Player = player;
        Enemy = enemy;
        Context = new CombatContext(player, enemy,onCharacterDamaged: OnCharacterDamaged);
    }

    // Combat States
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

    // Combat Result
    private enum CombatResult
    {
        None,
        Victory,
        Defeat
    }

    //Player Actions
    public abstract record PlayerAction;
    public sealed record EndTurnAction : PlayerAction;
    public sealed record PlayCardAction(int cardIndex, Character? Target) : PlayerAction;

    // Current State of the Combat
    public State CurrentState { get; private set; }


    // Result of the Combat (Defeat/Victory)
    private CombatResult combatResult { get; set; } = CombatResult.None;
    public bool PlayerWon => combatResult == CombatResult.Victory;

    public void TransitionToState(State newState)
    {
        CurrentState = newState;
        // Additional logic for entering the new state can be added here
    }
    public void OnCharacterDamaged(Character damagedCharacter)
    {
        if(damagedCharacter.HP <= 0)
        {
            if(damagedCharacter == Player)
            {
                combatResult = CombatResult.Defeat;
            }
            else if(damagedCharacter == Enemy)
            {
                combatResult = CombatResult.Victory;
            }
            TransitionToState(State.CombatEnd);
        }
    }

    public void SubmitPlayerAction(PlayerAction action)
    {
        if(CurrentState != State.PlayerWaitingAction)
        {
            throw new InvalidOperationException("It's not the player's turn to act.");
        }
        switch(action)
        {
            case EndTurnAction:
                TransitionToState(State.PlayerTurnEnd);
                break;
            case PlayCardAction playCardAction:
                if(playCardAction.cardIndex < 0 || playCardAction.cardIndex >= Player.Hand.Count)
                {
                    throw new ArgumentOutOfRangeException("Invalid card index.");
                }
                bool success = Player.TryPlayCard(Context,Player.Hand[playCardAction.cardIndex], playCardAction.Target);
                if(!success)
                {
                    throw new InvalidOperationException("Cannot play the selected card.");
                }
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
                Context.TriggerRelics(Player, TriggerEvent.CombatStart,null!);
                TransitionToState(State.PlayerTurnStart);
                break;
            case State.PlayerTurnStart:
                Player.StartTurn(Context,3);
                TransitionToState(State.PlayerWaitingAction);
                break;
            case State.PlayerWaitingAction:
                // Do nothing, UI calls SubmitPlayerAction
                break;
            case State.PlayerTurnEnd:
                Player.EndTurn(Context);
                TransitionToState(State.EnemyTurnStart);
                break;
            case State.EnemyTurnStart:
                Enemy.StartTurn(Context,3);
                TransitionToState(State.EnemyActing);
                break;
            case State.EnemyActing:
                if(Enemy is Enemy enemy && Player is Player player)
                {
                    enemy.TakeTurn(Context,player);
                }
                TransitionToState(State.EnemyTurnEnd);
                break;
            case State.EnemyTurnEnd:  
                Enemy.EndTurn(Context);
                TransitionToState(State.PlayerTurnStart);
                break;
            case State.CombatEnd:
                Context.TriggerRelics(Player, TriggerEvent.CombatEnd, Context);
                if(combatResult == CombatResult.Victory)
                {
                    List<Card> rewardOptions = CardLibrary.CreateRewardOptions(3, seed);   
                }
                else if(combatResult == CombatResult.Defeat)
                {
                }
                break;
        }
    }
}   