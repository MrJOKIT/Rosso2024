using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RandomRateProfile",fileName = "RandomRateData",order = 0)]
public class RandomRateProfile : ScriptableObject
{
    [Range(0, 100f)] public float labigonRate;
    [Range(0, 100f)] public float ironRate;
    [Range(0, 100f)] public float wizardRate;
}
