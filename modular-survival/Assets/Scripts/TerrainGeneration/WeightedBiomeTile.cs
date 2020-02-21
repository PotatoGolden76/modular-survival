using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(fileName = "New Weighted Biome Tile", menuName = "Weighted biome tile")]
public class WeightedBiomeTile : Tile
{
    public List<WeightedVariation> variations = new List<WeightedVariation>();

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        if(variations.Count == 1)
        {
            tileData.sprite = variations.ToArray()[0].sprite;
        }

    }

}

