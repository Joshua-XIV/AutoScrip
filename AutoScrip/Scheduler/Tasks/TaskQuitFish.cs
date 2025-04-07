using AutoScrip.Helpers;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskQuitFish
{
    internal static void Enqueue(bool test = false)
    {
        Generic.PluginLogInfoInstant("Quiting Fish Location");
        Plugin.taskManager.Enqueue(() => Quit(test));
    }

    internal unsafe static bool? Quit(bool test = false)
    {
        if (!Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.Gathering])
        {
            if (test)
            {
                DuoLog.Information("Spot Successful");
            }
            return true;
        }
        if (Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.Gathering])
        {
            if (EzThrottler.Throttle("Quit", 250))
                ActionHelper.ExecuteAction(ActionType.Action, 299);
            return false;
        }
        return false;
    }
}
