using System;
using System.Linq;

namespace CardGame
{
    public abstract partial class Character
    {
        public void TriggerStatusEffects(CombatContext context, StatusEffectTrigger trigger)
        {
            foreach (var effect in StatusEffects)
            {
                effect.OnTrigger(context, trigger, this);
            }
        }
        public void AddStatusEffect(StatusEffect effect)
        {
            if (IsImmuneTo(effect.Type))
            {
                return;
            }
            StatusEffects.Add(effect);
        }
        public void RefreshStatusEffects(CombatContext context)
        {
            foreach (var effect in StatusEffects)
            {
                effect.TickDuration();
            }
            StatusEffects.RemoveAll(e => e.IsExpired());
        }
        public bool IsImmuneTo(StatusEffectType type)
        {
            foreach (var relic in Relics)
            {
                if (relic.Immunities.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }
        public void ApplyPoison(int amount, int duration)
        {
            var poison = StatusEffects.OfType<Poison>().FirstOrDefault();
            if (poison != null)
            {
                poison.AddAmount(amount);
                poison.RefreshDuration(duration);
            }
            else
            {
                StatusEffects.Add(new Poison(amount, duration));
            }
        }
    }
}