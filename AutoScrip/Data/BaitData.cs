using System.Numerics;

namespace AutoScrip.Data;

public static class BaitData
{
    public static City ZoneName { get; set; } = City.Limsa;
    public static uint ZoneId { get; set; } = 129;
    public static AethernetInfo Aethernet { get; set; } = new AethernetInfo
        {
            AethernetZoneId = 129,
            AethernetName = "Arcanists' Guild",
            AethernetIndex = 3,
            AethernetGameObjectId = 4170363
        };
    public static Vector3 VendorLocation { get; set; } = new Vector3(-398.8193f, 3.0999959f, 78.34324f);
    public static uint VendorObjectId { get; set; } = 8267821;
}
