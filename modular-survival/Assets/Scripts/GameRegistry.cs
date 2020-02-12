using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameRegistry
{
    private static Dictionary<string, Item> registry = new Dictionary<string, Item>();

    public static void addItem(Item it) {
        if(!registry.ContainsKey(it.ItemId))
        {
            registry.Add(it.ItemId, it);
            Console.Log("Added item: <i>" + it.ItemId + "</i> to the registry");
        } else
        {
            Console.Log("Item: <i>" + it.ItemId + "</i> already exists");
        }
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

    public static Dictionary<string, Item>.ValueCollection getAllItems()
    {
        return registry.Values;
    }

}
