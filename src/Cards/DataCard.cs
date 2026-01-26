using System.ComponentModel;
using System.Security.Cryptography;

namespace CardGame;
public sealed class DataCard : Card
{
    private  CardDef _def;
    public bool IsUpgraded {get; private set; }

    public DataCard(CardDef def, bool upgraded = false)
    {
        SetDef(def, upgraded);
    }
    public void Upgrade()
    {
        if(_def.Upgraded == null)
            return;
        SetDef(_def.Upgraded, true);
    }

    private void SetDef(CardDef def, bool upgraded)
    {
        _def = def;
        IsUpgraded = upgraded;

        ID = def.ID;
        Name = def.Name;
        Description = def.Description;
        Cost = def.Cost;
        TargetType = def.TargetType;
        Rarity = def.Rarity;
        Type = def.Type;
    }
    protected override void OnPlay(CombatContext context, Character user, IReadOnlyList<Character> targets)
        => EffectExecutor.Execute(context, user, targets, _def.Ops);
}