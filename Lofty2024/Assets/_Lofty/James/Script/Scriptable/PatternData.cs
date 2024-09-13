using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pattern",fileName = "PatternData",order = 1)]
public class PatternData : ScriptableObject
{
    public AbilityType abilityType;
    public Transform patternPrefab;
}
