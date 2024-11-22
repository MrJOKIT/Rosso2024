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
    Revive,
    CoinReward,
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
    
    public void CallEffect(EffectName effectName,Transform instancePosition,float lifeTime)
    {
        GameObject vfx = Instantiate(GetEffect(effectName).vfxPrefab.gameObject, instancePosition.position,Quaternion.identity);
        Destroy(vfx,lifeTime);
    }

    private EffectData GetEffect(EffectName name)
    {
        return Array.Find(effectData, s => s._effectName == name);
    }
}
