using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskMount
{
    public static void Enqueue()
    {
        Generic.PluginLogInfoEnqueue("Mounting");
        Plugin.taskManager.Enqueue(Mount);
    }

    internal unsafe static bool? Mount()
    {
        if (Svc.Condition[ConditionFlag.Mounted] && StatusesHelper.PlayerNotBusy()) return true;

        if (!Svc.Condition[ConditionFlag.Casting] && !Svc.Condition[ConditionFlag.MountOrOrnamentTransition] && StatusesHelper.PlayerNotBusy())
        {
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 24);
        }

        return false;
    }
}