using System.Numerics;

namespace AutoScrip.Data;

public class FishingSpots
{
    public List<Vector3> Waypoints { get; set; }
    public Vector3 PointToFace { get; set; }
}

public class FishEntry
{
    public string FishName { get; set; }
    public uint FishId { get; set; }
    public string BaitName { get; set; }
    public uint ZoneId { get; set; }
    public string ZoneName { get; set; }
    public Vector3 RepairLocation { get; set; }
    public uint MenderGameObjectId { get; set; }
    public uint MerchantGameObjectId { get; set; }
    public string AutoHookPreset { get; set; }
    public FishingSpots FishingSpots { get; set; }
    public ScripColor ScripColor { get; set; }
    public int MaxScripTurnIn { get; set; }
    public uint GathererScripId { get; set; }
    public uint ScripTurnInId { get; set; }
    public int CollectiblesTurnInListIndex { get; set; }
}

public class FishTable
{
    public static readonly List<FishEntry> Table = new()
    {
        new FishEntry
        {
            FishName = "Zorgor Condor",
            FishId = 43761,
            BaitName = "Versatile Lure",
            ZoneId = 1190,
            ZoneName = "Shaaloani",
            RepairLocation = new Vector3(364.246f, -1.587f, 469.486f),
            MenderGameObjectId = 10389529,
            MerchantGameObjectId = 10389528,
            AutoHookPreset = AutoHookPresets.ZorgorCondorPreset,
            FishingSpots = new FishingSpots
            {
                Waypoints = new List<Vector3>
                {
                    new Vector3(-69.3567f, -24.8879f, 751.5612f),
                    new Vector3(59.27f, -2.0f, 735.09f),
                    new Vector3(133.546f, 5.468f, 720.258f),
                    new Vector3(207.238f, 12.437f, 753.353f)
                },
                PointToFace = new Vector3(8.076f, 6.07f, 1000f)
            },
            MaxScripTurnIn = 99,
            ScripColor = ScripColor.Orange,
            GathererScripId = 41785,
            ScripTurnInId = 39,
            CollectiblesTurnInListIndex = 6
        },
        new FishEntry
        {
            FishName = "Fleeting Brand",
            FishId = 36473,
            BaitName = "Versatile Lure",
            ZoneId = 959,
            ZoneName = "Mare Lamentorum",
            RepairLocation = new Vector3(-19.92f, -132.946f, -459.044f),
            MenderGameObjectId = 8865510,
            MerchantGameObjectId = 8865509,
            AutoHookPreset = AutoHookPresets.FleetingBrandPreset,
            FishingSpots = new FishingSpots
            {
                Waypoints = new List<Vector3>
                {
                    new Vector3(10.05f, 26.89f, 448.99f),
                    new Vector3(37.71f, 22.36f, 481.05f),
                    new Vector3(58.87f, 22.22f, 487.95f),
                    new Vector3(71.79f, 22.39f, 477.65f)
                },
                PointToFace = new Vector3(37.71f, 22.36f, 1000f)
            },
            MaxScripTurnIn = 78,
            ScripColor = ScripColor.Purple,
            GathererScripId = 33914,
            ScripTurnInId = 38,
            CollectiblesTurnInListIndex = 28
        }
    };
}