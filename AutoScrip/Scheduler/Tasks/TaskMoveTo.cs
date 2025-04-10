using AutoScrip.Helpers;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Numerics;
using static FFXIVClientStructs.FFXIV.Client.Game.GcArmyManager.Delegates;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskMoveTo
{
    public static void Enqueue(Vector3 targetPosition, string destination, float minDistance = 3f, bool fly = false, bool disablePathfind = false)
    {
        Generic.PluginLogInfoEnqueue($"Moving to {destination}");
        Plugin.taskManager.Enqueue(() => MoveTo(targetPosition, minDistance, fly, disablePathfind), 1000*90);
    }

    internal unsafe static bool? MoveTo(Vector3 targetPosition, float minDistance = 3f, bool fly = false, bool disablePathfind = false)
    {
        if (!Plugin.navmeshIPC.IsRunning() && !Plugin.navmeshIPC.PathfindInProgress() && !StatusesHelper.IsMoving() && DistanceHelper.GetDistanceToPlayer(targetPosition) <= minDistance)
        {
            return true;
        }
        if (DistanceHelper.GetDistanceToPlayer(targetPosition) <= minDistance)
        {
            Plugin.navmeshIPC.Stop();
        }

        if (StatusesHelper.IsMoving() && DistanceHelper.GetDistanceToPlayer(targetPosition) >= 6)
        {
            if (ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, 4) == 0 && ActionManager.Instance()->QueuedActionId != 4 && !Player.Character->IsCasting)
                ActionHelper.ExecuteAction(ActionType.GeneralAction, 4);
        }

        if (Plugin.navmeshIPC.PathfindInProgress() || Plugin.navmeshIPC.IsRunning() || StatusesHelper.IsMoving()) return false;

        if (disablePathfind)
            Plugin.navmeshIPC.MoveTo(new List<Vector3> { targetPosition }, fly);
        else
            Plugin.navmeshIPC.PathfindAndMoveTo(targetPosition, fly);
        Plugin.navmeshIPC.SetAlignCamera(true);
        return false;
    }
}
