using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;


[System.Serializable]
[CreateAssetMenu(fileName = "New Weighted Biome Tile", menuName = "Weighted biome tile")]
public class BiomeTile : Tile
{
    public List<WeightedVariation> variations = new List<WeightedVariation>();

    public bool isAnimated = false;
    public bool isVariated = false;

    public float animationSpeed = 0f;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        if (variations.Count == 1)
        {
            tileData.sprite = variations.ToArray()[0].sprite;
        }
        else
        {
            if (isVariated)
            {
                variations.Sort((a, b) => a.weight.CompareTo(b.weight));

                int totalWeight = 0;
                variations.ForEach(v => totalWeight += v.weight);

                tileData.sprite = variations.ToArray()[0].sprite;
            }

        }
    }

}