using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameRegistry
{
    private static Dictionary<string, Item> registry = new Dictionary<string, Item>();

    public static void addItem(Item it) {
        registry.Add(it.itemId, it);
    }

    public static Item getItem(string id)
    {
        if(registry.ContainsKey(id))
        {
            return registry[id];
        } else
        {
            Debug.Log("Item with id " + id + " not found.");
            return null;
        }
    }

}
