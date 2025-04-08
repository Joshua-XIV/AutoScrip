using AutoScrip.Helpers;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;

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
                Notify.Success("Spot Successful");
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
