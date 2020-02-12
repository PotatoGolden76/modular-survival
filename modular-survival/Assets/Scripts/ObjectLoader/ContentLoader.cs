using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ContentLoader
{
    string path;

    public ContentLoader(string pth)
    {
        path = pth;

        init();
    }

    private void init()
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

        if (!Directory.Exists(Path.Combine(path, "Items")))
        {
            Debug.LogError("Items folder not found, creating folder");

            try
            {
                Directory.CreateDirectory(Path.Combine(path, "Items"));
            }
            catch (Exception e)
            {
                Debug.Log("Unable to create folder");
                Debug.Log(e.StackTrace);
            }
        }

        //Debug.Log(path);
    }

    public void loadItems()
    {
        string[] files = Directory.GetFiles(path + "/Items", "*.json");

        foreach(string s in files)
        {
            using (StreamReader reader = File.OpenText(s))
            {
                string str;
                str = reader.ReadToEnd();

                Item it = JsonUtility.FromJson<Item>(str);

                GameRegistry.addItem(it);
            }
        }
    }
}
