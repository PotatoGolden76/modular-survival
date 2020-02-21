using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeightedVariation
{
    public Sprite sprite;

    [Range(0, 100)]
    public int weight;
}

