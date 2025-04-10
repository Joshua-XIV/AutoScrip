using AutoScrip.Scheduler;
using Dalamud.Interface.Colors;
using ECommons.Automation;
using ECommons.ImGuiMethods;
using ECommons.Reflection;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace AutoScrip.Helpers;

internal class Generic
{
    /// <summary>
    /// Sends a warning message through all logging locations, including in-game chat, Dalamud console log, and the plugin's debugging UI.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    internal static void AllWarningEnqueue(string message)
    {
        Plugin.taskManager.Enqueue(() => PluginLog.Warning(message));
        Plugin.taskManager.Enqueue(() => Notify.Warning(message));
        Plugin.taskManager.Enqueue(() => DuoLog.Warning(message));
        Plugin.taskManager.Enqueue(() => PluginLog.Debug(message));
    }

    internal static void AllWarningInstant(string message)
    {
        PluginLog.Warning(message);
        Notify.Warning(message);
        DuoLog.Warning(message);
        PluginLog.Debug(message);
    }

    /// <summary>
    /// Logs an informational message through Dalamud console log and the plugin's debugging log.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    internal static void PluginLogInfoEnqueue(string message)
    {
        Plugin.taskManager.Enqueue(() => PluginLog.Information(message));
        if (C.AdditionalLogging)
            Plugin.taskManager.Enqueue(() => PluginLog.Debug(message));
    }

    internal static void PluginLogInfoInstant(string message)
    {
        PluginLog.Information(message);
        PluginLog.Debug(message);
    }

    /// <summary>
    /// Logs a debug message through the plugin's debugging log.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    internal static void PluginDebugInfoEnqueue(string message)
    {
        Plugin.taskManager.Enqueue(() => PluginLog.Debug(message));
    }

    internal static void PluginDebugInfoInstant(string message)
    {
        PluginLog.Debug(message);
    }

    /// <summary>
    /// Logs a error message through the dalamud's log and in-game chat.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    internal static void PluginLogErrorEnqueue(string message)
    {
        Plugin.taskManager.Enqueue(() => DuoLog.Error(message));
    }

    internal static void PluginLogErrorInstant(string message)
    {
        DuoLog.Error(message);
    }

    /// <summary>
    /// Checks if a specific plugin is installed in Dalamud.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to check.</param>
    /// <returns>True if the plugin is installed, otherwise false.</returns>
    internal static bool IsPluginInstalled(string pluginName)
    {
        return DalamudReflector.TryGetDalamudPlugin(pluginName, out _, false, true);
    }

    /// <summary>
    /// Displays colored text in ImGui based on whether a plugin is installed.
    /// </summary>
    /// <param name="PluginInstalled">Whether the plugin is installed.</param>
    /// <param name="text">The text to display.</param>
    internal static void IsPluginInstalledColorText(bool PluginInstalled, string text)
    {
        if (PluginInstalled)
            ImGui.TextColored(ImGuiColors.HealerGreen, $"- {text}");
        else
            ImGui.TextColored(ImGuiColors.DalamudRed, $"- {text}");
    }

    /// <summary>
    /// Displays a checkmark or cross in ImGui based on a boolean condition.
    /// </summary>
    /// <param name="enabled">The condition to evaluate.</param>
    internal static void CheckMarkTip(bool enabled)
    {
        if (!enabled)
        {
            FontAwesome.Print(ImGuiColors.DalamudRed, FontAwesome.Cross);
        }
        else if (enabled)
        {
            FontAwesome.Print(ImGuiColors.HealerGreen, FontAwesome.Check);
        }
    }

    /// <summary>
    /// Displays a checkmark or cross in ImGui based on whether a plugin is installed, along with a tooltip and click-to-copy functionality for a URL.
    /// </summary>
    /// <param name="PluginInstalled">Whether the plugin is installed.</param>
    /// <param name="Text">The text to display in the tooltip.</param>
    /// <param name="Url">The URL to copy when clicked.</param>
    internal static void CheckMarkTipString(string pluginName, string pluginRepo)
    {
        var installed = IsPluginInstalled(pluginName);
        IsPluginInstalledColorText(installed, pluginName);
        ImGui.SameLine();
        CheckMarkTip(installed);
        if (ImGui.IsItemHovered() && !installed)
        {
            ImGui.BeginTooltip();
            ImGui.Text("Click to Copy Repo");
            ImGui.EndTooltip();
            if (ImGui.IsItemClicked())
            {
                ImGui.SetClipboardText(pluginRepo);
                Notify.Info("Repo URL Copied");
            }
        }
    }

    internal unsafe static void SelectYes()
    {
        if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var addon) && IsAddonReady(addon))
        {
            Callback.Fire(addon, true, 0);
        }
    }
}
