using System;
using System.IO;
using UnityEngine;

public class ContentLoader
{
    private string path;

    public ContentLoader(string pth)
    {
        path = pth;

        Initialise();
    }

    private void Initialise()
    {
        path += "/Modules";

        if (!Directory.Exists(path))
        {
            Debug.LogError("Modules folder not found, creating folder");

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Debug.Log("Unable to create folder");
                Debug.Log(e.StackTrace);
            }
        }

        AddSubfolder("Items");
        AddSubfolder("Chunks_Data");

    }

    private void AddSubfolder(string s)
    {
        if (!Directory.Exists(Path.Combine(path, s)))
        {
            Debug.LogError(s + " folder not found, creating folder");

            try
            {
                Directory.CreateDirectory(Path.Combine(path, s));
                Console.Log("Created " + s + " folder");
            }
            catch (Exception e)
            {
                Debug.Log("Unable to create folder");
                Debug.Log(e.StackTrace);
            }
        }
    }

    public void LoadBiomes()
    {
       foreach(Biome b in Resources.LoadAll("Biomes", typeof(Biome)))
        {
            GameRegistry.AddBiome(b);
        }
    }

    public void LoadItems()
    {
        string[] files = Directory.GetFiles(path + "/Items", "*.json");

        foreach(string s in files)
        {
            using (StreamReader reader = File.OpenText(s))
            {
                string str = reader.ReadToEnd();
                GameRegistry.AddItem(JsonUtility.FromJson<Item>(str));
            }
        }
    }
}
