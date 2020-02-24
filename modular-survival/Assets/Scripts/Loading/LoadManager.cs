using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    ContentLoader loader;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);

        Console.Log("Starting load procedure");
        Console.Log("Current data folder is: " + Application.dataPath);

        loader = new ContentLoader(Application.dataPath);

        loader.loadItems();
        loader.loadBiomes();
    }

}
