using AutoScrip.Helpers;
using ECommons.Configuration;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        if (StatusesHelper.IsFishing())
        {
            Generic.PluginLogInfoInstant("Fishing");
            Plugin.navmeshIPC.Stop();
            return true;
        }
        else if (Plugin.navmeshIPC.PathfindInProgress() || Plugin.navmeshIPC.IsRunning() && ActionManager.Instance()->GetActionStatus(ActionType.Action, 289) == 0)
        {
            if (EzThrottler.Throttle("CastLine", 50))
                ActionHelper.ExecuteAction(ActionType.Action, 289);
            return false;
        }

        Plugin.navmeshIPC.MoveTo([C.SelectedFish.FishingSpots.PointToFace], false);
        Plugin.navmeshIPC.SetAlignCamera(true);
        return false;
    }
}
