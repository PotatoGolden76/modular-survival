using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Biome", menuName = "Biome")]
public class Biome : ScriptableObject
{

    [Header(header:"Biome Variables")]
    public string BiomeId;

    [Range(0f, 10f)]
    public float MinimumElevation;

    [Header(header: "Biome Tiles")]
    public WeightedBiomeTile mainTile;

    [Space]
    public WeightedBiomeTile Ledge_Front = null;
    public WeightedBiomeTile Ledge_Back = null;
    public WeightedBiomeTile Ledge_Left = null;
    public WeightedBiomeTile Ledge_Right = null;

    [Space]
    public WeightedBiomeTile InCorner_Back_Left = null;
    public WeightedBiomeTile InCorner_Back_Right = null;
    public WeightedBiomeTile InCorner_Front_Left = null;
    public WeightedBiomeTile InCorner_Front_Right = null;

    [Space]
    public WeightedBiomeTile OutCorner_Back_Left = null;
    public WeightedBiomeTile OutCorner_Back_Right = null;
    public WeightedBiomeTile OutCorner_Front_Left = null;
    public WeightedBiomeTile OutCorner_Front_Right = null;

}
