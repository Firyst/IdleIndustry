using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class IslandScript : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    private Dictionary<Vector2, GameObject> tiles = new();
    private List<GameObject> decorations = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
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
            GenerateRandom();
        }
    }

    public bool isTile(int x, int y)
    {
        return tiles.ContainsKey(new Vector2(x, y));
    }

    public bool isTile(float x, float y)
    {
        return tiles.ContainsKey(new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y)));
    }

    public void GenerateRandom(int size=50)
    {
        GameObject putTile(Vector2 pos)
        {
            var newTile = Instantiate(tilePrefab, this.transform);
            newTile.transform.localPosition = pos * 1.28f;
            tiles.Add(pos, newTile.gameObject);
            if (Mathf.RoundToInt(pos.x + pos.y) % 2 == 0)
            {
                newTile.GetComponent<TileClass>().setSprite("base1");
            } else
            {
                newTile.GetComponent<TileClass>().setSprite("base2");
            }
            return newTile;
        }

        void DFSGen(int x, int y, int dist)
        {
            // await Task.Delay(20);
            // check if a tile is already here
            if (isTile(x, y))
            {
                return;
            }

            float chance = Mathf.Clamp(size - dist * 2, -100, 80) + Mathf.Pow( 
                (isTile(x + 1, y) ? 3.34f : 0) + 
                (isTile(x, y + 1) ? 3.34f : 0) + 
                (isTile(x - 1, y) ? 3.34f : 0) + 
                (isTile(x, y - 1) ? 3.34f : 0), 2);

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

        // decorate shores

        void spawnDecor(float x, float y, int rotation, string spriteName, float z=0)
        {
            var tile = Instantiate(tilePrefab, this.transform);
            tile.transform.localPosition = new Vector3(x, y, z) * 1.28f;
            tile.GetComponent<TileClass>().buildable = false;
            tile.GetComponent<TileClass>().setSprite(spriteName);
            tile.transform.eulerAngles = new Vector3(0, 0, 90 * rotation);
            decorations.Add(tile);
        }
        
        foreach (var tilePair in tiles)
        {
            // hardcoded if so some sprites could be changed in case
            var tilePos = tilePair.Key;

            bool f(int x, int y)
            {
                return !isTile(tilePos.x + x, tilePos.y + y);
            }

            // straight lines
            if (f(1, 0)) {
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
}
