namespace CardGame;
public class BlockRelic : Relic
{    public string name = "Shielding Idol";
    public override void OnEvent(
        TriggerEvent trigger,
        Character owner,
        CombatContext context,
        object? eventData = null)
    {
        if(trigger == TriggerEvent.BlockGained)
        {
            owner.GainBlockRaw(1);
        }
    }
}