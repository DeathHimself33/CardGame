using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public enum RunStepKind
    {
        None,
        EnterRoom,
        NeedCombat,
        NeedShopChoice,
        NeedRestChoice,
        NeedRewardsChoice,
        RunEnded,
        PlayerDied
    }

    public abstract record RunStepResult(RunStepKind Kind);
    public sealed record EnterRoomResult(RunController.RoomType RoomType, int Floor)
        : RunStepResult(RunStepKind.EnterRoom);
    
    public sealed record CombatNeededResult(List<Enemy> Enemies, RunController.RoomType RoomType)
        : RunStepResult(RunStepKind.NeedCombat);
    
    public sealed record ShopNeededResult(List<Card> CardOffers, List<Relic> RelicOffers)
        : RunStepResult(RunStepKind.NeedShopChoice);

    public sealed record RestNeededResult(int HealAmount)
        : RunStepResult(RunStepKind.NeedRestChoice);

    public sealed record RunEndedResult(bool PlayerWon)
        : RunStepResult(RunStepKind.RunEnded);

    public sealed record RewardsNeededResult(List<Card> CardOffers, List<Relic> RelicOffers, int goldGained)
        : RunStepResult(RunStepKind.NeedRewardsChoice);
}