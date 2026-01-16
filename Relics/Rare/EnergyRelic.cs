namespace CardGame;
public class EnergyRelic : Relic
{
    public string name = "Energy Amulet";
    public override void OnEvent(
        TriggerEvent trigger,
        Character owner,
        CombatContext context,
        object? eventData = null)
    {
        if(trigger == TriggerEvent.TurnStart)
        {
            owner.GainEnergy(1);
        }
    }
}