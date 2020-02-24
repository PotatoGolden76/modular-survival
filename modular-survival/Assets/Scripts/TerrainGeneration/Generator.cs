using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Generator
{
    #region Noise Settings
    [Range(0.001f, 0.1f)]
    private static float frequency = 0.001f;
    private static int seed = 23127108;

    private static FastNoise TerrainNoise;
    private static FastNoise VariationNoise;
    #endregion

    private static void initialiseNoise()
    {
        TerrainNoise = new FastNoise();
        VariationNoise = new FastNoise();

        TerrainNoise.SetNoiseType(FastNoise.NoiseType.Simplex);
        TerrainNoise.SetSeed(seed);
        TerrainNoise.SetFrequency(frequency);

        VariationNoise.SetNoiseType(FastNoise.NoiseType.WhiteNoise);
        VariationNoise.SetSeed(seed);
        VariationNoise.SetFrequency(frequency);

        Console.Log("Noise settings:");
        Console.Log("\tFrequency: " + frequency);
        Console.Log("\tSeed: " + seed);
    }

    public static void Start()
    {
        initialiseNoise();
    }

    public static void GenerateChunk(ref Chunk c)
    {
        for (int i = c.tilemap.origin.x; i < c.tilemap.origin.x + Chunk.CHUNK_SIZE_X; i++)
        {
            for (int j = c.tilemap.origin.y; j < c.tilemap.origin.y + Chunk.CHUNK_SIZE_Y; j++)
            {
                c.tilemap.SetTile(new Vector3Int(i, j, 0), getTile(new Vector2(i + c.corner.x, j + c.corner.y)));
            }
        }
    }

    public static float GetVariationValue(Vector2 v, int totalWeight)
    {
        return (VariationNoise.GetWhiteNoise(v.x, v.y) * totalWeight/2) + totalWeight/2;
    }

    private static float GetTerrainValue(Vector2 v)
    {
        return (TerrainNoise.GetNoise(v.x, v.y) * 5) + 5;
    }

    private static Biome GetBiomeAtCoords(Vector2 v)
    {
        float val = GetTerrainValue(v);

        foreach (Biome b in GameRegistry.getBiomes())
        {
            if (val > b.MinimumElevation)
            {
                return b;
            }
        }

        Console.Log("No biome at elevation: " + val);
        return null;
    }

    private static WeightedBiomeTile getTile(Vector2 v)
    {
        Biome b = GetBiomeAtCoords(v);
        string Id = b.BiomeId;

        List<Vector2Int> directions = new List<Vector2Int>() { new Vector2Int(-1, 1),
                                                         new Vector2Int(0, 1),
                                                         new Vector2Int(1, 1), 
                                                         new Vector2Int(1, 0), 
                                                         new Vector2Int(1, -1), 
                                                         new Vector2Int(0, -1), 
                                                         new Vector2Int(-1, -1), 
                                                         new Vector2Int(-1, 0)};

        byte[] t = new byte[4];

        for(int i = 1; i < 8; i += 2)
        {
            if(GetBiomeAtCoords(v + directions.ToArray()[i]).BiomeId.Equals(Id)) {
                t[i / 2] = 1;
            } else
            {
                t[i / 2] = 0;
            }
        }

        bool check = true;
        //Fill and OCorners
        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.Fill[i])
            {
                check = false;
                break;
            }
        }

        if (check)
        {
            for (int i = 0; i < 8; i += 2)
            {
                if (GetBiomeAtCoords(v + directions.ToArray()[i]).BiomeId.Equals(Id))
                {
                    t[i / 2] = 1;
                }
                else
                {
                    t[i / 2] = 0;
                }
            }

            //Ledges
            for (int i = 0; i < 4; i++)
            {
                check = true;
                if (t[i] != TransitionCodes.ICorner_Back_Left[i])
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                return b.InCorner_Back_Left;
            }

            for (int i = 0; i < 4; i++)
            {
                check = true;
                if (t[i] != TransitionCodes.ICorner_Back_Right[i])
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                return b.InCorner_Back_Right;
            }

            for (int i = 0; i < 4; i++)
            {
                check = true;
                if (t[i] != TransitionCodes.ICorner_Front_Left[i])
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                return b.InCorner_Front_Left;
            }

            for (int i = 0; i < 4; i++)
            {
                check = true;
                if (t[i] != TransitionCodes.ICorner_Front_Right[i])
                {
                    check = false;
                    break;
                }
            }

            if (check)
            {
                return b.InCorner_Front_Right;
            }

            return b.mainTile;
        }

        //InCorners
        for (int i = 0; i < 4; i++)
        {
            check = true;
            if(t[i] != TransitionCodes.OCorner_Back_Left[i])
            {
                check = false;
                break;
            }           
        }

        if(check)
        {
            return b.OutCorner_Back_Left;
        }

        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.OCorner_Back_Right[i])
            {
                check = false;
                break;
            }
        }

        if(check)
        {
            return b.OutCorner_Back_Right;
        }

        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.OCorner_Front_Left[i])
            {
                check = false;
                break;
            }
        }

        if(check)
        {
            return b.OutCorner_Front_Left;
        }

        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.OCorner_Front_Right[i])
            {
                check = false;
                break;
            }
        }

        if(check)
        {
            return b.OutCorner_Front_Right;
        }

        //Ledges
        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.Ledge_Back[i])
            {
                check = false;
                break;
            }
        }

        if (check)
        {
            return b.Ledge_Back;
        }

        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.Ledge_Front[i])
            {
                check = false;
                break;
            }
        }

        if (check)
        {
            return b.Ledge_Front;
        }

        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.Ledge_Left[i])
            {
                check = false;
                break;
            }
        }

        if (check)
        {
            return b.Ledge_Left;
        }

        for (int i = 0; i < 4; i++)
        {
            check = true;
            if (t[i] != TransitionCodes.Ledge_Right[i])
            {
                check = false;
                break;
            }
        }

        if (check)
        {
            return b.Ledge_Right;
        }

        return b.mainTile;
    }

}
