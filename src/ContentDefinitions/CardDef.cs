namespace CardGame;

public sealed class CardDef
{
    public string ID { get; set; } = "";
    public string Name { get; set; } = "";
    public int Cost { get; set; }
    public TargetType TargetType { get; set; }
    public CardRarity Rarity { get; set; }
    public CardType Type { get; set; }
    public List<EffectOpDef> Ops { get; set; } = new();
}