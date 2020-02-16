using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GameRegistry
{
    private static Dictionary<string, Item> ItemRegistry = new Dictionary<string, Item>();
    //private static Dictionary<string, Biome> BiomeRegistry = new Dictionary<string, Biome>();

    //ItemRegistry methods
    public static void addItem(Item it) {
        if(!ItemRegistry.ContainsKey(it.ItemId))
        {
            ItemRegistry.Add(it.ItemId, it);
            Console.Log("Added item: <i>" + it.ItemId + "</i> to the registry");
        } else
        {
            Console.Log("Item: <i>" + it.ItemId + "</i> already exists");
        }
    }

    public static Item getItem(string id)
    {
        if(ItemRegistry.ContainsKey(id))
        {
            return ItemRegistry[id];
        } else
        {
            //Debug.Log("Item with id " + id + " not found.");
            return null;
        }
    }

    public static Dictionary<string, Item>.ValueCollection getAllItems()
    {
        return ItemRegistry.Values;
    }

    //BiomeRegistry methods
    //public static void addBiome(Biome b)
    //{
    //    throw new NotImplementedException();
    //}

    //public static Biome getBiome(string id)
    //{
    //    if (BiomeRegistry.ContainsKey(id))
    //    {
    //        return BiomeRegistry[id];
    //    }
    //    else
    //   {
    //        //Console.Log("Biome with id " + id + " not found.");
    //        return null;
    //    }
    //}

    //public static Dictionary<string, Biome>.ValueCollection getAllBiomes()
    //{
    //    return BiomeRegistry.Values;
    //}

}
