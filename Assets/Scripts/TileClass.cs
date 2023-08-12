using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileClass : MonoBehaviour
{

    
    [SerializeField] private SpriteRenderer TileSprite;

    public Vector2 gridPos;
    public bool buildable = true;

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
        TileSprite.sprite = UtilityScript.loadSprite(path);
    }
}
