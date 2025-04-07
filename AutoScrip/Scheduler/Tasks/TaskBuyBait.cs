using AutoScrip.Data;
using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Scheduler.Tasks;

internal static class TaskBuyBait
{
    internal static void Enqueue()
    {
        Generic.PluginLogInfoEnqueue("Buying Bait");
        Plugin.taskManager.Enqueue(BuyBait);
    }

    internal unsafe static bool? BuyBait()
    {
        AtkUnitBase* addon;
        if (InventoryHelper.HasFishingBait() || InventoryHelper.GetGil()/300 == 0)
        {
            if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out addon) && IsAddonReady(addon))
            {
                Callback.Fire(addon, true, -1);
                return false;
            }
            if (TryGetAddonByName<AtkUnitBase>("Shop", out addon) && IsAddonReady(addon))
            {
                Callback.Fire(addon, true, -1);
                return false;
            }
            if (TryGetAddonByName<AtkUnitBase>("SelectIconString", out addon) && IsAddonReady(addon))
            {
                Callback.Fire(addon, true, -1);
                return false;
            }
            return true;
        }
        else if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("SelectYes"))
                Callback.Fire(addon, true, 0);
        }
        else if (TryGetAddonByName<AtkUnitBase>("Shop", out addon) && IsAddonReady(addon))
        {
            var quantity = InventoryHelper.GetGil() / 300;
            var maxLimit = C.BuyBait ? 99 : 999;
            if (EzThrottler.Throttle("Purchase"))
                Callback.Fire(addon, true, 0, 3, Math.Min(quantity, maxLimit));
        }
        else if (!TryGetAddonByName<AtkUnitBase>("Shop", out addon) || !IsAddonReady(addon))
        {
            Generic.PluginDebugInfoInstant("Attempting OpenExchange");
            IGameObject? gameObject;
            if ((gameObject = ObjectHelper.GetObjectByGameObjectId(BaitData.VendorObjectId)) == null)
                return false;

            if ((addon = ObjectHelper.InteractWithObjectUntilAddon(gameObject, "SelectIconString")) == null)
                return false;

            if (EzThrottler.Throttle("SelectScripExchange"))
            {
                Callback.Fire(addon, true, 0);
            }
        }

        return false;
    }
}
