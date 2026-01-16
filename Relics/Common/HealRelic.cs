namespace CardGame;
public class HealRelic : Relic
{
    public string name = "Healing Stone";
    public override void OnEvent(
        TriggerEvent trigger,
        Character owner,
        CombatContext context,
        object? eventData = null)
    {
        if(trigger == TriggerEvent.CombatEnd)
        {
            CombatContext combatContext = context;
            owner.Heal(combatContext,2);
        }
    }
}