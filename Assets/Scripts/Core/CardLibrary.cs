using System.Collections.Generic;
using System;
using System.Linq;

namespace CardGame
{
    public class CardLibrary
    {
        private readonly List<CardDef> _defs;
        private readonly IReadOnlyDictionary<string, CardDef> _defsById;

        public CardLibrary(IReadOnlyDictionary<string, CardDef> defsById)
        {
            _defsById = defsById;
            _defs = defsById.Values.ToList();
        }

        public Card Create(string id) => new DataCard(_defsById[id]);

        //Later add rarities and weights and such
        public List<Card> CreateRewardOptions(int count, Random rng, RewardProfile rewardProfile)
        {
            List<Card> rewardOptions = new();
            HashSet<string> chosenIds = new(StringComparer.OrdinalIgnoreCase);

            while (rewardOptions.Count < count)
            {
                CardRarity rarity = RewardProfiles.RollCard(rewardProfile, rng);
                var pool = _defs.Where(d => d.Rarity == rarity && !chosenIds.Contains(d.ID)).ToList();
                if (pool.Count == 0)
                {
                    pool = _defs.Where(d => !chosenIds.Contains(d.ID)).ToList();
                }
                if (pool.Count == -0)
                    break;

                var chosenDef = pool[rng.Next(pool.Count)];

                rewardOptions.Add(new DataCard(chosenDef));
                chosenIds.Add(chosenDef.ID);
            }
            return rewardOptions;
        }
    }
}