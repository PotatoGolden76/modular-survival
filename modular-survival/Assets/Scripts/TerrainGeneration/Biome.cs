﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(fileName = "New Biome", menuName = "Biome")]
public class Biome : ScriptableObject, IComparable
{

    [Header(header: "Biome Variables")]
    public string BiomeId;
    public bool IsStatic;

    [Range(0f, 10f)]
    public float MinimumElevation;
    [Range(0f, 10f)]
    public float TreeDensity;

    [Header(header: "Biome Tiles")]
    public TerrainTile MainTile;

    [Space]
    public TerrainTile Ledge_Front = null;
    public TerrainTile Ledge_Back = null;
    public TerrainTile Ledge_Left = null;
    public TerrainTile Ledge_Right = null;

    [Space]
    public TerrainTile InCorner_Back_Left = null;
    public TerrainTile InCorner_Back_Right = null;
    public TerrainTile InCorner_Front_Left = null;
    public TerrainTile InCorner_Front_Right = null;

    [Space]
    public TerrainTile OutCorner_Back_Left = null;
    public TerrainTile OutCorner_Back_Right = null;
    public TerrainTile OutCorner_Front_Left = null;
    public TerrainTile OutCorner_Front_Right = null;

    [Space]
    public Tile tree = null;
    public List<BiomeDecoration> decorations = new List<BiomeDecoration>();

    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            return 1;
        }

        Biome b = obj as Biome;
        if (this.MinimumElevation > b.MinimumElevation)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

}

public static class TransitionCodes
{
    public static readonly byte[] OCorner_Back_Left = { 0, 1, 1, 0 };
    public static readonly byte[] OCorner_Back_Right = { 0, 0, 1, 1 };
    public static readonly byte[] OCorner_Front_Left = { 1, 1, 0, 0 };
    public static readonly byte[] OCorner_Front_Right = { 1, 0, 0, 1 };

    public static readonly byte[] Ledge_Front = { 1, 1, 0, 1 };
    public static readonly byte[] Ledge_Back = { 0, 1, 1, 1 };
    public static readonly byte[] Ledge_Left = { 1, 1, 1, 0 };
    public static readonly byte[] Ledge_Right = { 1, 0, 1, 1 };

    public static readonly byte[] Fill = { 1, 1, 1, 1 };

    public static readonly byte[] ICorner_Back_Left = { 1, 1, 0, 1 };
    public static readonly byte[] ICorner_Back_Right = { 1, 1, 1, 0 };
    public static readonly byte[] ICorner_Front_Left = { 1, 0, 1, 1 };
    public static readonly byte[] ICorner_Front_Right = { 0, 1, 1, 1 };
}