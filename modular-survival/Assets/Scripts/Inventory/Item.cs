using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Item
{
    public string ItemId;
    public string ItemName;
    public ItemType Type; 

    public static string toString(Item it)
    {
        return (it.ItemId + "\n" + it.Type + "\n" + it.ItemName);
    }
}
