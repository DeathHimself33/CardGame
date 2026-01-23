using System.ComponentModel.DataAnnotations;

namespace CardGame;
public enum TriggerEvent
{
    TurnStart, 
    TurnEnd,
    CombatStart,
    CombatEnd,
    CardPlayed,
    DamageTaken,
    BlockGained,
    RelicPickup,
    Healed
}
public enum RelicRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}
public abstract class Relic
{
    public string Name{ get; set; } = "";
    public RelicRarity Rarity { get; protected set; }
    public virtual IReadOnlyCollection<StatusEffectType> Immunities => Array.Empty<StatusEffectType>();
    public virtual void OnEvent(
        TriggerEvent trigger,
        Character owner,
        CombatContext context,
        object? eventData = null)
    {
        throw new NotImplementedException("OnEvent method must be overridden in derived Relic classes.");
    }
    public void Pickup(Character owner,CombatContext context)
    {
        owner.Relics.Add(this);
        OnEvent(TriggerEvent.RelicPickup, owner, context);  
    }
}
public sealed class RelicDef
{
    public string ID {get; set;} = "";
    public string Name {get; set;} = "";
    public RelicRarity Rarity {get; set;}
    public List<StatusEffectType> Immunities {get; set;} = new ();
    public Dictionary<TriggerEvent, List<EffectOpDef>> Triggers {get; set;} = new();
}
public sealed class DataRelic : Relic
{
    private readonly RelicDef _def;
    public DataRelic(RelicDef def)
    {
        _def = def;
        Name = def.Name;
        Rarity = def.Rarity;
    }
    public override IReadOnlyCollection<StatusEffectType> Immunities => _def.Immunities;
    public override void OnEvent(TriggerEvent trigger, Character owner, CombatContext context, object? eventData = null)
    {
        if(!_def.Triggers.TryGetValue(trigger, out var ops) || ops.Count == 0)
            return;
        EffectExecutor.Execute(context, owner, new[] {owner}, ops);
    }
}