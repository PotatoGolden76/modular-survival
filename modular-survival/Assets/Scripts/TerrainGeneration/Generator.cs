using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Generator
{
    #region Noise Settings
    [Range(0.001f, 0.1f)]
    private static float biomeFrequency = 0.001f, treeFrequency = 0.005f;
    private static int seed = 23127108;

    private static FastNoise TerrainNoise;
    private static FastNoise TreeNoise;
    #endregion

    private static void initialiseNoise()
    {
        TerrainNoise = new FastNoise();
        TreeNoise = new FastNoise();

        TerrainNoise.SetNoiseType(FastNoise.NoiseType.Simplex);
        TerrainNoise.SetSeed(seed);
        TerrainNoise.SetFrequency(biomeFrequency);

        TreeNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        TreeNoise.SetFractalOctaves(7);
        TreeNoise.SetSeed(seed);
        TreeNoise.SetFrequency(treeFrequency);
    }

    public static void Start()
    {
        initialiseNoise();
    }

    public static void GenerateChunk(ref Chunk c)
    {
        for (int i = c.ground_tilemap.origin.x; i < c.ground_tilemap.origin.x + Chunk.CHUNK_SIZE_X; i++)
        {
            for (int j = c.ground_tilemap.origin.y; j < c.ground_tilemap.origin.y + Chunk.CHUNK_SIZE_Y; j++)
            {
                c.ground_tilemap.SetTile(new Vector3Int(i, j, 0), GetTerrainTileAtCoords(new Vector2(i + c.corner.x, j + c.corner.y)));
               
                if(c.ground_tilemap.GetTile(new Vector3Int(i, j, 0)).Equals(GetBiomeAtCoords(new Vector2(i + c.corner.x, j + c.corner.y)).mainTile))
                {
                    c.object_tilemap.SetTile(new Vector3Int(i, j, 0), GetTreeAtCoords(new Vector2(i + c.corner.x, j + c.corner.y)));
                }

            }
        }
    }

    public static float GetTreeNoiseValue(Vector2 v)
    {
        return TreeNoise.GetNoise(v.x, v.y);
    }

    private static float GetTerrainNoiseValue(Vector2 v)
    {
        return (TerrainNoise.GetNoise(v.x, v.y) * 5) + 5;
    }

    private static Biome GetBiomeAtCoords(Vector2 v)
    {
        float val = GetTerrainNoiseValue(v);

        foreach (Biome b in GameRegistry.getBiomes())
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
        if(val > 0)
        {
            return GetBiomeAtCoords(v).tree;
        }

        return null;
    }


    //TODO: clean this shit up, it can be done so much easier
    private static BiomeTile GetTerrainTileAtCoords(Vector2 v)
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
