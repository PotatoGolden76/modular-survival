using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generator : MonoBehaviour
{

    public List<Biome> biomes;

    [Range(0.001f, 0.1f)]
    public float frequency = 0.01f;
    public int seed = 23127108;

    private FastNoise TerrainNoise;
    private FastNoise VariationNoise;

    public void initialiseNoise()
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

    private void Start()
    {
        initialiseNoise();
        sortBiomes();

        foreach(Biome b in biomes)
        {
            Debug.Log(b.BiomeId);
        }
    }

    private void sortBiomes()
    {
        biomes.Sort(biomeCompare);
    }

    private int biomeCompare(Biome x, Biome y)
    {
        if(x.MinimumElevation > y.MinimumElevation)
        {
            return -1;
        } else
        {
            return 1;
        }
    }

    public void GenerateChunk(ref Chunk c)
    {
        for(int i = c.tilemap.origin.x; i < c.tilemap.origin.x + Chunk.CHUNK_SIZE_X; i++)
        {
            for (int j = c.tilemap.origin.y; j < c.tilemap.origin.y + Chunk.CHUNK_SIZE_Y; j++)
            {
                float val = TerrainNoise.GetNoise(c.corner.x + i, c.corner.y + j);

                if(val < 0)
                {
                    c.tilemap.SetTile(new Vector3Int(i, j, 0), biomes.ToArray()[0].mainTile);
                } else
                {
                    c.tilemap.SetTile(new Vector3Int(i, j, 0), biomes.ToArray()[1].mainTile);
                }
            }
        }
    }

    public float GetVariationValue(int i, int j)
    {
        return VariationNoise.GetWhiteNoise(i, j);
    }

    private float GetTerrainValue(int i, int j)
    {
        return TerrainNoise.GetNoise(i, j);
    }

    private Biome GetBiomeAtCoords(int i, int j)
    {
        float val = GetTerrainValue(i, j);

        foreach(Biome b in biomes)
        {
            if(val > b.MinimumElevation)
            {
                return b;
            }
        }

        Console.Log("No biome at elevation: " + val);
        return null;
    }

}
