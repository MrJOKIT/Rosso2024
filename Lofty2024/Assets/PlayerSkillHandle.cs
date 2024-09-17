using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SkillSlot
{
    public Image skillImage;
}
public class PlayerSkillHandle : MonoBehaviour
{
    [Header("Skill Point")] 
    [SerializeField] private int skillPoint;

    [Space(10)] 
    [Header("Skill Slot")] 
    [SerializeField] private List<SkillSlot> _skillSlots;
}
