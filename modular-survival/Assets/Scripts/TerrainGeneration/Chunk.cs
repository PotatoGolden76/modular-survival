using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk
{
    public const int CHUNK_SIZE_X = 16, CHUNK_SIZE_Y = 16;

    public Tilemap tilemap;
    public Vector2Int corner;

    public Chunk(int i, int j, GameObject tileMapPrefab)
    {
        tilemap = GameObject.Instantiate(tileMapPrefab, new Vector3(i, j, 0), Quaternion.identity).GetComponent<Tilemap>();
        corner.x = i;
        corner.y = j;
    }
    

    public void unloadChunk()
    {
        GameObject.Destroy(tilemap.gameObject);
    }
}
