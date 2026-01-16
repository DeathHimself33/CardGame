namespace CardGame;
public class Enemy : Character
{
    //Enemy-specific logic here :)
    public enum EnemyIntent
    {
        Attack,
        Defend,
        Buff,
        Debuff
    }
    public Enemy(int maxHP)
    {
        MaxHP = maxHP;
        HP = maxHP;
    }
    public EnemyIntent DeclareIntent()
    {
        EnemyIntent intent;
        if(HP <= MaxHP / 4)
        {
            intent = EnemyIntent.Defend;
            return intent;
        }
        else
        {
            intent = EnemyIntent.Attack;
            return intent;
        }
    }
    public void TakeTurn(CombatContext context, Player player)
    {
        EnemyIntent intent = DeclareIntent();
        if(intent == EnemyIntent.Attack)
        {
            int damage = 5;
            DealDamage(context, player, damage);
        }
        else if(intent == EnemyIntent.Defend)
        {
            int blockAmount = 3;
            GainBlock(context, blockAmount);
        }
    }
}