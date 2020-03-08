﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    public PlayerController player;
    public Grid grid;

    private Vector3 oldPlayerPos = new Vector3(0, 0, 0);


    public GameObject chunkPrefab;

    private List<Chunk> loadedChunks = new List<Chunk>();

    [Range(1, 100)]
    public int RenderDistance = 2;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        Generator.Start();
    }

    private void Update()
    {
        if (!player.transform.position.Equals(oldPlayerPos))
        {
            oldPlayerPos = player.transform.position;
            for (int i = (int)player.transform.position.x - (RenderDistance * Chunk.CHUNK_SIZE_X); i <= (int)player.transform.position.x + (RenderDistance * Chunk.CHUNK_SIZE_X); i = i + Chunk.CHUNK_SIZE_X)
            {
                for (int j = (int)player.transform.position.y - (RenderDistance * Chunk.CHUNK_SIZE_Y); j <= (int)player.transform.position.y + (RenderDistance * Chunk.CHUNK_SIZE_Y); j = j + Chunk.CHUNK_SIZE_Y)
                {
                    Vector2Int itChunk = Chunk.getChunkAt(i, j);
                    if (!loadedChunks.Exists(c => (c.corner.x == itChunk.x && c.corner.y == itChunk.y)))
                    {
                        Chunk chk = new Chunk(itChunk.x, itChunk.y, chunkPrefab);
                        chk.ground_tilemap.transform.parent = grid.transform;

                        //Debug.Log(itChunk);

                        Generator.GenerateChunk(ref chk);

                        loadedChunks.Add(chk);
                    }
                }
            }

            loadedChunks.RemoveAll(outsideRenderView);

            //Debug.Log(Chunk.getChunkAt((int)player.transform.position.x, (int)player.transform.position.y));
        }

        //loadedChunks.ForEach(c => Debug.Log(c.corner));
        //Debug.Log("-----------");
    }

    private bool outsideRenderView(Chunk c)
    {
        float dis = Vector2Int.Distance(c.corner, new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y));
        if (dis > Chunk.CHUNK_SIZE_X * RenderDistance * Mathf.Sqrt(2) * 2)
        {
            c.unloadChunk();
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        //Gizmos.DrawSphere(player.transform.position, (float)(Chunk.CHUNK_SIZE_X * RenderDistance * Mathf.Sqrt(2) * 1.5));
        loadedChunks.ForEach(c => Gizmos.DrawLine(player.transform.position, new Vector3(c.corner.x, c.corner.y, 0)));

        Gizmos.color = Color.cyan;
        loadedChunks.ForEach(c =>
        {
            Gizmos.DrawLine(new Vector3(c.corner.x, c.corner.y, 0), new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y, 0));
            Gizmos.DrawLine(new Vector3(c.corner.x, c.corner.y, 0), new Vector3(c.corner.x, c.corner.y + Chunk.CHUNK_SIZE_Y, 0));
            Gizmos.DrawLine(new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y, 0), new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y + Chunk.CHUNK_SIZE_Y, 0));
            Gizmos.DrawLine(new Vector3(c.corner.x, c.corner.y + Chunk.CHUNK_SIZE_Y, 0), new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y + Chunk.CHUNK_SIZE_Y, 0));
        });
    }


}
