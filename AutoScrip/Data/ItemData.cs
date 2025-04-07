namespace AutoScrip.Data;

public class ScripItem
{
    public string ItemName { get; set; }
    public ScripColor ScripColor { get; set; }
    public int CategoryMenu { get; set; }
    public int SubcategoryMenu { get; set; }
    public int ListIndex { get; set; }
    public int Price { get; set; }

    public ScripItem(string itemName, ScripColor scripColor, int categoryMenu, int subcategoryMenu, int listIndex, int price)
    {
        ItemName = itemName;
        ScripColor = scripColor;
        CategoryMenu = categoryMenu;
        SubcategoryMenu = subcategoryMenu;
        ListIndex = listIndex;
        Price = price;
    }
}

public static class ScripExchangeItems
{
    public static readonly List<ScripItem> Items = new()
    {
        new ScripItem("Mount Token", ScripColor.Orange, 4, 8, 6, 1000),
        new ScripItem("Hi-Cordial", ScripColor.Purple, 4, 1, 0, 20),
        new ScripItem("Gatherer's Guerdon Materia XI (Gathering +20)", ScripColor.Purple, 5, 1, 0, 250),
        new ScripItem("Gatherer's Guile Materia XI (Perception +20)", ScripColor.Purple, 5, 1, 1, 250),
        new ScripItem("Gatherer's Grasp Materia XI (GP +9)", ScripColor.Purple, 5, 1, 2, 250),
        new ScripItem("Gatherer's Guerdon Materia XII (Gathering +36)", ScripColor.Orange, 5, 2, 0, 500),
        new ScripItem("Gatherer's Guile Materia XII (Perception +36)", ScripColor.Orange, 5, 2, 1, 500),
        new ScripItem("Gatherer's Grasp Materia XII (GP +11)", ScripColor.Orange, 5, 2, 2, 500)
    };
}
