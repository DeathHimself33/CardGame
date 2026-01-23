namespace CardGame;
public sealed class DataCard : Card
{
    private readonly CardDef _def;

    public DataCard(CardDef def)
    {
        _def = def;
        Name = def.Name;
        Cost = def.Cost;
        TargetType = def.TargetType;
        Rarity = def.CardRarity;
        Type = def.Type;
    }

    protected override void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets)
        => EffectExecutor.Execute(context, user, targets, _def.Ops);
}