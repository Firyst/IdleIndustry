using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileClass : MonoBehaviour
{

    
    [SerializeField] private SpriteRenderer TileSprite;

    public Vector2 gridPos;
    public bool buildable = true;

    public string textureName;
    public BuildingInstance building;
    // public GameObject tileBuilding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSprite(string path)
    {
        textureName = path;
        TileSprite.sprite = UtilityScript.loadSprite(path);
    }

    /// <summary>
    /// Used for packing data for save in more compact way.
    /// </summary>
    /// <returns></returns>
    public TileData getSaveData()
    {
        TileData res = new();
        res.pos = gridPos;
        res.bld = building;
        return res;
    }
}

/// <summary>
/// Class for storing tile data.
/// </summary>
public class TileData
{
    public Vector2 pos;
    public BuildingInstance bld;
}