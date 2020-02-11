using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Item
{
    public string itemId { get; set; }
    public string itemName { get; set; }
    public string type { get; set; }

    public Item()
    {

    }

    public string toString()
    {
        return (itemId + " " + type + " " + itemName);
    }
}
