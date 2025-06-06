﻿using ECommons;
using ECommons.UIHelpers;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoScrip.Helpers;

internal class AethernetHelper
{

    internal unsafe class ReaderTelepotTown(AtkUnitBase* UnitBase, int BeginOffset = 0) : AtkReader(UnitBase, BeginOffset)
    {
        internal uint NumEntries => ReadUInt(0) ?? 0;
        internal uint CurrentDestination => ReadUInt(1) ?? 0;
        internal List<Data> DestinationData => Loop<Data>(6, 4, 20);
        internal List<Names> DestinationName => Loop<Names>(262, 1, 20);

        internal unsafe class Names(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
        {
            internal string Name => ReadSeString(0).GetText();
        }

        internal unsafe class Data(nint UnitBasePtr, int BeginOffset = 0) : AtkReader(UnitBasePtr, BeginOffset)
        {
            internal uint Type => ReadUInt(0).Value;
            internal uint State => ReadUInt(1).Value;
            internal uint IconID => ReadUInt(2).Value;
            internal uint CallbackData => ReadUInt(3).Value;
        }
    }

    internal unsafe static uint GetAethernetCallback(string aethernetName)
    {
        if (TryGetAddonByName("TelepotTown", out AtkUnitBase* addon) && IsAddonReady(addon))
        {
            var readerTelepotTown = new ReaderTelepotTown(addon);
            for (int i = 0; i < readerTelepotTown.DestinationData.Count; i++)
            {
                if (aethernetName == readerTelepotTown.DestinationName[i].Name)
                    return readerTelepotTown.DestinationData[i].CallbackData;
            }
        }
        return 0;
    }
}
