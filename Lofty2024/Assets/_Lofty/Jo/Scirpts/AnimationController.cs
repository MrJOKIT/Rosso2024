using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(PlayAnimationAndRemove());
    }

    private IEnumerator PlayAnimationAndRemove()
    {
        
        animator.SetTrigger("PlayAnimation"); 

       
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

       
        Destroy(animator);
    }
}