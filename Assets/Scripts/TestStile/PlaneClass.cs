using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Game
{
    public enum TypePlane
    {
        None,
        Ground,
        Beach,
        Sea,
    }

    public class PlaneClass : MonoBehaviour
    {
        public TypePlane SelfType { get; private set; }
        [SerializeField] private TileClass tilePrefab;
        private Island island;
        private Vector2 chords;
        private List<TileClass> decorations;

        public void Init(Island island, Vector2 chords, TypePlane type = TypePlane.Ground)
        {
            this.island = island;
            this.chords = chords;
            decorations = new();
            SelfType = type;
            transform.localPosition = chords * 1.28f;

            var tile = Instantiate(tilePrefab, this.transform);
            tile.GetComponent<TileClass>().setSprite(type == TypePlane.Ground ? "base1" : "base2");
            decorations.Add(tile);
        }

        public void UpdateState()
        {
            /* 
            Ќумераци€ справа против часовой стрелки:
                | |3| |
                |2|x|0|
                | |1| |
             */
            TypePlane[] tmp = new TypePlane[4]
            {
                island.CheckChordsType(chords + new Vector2(1, 0)),
                island.CheckChordsType(chords + new Vector2(0, -1)),
                island.CheckChordsType(chords + new Vector2(-1, 0)),
                island.CheckChordsType(chords + new Vector2(0, 1))
            };

            /* 
           Ќумераци€ справого верхнего против часовой стрелки:
               |3| |0|
               | |x| |
               |2| |1|
            */
            TypePlane[] tmpX = new TypePlane[4]
            {
                island.CheckChordsType(chords + new Vector2(1, 1)),
                island.CheckChordsType(chords + new Vector2(1, -1)),
                island.CheckChordsType(chords + new Vector2(-1, -1)),
                island.CheckChordsType(chords + new Vector2(-1, 1))
            };

            

            
            if (tmp.Any(n => n == TypePlane.None) || tmpX.Any(n => n == TypePlane.None))
            {
                    SelfType = TypePlane.Beach;
            }
            else
            {
                SelfType = TypePlane.Ground;
            }
            UpdateDrowState();
        }

        public void UpdateDrowState()
        {
            void spawnDecor(string spriteName, int rotation = 0, int z = 0)
            {
                var tile = Instantiate(tilePrefab, this.transform);
                tile.GetComponent<TileClass>().setSprite(spriteName);
                tile.transform.eulerAngles = new Vector3(0, 0, 90 * rotation);
                tile.GetComponentInChildren<SpriteRenderer>().sortingOrder = z;
                decorations.Add(tile);
            }

            foreach (TileClass decor in decorations)
            {
                Destroy(decor.gameObject);
            }
            decorations.Clear();


            /* 
           Ќумераци€ справого верхнего против часовой стрелки:
               |5|6|7|
               |4|x|0|
               |3|2|1|
            */
            TypePlane[] tmp = new TypePlane[8]
                    {
                        island.CheckChordsType(chords + new Vector2(1, 0)),
                        island.CheckChordsType(chords + new Vector2(1, -1)),
                        island.CheckChordsType(chords + new Vector2(0, -1)),
                        island.CheckChordsType(chords + new Vector2(-1, -1)),
                        island.CheckChordsType(chords + new Vector2(-1, 0)),
                        island.CheckChordsType(chords + new Vector2(-1, 1)),
                        island.CheckChordsType(chords + new Vector2(0, 1)),
                        island.CheckChordsType(chords + new Vector2(1, 1)),
                    };
            switch (SelfType)
            {
                case TypePlane.Ground:
                    if (Mathf.RoundToInt(chords.x + chords.y) % 2 == 0)
                    {
                        spawnDecor("base1");
                    }
                    else
                    {
                        spawnDecor("base2");
                    }
                    break;

                case TypePlane.Beach:
                        for (int i = 0; i < 4; i++)
                        {
                            if (tmp[(i * 2) % 8] == TypePlane.Ground)
                            {
                                spawnDecor("beach-straight", 2 - i);
                                if (tmp[(i * 2 + 2) % 8] == TypePlane.Ground) 
                                {
                                    spawnDecor("beach-corner-inner", 2 - i, 1);
                                }
                                    
                            }
                            if (tmp[i * 2] == TypePlane.Beach && tmp[(i + 1) * 2 % 8] == TypePlane.Beach && tmp[i * 2 + 1] == TypePlane.Ground)
                                    spawnDecor("beach-corner", 1 - i);
                        }
                    break;
            }
        }
    }
}
