using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Leguar.TotalJSON;
using System.IO;



public class MainDataHandler : MonoBehaviour
{
    public MyDatabase myDB;

    [SerializeField] private UIBuilder UiBuilder;
    // Start is called before the first frame update
    void Start()
    {

        var file = new CSVFile("Data/items");

        CreateDebugDB();

        LoadDB();
    }

    private void LoadDB()
    {
        myDB = new();

        // load all buildings
        Debug.Log("Loading buildings...");
        myDB.buildings = JSON.ParseString(Resources.Load<TextAsset>("Data/buildings").text).Deserialize<Dictionary<string, Building>>();
        Debug.Log(string.Format("<color=green>Successfully loaded {0} building entries.</color>", myDB.buildings.Count));

        UiBuilder.Init();

    }

    private void CreateDebugDB()
    {
        /* NON-PRODUCTION 
         * Creates a example json db using existing structure.
         * 
         */

        Debug.Log("Creating debug game database...");

        /*
        var db = new MyDatabase(true);

        Recipe test2 = new();
        test2.id = "recipe.steel";
        test2.TechIdRequired = "tech.metallurgy";
        test2.inputItems = new();
        test2.inputItems.Add("item.iron_ore", 2);
        test2.inputItems.Add("item.coal", 1);
        test2.outputItems = new();
        test2.outputItems.Add("item.steel", 1);
        test2.baseSpeed = 1.0f;

        var test3 = test2;
        test3.id = "recipe.steel2";

        List<Recipe> recipes = new List<Recipe> { test2, test3 };

        var test4 = new Test();
        test4.rec = recipes;

        var json = (JSON.Serialize(test2).CreatePrettyString());
        print(json);



        var testRecipe = JSON.ParseString(json).Deserialize<Recipe>();
        print(JSON.Serialize(testRecipe).CreatePrettyString());
        print(JSON.Serialize(testRecipe).CreatePrettyString() == json);

        print(JSON.Serialize(test4).CreatePrettyString());
        */

        var db = new MyDatabase();

        Dictionary<string, Building> buildings = new();

        buildings.Add("test", new Building());

        var bld = new Building();


        var tier = new Building.BuildingTier();
        tier.TechIdRequired = "-1";
        tier.cost = new();
        tier.cost.Add("item.stone", 10);

        bld.tiers = new();
        bld.tiers.Add("tier.0", tier);

        tier.cost["item.stone"] = 25;
        bld.tiers.Add("tier.1", tier);

        buildings.Add("test2", bld);

        var json = (JSON.Serialize(buildings).CreatePrettyString());
        // print(json);

    }
}

public class Test
{
    public List<Recipe> rec;
}


/// <summary>
/// Used for saving and loading island data.
/// </summary>
public class IslandData
{
    public string islandName;
    public Dictionary<string, TileData> tiles;
}

/// <summary>
/// This class is used to describe buldlings on the map
/// </summary>
public class BuildingInstance
{
    // used id
    public string id;

    // current recipe
    public string selectedRecipe;

    // rate that changes in case of throttling
    public float externalRate;
}


/// <summary>
/// Describes a building blueprint. This is used just for storing data and should not be edited.
/// For building placed in-game BuildingInstance class is used.
/// </summary>
public class Building
{
    // global bulding id
    public string id;
    // some speecial data, like builing is a port
    public string specialData;

    // available recipes for current building.
    public List<string> recipes;

    // texture data
    [System.NonSerialized]
    public Texture2D texture;  // texture to be set
    [System.NonSerialized]
    public Texture2D texture2;  // secondary texture to be set (unnecessary)
    [SerializeField]
    private string textureName;  // texture name to be stored
    [SerializeField]
    private string textureName2;  // secondary texture name to be stored

    /// <summary>
    /// Represents a tier (level) of current building. It also describes the cost.
    /// </summary>
    public class BuildingTier
    {
        // id of the tier.
        public string tierId;

        // id of techonolgy that is required to unlock this tier
        public string TechIdRequired;

