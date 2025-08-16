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
    const int MATERIALS_CATEGORY_INDEX = 4;
    const int MATERIALS_MISC_SUBCATEGORY_INDEX = 1;
    const int MATERIALS_50_SUBCATEGORY_INDEX = 2;
    const int MATERIALS_60_SUBCATEGORY_INDEX = 3;
    const int MATERIALS_70_SUBCATEGORY_INDEX = 4;
    const int MATERIALS_80_SUBCATEGORY_INDEX = 5;
    const int MATERIALS_90_SUBCATEGORY_INDEX = 6;
    const int MATERIALS_100_FOLKLORE_SUBCATEGORY_INDEX = 7;
    const int MATERIALS_100_SUBCATEGORY_INDEX = 8;
    const int MOUNT_TOKEN_PRICE = 1000;
    const int HI_CORDIAL_PRICE = 20;

    const int MATERIA_CATEGORY_INDEX = 5;
    const int MATERIA_SUBCATEGORY_PURPLE_INDEX = 1;
    const int MATERIA_SUBCATEGORY_ORANGE_INDEX = 2;
    const int MATERIA_12_PRICE = 500;
    const int MATERIA_11_10_PRICE = 250;

    public static readonly List<ScripItem> Items = new()
    {
        new ScripItem("Mount Token", ScripColor.Orange, MATERIALS_CATEGORY_INDEX, MATERIALS_100_SUBCATEGORY_INDEX, 7, MOUNT_TOKEN_PRICE),
        new ScripItem("Hi-Cordial", ScripColor.Purple, MATERIALS_CATEGORY_INDEX, MATERIALS_MISC_SUBCATEGORY_INDEX, 0, HI_CORDIAL_PRICE),
        new ScripItem("Gatherer's Guerdon Materia XI (Gathering +20)", ScripColor.Purple, MATERIA_CATEGORY_INDEX, MATERIA_SUBCATEGORY_PURPLE_INDEX, 0, MATERIA_11_10_PRICE),
        new ScripItem("Gatherer's Guile Materia XI (Perception +20)", ScripColor.Purple, MATERIA_CATEGORY_INDEX, MATERIA_SUBCATEGORY_PURPLE_INDEX, 1, MATERIA_11_10_PRICE),
        new ScripItem("Gatherer's Grasp Materia XI (GP +9)", ScripColor.Purple, MATERIA_CATEGORY_INDEX, MATERIA_SUBCATEGORY_PURPLE_INDEX, 2, MATERIA_11_10_PRICE),
        new ScripItem("Gatherer's Guerdon Materia XII (Gathering +36)", ScripColor.Orange, MATERIA_CATEGORY_INDEX, MATERIA_SUBCATEGORY_ORANGE_INDEX, 0, MATERIA_12_PRICE),
        new ScripItem("Gatherer's Guile Materia XII (Perception +36)", ScripColor.Orange, MATERIA_CATEGORY_INDEX, MATERIA_SUBCATEGORY_ORANGE_INDEX, 1, MATERIA_12_PRICE),
        new ScripItem("Gatherer's Grasp Materia XII (GP +11)", ScripColor.Orange, MATERIA_CATEGORY_INDEX, MATERIA_SUBCATEGORY_ORANGE_INDEX, 2, MATERIA_12_PRICE)
    };
}
