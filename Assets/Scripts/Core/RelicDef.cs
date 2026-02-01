using System.Collections.Generic;

namespace CardGame
{
    public sealed class RelicDef
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public RelicRarity Rarity { get; set; }
        public List<StatusEffectType> Immunities { get; set; }
        public Dictionary<TriggerEvent, List<EffectOpDef>> Triggers { get; set; }
    }
}