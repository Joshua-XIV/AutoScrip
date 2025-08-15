using AutoScrip.Helpers;
using ECommons.Automation;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskPreserveCollectable
{
    internal static void Enqueue()
    {
        Generic.PluginLogInfoEnqueue("Waiting for last fish");
        Plugin.taskManager.Enqueue(Preserve, 1000*90);
    }

    internal unsafe static bool? Preserve()
    {
        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var addon) && IsAddonReady(addon))
        {
            Callback.Fire(addon, true, 0);
            return true;
        }
        else if (!StatusesHelper.IsFishing() && !Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.ExecutingGatheringAction])
        {
            return true;
        }
        return false;
    }
}