        // production rate
        public float rate;

        // upgrade cost: item id and amount
        public Dictionary<string, long> cost;
    }

    // tier data
    public Dictionary<string, BuildingTier> tiers;
}

public class Recipe
{
    // global recipe id
    public string id;

    // id of techonolgy that is required to unlock this recipe
    public string TechIdRequired;

    // item_id: amount
    public Dictionary<string, int> inputItems;
    public Dictionary<string, int> outputItems;

    // speed multiplier for recipe
    public float baseSpeed;

}


/// <summary>
/// Describes an in-game item. May be used to access texture.
/// </summary>
public class Item
{
    public int id;
    public BigInteger amount;
    public long value;
    public Sprite itemTexture;

    public Item(int id, BigInteger amount, long value, string textureName)
    {
        this.id = id;
        this.amount = amount;
        this.value = value;
        itemTexture = UtilityScript.loadSprite(textureName);
    }

}

public class MyDatabase
{
    [SerializeField]
    public Dictionary<string, Item> items;
    [SerializeField]
    public Dictionary<string, Building> buildings;

    public MyDatabase(Dictionary<string, Item> items)
    {
        this.items = items;
    }

    public MyDatabase()
    {
        // empty init
        this.items = new();
        this.buildings = new();
    }

    public MyDatabase(bool auto)
    {
        items = new();

        var itemsFile = new CSVFile("Data/items");

        foreach (var KVPair in itemsFile.data)
        {
            if (KVPair.Key.Length == 0)
            {
                // empty string
                continue;
            }
            

            items.Add(KVPair.Key,
                new Item(
                    int.Parse(KVPair.Key),
                    BigInteger.Parse(KVPair.Value["StoredAmount"]),
                    long.Parse(KVPair.Value["Value"]),
                    KVPair.Value["SpriteName"]));
        }

        Debug.Log("Loaded " + items.Count.ToString() + " items.");
    }

    public Item getItemByID(string id)
    {
        if (items.ContainsKey(id))
        {
            return items[id];
        } else
        {
            Debug.LogError("Cannot find item with ID " + id + " in database!");
            return null;
        }
    }
}

public class CSVFile
{
    public Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
    public CSVFile(string filepath, char splitter=';')
    {
        var file = Resources.Load<TextAsset>(filepath);

        // check file exists
        if (file == null)
        {
            Debug.LogWarning("CSVFile: unable to load file " + filepath);
            return;
        }

        string[] lines = file.text.Split('\n');

        if (lines.Length < 2)
        {
            Debug.LogWarning("CSVFile: file " + filepath + " seems to be invalid, not enough data to parse.");
        }

        // parse first line as header
        string[] headers = lines[0].Split(splitter);

        if (headers.Length < 2)
        {
            Debug.LogWarning("CSVFile: file " + filepath + " seems to be invalid, not enough data to parse. (less than two cols)");
        }

        // go through all lines
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(splitter);

            // check if is a valid line
            if (values.Length == headers.Length)
            {
                Dictionary<string, string> rowData = new Dictionary<string, string>();

                if (data.ContainsKey(values[0]))
                {
                    continue;
                }

                // parse all data except the first one (the key)
                for (int j = 1; j < headers.Length; j++)
                {
                    string value = values[j];
                    if (value.Length > 1 && value[value.Length - 1] == '\r')
                    {
                        value = value.Substring(0, value.Length - 1);
                    }
                    rowData.Add(headers[j].Replace("\r", ""), value);
                }
                
                data.Add(values[0], rowData);
            }
        }
    }

    public int lines()
    {
        return data.Count;
    }

    public string formatLine(string key)
    {
        string output;
        if (data.ContainsKey(key))
        {
            output = "";
            foreach (var KVPair in data[key])
            {
                output = output + KVPair.Key + ": " + KVPair.Value + ", ";
            }
        } else
        {
            output = "Invalid line with key " + key;
        }

        return output;
    }
}

