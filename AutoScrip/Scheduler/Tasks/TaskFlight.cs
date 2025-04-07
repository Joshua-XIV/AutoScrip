using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskFlight
{
    public static void Enqueue()
    {
        Generic.PluginLogInfoEnqueue("Flight");
        Plugin.taskManager.Enqueue(Flight);
    }

    internal unsafe static bool? Flight()
    {
        if (Svc.Condition[ConditionFlag.InFlight] && Svc.Condition[ConditionFlag.Mounted] && StatusesHelper.PlayerNotBusy()) return true;

        if (!Svc.Condition[ConditionFlag.InFlight] && Svc.Condition[ConditionFlag.Mounted] && StatusesHelper.PlayerNotBusy())
        {
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 2);
            return false;
        }

        if (!Svc.Condition[ConditionFlag.Casting] && !Svc.Condition[ConditionFlag.Unknown57] && StatusesHelper.PlayerNotBusy())
        {
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 24);
            return false;
        }

        return false;
    }
}
