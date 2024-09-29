using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skill",fileName = "SkillData",order = 0)]
public class SkillData : ScriptableObject
{
    public string skillName;
    public CurseType skillCurse;
    public int skillCost;
    public Sprite skillImage;
    public Transform skillPattern;
}
