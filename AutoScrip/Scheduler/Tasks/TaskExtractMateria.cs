using AutoScrip.Helpers;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Scheduler.Tasks;

internal class TaskExtractMateria
{
    internal static void Enqueue()
    {
        Generic.PluginDebugInfoEnqueue("Extracting Materia");
        Plugin.taskManager.Enqueue(OpenExtract);
        Plugin.taskManager.Enqueue(Extract, 1000 * 60);
    }

    internal unsafe static bool? Extract()
    {
        if (!RepairAndExtractHelper.NeedsExtract())
        {
            if (TryGetAddonByName<AtkUnitBase>("Materialize", out var addon) && IsAddonReady(addon))
            {
                Callback.Fire(addon, true, -1);
            }
            return true;
        }
        else if (StatusesHelper.IsExtracting())
        {
            return false;
        }
        else if (TryGetAddonByName<AtkUnitBase>("MaterializeDialog", out var dialog) && IsAddonReady(dialog))
        {
            if (EzThrottler.Throttle("ExtractingCall"))
            {
                Callback.Fire(dialog, true, 0);
            }
            return false;
        }
        else if (TryGetAddonByName<AtkUnitBase>("Materialize", out var addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("Selection"))
            {
                Callback.Fire(addon, true, 2, 0);
            }
            return false;
        }
        return false;
    }

    internal unsafe static bool? OpenExtract()
    {
        if (TryGetAddonByName<AtkUnitBase>("Materialize", out var addon) && IsAddonReady(addon))
        {
            return true;
        }
        if (EzThrottler.Throttle("OpeningExtract", 500))
            ActionHelper.ExecuteAction(ActionType.GeneralAction, 14);
        return false;
    }
}
