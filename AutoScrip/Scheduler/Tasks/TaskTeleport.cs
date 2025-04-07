using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.ComponentModel.Design;
namespace AutoScrip.Scheduler.Tasks;

internal static class TaskTeleport
{
    private static bool forceToTeleport = true;
    public static void Enqueue(uint aetheryteId, uint targetZoneId, bool forceTeleport = false)
    {
        Generic.PluginLogInfoEnqueue($"Teleporting to {ZonesHelper.GetZoneName(targetZoneId)}");
        forceToTeleport = forceTeleport;
        Plugin.taskManager.Enqueue(() => TeleportToAetheryte(aetheryteId, targetZoneId), 1000 * 15);
    }

    internal static unsafe bool? TeleportToAetheryte(uint aetheryteId, uint targetZoneId)
    {
        if (ZonesHelper.IsInZone(targetZoneId) && StatusesHelper.PlayerNotBusy())
        {
            IGameObject? gameObject = ObjectHelper.GetObjectByObjectKind(Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte);
            if (!forceToTeleport) return true;
            if (gameObject != null && DistanceHelper.GetDistanceToPlayer(gameObject.Position) < 30)
            {
                forceToTeleport = false;
                return false;
            }
            else if (CanTeleport())
            {
                Telepo.Instance()->Teleport(aetheryteId, 0);
                return false;
            }
        }

        if (CanTeleport() && !ZonesHelper.IsInZone(targetZoneId))
        {
            Telepo.Instance()->Teleport(aetheryteId, 0);
            return false;
        }

        return false;
    }

    private static bool CanTeleport()
    {
        return !Svc.Condition[ConditionFlag.Casting] &&
               StatusesHelper.PlayerNotBusy() &&
               EzThrottler.Throttle("Teleporting", 300) &&
               !Svc.Condition[ConditionFlag.BetweenAreas];
    }
}