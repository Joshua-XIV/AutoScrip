using ECommons.DalamudServices;
using ECommons.UIHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
namespace AutoScrip.Helpers;

internal static class ZonesHelper
{
    internal static uint GetZoneId() => Svc.ClientState.TerritoryType;
    internal static bool IsInZone(uint zoneID) => Svc.ClientState.TerritoryType == zoneID;

    internal static string GetZoneName(uint zoneID) => GetRow<TerritoryType>(zoneID)!.Value.PlaceName.Value.Name.ToString();

    internal static uint GetAetheryteId(uint zoneID)
    {
        var aethertyes = Svc.Data.GetExcelSheet<Aetheryte>();
        uint aetheryteId = 0;
        foreach (var data in aethertyes)
        {
            if (!data.IsAetheryte) continue;
            if (data.Territory.ValueNullable == null) continue;
            if (data.PlaceName.ValueNullable == null) continue;
            if (data.Territory.Value.RowId == zoneID)
            {
                return data.RowId;
            }
        }
        return 0;
    }
}
