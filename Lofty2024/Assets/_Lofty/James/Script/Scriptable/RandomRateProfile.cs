using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RandomRateProfile",fileName = "RandomRateData",order = 0)]
public class RandomRateProfile : ScriptableObject
{
    [Range(0,1)] public float pawnRate;
    [Range(0,1)] public float rookRate;
    [Range(0,1)] public float knightRate;
    [Range(0,1)] public float bishopRate;
    [Range(0,1)] public float queenRate;
    [Range(0,1)] public float kingRate;
}
