using AutoScrip.Data;
using ECommons.Configuration;
using ECommons.DalamudServices;
using Newtonsoft.Json;
using System.Numerics;

namespace AutoScrip.Configuration;

public class Config : IEzConfig
{
    // Turn In Conditions
    public bool SetTurnInConditions { get; set; } = false;
    public int FreeRemainingSlots { get; set; } = 10;
    public int MinimumFishToTurnin { get; set; } = 0;

    // Fishing Conditions
    public bool SetExactTime { get; set; } = false;
    public int InitialTime { get; set; } = 15;
    public int FinalTime { get; set; } = 20;
    public bool BuyBait { get; set; } = false;
    public bool BuyMaxBait { get; set; } = false;
    public bool UseAndOperator { get; set; } = true;


    // Set Name Config
    public string FishSetName { get; set; } = "Fisher";

    // Cordial Config
    public bool UseCordial { get; set; } = false;
    public bool ReverseCordialPrio { get; set; } = false;

    // Extract Config
    public bool ExtractMateria { get; set; } = false;
    public bool ExtractDuringFishing { get; set; } = false;

    // Repair Config
    public bool RepairGear { get; set; } = false;
    public bool SelfRepair { get; set; } = false;
    public bool RepairDuringFishing { get; set; } = false;
    public int RepairThreshold { get; set; } = 0;
    public bool BuyDarkMatter { get; set; } = false;
    public bool BuyMaxDarkMatter { get; set; } = false;

    // GP Config
    public bool SetGPThreshold { get; set; } = false;
    public int GPThreshold { get; set; } = 500;
    public bool AboveThreshold { get; set; } = false;

    // Waypoints
    public bool SetCustomZorgorWaypoints { get; set; } = false;
    public bool SetCustomFleetingBrandWaypoints { get; set; } = false;
    public List<Vector3> CustomZorgorWaypoints { get; set; } = new List<Vector3>() { Vector3.Zero, Vector3.Zero };
    public List<Vector3> CustomFleetingBrandWaypoints { get; set; } = new List<Vector3>() { Vector3.Zero, Vector3.Zero };


    // Hub Config
    public HubCity SelectedCity = HubCities.Cities.FirstOrDefault(city => city.ZoneName == City.SolutionNine)!;

    // Scrip Color and Item Selection
    public ScripColor SelectedScripColor { get; set; } = ScripColor.Orange;
    public ScripItem OrangeScripItem { get; set; } = ScripExchangeItems.Items.FirstOrDefault(item => item.ScripColor == ScripColor.Orange)!;
    public ScripItem PurpleScripItem { get; set; } = ScripExchangeItems.Items.FirstOrDefault(item => item.ScripColor == ScripColor.Purple)!;
    public int SelectedOrangeItemIndex { get; set; } = 0;
    public int SelectedPurpleItemIndex { get; set; } = 0;
    public FishEntry SelectedFish { get; set; } = FishTable.Table.FirstOrDefault(fish => fish.ScripColor == ScripColor.Orange)!;

    // Additional Tabs/Windows
    public bool DEBUG { get; set; } = false;

    public bool AdditionalLogging { get; set; } = false;
    public bool AcceptedDisclaimer { get; set; } = false;

    public void Save()
    {
        EzConfig.Save();
    }
}
