using System.Reflection.Metadata.Ecma335;

namespace CardGame;

public class CardLibrary
{
    private readonly List<CardDef> _defs;
    private readonly IReadOnlyDictionary<string, CardDef> _defsById;

    public CardLibrary(IReadOnlyDictionary<string,CardDef> defsById)
    {
        _defsById = defsById;
        _defs = defsById.Values.ToList();
    }

    public Card Create(string id) => new DataCard(_defsById[id]);

    //Later add rarities and weights and such
    public List<Card> CreateRewardOptions(int count, Random rng)
    {
        if(count > _defs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds available unique cards.");
        }
        HashSet<int> chosenIndices = new HashSet<int>();
        List<Card> rewardOptions = new List<Card>();
        while(rewardOptions.Count < count)
        {
            int index = rng.Next(_defs.Count);
            if(!chosenIndices.Contains(index))
            {
                chosenIndices.Add(index);
                rewardOptions.Add(new DataCard(_defs[index]));
            }
        }
        return rewardOptions;
    }
}