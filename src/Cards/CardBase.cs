#nullable enable
namespace CardGame;
public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
public enum CardType
{
    Attack,
    Skill,
    Power
}
public enum TargetType
{
    None,
    Self,
    SingleEnemy,
    AllEnemies
}
public abstract class Card
{
    public string Name { get; set; } = "";
    public int Cost { get; set; }
    public TargetType TargetType { get; set; }
    public CardRarity Rarity { get; set; }
    public CardType Type { get; set; }

    public void PlayCard(CombatContext context, Character user, IReadOnlyList<Character> targets)
    {
        OnPlay(context, user, targets);
    }
    protected abstract void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets);
}