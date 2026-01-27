#nullable enable
using System.Diagnostics.Contracts;

namespace CardGame
{
    /// <summary>
    /// Base class for any combat participant.
    /// Owns all persistent combat data.
    /// </summary>
    public abstract partial class Character
    {
        // Core States
        public int HP { get; protected set; }
        public int MaxHP { get; protected set; }
        public int Block { get; protected set; }
        public int Energy { get; protected set; }

        //Card zones
        public List<Card> Hand { get; } = new();
        public List<Card> Deck { get; } = new();
        public List<Card> DiscardPile { get; } = new();

        //Passive modifiers
        public List<Relic> Relics { get; } = new();

        //Active status effects
        public List<StatusEffect> StatusEffects { get; } = new();
    }
}