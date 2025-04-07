using System.Numerics;

namespace AutoScrip.Data;

public class AethernetInfo
{
    public uint AethernetZoneId { get; set; }
    public string AethernetName { get; set; } = string.Empty;
    public uint AethernetIndex { get; set; }
    public uint AethernetGameObjectId { get; set; }
}

public class ServiceLocation
{
    public Vector3 Position { get; set; }
    public Vector3 Position2 { get; set; }
    public uint ScripExchangeGameObjectId { get; set; }
    public uint CollectableAppraiserGameObjectId { get; set; }
}

public class HubCity
{
    public City ZoneName { get; set; }
    public uint ZoneId { get; set; }
    public AethernetInfo Aethernet { get; set; }
    public ServiceLocation RetainerBell { get; set; }
    public ServiceLocation ScripExchange { get; set; }
}

public static class HubCities
{
    public static List<HubCity> Cities = new List<HubCity>
    {
        new HubCity
        {
            ZoneName = City.Limsa,
            ZoneId = 129,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 129,
                AethernetName = "Hawkers' Alley",
                AethernetIndex = 6,
                AethernetGameObjectId = 4203159
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(-123.88806f, 17.990356f, 21.469421f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(-258.52585f, 16.2f, 40.65883f),
                ScripExchangeGameObjectId = 6938730,
                CollectableAppraiserGameObjectId = 6938729
            }
        },
        new HubCity
        {
            ZoneName = City.Gridania,
            ZoneId = 132,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 133,
                AethernetName = "Leatherworkers' Guild & Shaded Bower",
                AethernetIndex = 2,
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(168.72f, 15.5f, -100.06f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(142.15f, 13.74f, -105.39f),
                ScripExchangeGameObjectId = 6938744,
                CollectableAppraiserGameObjectId = 6938743
            }
        },
        new HubCity
        {
            ZoneName = City.Uldah,
            ZoneId = 130,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 131,
                AethernetName = "Sapphire Avenue Exchange",
                AethernetIndex = 9,
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(148f, 3f, -45f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(148.39f, 3.99f, -18.4f),
                ScripExchangeGameObjectId = 6938727,
                CollectableAppraiserGameObjectId = 6938726
            }
        },
        new HubCity
        {
            ZoneName = City.Idyllshire,
            ZoneId = 478,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 478,
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(0.5379f, 206.4994f, 51.4454f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(-17.803f, 206.4994f, 47.8774f),
                ScripExchangeGameObjectId = 5881196,
                CollectableAppraiserGameObjectId = 5881195
            }
        },
        new HubCity
        {
            ZoneName = City.RhalgrsReach,
            ZoneId = 635,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 635,
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(-57.7682f, 0.01f, 49.43f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(-64.725f, 0.01f, 64.527f),
                ScripExchangeGameObjectId = 6708423,
                CollectableAppraiserGameObjectId = 6708422
            }
        },
        new HubCity
        {
            ZoneName = City.Eulmore,
            ZoneId = 820,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 820,
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(7.592f, 82.05f, 30.863f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(17f, 82.05f, -19.1138f),
                ScripExchangeGameObjectId = 7796087,
                CollectableAppraiserGameObjectId = 7796088
            }
        },
        new HubCity
        {
            ZoneName = City.RadzAtHan,
            ZoneId = 963,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 963,
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(26.7718f, -2.5034f, -53.7293f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(24.5028f, 0.92f, -68.877f),
                Position2 = new Vector3(17.987f, 0.92f, -87.092f),
                ScripExchangeGameObjectId = 8861059,
                CollectableAppraiserGameObjectId = 8861058
            }
        },
        new HubCity
        {
            ZoneName = City.SolutionNine,
            ZoneId = 1186,
            Aethernet = new AethernetInfo
            {
                AethernetZoneId = 1186,
                AethernetName = "Nexus Arcade",
                AethernetIndex = 6,
                AethernetGameObjectId = 10653774
            },
            RetainerBell = new ServiceLocation
            {
                Position = new Vector3(-152.465f, 0.660f, -13.557f),
            },
            ScripExchange = new ServiceLocation
            {
                Position = new Vector3(-158.019f, 0.922f, -37.884f),
                ScripExchangeGameObjectId = 10361836,
                CollectableAppraiserGameObjectId = 10361834
            }
        },
    };
}