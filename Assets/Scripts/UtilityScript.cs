using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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


    /// <summary>
    /// Write to Assets/Folder folder.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="textToSave"></param>
    public static void SaveTextToFile(string filePath, string textToSave)
    {
        filePath = "Assets/Resources/Data/" + filePath;

        // Check folder
        if (!Directory.Exists("Assets/Resources/Data"))
        {
            Directory.CreateDirectory("Assets/Resources/Data");
        }

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(textToSave);
            }

            Debug.Log("Text saved to file: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving text to file: " + e.Message);
        }
    }
}
