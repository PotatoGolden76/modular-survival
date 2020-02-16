using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generator : MonoBehaviour
{
   
    public Tile[] TileAtlas;

    public PlayerController player;
    public Grid grid;

    public GameObject chunkPrefab;
    
    private List<Chunk> loadedChunks = new List<Chunk>();

    [Range(1, 10)]
    public int RenderDistance = 2;

    [Range(0.001f, 0.1f)]
    public float frequency = 0.01f;
    public int seed = 23127108;

    private FastNoise fNoise;

    private Vector3 oldPlayerPos = new Vector3(0, 0, 0);

    public void init()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        fNoise = new FastNoise();

        fNoise.SetNoiseType(FastNoise.NoiseType.Simplex);
        fNoise.SetSeed(seed);
        fNoise.SetFrequency(frequency);

        Console.Log("Noise settings:");
        Console.Log("\tFrequency: " + frequency);
        Console.Log("\tSeed: " + seed);
        Console.Log("\tType: Simplex");

        //Testing
        /*for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                grid.GetComponentInChildren<Tilemap>().SetTile(new Vector3Int(i, j, 0), TileAtlas[0]);
            }
        }*/
    }

    private void Start()
    {
        init();
    }

    private void Update()
    {
        //Debug.Log(getCurrentChunk((int)player.transform.position.x, (int)player.transform.position.y));
        if(!player.transform.position.Equals(oldPlayerPos))
        {
            oldPlayerPos = player.transform.position;
            for(int i = (int)player.transform.position.x - (RenderDistance*Chunk.CHUNK_SIZE_X); i <= (int)player.transform.position.x + (RenderDistance * Chunk.CHUNK_SIZE_X); i = i + Chunk.CHUNK_SIZE_X)
            {
                for (int j = (int)player.transform.position.y - (RenderDistance * Chunk.CHUNK_SIZE_Y); j <= (int)player.transform.position.y + (RenderDistance * Chunk.CHUNK_SIZE_Y); j = j + Chunk.CHUNK_SIZE_Y)
                {
                    Vector2Int itChunk = getCurrentChunk(i, j);
                    if (!loadedChunks.Exists(c => (c.corner.x == itChunk.x && c.corner.y == itChunk.y)))
                    {
                        Chunk chk = new Chunk(itChunk.x, itChunk.y, chunkPrefab);
                        chk.tilemap.transform.parent = grid.transform;

                        Debug.Log(itChunk);

                        Generate(ref chk);

                        loadedChunks.Add(chk);
                    }
                }
            }

            loadedChunks.RemoveAll(outsideRenderView);
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
        Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(player.transform.position, (float)(Chunk.CHUNK_SIZE_X * RenderDistance * Mathf.Sqrt(2) * 1.5));
        loadedChunks.ForEach(c => Gizmos.DrawLine(player.transform.position, new Vector3(c.corner.x, c.corner.y, 0)));
    }

    private void Generate(ref Chunk c)
    {
        for(int i = c.tilemap.origin.x; i < c.tilemap.origin.x + Chunk.CHUNK_SIZE_X; i++)
        {
            for (int j = c.tilemap.origin.y; j < c.tilemap.origin.y + Chunk.CHUNK_SIZE_Y; j++)
            {
                float val = fNoise.GetNoise(c.corner.x + i, c.corner.y + j);

                if(val < 0)
                {
                    c.tilemap.SetTile(new Vector3Int(i, j, 0), TileAtlas[0]);
                } else
                {
                    c.tilemap.SetTile(new Vector3Int(i, j, 0), TileAtlas[1]);
                }
            }
        }
    }


    public Vector2Int getCurrentChunk(int i, int j)
    {
        float px;
        if (i >= 0)
        {
            px = (i / Chunk.CHUNK_SIZE_X) * Chunk.CHUNK_SIZE_X; 
        } else
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
