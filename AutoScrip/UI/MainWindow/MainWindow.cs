﻿using AutoScrip.UI.DisclaimerWindow;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using System.Numerics;
using ImGuiNET;
using Dalamud.Interface.Textures.TextureWraps;

namespace AutoScrip.UI.MainWindow;

class MainWindow : Window
{
    private static IDalamudTextureWrap? orangeScripImage = Plugin.TextureProvider.GetFromFile(Plugin.orangeImagePath).GetWrapOrDefault();
    private static IDalamudTextureWrap? purpleScripImage = Plugin.TextureProvider.GetFromFile(Plugin.orangeImagePath).GetWrapOrDefault();
    public MainWindow() : base($"AutoScrip ###AutoScripMainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(400, 300),
            MaximumSize = new(9999, 9999)
        };
        Plugin.windowSystem.AddWindow(this);
    }

    public override void Draw()
    {
        if (!C.AcceptedDisclaimer)
        {
            Disclaimer.Draw();
        }
        else
        {
            if (C.DEBUG)
            {
                ImGuiEx.EzTabBar
                    ("Tab Bar",
                    ("Main", MainTab.Draw, null, true),
                    ("Config", ConfigTab.Draw, null, true),
                    ("Debug", DebugTab.Draw, null, true),
                    ("Logs", LogTab.Draw, null, true));
            }
            else
            {
                ImGuiEx.EzTabBar
                    ("Tab Bar",
                    ("Main", MainTab.Draw, null, true),
                    ("Config", ConfigTab.Draw, null, true));
            }
        }
    }
}
