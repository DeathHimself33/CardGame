namespace CardGame;
public sealed class DataCard : Card
{
    private readonly CardDef _def;

    public DataCard(CardDef def)
    {
        _def = def;
        ID = def.ID;
        Name = def.Name;
        Cost = def.Cost;
        TargetType = def.TargetType;
        Rarity = def.Rarity;
        Type = def.Type;
    }

    protected override void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets)
        => EffectExecutor.Execute(context, user, targets, _def.Ops);
}