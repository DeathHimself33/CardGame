namespace CardGame
{
    public sealed class DataRelic : Relic
    {
        private readonly RelicDef _def;
        public DataRelic(RelicDef def)
        {
            _def = def;
            Name = def.Name;
            Rarity = def.Rarity;
            ID = def.ID;
        }
        public override IReadOnlyCollection<StatusEffectType> Immunities => _def.Immunities;
        public override void OnEvent(TriggerEvent trigger, Character owner, CombatContext context, object? eventData = null)
        {
            if (!_def.Triggers.TryGetValue(trigger, out var ops) || ops.Count == 0)
                return;
            EffectExecutor.Execute(context, owner, new[] { owner }, ops);
        }
    }
}