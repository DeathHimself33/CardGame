#nullable enable
using System;

namespace CardGame;

public class Enemy : Character
{
    public enum EnemyIntent
    {
        Attack,
        Defend,
        Buff,
        Debuff
    }

    public sealed record PlannedAction(EnemyIntent Intent, int Amount);

    public PlannedAction? plannedAction { get; private set; }

    public Enemy(int maxHP)
    {
        MaxHP = maxHP;
        HP = maxHP;
    }

    public EnemyIntent DeclareIntent()
    {
        // Example logic: defend when low, otherwise attack
        if (HP <= MaxHP / 4)
            return EnemyIntent.Defend;

        return EnemyIntent.Attack;
    }

    // Plan at start of PLAYER turn (locked-in)
    public void PlanNextAction()
    {
        var intent = DeclareIntent();
        plannedAction = intent switch
        {
            EnemyIntent.Attack => new PlannedAction(intent, 5),
            EnemyIntent.Defend => new PlannedAction(intent, 3),
            _ => new PlannedAction(intent, 0)
        };
    }

    // Execute on ENEMY turn (no replanning)
    public void ExecutePlannedAction(CombatContext context, Player player)
    {
        if (plannedAction is null)
            throw new InvalidOperationException("Enemy has no planned action.");

        switch (plannedAction.Intent)
        {
            case EnemyIntent.Attack:
                DealDamage(context, player, plannedAction.Amount);
                break;

            case EnemyIntent.Defend:
                GainBlock(context, plannedAction.Amount);
                break;
        }

        plannedAction = null;
    }
}
