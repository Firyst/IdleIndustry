using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Island : MonoBehaviour
    {
        [SerializeField] private Game.PlaneClass tilePrefab;
        private Dictionary<Vector2, Game.PlaneClass> tiles = new();

        void Update()
        {
            if (Input.GetKeyDown("q"))
            {
                foreach (var pair in tiles)
                {
                    Destroy(pair.Value.gameObject);
                }

                tiles.Clear();
                GenerateRandom();
            }
            if (Input.GetKeyDown("w"))
            {
                foreach (var tilePair in tiles)
                {
                    tilePair.Value.UpdateState();
                }
            }
        }

        public TypePlane CheckChordsType(Vector2 tmp)
        {
            if (tiles.ContainsKey(tmp))
            {
                return tiles[tmp].SelfType;
            }
            return TypePlane.None;
        }
        public TypePlane CheckChordsType(int x, int y)
        {
            Vector2 tmp = new Vector2(x, y);
            if (tiles.ContainsKey(tmp))
            {
                return tiles[tmp].SelfType;
            }
            return TypePlane.None;
        }
        public TypePlane CheckChordsType(float x, float y)
        {
            Vector2 tmp = new Vector2(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
            if (tiles.ContainsKey(tmp))
            {
                return tiles[tmp].SelfType;
            }
            return TypePlane.None;
        }

        public void DeletTile(Vector2 tmp)
        {
            tiles.Remove(tmp);
        }

        public void GenerateRandom(int size = 50)
        {
            GameObject putTile(Vector2 pos)
            {
                var newTile = Instantiate(tilePrefab, this.transform);
                newTile.Init(this, pos);
                tiles.Add(pos, newTile);
                return newTile.gameObject;
            }

            void DFSGen(int x, int y, int dist)
            {
                // await Task.Delay(20);
                // check if a tile is already here
                if (CheckChordsType(x, y) != 0)
                {
                    return;
                }

                int neighborTiles = (CheckChordsType(x + 1, y) != 0 ? 1 : 0) + (CheckChordsType(x, y + 1) != 0 ? 1 : 0) + (CheckChordsType(x - 1, y) != 0 ? 1 : 0) + (CheckChordsType(x, y - 1) != 0 ? 1 : 0);

                float chance = Mathf.Clamp(size - dist * 2, -100, 80) + ((neighborTiles > 2) ? 1000 : neighborTiles * 50);

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

            List<Vector2> AddBeach = new();
            foreach (var tilePair in tiles)
            {
                Vector2 chords = tilePair.Key;
                for (int i = -1; i < 2; i++)
                {
                    for(int j = -1; j < 2; j++)
                    {
                        if(CheckChordsType(chords + new Vector2(i, j)) == 0)
                        {
                            AddBeach.Add(chords + new Vector2(i, j));
                        }
                    }
                }
            }
            foreach(Vector2 chords in AddBeach)
            {
                if (CheckChordsType(chords) == 0)
                {
                    Debug.Log(CheckChordsType(chords));
                    var newTile = Instantiate(tilePrefab, this.transform);
                    newTile.Init(this, chords, TypePlane.Beach);
                    tiles.Add(chords, newTile);
                }
            }

        }

        private void UpdateAllPlanes()
        {
            foreach (var tilePair in tiles)
            {
                tilePair.Value.UpdateState();
            }
        }
    }
}
