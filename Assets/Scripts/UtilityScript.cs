using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityScript
{
    public static Sprite loadSprite(string path)
    {

        // try open sprite
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite != null)
        {
            return sprite;
        }

        // if nothing found, try search in debug stuff... 

        sprite = Resources.Load<Sprite>("Debug/" + path);

        if (sprite != null)
        {
            return sprite;
        }

        return null;
    }
}
