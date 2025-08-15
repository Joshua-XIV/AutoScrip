using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using SharpDX;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskFlight
{
    public static void Enqueue(float minHeight = 5, float maxHeight = 8)
    {
        Random randomClass = new Random();
        float randomFloat = randomClass.NextFloat(minHeight, maxHeight);
        Generic.PluginLogInfoEnqueue("Flight");
        Plugin.taskManager.Enqueue(Flight);
        Plugin.taskManager.Enqueue(() => Chat.Instance.ExecuteCommand($"/vnav flydir 0 {randomFloat} .5"));
    }

    internal unsafe static bool? Flight()
    {
        if (Svc.Condition[ConditionFlag.InFlight] && Svc.Condition[ConditionFlag.Mounted])
        {
            return true;
        }
        else if (!Svc.Condition[ConditionFlag.InFlight] && Svc.Condition[ConditionFlag.Mounted] && StatusesHelper.PlayerNotBusy())
        {
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 2);
            return false;
        }
        else if (!Svc.Condition[ConditionFlag.Casting] && !Svc.Condition[ConditionFlag.MountOrOrnamentTransition] && !Svc.Condition[ConditionFlag.Mounted] &&  StatusesHelper.PlayerNotBusy())
        {
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 24);
            return false;
        }

        return false;
    }
}
