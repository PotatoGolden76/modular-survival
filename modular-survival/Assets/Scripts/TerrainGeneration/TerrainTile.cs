using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(fileName = "New Terrain Tile", menuName = "Terrain tile")]
public class TerrainTile : Tile
{
    public List<Sprite> frames = new List<Sprite>();

    public bool isAnimated = false;
    public float animationSpeed = 0f;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = frames.ToArray()[0];

    }
}