using AutoScrip.Helpers;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Numerics;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskGoToFishLocation
{
    internal static void Enqueue()
    {
        Vector3 randomLocation;
        if (C.SelectedScripColor == ScripColor.Orange && C.SetCustomZorgorWaypoints)
        {
            var zongorFilter = C.CustomZorgorWaypoints.OrderBy(spot => DistanceHelper.GetDistanceToPlayer(spot)).Skip(1).ToList();
            randomLocation = zongorFilter[new Random().Next(zongorFilter.Count)];
        }
        else if (C.SelectedScripColor == ScripColor.Purple && C.SetCustomFleetingBrandWaypoints)
        {
            var fleetingFilter = C.CustomFleetingBrandWaypoints.OrderBy(spot => DistanceHelper.GetDistanceToPlayer(spot)).Skip(1).ToList();
            randomLocation = fleetingFilter[new Random().Next(fleetingFilter.Count)];
        }
        else
        {
            randomLocation = C.SelectedFish.FishingSpots.Waypoints.Where(spot => DistanceHelper.GetDistanceToPlayer(spot) > 25).GetRandom();
        }
        TaskMount.Enqueue();
        TaskFlight.Enqueue();
        TaskMoveTo.Enqueue(randomLocation, "Fishing Location", 1f, true);
        TaskDisMount.Enqueue();
        Plugin.taskManager.Enqueue(GoToFishLocation, 1000*20);
    }

    internal unsafe static bool? GoToFishLocation()
    {
        if (StatusesHelper.IsFishing() || Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.Gathering])
        {
            Generic.PluginLogInfoInstant("Fishing");
            Plugin.navmeshIPC.Stop();
            return true;
        }
        else if (Plugin.navmeshIPC.PathfindInProgress() || Plugin.navmeshIPC.IsRunning() && ActionManager.Instance()->GetActionStatus(ActionType.Action, 289) == 0)
        {
            if (Svc.ClientState.LocalPlayer.CurrentGp >= 230)
            {
                if (EzThrottler.Throttle("CastLine", 50))
                    Chat.Instance.ExecuteCommand("/ahstart");
            }
            else
            {
                if (EzThrottler.Throttle("CastLine", 50))
                    ActionHelper.ExecuteAction(ActionType.Action, 289);
            }
            return false;
        }
        else if (InventoryHelper.CurrentBait != 29717)
        {
            if (EzThrottler.Throttle("SwapBait"))
                Chat.Instance.ExecuteCommand($"/bait 29717");
            return false;
        }

            Plugin.navmeshIPC.MoveTo([C.SelectedFish.FishingSpots.PointToFace], false);
        Plugin.navmeshIPC.SetAlignCamera(true);
        return false;
    }
}
