using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace AutoScrip.Helpers;

internal static class StatusesHelper
{
    /// <summary>
    /// Checks if the player is not busy and available to perform actions.
    /// </summary>
    /// <returns>
    /// True if the player is available, not casting, not jumping, targetable, and not animation-locked; otherwise, false.
    /// </returns>
    internal static bool PlayerNotBusy()
    {
        return Player.Available
               && Player.Object.CastActionId == 0
               && !IsOccupied()
               && !Svc.Condition[ConditionFlag.Jumping]
               && Player.Object.IsTargetable
               && !Player.IsAnimationLocked;
    }

    internal static bool IsFishing() => Svc.Condition[ConditionFlag.Fishing];

    internal unsafe static bool IsMoving() => AgentMap.Instance()->IsPlayerMoving;

    internal static bool IsExtracting() => Svc.Condition[ConditionFlag.Occupied39];

    internal static bool IsMounted() => Svc.Condition[ConditionFlag.Mounted];

    internal static bool InFlight() => Svc.Condition[ConditionFlag.InFlight];
}
