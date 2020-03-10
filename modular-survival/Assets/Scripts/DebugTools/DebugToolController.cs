using UnityEngine;
using UnityEngine.UI;

public class DebugToolController : MonoBehaviour
{
    public CanvasGroup console;
    public CanvasGroup registry;

    public GameObject itemButtonPrefab;
    public GameObject content;

    void Start()
    {
        foreach (Item it in GameRegistry.getAllItems())
        {
            GameObject obj = Instantiate(itemButtonPrefab, content.transform.position, Quaternion.identity, content.transform);
            obj.GetComponentInChildren<Text>().text = it.ItemId;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (console.alpha == 0f)
            {
                console.alpha = 1f;
            }
            else
            {
                console.alpha = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (registry.alpha == 0f)
            {
                registry.alpha = 1f;
            }
            else
            {
                registry.alpha = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.V) && console.alpha == 1f)
        {
            Console.ClearConsole();
        }
    }
}
