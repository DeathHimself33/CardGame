using System.Collections;
using System.Runtime;

namespace CardGame;
public enum StatusEffectType
{
    Strength,
    Weak,
    Vulnerable,
    Poison
}
public enum StatusCategory
{
    Buff,
    Debuff
}
public enum StatusEffectTrigger
{
    TurnStart,
    TurnEnd,
    DamageDealt,
    DamageTaken
}
public abstract class StatusEffect
{
    public StatusEffectType Type { get; }
    // Not needed yet
    // public StatusCategory Category
    // {
    //     get
    //     {
    //         return Type switch
    //         {
    //             StatusEffectType.Strength => StatusCategory.Buff,
    //             StatusEffectType.Weak => StatusCategory.Debuff,
    //             StatusEffectType.Vulnerable => StatusCategory.Debuff,
    //             StatusEffectType.Poison => StatusCategory.Debuff,
    //             _ => throw new ArgumentOutOfRangeException()
    //         };
    //     }
    // }
    public int Amount{ get; protected set;}
    public int Duration{ get; protected set;} // -1 = permanent

    protected StatusEffect(StatusEffectType type, int amount, int duration)
    {
        Type = type;
        Amount = amount;
        Duration = duration;
    }

    public virtual int ModifyDamageDealt(int damage) => damage;
    public virtual int ModifyDamageTaken(int damage) => damage;

    public virtual void OnTrigger(CombatContext context, StatusEffectTrigger trigger, Character owner) { }
    public void AddAmount(int amount)
    {
        Amount += amount;
    }
    public void RefreshDuration(int duration)
    {
        Duration = Math.Max(Duration,duration);
    }

    public void TickDuration()
    {
        if(Duration > 0)
        {
            Duration--;
        }
    }
    public bool IsExpired() => Duration == 0;
}
public class Strength : StatusEffect
{
    public Strength(int amount, int duration) : base(StatusEffectType.Strength, amount, duration) { }
    public override int ModifyDamageDealt(int damage)
    {
        return base.ModifyDamageDealt(damage + Amount);
    }
}
public class Weak : StatusEffect
{
    public Weak(int duration) : base(StatusEffectType.Weak, 25, duration) { }
    public override int ModifyDamageDealt(int damage)
    {
        return base.ModifyDamageDealt(damage * 3 / 4);
    }
}
public class Vulnerable : StatusEffect
{
    public Vulnerable(int duration) : base(StatusEffectType.Vulnerable, 50, duration) { }
    public override int ModifyDamageTaken(int damage)
    {
        return base.ModifyDamageTaken(damage * 3 / 2);
    }
}
public class Poison : StatusEffect
{
    public Poison(int amount, int duration) : base(StatusEffectType.Poison, amount, duration) { }
    public override void OnTrigger(CombatContext context, StatusEffectTrigger trigger, Character owner)
    {
        if(trigger == StatusEffectTrigger.TurnStart)
        {
            owner.TakeDamageDirect(context, Amount);
        }
    }
}
