using UnityEngine;

public class LoadManager : MonoBehaviour
{
    private ContentLoader loader;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);

        loader = new ContentLoader(Application.dataPath);

        Console.Log("Starting load procedure");
        Console.Log("Current data folder is: " + Application.dataPath);

        loader.LoadItems();
        loader.LoadBiomes();
    }

}
