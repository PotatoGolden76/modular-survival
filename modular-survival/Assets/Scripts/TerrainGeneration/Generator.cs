using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Generator
{
    #region Noise Settings
    [Range(0.001f, 0.1f)]
    private static readonly float biomeFrequency = 0.0005f, treeFrequency = 0.005f;
    private static int seed = 23127108;

    private static FastNoise TerrainNoise;
    private static FastNoise TreeNoise;
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
        TreeNoise = new FastNoise();

        TerrainNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        TerrainNoise.SetFractalOctaves(4);
        TerrainNoise.SetSeed(seed);
        TerrainNoise.SetFrequency(biomeFrequency);

        TreeNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        TreeNoise.SetFractalOctaves(7);
        TreeNoise.SetSeed(seed);
        TreeNoise.SetFrequency(treeFrequency);
    }

    public static void Start()
    {
        InitialiseNoise();
    }
    #endregion

    #region Generation Variables
    private static Biome b = null;
    private static float terrainValue = 0f;
    private static float treeValue = 0f;
    private static float decoValue = 0f;

    private static TileBase[] terrainTiles = new TileBase[256];
    private static TileBase[] decoTiles = new TileBase[256];
    private static TileBase[] treeTiles = new TileBase[256];

    private static Vector3Int[] positions = new Vector3Int[256];

    private static byte[] variationCode = null;
    #endregion
    public static void GenerateChunk(ref Chunk c)
    {
        terrainTiles = new TileBase[256];
        decoTiles = new TileBase[256];
        treeTiles = new TileBase[256];

        for (int i = 0; i < Chunk.CHUNK_SIZE_X*Chunk.CHUNK_SIZE_Y; i++)
        {
            positions[i] = new Vector3Int(i / Chunk.CHUNK_SIZE_X, i % Chunk.CHUNK_SIZE_Y, 0);

            b = GetBiomeAtCoords(new Vector2((i / Chunk.CHUNK_SIZE_X) + c.corner.x, (i % Chunk.CHUNK_SIZE_X) + c.corner.y));
            terrainTiles[i] = GetTerrainTileAtCoords(new Vector2((i / Chunk.CHUNK_SIZE_X) + c.corner.x, (i % Chunk.CHUNK_SIZE_X) + c.corner.y));

            if (!b.IsStatic)
            {
                if (terrainTiles[i].Equals(b.MainTile))
                {
                    SetDecoAtCoords(new Vector2Int((i / Chunk.CHUNK_SIZE_X) + c.corner.x, (i % Chunk.CHUNK_SIZE_X) + c.corner.y), i);
                }
            }

        }

        c.ground_tilemap.SetTiles(positions, terrainTiles);
        c.decoration_tilemap.SetTiles(positions, decoTiles);
        c.object_tilemap.SetTiles(positions, treeTiles);
    }

    #region Noise
    public static float GetTreeNoiseValue(Vector2 v)
    {
        return (TreeNoise.GetNoise(v.x, v.y) * 5) + 5;
    }

    private static float GetTerrainNoiseValue(Vector2 v)
    {
        return (TerrainNoise.GetNoise(v.x, v.y) * 5) + 5;
    }

    #endregion

    private static Biome GetBiomeAtCoords(Vector2 v)
    {
        terrainValue = GetTerrainNoiseValue(v);

        for (int i = 0; i < GameRegistry.BiomeRegistry.Count; i++)
        {
            if (terrainValue >= GameRegistry.BiomeRegistry[i].MinimumElevation)
            {
                return GameRegistry.BiomeRegistry[i];
            }
        }

        Console.Log("No biome at elevation: " + terrainValue);
        return null;
    }

    private static void SetDecoAtCoords(Vector2Int v, int index)
    {
        treeValue = GetTreeNoiseValue(v);

        if (treeValue >= 10f - b.TreeDensity)
        {
            treeTiles[index] = b.tree;
        }
        else
        {
            decoValue = Random.Range(0, b.decorations.Count * 2);
            for (int i = 0; i < b.decorations.Count; i++)
            {
                if (decoValue >= i && decoValue <= i + 1)
                {
                    decoTiles[index] = b.decorations.ToArray()[i];
                }
            }
        }
    }

    private static TerrainTile GetTerrainTileAtCoords(Vector2 v)
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

    private static bool CompareCode(byte[] t, byte[] code)
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
