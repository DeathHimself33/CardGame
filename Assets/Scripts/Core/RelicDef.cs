using System.Collections.Generic;

namespace CardGame
{
    public sealed class RelicDef
    {
        public string ID;
        public string Name;
        public RelicRarity Rarity;
        public List<StatusEffectType> Immunities;
        public Dictionary<TriggerEvent, List<EffectOpDef>> Triggers;
    }
}