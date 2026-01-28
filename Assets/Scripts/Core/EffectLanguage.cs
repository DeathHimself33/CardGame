using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public enum EffectOpKind
    {
        Damage,
        Block,
        GainBlockRaw,
        Heal,
        Poison,
        GainEnergy,
        Draw
    }
    [Serializable]
    public sealed class EffectOpDef
    {
        public string Op;
        public int Amount;
        public int Duration;

        public EffectOpKind ParsedOP =>
            Enum.TryParse<EffectOpKind>(Op, ignoreCase: true, out var result)
                ? result
                : throw new Exception("Op could not be parsed for card!"); 
    }
    public static class EffectExecutor
    {
        public static void Execute(
            CombatContext context,
            Character user,
            IReadOnlyList<Character> targets,
            IReadOnlyList<EffectOpDef> ops)
        {
            Debug.Log($"Executing {ops?.Count ?? -1} ops for {user.GetType().Name}");
            foreach (var op in ops)
            {
                Debug.Log($"Op={op.Op} Amount={op.Amount} Duration={op.Duration}");
                switch (op.ParsedOP)
                {
                    case EffectOpKind.Damage:
                        foreach (var t in targets)
                            user.DealDamage(context, t, op.Amount);
                        break;
                    case EffectOpKind.Block:
                        user.GainBlock(context, op.Amount);
                        break;
                    case EffectOpKind.GainBlockRaw:
                        user.GainBlockRaw(op.Amount);
                        break;
                    case EffectOpKind.Heal:
                        user.Heal(context, op.Amount);
                        break;
                    case EffectOpKind.Poison:
                        foreach (var t in targets)
                            t.ApplyPoison(op.Amount, op.Duration);
                        break;
                    case EffectOpKind.GainEnergy:
                        user.GainEnergy(op.Amount);
                        break;
                    case EffectOpKind.Draw:
                        user.DrawCards(op.Amount);
                        break;
                }
            }
        }
    }
}