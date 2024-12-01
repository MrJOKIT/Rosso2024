using System;
using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using GD.MinMaxSlider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class SkillSlot
{
    [Header("Data")]
    public SkillData skillData;
    
    [Space(10)]
    [Header("UI")]
    public Image skillImage;
    
}
public class PlayerSkillHandle : MonoBehaviour
{
    [Header("Skill Setting")] 
    [SerializeField] private Transform skillParent;
    public GameObject makeSureUI;

    [Space(10)] 
    [Header("Skill Point")] 
    [GD.MinMaxSlider.MinMaxSlider(0,10)][SerializeField] private Vector2Int minMaxSkillPoint;
    [SerializeField] private int skillPoint;

    [Space(10)] 
    [Header("Skill Slot")] 
    [SerializeField] private List<SkillSlot> _skillSlots;
    [SerializeField] private List<SkillData> _skillDatas;

    [Space(10)] [Header("Skill Point UI")] 
    public TextMeshProUGUI skillPointText;
    public TextMeshProUGUI maxSkillPointText;
    
    [Space(10)] 
    [ReadOnly] public int slotSelect;
    [ReadOnly] public Transform currentSkill; 

    private void Awake()
    {
        makeSureUI.SetActive(false);
        RandomSetSkill();
    }

    private void RandomSetSkill()
    {
        foreach (SkillSlot slot in _skillSlots)
        {
            if (_skillDatas.Count == 0)
            {
                slot.skillImage.gameObject.SetActive(false);
            }
            else
            {
                SkillData randomSkill = _skillDatas[Random.Range(0, _skillDatas.Count - 1)];
                slot.skillData = randomSkill;
                slot.skillImage.sprite = randomSkill.skillImage;
                _skillDatas.Remove(randomSkill);
            }
        }
    }

    public void AddSkillPoint(int count)
    {
        if (skillPoint >= minMaxSkillPoint.y + GetComponent<PlayerArtifact>().SkillPoint)
        {
            return;
        }

        skillPoint += count;  
        SkillPointUiUpdate();
    }

    public void ResetSkillPoint()
    {
        skillPoint = minMaxSkillPoint.x;
        SkillPointUiUpdate();
    }
    
    public void UseSkill(int slotSkillIndex)
    {
        GetComponent<PlayerMovementGrid>().currentState = MovementState.Freeze;
        slotSelect = slotSkillIndex;
        if (skillPoint >= _skillSlots[slotSkillIndex].skillData.skillCost - GetComponent<PlayerArtifact>().SkillDiscount)
        {
            _skillSlots[slotSkillIndex].skillImage.GetComponent<Button>().interactable = false;
            currentSkill = Instantiate(_skillSlots[slotSkillIndex].skillData.skillPattern,skillParent);
        }
        makeSureUI.SetActive(true);
        SkillPointUiUpdate();
    }

    private void SkillPointUiUpdate()
    {
        skillPointText.text = "" + skillPoint;
        maxSkillPointText.text = "" + minMaxSkillPoint.y + GetComponent<PlayerArtifact>().SkillPoint;
    }

    [Button("Confirm Skill")]
    public void ConfirmSkill()
    {
        skillPoint -= _skillSlots[slotSelect].skillData.skillCost - GetComponent<PlayerArtifact>().SkillDiscount;
        currentSkill.GetComponent<SkillAction>().ActiveSkill();
        currentSkill = null;
        ClearSlot(slotSelect);
        GetComponent<PlayerMovementGrid>().currentState = MovementState.Idle;
        GetComponent<PlayerMovementGrid>().EndTurn();
    }

    [Button("Cancel Skill")]
    public void CancelSkill()
    {
        GetComponent<PlayerMovementGrid>().currentState = MovementState.Combat;
        Destroy(currentSkill.gameObject);
        currentSkill = null;
        _skillSlots[slotSelect].skillImage.GetComponent<Button>().interactable = true;
    }

    private void ClearSlot(int slotIndex)
    {
        _skillSlots[slotIndex].skillData = null;
        _skillSlots[slotIndex].skillImage.gameObject.SetActive(false);
    }
}
