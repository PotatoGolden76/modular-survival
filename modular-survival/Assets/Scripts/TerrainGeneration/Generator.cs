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

    public static void GenerateChunk(ref Chunk c)
    {
        for (int i = c.ground_tilemap.origin.x; i < c.ground_tilemap.origin.x + Chunk.CHUNK_SIZE_X; i++)
        {
            for (int j = c.ground_tilemap.origin.y; j < c.ground_tilemap.origin.y + Chunk.CHUNK_SIZE_Y; j++)
            {
                c.ground_tilemap.SetTile(new Vector3Int(i, j, 0), GetTerrainTileAtCoords(new Vector2(i + c.corner.x, j + c.corner.y)));

                if (c.ground_tilemap.GetTile(new Vector3Int(i, j, 0)).Equals(GetBiomeAtCoords(new Vector2(i + c.corner.x, j + c.corner.y)).MainTile))
                {
                   c.object_tilemap.SetTile(new Vector3Int(i, j, 0), GetTreeAtCoords(new Vector2(i + c.corner.x, j + c.corner.y)));
                }

            }
        }
    }

    #region Noise
    public static float GetTreeNoiseValue(Vector2 v)
    {
        return TreeNoise.GetNoise(v.x, v.y);
    }

    private static float GetTerrainNoiseValue(Vector2 v)
    {
        return (TerrainNoise.GetNoise(v.x, v.y) * 5) + 5;
    }
    #endregion

    private static Biome GetBiomeAtCoords(Vector2 v)
    {
        float val = GetTerrainNoiseValue(v);

        foreach (Biome b in GameRegistry.GetBiomeList())
        {
            if (val >= b.MinimumElevation)
            {
                return b;
            }
        }

        Console.Log("No biome at elevation: " + val);
        return null;
    }

    private static Tile GetTreeAtCoords(Vector2 v)
    {
        float val = GetTreeNoiseValue(v);
        if (val > 0)
        {
            return GetBiomeAtCoords(v).tree;
        }

        return null;
    }

    private static BiomeTile GetTerrainTileAtCoords(Vector2 v)
    {
        Biome b = GetBiomeAtCoords(v);
        string Id = b.BiomeId;

        if(b.IsStatic)
        {
            return b.MainTile;
        }

        byte[] t = new byte[4];

        for (int i = 1; i < 8; i += 2)
        {
            if (GetBiomeAtCoords(v + transitionDirections.ToArray()[i]).BiomeId.Equals(Id))
            {
                t[i / 2] = 1;
            }
            else
            {
                t[i / 2] = 0;
            }
        }

        #region Code checking
        if (CompareCode(t, TransitionCodes.Fill))
        {
            for (int i = 0; i < 8; i += 2)
            {
                if (GetBiomeAtCoords(v + transitionDirections.ToArray()[i]).BiomeId.Equals(Id))
                {
                    t[i / 2] = 1;
                }
                else
                {
                    t[i / 2] = 0;
                }
            }

            //In Corners   
            if (CompareCode(t, TransitionCodes.ICorner_Back_Left))
            {
                return b.InCorner_Back_Left;
            }

            if (CompareCode(t, TransitionCodes.ICorner_Back_Right))
            {
                return b.InCorner_Back_Right;
            }

            if (CompareCode(t, TransitionCodes.ICorner_Front_Left))
            {
                return b.InCorner_Front_Left;
            }

            if (CompareCode(t, TransitionCodes.ICorner_Front_Right))
            {
                return b.InCorner_Front_Right;
            }

            return b.MainTile;
        }

        //OCorners
        if (CompareCode(t, TransitionCodes.OCorner_Back_Left))
        {
            return b.OutCorner_Back_Left;
        }

        if (CompareCode(t, TransitionCodes.OCorner_Back_Right))
        {
            return b.OutCorner_Back_Right;
        }

        if (CompareCode(t, TransitionCodes.OCorner_Front_Left))
        {
            return b.OutCorner_Front_Left;
        }

        if (CompareCode(t, TransitionCodes.OCorner_Front_Right))
        {
            return b.OutCorner_Front_Right;
        }

        //Ledges
        if (CompareCode(t, TransitionCodes.Ledge_Back))
        {
            return b.Ledge_Back;
        }

        if (CompareCode(t, TransitionCodes.Ledge_Front))
        {
            return b.Ledge_Front;
        }

        if (CompareCode(t, TransitionCodes.Ledge_Left))
        {
            return b.Ledge_Left;
        }

        if (CompareCode(t, TransitionCodes.Ledge_Right))
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
