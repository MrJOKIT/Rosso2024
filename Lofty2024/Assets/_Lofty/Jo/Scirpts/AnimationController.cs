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
        // เริ่มเล่น Animation
        animator.SetTrigger("PlayAnimation"); // เปลี่ยนให้ตรงกับ Trigger ที่คุณตั้งไว้

        // รอจนกว่า Animation จะเล่นเสร็จ
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // ลบ Animation Component
        Destroy(animator);
    }
}