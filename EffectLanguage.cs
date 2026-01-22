using CardGame;

public enum EffectOpKind{
    Damage, 
    Block, 
    Heal,
    Poison, 
    GainEnergy}
public sealed class EffectOpDef
{
    public EffectOpKind Op{get;set;}
    public int Amount {get; set;}
    public int Duration {get;set;} // Used only for poison etc.
}
public static class EffectExecutor
{
    public static void Execute(
        CombatContext context,
        Character user,
        IReadOnlyList<Character> targets,
        IReadOnlyList<EffectOpDef> ops)
    {
        foreach(var op in ops)
        {
            switch (op.Op)
            {
                case EffectOpKind.Damage:
                    foreach(var t in targets)
                        user.DealDamage(context, t, op.Amount);
                    break;
                case EffectOpKind.Block:
                    user.GainBlock(context, op.Amount);
                    break;
                case EffectOpKind.Heal:
                    user.Heal(context, op.Amount);
                    break;
                case EffectOpKind.Poison:
                    foreach(var t in targets)
                        t.ApplyPoison(op.Amount, op.Duration);
                    break;
                case EffectOpKind.GainEnergy:
                    user.GainEnergy(op.Amount);
                    break;
            }
        }
    }
}