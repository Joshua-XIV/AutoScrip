using AutoScrip.Data;
using AutoScrip.Helpers;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation;
using ECommons.Automation.NeoTaskManager.Tasks;
using ECommons.Configuration;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Scheduler.Tasks;

internal class TaskExchange
{
    private static ScripItem itemToBuy = C.SelectedScripColor == ScripColor.Orange ? C.OrangeScripItem : C.PurpleScripItem;
    internal static void Enqueue()
    {
        itemToBuy = C.SelectedScripColor == ScripColor.Orange ? C.OrangeScripItem : C.PurpleScripItem;
        Generic.PluginLogInfoEnqueue("Attempting Exchange");
        Plugin.taskManager.Enqueue(OpenExchange);
        Plugin.taskManager.DelayNext(500);
        Plugin.taskManager.Enqueue(() => OpenCategory(itemToBuy.CategoryMenu));
        Plugin.taskManager.DelayNext(500);
        Plugin.taskManager.Enqueue(() => OpenSubCategory(itemToBuy.SubcategoryMenu));
        Plugin.taskManager.DelayNext(500);
        Plugin.taskManager.Enqueue(() => OpenPurchase(itemToBuy.ListIndex, itemToBuy.Price));
        Plugin.taskManager.DelayNext(500);
        Plugin.taskManager.Enqueue(Purchase);
        Plugin.taskManager.DelayNext(500);
        Plugin.taskManager.Enqueue(CloseExchange);
    }

    internal unsafe static bool? OpenExchange()
    {
        AtkUnitBase* addon;
        if (InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) < itemToBuy.Price)
            return true;
        if (TryGetAddonByName<AtkUnitBase>("InclusionShop", out addon) && IsAddonReady(addon))
        {
            return true;
        }
        else if (!TryGetAddonByName<AtkUnitBase>("InclusionShop", out addon) || !IsAddonReady(addon))
        {
            Generic.PluginDebugInfoInstant("Attempting OpenExchange");
            IGameObject? gameObject;
            if ((gameObject = ObjectHelper.GetObjectByGameObjectId(C.SelectedCity.ScripExchange.ScripExchangeGameObjectId)) == null)
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

    internal unsafe static bool? OpenCategory(int categoryIndex)
    {
        if (InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) < itemToBuy.Price)
            return false;
        if (TryGetAddonByName<AtkUnitBase>("InclusionShop", out var addon) && IsAddonReady(addon))
        {
            Generic.PluginDebugInfoInstant("Attempting OpenCategory");
            Callback.Fire(addon, true, 12, categoryIndex);
            return true;
        }
        return false;
    }

    internal unsafe static bool? OpenSubCategory(int subCategoryIndex)
    {
        if (InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) < itemToBuy.Price)
            return false;
        if (TryGetAddonByName<AtkUnitBase>("InclusionShop", out var addon) && IsAddonReady(addon))
        {
            Generic.PluginDebugInfoInstant("Attempting OpenSubCategory");
            Callback.Fire(addon, true, 13, subCategoryIndex);
            return true;
        }
        return false;
    }

    internal unsafe static bool? OpenPurchase(int listIndex, int price)
    {
        var quantity = InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) / price;
        AtkUnitBase* addon;
        if (InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) < itemToBuy.Price)
            return true;
        if (TryGetAddonByName<AtkUnitBase>("ShopExchangeItemDialog", out addon)  && IsAddonReady(addon))
        {
            return true;
        }
        else if (TryGetAddonByName<AtkUnitBase>("InclusionShop", out addon) && IsAddonReady(addon))
        {
            Generic.PluginDebugInfoInstant("Attempting OpenPurchase");
            if (EzThrottler.Throttle("OpenPurchase"))
                Callback.Fire(addon, true, 14, listIndex, Math.Min(quantity, 99));
        }
        return false;
    }

    internal unsafe static bool? Purchase()
    {
        AtkUnitBase* addon;
        if (InventoryHelper.GetItemCount(C.SelectedFish.GathererScripId) < itemToBuy.Price)
            return true;
        if (!TryGetAddonByName<AtkUnitBase>("ShopExchangeItemDialog", out addon) || !IsAddonReady(addon))
        {
            return true;
        }
        else if (TryGetAddonByName<AtkUnitBase>("ShopExchangeItemDialog", out addon) && IsAddonReady(addon))
        {
            Generic.PluginDebugInfoInstant("Attempting Purchase");
            if (EzThrottler.Throttle("Purchase"))
                Callback.Fire(addon, true, 0);
        }
        return false;
    }

    internal unsafe static bool? CloseExchange()
    {
        AtkUnitBase* addon;
        Generic.PluginDebugInfoInstant("Attempting CloseExchange");
        if (TryGetAddonByName<AtkUnitBase>("InclusionShop", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("ExitShop"))
            {
                Callback.Fire(addon, true, -1);
            }
            return false;
        }
        else if (TryGetAddonByName<AtkUnitBase>("SelectIconString", out addon) && IsAddonReady(addon))
        {
            if (EzThrottler.Throttle("ExitSelectString"))
            {
                Callback.Fire(addon, true, -1);
            }
            return false;
        }
        else
        {
            return true;
        }
    }
}
