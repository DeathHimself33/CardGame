using System.Diagnostics.Contracts;

namespace CardGame;
public class WeakImmunityRelic : Relic
{
    public string name = "Amulet of Fortitude";
    private readonly HashSet<StatusEffectType> _immunities;
    public WeakImmunityRelic(params StatusEffectType[] immunities)
    {
        Name = name;
        Rarity = RelicRarity.Rare;
        _immunities = new HashSet<StatusEffectType>(immunities);
    }
    public override IReadOnlyCollection<StatusEffectType> Immunities => _immunities;
}