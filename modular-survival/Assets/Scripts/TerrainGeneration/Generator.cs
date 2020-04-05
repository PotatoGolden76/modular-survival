using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public static class Generator
{
    #region Noise Settings
    [Range(0.001f, 0.1f)]
    private static readonly float biomeFrequency = 0.0005f, treeFrequency = 0.005f;
    private static int seed = 23127108;

    private static FastNoise TerrainNoise;
    private static FastNoise ObjectNoise;
    #endregion

    private static readonly List<Vector2Int> transitionDirections = new List<Vector2Int>() { new Vector2Int(-1, 1),
                                                                         new Vector2Int(0, 1),
                                                                         new Vector2Int(1, 1),
                                                                         new Vector2Int(1, 0),
                                                                         new Vector2Int(1, -1),
                                                                         new Vector2Int(0, -1),
                                                                         new Vector2Int(-1, -1),
                                                                         new Vector2Int(-1, 0)};

    #region Initialisation
    private static void InitialiseNoise()
    {
        TerrainNoise = new FastNoise();
        ObjectNoise = new FastNoise();

        TerrainNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        TerrainNoise.SetFractalOctaves(4);
        TerrainNoise.SetSeed(seed);
        TerrainNoise.SetFrequency(biomeFrequency);

        ObjectNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        ObjectNoise.SetFractalOctaves(7);
        ObjectNoise.SetSeed(seed);
        ObjectNoise.SetFrequency(treeFrequency);
    }

    public static void Start()
    {
        InitialiseNoise();

        for (int i = 0; i < Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y; i++)
        {
            positions[i] = new Vector3Int(i / Chunk.CHUNK_SIZE_X, i % Chunk.CHUNK_SIZE_Y, 0);
        }
    }
    #endregion

    private static TileBase[] terrainTiles = new TileBase[Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y];
    private static TileBase[] objectTiles = new TileBase[Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y];

    private static TileGenerator[] workers = new TileGenerator[Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y];
    private static Task[] tasks;
    private static Vector2 coord;

    private static int workerNumber = Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y;

    private static Vector3Int[] positions = new Vector3Int[Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y];

    public static void GenerateChunk(ref Chunk c)
    {
        terrainTiles = new TileBase[Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y];
        objectTiles = new TileBase[Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y];

        workerNumber = Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y;

        coord = new Vector2( c.corner.x,  c.corner.y);

        tasks = Enumerable.Range(0, workerNumber).Select(i => Task.Factory.StartNew(() => {
            workers[i] = new TileGenerator(coord + new Vector2((i / Chunk.CHUNK_SIZE_X), (i % Chunk.CHUNK_SIZE_X)));
            workers[i].Callback(); })
        ).ToArray();

        Task.WaitAll(tasks);
        Debug.Log("done generating chunk tiles for: " + c.corner);

        for (int i = 0; i < Chunk.CHUNK_SIZE_X * Chunk.CHUNK_SIZE_Y; i++)
        {
            terrainTiles[i] = workers[i].TerrainTile;
            objectTiles[i] = workers[i].ObjectTile;
        }

        c.ground_tilemap.SetTiles(positions, terrainTiles);
        c.object_tilemap.SetTiles(positions, objectTiles);
    }


    //Generator Worker Class
    public class TileGenerator
    {
        private Vector2 position;

        public TileBase TerrainTile { get; private set; }
        public TileBase ObjectTile { get; private set; }

        #region Generation Variables
        private Biome b = null;

        private float terrainNoiseValue = 0f;
        private float ObjectNoiseValue = 0f;

        private byte[] variationCode = null;
        #endregion

        public TileGenerator(Vector2 pos)
        {
            position = pos;
        }

        public void Callback()
        {
            b = GetBiomeAtCoords(position);

            TerrainTile = GetTerrainTileAtCoords(position);

            if (!b.IsStatic)
            {
                if (TerrainTile.Equals(b.MainTile))
                {
                    ObjectTile = GetObjectTileAtCoords(position);
                }
            }
        }

        #region Noise
        public float GetObjectNoiseValue(Vector2 v)
        {
            return (Generator.ObjectNoise.GetNoise(v.x, v.y) * 5) + 5;
        }

        private float GetTerrainNoiseValue(Vector2 v)
        {
            return (Generator.TerrainNoise.GetNoise(v.x, v.y) * 5) + 5;
        }

        #endregion

        private Biome GetBiomeAtCoords(Vector2 v)
        {
            terrainNoiseValue = GetTerrainNoiseValue(v);

            for (int i = 0; i < GameRegistry.BiomeRegistry.Count; i++)
            {
                if (terrainNoiseValue >= GameRegistry.BiomeRegistry[i].MinimumElevation)
                {
                    return GameRegistry.BiomeRegistry[i];
                }
            }

            Console.Log("No biome at elevation: " + terrainNoiseValue);
            return null;
        }

        private TileBase GetTerrainTileAtCoords(Vector2 v)
        {
            if (b.IsStatic)
            {
                return b.MainTile;
            }

            variationCode = new byte[4];

            for (int i = 1; i < 8; i += 2)
            {
                if (GetBiomeAtCoords(v + transitionDirections.ToArray()[i]).BiomeId.Equals(b.BiomeId))
                {
                    variationCode[i / 2] = 1;
                }
                else
                {
                    variationCode[i / 2] = 0;
                }
            }

            #region Code checking
            if (CompareCode(variationCode, TransitionCodes.Fill))
            {
                for (int i = 0; i < 8; i += 2)
                {
                    if (GetBiomeAtCoords(v + transitionDirections.ToArray()[i]).BiomeId.Equals(b.BiomeId))
                    {
                        variationCode[i / 2] = 1;
                    }
                    else
                    {
                        variationCode[i / 2] = 0;
                    }
                }

                //In Corners   
                if (CompareCode(variationCode, TransitionCodes.ICorner_Back_Left))
                {
                    return b.InCorner_Back_Left;
                }

                if (CompareCode(variationCode, TransitionCodes.ICorner_Back_Right))
                {
                    return b.InCorner_Back_Right;
                }

                if (CompareCode(variationCode, TransitionCodes.ICorner_Front_Left))
                {
                    return b.InCorner_Front_Left;
                }

                if (CompareCode(variationCode, TransitionCodes.ICorner_Front_Right))
                {
                    return b.InCorner_Front_Right;
                }

                return b.MainTile;
            }

            //OCorners
            if (CompareCode(variationCode, TransitionCodes.OCorner_Back_Left))
            {
                return b.OutCorner_Back_Left;
            }

            if (CompareCode(variationCode, TransitionCodes.OCorner_Back_Right))
            {
                return b.OutCorner_Back_Right;
            }

            if (CompareCode(variationCode, TransitionCodes.OCorner_Front_Left))
            {
                return b.OutCorner_Front_Left;
            }

            if (CompareCode(variationCode, TransitionCodes.OCorner_Front_Right))
            {
                return b.OutCorner_Front_Right;
            }

            //Ledges
            if (CompareCode(variationCode, TransitionCodes.Ledge_Back))
            {
                return b.Ledge_Back;
            }

            if (CompareCode(variationCode, TransitionCodes.Ledge_Front))
            {
                return b.Ledge_Front;
            }

            if (CompareCode(variationCode, TransitionCodes.Ledge_Left))
            {
                return b.Ledge_Left;
            }

            if (CompareCode(variationCode, TransitionCodes.Ledge_Right))
            {
                return b.Ledge_Right;
            }
            #endregion

            return b.MainTile;
        }

        private TileBase GetObjectTileAtCoords(Vector2 v)
        {
            ObjectNoiseValue = GetObjectNoiseValue(v);

            if (ObjectNoiseValue >= 10f - b.TreeDensity)
            {
                return b.tree;
            }
            else
            {
                return null;
            }
        }

        private bool CompareCode(byte[] t, byte[] code)
        {
            for (int i = 0; i < 4; i++)
            {
                if (t[i] != code[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}


