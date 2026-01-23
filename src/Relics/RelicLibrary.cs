using System.Reflection.Metadata;

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
    public List<Relic> CreateRewardOptions(int count, Random rng, Character owner)
    {
        var owned = owner.Relics.Select(r => r.ID).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var available = _defs.Where(d => !owned.Contains(d.ID)).ToList();

        if(available.Count == 0)
            //Later can offer gold or cards or a different reward 
            return new List<Relic>();
        
        if(count > available.Count)
        {
            count = available.Count;
        }
        HashSet<int> chosenIndices = new HashSet<int>();
        List<Relic> rewardOptions = new List<Relic>();
        while(rewardOptions.Count < count)
        {
            int index = rng.Next(available.Count);
            if(!chosenIndices.Contains(index))
            {
                chosenIndices.Add(index);
                rewardOptions.Add(new DataRelic(available[index]));
            }
        }
        return rewardOptions;
    }
}