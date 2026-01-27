#nullable enable
using System;
using System.Runtime;

namespace CardGame
{
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
            //Basic enemy AI, replace later
            if (HP <= MaxHP / 4)
                return EnemyIntent.Defend;

            if (!this.StatusEffects.Any(s => s.Type == StatusEffectType.Strength))
            {
                return EnemyIntent.Buff;
            }
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
                EnemyIntent.Buff => new PlannedAction(intent, 3), // Integer is amount
                EnemyIntent.Debuff => new PlannedAction(intent, 2), // Integer is duration
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
                case EnemyIntent.Buff:
                    AddStatusEffect(new Strength(plannedAction.Amount, 3));
                    break;
                case EnemyIntent.Debuff:
                    player.AddStatusEffect(new Weak(plannedAction.Amount));
                    break;
            }

            plannedAction = null;
        }
    }
}
