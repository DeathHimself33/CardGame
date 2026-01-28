using System.Collections.Generic;
using System;
using System.Linq;

namespace CardGame
{
    public class RelicLibrary
    {
        private readonly List<RelicDef> _defs;
        private readonly IReadOnlyDictionary<string, RelicDef> _defsById;

        public RelicLibrary(IReadOnlyDictionary<string, RelicDef> defsById)
        {
            _defsById = defsById;
            _defs = defsById.Values.ToList();
        }

        public Relic Create(string id) => new DataRelic(_defsById[id]);

        //Later add rarities and weights and such
        public List<Relic> CreateRewardOptions(int count, Random rng, Character owner, RewardProfile rewardProfile)
        {
            var owned = owner.Relics.Select(r => r.ID).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var available = _defs.Where(d => !owned.Contains(d.ID)).ToList();

            List<Relic> rewardOptions = new();
            HashSet<string> chosenIds = new(StringComparer.OrdinalIgnoreCase);

            count = Math.Min(count, available.Count);

            while (rewardOptions.Count < count)
            {
                RelicRarity rarity = RewardProfiles.RollRelic(rewardProfile, rng);
                var pool = available.Where(d => d.Rarity == rarity && !chosenIds.Contains(d.ID)).ToList();
                if (pool.Count == 0)
                {
                    pool = available.Where(d => !chosenIds.Contains(d.ID)).ToList();
                }
                if (pool.Count == 0)
                    break;

                var chosenDef = pool[rng.Next(pool.Count)];

                rewardOptions.Add(new DataRelic(chosenDef));
                chosenIds.Add(chosenDef.ID);
            }
            return rewardOptions;
        }
    }
}