namespace CardGame;
public class RelicLibrary
{
    private readonly List<RelicDef> _defs;
    private readonly IReadOnlyDictionary<string, RelicDef> _defsById;

    public RelicLibrary(IReadOnlyDictionary<string,RelicDef> defsById)
    {
        _defsById = defsById;
        _defs = defsById.Values.ToList();
    }

    public Relic Create(string id) => new DataRelic(_defsById[id]);

    //Later add rarities and weights and such
    public List<Relic> CreateRewardOptions(int count, Random rng)
    {
        if(count > _defs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds available unique relics.");
        }
        HashSet<int> chosenIndices = new HashSet<int>();
        List<Relic> rewardOptions = new List<Relic>();
        while(rewardOptions.Count < count)
        {
            int index = rng.Next(_defs.Count);
            if(!chosenIndices.Contains(index))
            {
                chosenIndices.Add(index);
                rewardOptions.Add(new DataRelic(_defs[index]));
            }
        }
        return rewardOptions;
    }
}