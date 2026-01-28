namespace CardGame
{
    public enum TriggerEvent
    {
        TurnStart,
        TurnEnd,
        CombatStart,
        CombatEnd,
        CardPlayed,
        DamageTaken,
        BlockGained,
        RelicPickup,
        Healed
    }
    public enum RelicRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }
}