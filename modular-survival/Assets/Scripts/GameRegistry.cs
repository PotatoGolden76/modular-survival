using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GameRegistry
{
    private static Dictionary<string, Item> ItemRegistry = new Dictionary<string, Item>();

    private static List<Biome> BiomeRegistry = new List<Biome>();

    #region ItemRegistry Methods
    public static void AddItem(Item it)
    {
        if (!ItemRegistry.ContainsKey(it.ItemId))
        {
            ItemRegistry.Add(it.ItemId, it);
            Console.Log("Added item: <i>" + it.ItemId + "</i> to the registry");
        }
        else
        {
            Console.Log("Item: <i>" + it.ItemId + "</i> already exists");
        }
    }

    public static Item GetItem(string id)
    {
        if (ItemRegistry.ContainsKey(id))
        {
            return ItemRegistry[id];
        }
        else
        {
            //Debug.Log("Item with id " + id + " not found.");
            return null;
        }
    }

    public static Dictionary<string, Item>.ValueCollection getAllItems()
    {
        return ItemRegistry.Values;
    }
    #endregion

    #region BiomeRegistry Methods
    public static void AddBiome(Biome b)
    {
        Console.Log("Added item: <i>" + b.BiomeId + "</i> to the registry");
        BiomeRegistry.Add(b);
        BiomeRegistry.Sort();
    }

    public static Biome GetBiome(string id)
    {
        return BiomeRegistry.Find(b => b.BiomeId.Equals(id));
    }

    public static List<Biome> GetBiomeList()
    {
        return BiomeRegistry;
    }
    #endregion

}
