using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectName
{
    Summon,
    Stun,
    Slash,
    Bite,
    Blood,
}
[Serializable]
public class EffectData
{
    public EffectName _effectName;
    public Transform vfxPrefab;
}
public class VisualEffectManager : Singeleton<VisualEffectManager>
{
    public EffectData[] effectData;
    
    public void CallEffect(EffectName effectName,Transform instancePosition)
    {
        GameObject vfx = Instantiate(GetEffect(effectName).vfxPrefab.gameObject, instancePosition.position,Quaternion.identity);
        Destroy(vfx,1f);
    }

    private EffectData GetEffect(EffectName name)
    {
        return Array.Find(effectData, s => s._effectName == name);
    }
}
