namespace CardGame;
public class ExtraDrawRelic : Relic
{
    public string name = "Scroll of Wisdom";
    public override void OnEvent(
        TriggerEvent trigger,
        Character owner,
        CombatContext context,
        object? eventData = null)
    {
        if(trigger == TriggerEvent.TurnStart)
        {
            owner.DrawCards(1);
        }
    }
}