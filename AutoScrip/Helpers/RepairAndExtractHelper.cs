using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
namespace AutoScrip.Helpers;

internal unsafe static class RepairAndExtractHelper
{
    /// <summary>
    /// Checks if equipped gear is under certain threshold.
    /// </summary>
    /// <param name="below">The threshold given to repair gear under condition, default as 0</param>
    /// <returns>True if gear is met at threshold, otherwise false</returns>
    internal static unsafe bool NeedsRepair(float below = 0)
    {
        var inventory = InventoryManager.Instance();
        if (inventory == null)
        {
            Svc.Log.Error("InventoryManager was null");
            return false;
        }

        var equipped = inventory->GetInventoryContainer(InventoryType.EquippedItems);
        if (equipped == null)
        {
            Svc.Log.Error("InventoryContainer was null");
            return false;
        }

        if (!equipped->IsLoaded)
        {
            Svc.Log.Error($"InventoryContainer is not loaded");
            return false;
        }

        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item == null)
                continue;

            var itemCondition = Convert.ToInt32(Convert.ToDouble(item->Condition) / 30000.0 * 100.0);

            if (itemCondition <= below)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if equipped gear is under certain threshold.
    /// </summary>
    /// <returns>True if gear is met at threshold, otherwise false</returns>
    internal static unsafe bool NeedsExtract()
    {
        var inventory = InventoryManager.Instance();
        if (inventory == null)
        {
            Svc.Log.Error("InventoryManager was null");
            return false;
        }

        var equipped = inventory->GetInventoryContainer(InventoryType.EquippedItems);
        if (equipped == null)
        {
            Svc.Log.Error("InventoryContainer was null");
            return false;
        }

        if (!equipped->IsLoaded)
        {
            Svc.Log.Error($"InventoryContainer is not loaded");
            return false;
        }

        for (var index = 0; index < equipped->Size; index++)
        {
            var item = equipped->GetInventorySlot(index);
            if (item == null)
                continue;

            var spiritBond = (item->SpiritbondOrCollectability);

            if (spiritBond == 10000)
            {
                return true;
            }
        }
        return false;
    }
}