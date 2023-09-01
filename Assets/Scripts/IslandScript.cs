using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Leguar.TotalJSON;

public class IslandScript : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    private Dictionary<Vector2, GameObject> tiles = new();
    private List<GameObject> decorations = new();

    [SerializeField] private TextMeshProUGUI debugSaveText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            ClearIsland();
            GenerateRandom();
        }
    }

    /// <summary>
    /// Clear all island tiles from map.
    /// </summary>
    public void ClearIsland()
    {
        foreach (var pair in tiles)
        {
            Destroy(pair.Value);
        }

        foreach (var decor in decorations)
        {
            Destroy(decor);
        }

        tiles.Clear();
        decorations.Clear();
    }

    /// <summary>
    /// Check if tile is put on some location.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool isTile(int x, int y)
    {
        return tiles.ContainsKey(new Vector2(x, y));
    }

    public bool isTile(float x, float y)
    {
        return tiles.ContainsKey(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)));
    }

    /// <summary>
    /// Put a tile on island. Used for generation/loading.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    GameObject putTile(Vector2 pos)
    {
        var newTile = Instantiate(tilePrefab, this.transform);
        newTile.transform.localPosition = pos * 1.28f;
        tiles.Add(pos, newTile.gameObject);
        if (Mathf.RoundToInt(pos.x + pos.y) % 2 == 0)
        {
            newTile.GetComponent<TileClass>().setSprite("base1");
        }
        else
        {
            newTile.GetComponent<TileClass>().setSprite("base2");
        }
        newTile.GetComponent<TileClass>().gridPos = pos;
        return newTile;
    }

    /// <summary>
    /// Spawns decoration for island.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="rotation">Rotation: int 0-3 </param>
    /// <param name="spriteName">A name of sprite to load</param>
    /// <param name="z">Layer</param>
    void spawnDecor(float x, float y, int rotation, string spriteName, float z = 0)
    {
        var tile = Instantiate(tilePrefab, this.transform);
        tile.transform.localPosition = new Vector3(x, y, z) * 1.28f;
        tile.GetComponent<TileClass>().buildable = false;
        tile.GetComponent<TileClass>().setSprite(spriteName);
        tile.transform.eulerAngles = new Vector3(0, 0, 90 * rotation);
        decorations.Add(tile);
    }

    /// <summary>
    /// Basic island generation
    /// </summary>
    /// <param name="size">Default 50. Defines island size</param>
    public void GenerateRandom(int size=50)
    {


        void DFSGen(int x, int y, int dist)
        {
            // await Task.Delay(20);
            // check if a tile is already here
            if (isTile(x, y))
            {
                return;
            }

            int neighborTiles = (isTile(x + 1, y) ? 1 : 0) + (isTile(x, y + 1) ? 1 : 0) + (isTile(x - 1, y) ? 1 : 0) + (isTile(x, y - 1) ? 1 : 0);

            float chance = Mathf.Clamp(size - dist * 2, -100, 80) + ((neighborTiles > 2) ? 1000 : neighborTiles*50);

            if (Random.Range(0, 100) <= chance)
            {
                putTile(new Vector2(x, y));

                DFSGen(x + 1, y, dist + 1);
                DFSGen(x, y + 1, dist + 1);
                DFSGen(x - 1, y, dist + 1);
                DFSGen(x, y - 1, dist + 1);
            }
           
        }

        // put first tile
        putTile(Vector2.zero);

        // start generating new tiles in all directions
        DFSGen(1, 0, 0);
        DFSGen(0, 1, 0);
        DFSGen(-1, 0, 0);
        DFSGen(0, -1, 0);


        decorateIsland();
    }

    /// <summary>
    /// Decorates the island. Does not affect the save.
    /// </summary>
    public void decorateIsland()
    {
        // decorate shores
        foreach (var tilePair in tiles)
        {
            // hardcoded if so some sprites could be changed in case
            var tilePos = tilePair.Key;

            bool f(int x, int y)
            {
                return !isTile(tilePos.x + x, tilePos.y + y);
            }

            // straight lines
            if (f(1, 0))
            {
                spawnDecor(tilePos.x + 1, tilePos.y, 0, "beach-straight");
            }

            if (f(0, 1))
            {
                spawnDecor(tilePos.x, tilePos.y + 1, 1, "beach-straight");
            }

            if (f(-1, 0))
            {
                spawnDecor(tilePos.x - 1, tilePos.y, 2, "beach-straight");
            }

            if (f(0, -1))
            {
                spawnDecor(tilePos.x, tilePos.y - 1, 3, "beach-straight");
            }

            // outer corners
            if (f(1, 0) && f(1, 1) && (f(0, 1)))
            {
                // top-right
                spawnDecor(tilePos.x + 1, tilePos.y + 1, 0, "beach-corner");
            }

            if (f(1, 0) && f(1, -1) && (f(0, -1)))
            {
                // bottom-right
                spawnDecor(tilePos.x + 1, tilePos.y - 1, 3, "beach-corner");
            }

            if (f(-1, 0) && f(-1, 1) && (f(0, 1)))
            {
                // top-left
                spawnDecor(tilePos.x - 1, tilePos.y + 1, 1, "beach-corner");
            }

            if (f(-1, 0) && f(-1, -1) && (f(0, -1)))
            {
                // bottom-left
                spawnDecor(tilePos.x - 1, tilePos.y - 1, 2, "beach-corner");
            }



            // inner corners
            if (!f(1, 0) && f(1, 1) && (!f(0, 1)))
            {
                // top-right
                spawnDecor(tilePos.x + 1, tilePos.y + 1, 1, "beach-corner-inner", -0.01f);
            }

            if (!f(1, 0) && f(1, -1) && (!f(0, -1)))
            {
                // bottom-right
                spawnDecor(tilePos.x + 1, tilePos.y - 1, 0, "beach-corner-inner", -0.01f);
            }

            if (!f(-1, 0) && f(-1, 1) && (!f(0, 1)))
            {
                // top-left
                spawnDecor(tilePos.x - 1, tilePos.y + 1, 2, "beach-corner-inner", -0.01f);
            }

            if (!f(-1, 0) && f(-1, -1) && (!f(0, -1)))
            {
                // bottom-left
                spawnDecor(tilePos.x - 1, tilePos.y - 1, 3, "beach-corner-inner", -0.01f);
            }
        }
    }

    /// <summary>
    /// Save entire island data to JSON file.
    /// </summary>
    public void SaveIsland()
    {
        Debug.Log("Saving island as " + debugSaveText.text);

        IslandData data = new();
        data.islandName = debugSaveText.text;
        data.tiles = new();

        foreach (KeyValuePair<Vector2, GameObject> kvp in tiles)
        {
            // turn vector2 into a string to store.
            string newKey = Mathf.RoundToInt(kvp.Key.x).ToString() + ";" + Mathf.RoundToInt(kvp.Key.y).ToString();
            data.tiles.Add(newKey, kvp.Value.GetComponent<TileClass>().getSaveData());

            Debug.Log(JSON.Serialize(kvp.Value.GetComponent<TileClass>()).CreatePrettyString());
        }

        UtilityScript.SaveTextToFile("Island" + debugSaveText.text + ".json", (JSON.Serialize(data).CreatePrettyString()));
    }

    /// <summary>
    /// Load island from save.
    /// </summary>
    public void LoadIsland()
    {

        string islandName = debugSaveText.text;
        var file = Resources.Load<TextAsset>("Data/Island" + islandName);
        Debug.Log("Data/Island" + islandName + ".json");
        // check file exists
        if (file == null)
        {
            Debug.LogWarning("Cannot load island (no file): " + islandName);
            return;
        }


        // clear everyting
        ClearIsland();

        var islandData = JSON.ParseString(file.text).Deserialize<IslandData>();
        foreach (var kvp in islandData.tiles)
        {
            putTile(kvp.Value.pos);
        }

        // run decoration script
        decorateIsland();
    }
}
