using System.Reflection.Metadata.Ecma335;

namespace CardGame;

public class CardLibrary
{
    private readonly List<Func<Card>> AllCards = new List<Func<Card>>()
    {
        () => new Strike(),
        () => new Block(),
        () => new Heal(),
        () => new Cleave()
    };

    //Later add rarities and weights and such
    public List<Card> CreateRewardOptions(int count, Random rng)
    {
        if(count > AllCards.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds available unique cards.");
        }
        HashSet<int> chosenIndices = new HashSet<int>();
        List<Card> rewardOptions = new List<Card>();
        while(rewardOptions.Count < count)
        {
            int index = rng.Next(AllCards.Count);
            if(!chosenIndices.Contains(index))
            {
                chosenIndices.Add(index);
                rewardOptions.Add(AllCards[index]());
            }
        }
        return rewardOptions;
    }
}