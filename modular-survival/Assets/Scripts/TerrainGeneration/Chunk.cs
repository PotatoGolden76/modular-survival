using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk
{
    public const int CHUNK_SIZE_X = 16, CHUNK_SIZE_Y = 16;

    public Tilemap ground_tilemap, object_tilemap, decoration_tilemap;
    public Vector2Int corner;

    public bool active = false;

    public Chunk(int i, int j, GameObject chunkPrefab)
    {
        active = true;
        ground_tilemap = GameObject.Instantiate(chunkPrefab, new Vector3(i, j, 0), Quaternion.identity).GetComponent<Tilemap>();
        ground_tilemap.gameObject.SetActive(active);

        Tilemap[] maps = new Tilemap[3];
        maps = ground_tilemap.gameObject.GetComponentsInChildren<Tilemap>();

        if(maps[0] != ground_tilemap)
        {
            object_tilemap = maps[0];
        } else
        {
            object_tilemap = maps[1];
            decoration_tilemap = maps[2];
        }
        corner.x = i;
        corner.y = j;
    }

    public void setPositionAndActivate(Vector2Int v)
    {
        ground_tilemap.transform.position = new Vector3(v.x, v.y, 0);
        corner = new Vector2Int(v.x, v.y);
        active = true;
        ground_tilemap.gameObject.SetActive(true);
    }


    public void unloadChunk()
    {
        active = false;
        ground_tilemap.gameObject.SetActive(false);
    }

    public static Vector2Int getChunkAt(int i, int j)
    {
        float px;
        if (i >= 0)
        {
            px = (i / Chunk.CHUNK_SIZE_X) * Chunk.CHUNK_SIZE_X;
        }
        else
        {
            px = ((i - Chunk.CHUNK_SIZE_X + 1) / Chunk.CHUNK_SIZE_X) * Chunk.CHUNK_SIZE_X;
        }

        float py;
        if (j >= 0)
        {
            py = (j / Chunk.CHUNK_SIZE_Y) * Chunk.CHUNK_SIZE_Y;
        }
        else
        {
            py = ((j - Chunk.CHUNK_SIZE_Y + 1) / Chunk.CHUNK_SIZE_Y) * Chunk.CHUNK_SIZE_Y;
        }

        return new Vector2Int((int)px, (int)py);
    }
}
