using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    #region Objects
    public PlayerController player;
    public Grid grid;

    private Vector2Int oldPlayerChunk = Vector2Int.zero;
    public GameObject chunkPrefab;
    #endregion

    private List<Chunk> chunkPool = new List<Chunk>();

    [Range(1, 100)]
    public int RenderDistance = 2;

    #region General purpose variables
    private Chunk chk = null;
    private bool start = true;
    private float distanceFromPlayer;
    private Vector2Int currentChunk;
    #endregion

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        //oldPlayerChunk = Chunk.getChunkAt(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.y));

        Generator.Start();

        //Init Chunk pool
        for (int i = 0; i < Mathf.Pow((RenderDistance * 2 + 1), 2); i++)
        {
            chk = new Chunk(0, 0, chunkPrefab);
            chk.ground_tilemap.transform.parent = grid.transform;
            chk.unloadChunk();

            chunkPool.Add(chk);
        }
        start = true;
    }

    private void Update()
    {
        currentChunk = Chunk.getChunkAt(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.y));
        if (!oldPlayerChunk.Equals(currentChunk) || start) { 
            foreach (Chunk c in chunkPool)
            {
                OutsideRenderView(c);
            }

            for (int i = currentChunk.x - (RenderDistance * Chunk.CHUNK_SIZE_X); i <= currentChunk.x + (RenderDistance * Chunk.CHUNK_SIZE_X); i = i + Chunk.CHUNK_SIZE_X)
            {
                for (int j = currentChunk.y - (RenderDistance * Chunk.CHUNK_SIZE_Y); j <= currentChunk.y + (RenderDistance * Chunk.CHUNK_SIZE_Y); j = j + Chunk.CHUNK_SIZE_Y)
                {
                    if (!chunkPool.Exists(c => c.corner.x == i && c.corner.y == j && c.active))
                    {
                        chk = chunkPool.Find(c => c.ground_tilemap.gameObject.activeSelf == false);

                        chk.setPositionAndActivate(Chunk.getChunkAt(i, j));
                        Generator.GenerateChunk(ref chk);
                    }
                }
            }

            oldPlayerChunk = currentChunk;
        }

        start = false;
    }

    private void OutsideRenderView(Chunk c)
    {
        distanceFromPlayer = Vector2Int.Distance(c.corner, currentChunk);
        if (distanceFromPlayer > Chunk.CHUNK_SIZE_X * RenderDistance)
        {
            c.unloadChunk();
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.white;
        //loadedChunks.ForEach(c => Gizmos.DrawLine(player.transform.position, new Vector3(c.corner.x, c.corner.y, 0)));

        Gizmos.color = Color.cyan;
        chunkPool.ForEach(c =>
        {
            Gizmos.DrawLine(new Vector3(c.corner.x, c.corner.y, 0), new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y, 0));
            Gizmos.DrawLine(new Vector3(c.corner.x, c.corner.y, 0), new Vector3(c.corner.x, c.corner.y + Chunk.CHUNK_SIZE_Y, 0));
            Gizmos.DrawLine(new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y, 0), new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y + Chunk.CHUNK_SIZE_Y, 0));
            Gizmos.DrawLine(new Vector3(c.corner.x, c.corner.y + Chunk.CHUNK_SIZE_Y, 0), new Vector3(c.corner.x + Chunk.CHUNK_SIZE_X, c.corner.y + Chunk.CHUNK_SIZE_Y, 0));
        });
    }


}
