using System.ComponentModel.DataAnnotations;

namespace CardGame;
public abstract class Relic
{
    public string ID {get; set;} = "";
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
