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
public class Strike : Card
{
    public Strike()
    {
        Name = "Strike";
        Cost = 1;
        TargetType = TargetType.SingleEnemy;
        Rarity = CardRarity.Common;
        Type = CardType.Attack;

    }
    protected override void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets)
    {
        if(targets == null || targets.Count == 0 || targets.Count > 1)
        {
            throw new ArgumentNullException(nameof(targets), "Targets invalid for strike card");
        }
        user.DealDamage(context, targets[0], 6);
    }
}
public class Block : Card
{
    public Block()
    {
        Name = "Block";
        Cost = 1;
        TargetType = TargetType.Self;
        Rarity = CardRarity.Common;
        Type = CardType.Skill;
    }
    protected override void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets)
    {
        if(targets == null || targets.Count != 1)
        {
            throw new ArgumentNullException(nameof(targets), "Targets invalid for block card");
        }
        targets[0].GainBlock(context, 2);
    }
}
public class Heal : Card
{
    public Heal()
    {
        Name = "Heal";
        Cost = 2;
        TargetType = TargetType.Self;
        Rarity = CardRarity.Uncommon;
        Type = CardType.Skill;
    }
    protected override void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets)
    {
        if(targets == null || targets.Count != 1)
        {
            throw new ArgumentNullException(nameof(targets), "Targets invalid for heal card");
        }
        targets[0].Heal(context, 3);
    }
}
public class Cleave : Card
{
    public Cleave()
    {
        Name = "Cleave";
        Cost = 1;
        TargetType = TargetType.AllEnemies;
        Rarity = CardRarity.Common;
        Type = CardType.Attack;
    }
    protected override void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets)
    {
        if(targets.Count == 0)
        {
            throw new ArgumentNullException(nameof(targets), "No targets available for cleave card");
        }
        foreach (var target in targets)
        {
            user.DealDamage(context, target, 3);
        }
    }
}