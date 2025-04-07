using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Helpers;

internal unsafe class InventoryHelper
{
    internal static uint GetFreeInventorySlots() => InventoryManager.Instance()->GetEmptySlotsInBag();
    internal static int GetHqItemCount(uint itemID) => InventoryManager.Instance()->GetInventoryItemCount(itemID, true);
    internal static int GetItemCount(uint itemID, bool includeHQ = true)
       => includeHQ ? InventoryManager.Instance()->GetInventoryItemCount(itemID, true) + InventoryManager.Instance()->GetInventoryItemCount(itemID) + InventoryManager.Instance()->GetInventoryItemCount(itemID + 500_000)
       : InventoryManager.Instance()->GetInventoryItemCount(itemID) + InventoryManager.Instance()->GetInventoryItemCount(itemID + 500_000);

    internal static int GetFishItemCount(uint itemId) => InventoryManager.Instance()->GetInventoryItemCount(itemId + 500_000);

    internal static bool HasDarkMatter() => GetItemCount(33916) > 0;

    internal static bool HasFishingBait() => GetItemCount(29717) > 0;

    internal static int GetGil() => (int)InventoryManager.Instance()->GetGil();

    internal static bool CanTurnIn()
    {
        if (C.SetTurnInConditions)
        {
            if (GetFreeInventorySlots() <= C.FreeRemainingSlots && GetFishItemCount(C.SelectedFish.FishId) > C.MinimumFishToTurnin)
                return true;
        }
        else
        {
            if (GetFreeInventorySlots() <= 10 && GetFishItemCount(C.SelectedFish.FishId) > 0)
                return true;
        }
        return false;
    }

    internal static bool HasInvetorySpace()
    {
        if (C.SetTurnInConditions)
        {
            if (GetFreeInventorySlots() == 0 && GetFishItemCount(C.SelectedFish.FishId) < C.MinimumFishToTurnin)
                return false;
        }
        else
        {
            if (GetFreeInventorySlots() == 0 && GetFishItemCount(C.SelectedFish.FishId) == 0)
                return false;
        }
        return true;
    }
}
