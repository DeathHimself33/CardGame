using System.Collections.Generic;
using System;
using System.Linq;

namespace CardGame
{
    public abstract partial class Character
    {
        private static Random rng = new Random();
        public bool TryPlayCard(CombatContext context, Card card, Character target)
        {
            IReadOnlyList<Character> targets = Array.Empty<Character>();
            if (!Hand.Contains(card))
            {
                return false;
            }
            if (Energy < card.Cost)
            {
                return false;
            }
            switch (card.TargetType)
            {
                case TargetType.None:
                    {
                        if (target != null)
                        {
                            throw new ArgumentException("This card does not require a target.");
                        }
                        break;
                    }
                case TargetType.Self:
                    {
                        if (target != null)
                        {
                            throw new ArgumentException("Self-targeting cards do not take a target.");
                        }
                        targets = new List<Character> { this };
                        break;
                    }
                case TargetType.SingleEnemy:
                    {
                        if (target == null || target == this)
                        {
                            throw new ArgumentNullException(nameof(target), "A valid target must be provided for this card.");
                        }
                        targets = new List<Character> { target };
                        break;
                    }
                case TargetType.AllEnemies:
                    {
                        if (target != null)
                        {
                            throw new ArgumentNullException(nameof(target), "AllEnemies cards do not take a target");
                        }
                        targets = context.GetEnemiesOf(this).Where(e => e.HP > 0).ToList();
                        if (targets.Count == 0)
                        {
                            throw new ArgumentException("No valid targets available for this card.");
                        }
                        break;
                    }
            }
            Energy -= card.Cost;
            card.PlayCard(context, this, targets);
            Hand.Remove(card);
            DiscardPile.Add(card);
            context.TriggerRelics(this, TriggerEvent.CardPlayed, context);
            return true;
        }

        public virtual void StartTurn(CombatContext context, int EnergyForTurn)
        {
            Energy = EnergyForTurn;
            Block = 0;
            ShuffleDeck();
            DrawCards(5);
            TriggerStatusEffects(context, StatusEffectTrigger.TurnStart);
            context.TriggerRelics(this, TriggerEvent.TurnStart, context);
        }
        public void DrawCards(int DrawAmount)
        {
            for (int i = 0; i < DrawAmount; i++)
            {
                if (Deck.Count == 0)
                {
                    if (DiscardPile.Count == 0)
                    {
                        break;
                    }

                    Deck.AddRange(DiscardPile);
                    DiscardPile.Clear();
                    ShuffleDeck();
                }

                if (Deck.Count > 0)
                {
                    Card drawnCard = Deck[0];
                    Hand.Add(drawnCard);
                    Deck.RemoveAt(0);
                }
            }
        }
        public void ShuffleDeck()
        {
            for (int i = Deck.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                var temp = Deck[i];
                Deck[i] = Deck[j];
                Deck[j] = temp;
            }
        }
        public void EndTurn(CombatContext context)
        {
            DiscardPile.AddRange(Hand);
            Hand.Clear();
            RefreshStatusEffects(context);
            TriggerStatusEffects(context, StatusEffectTrigger.TurnEnd);
            context.TriggerRelics(this, TriggerEvent.TurnEnd, context);
        }

        public void BetweenFloorsReset()
        {
            DiscardPile.AddRange(Hand);
            Hand.Clear();

            Hand.AddRange(DiscardPile);
            DiscardPile.Clear();

            ShuffleDeck();

            StatusEffects.Clear();

            Block = 0;
            Energy = 0;
        }

    }
}