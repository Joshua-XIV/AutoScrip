using ECommons.DalamudServices;

namespace AutoScrip.Helpers;

internal static class AutoRetainerHelper
{
    internal static int ToUnixTimestamp(this DateTime value) => (int)Math.Truncate(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    private static unsafe ParallelQuery<ulong> GetAllEnabledCharacters() => Plugin.autoRetainerApi.GetRegisteredCharacters().AsParallel().Where(c => Plugin.autoRetainerApi.GetOfflineCharacterData(c).Enabled);
    internal static unsafe bool ARRetainersWaitingToBeProcessed(bool allCharacters = false)
    {
        return !allCharacters
            ? Plugin.autoRetainerApi.GetOfflineCharacterData(Svc.ClientState.LocalContentId).RetainerData.AsParallel().Any(x => x.HasVenture && x.VentureEndsAt <= DateTime.Now.ToUnixTimestamp())
            : GetAllEnabledCharacters().Any(character => Plugin.autoRetainerApi.GetOfflineCharacterData(character).RetainerData.Any(x => x.HasVenture && x.VentureEndsAt <= DateTime.Now.ToUnixTimestamp()));
    }
}
