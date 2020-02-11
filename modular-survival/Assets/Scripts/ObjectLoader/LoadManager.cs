using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    ContentLoader loader;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting load procedure");
        Debug.Log("Current data folder is: " + Application.dataPath);

        loader = new ContentLoader(Application.dataPath);

        loader.loadItems();

        Debug.Log(GameRegistry.getItem("test_item_1").toString());
    }


}
