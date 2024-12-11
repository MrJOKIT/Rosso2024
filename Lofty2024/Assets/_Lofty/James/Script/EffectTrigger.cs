using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    public Transform vfx;

    public void ActiveVfx()
    {
        Transform effect = Instantiate(vfx,transform.position,Quaternion.identity); 
        Destroy(effect.gameObject,1f);
    }
}
