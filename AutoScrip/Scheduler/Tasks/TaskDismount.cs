using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskDisMount
{
    public static void Enqueue()
    {
        Generic.PluginLogInfoEnqueue("DisMounting");
        Plugin.taskManager.Enqueue(DisMount);
    }

    internal unsafe static bool? DisMount()
    {
        if (!Svc.Condition[ConditionFlag.Mounted] && StatusesHelper.PlayerNotBusy()) return true;

        if (Svc.Condition[ConditionFlag.Mounted] && StatusesHelper.PlayerNotBusy())
        {
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 23);
            return false;
        }

        return false;
    }
}
