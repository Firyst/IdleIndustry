using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Leguar.TotalJSON;


public class MainDataHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        var file = new CSVFile("Data/items");

        CreateDebugDB();
    }

    private void CreateDebugDB()
    {
        /* NON-PRODUCTION 
         * Creates a example json db using existing structure.
         * 
         */
        Debug.Log("Creating debug game database...");
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
    }
}

public class Test
{
    public List<Recipe> rec;
}

public class Building
{
    // global bulding id
    public string id;


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
    private Dictionary<string, Item> items;

    public MyDatabase(Dictionary<string, Item> items)
    {
        this.items = items;
    }

    public MyDatabase()
    {
        this.items = new();
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