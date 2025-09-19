using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Animator animator;
    private bool isDestroyed = false;
    public bool isMelting = false;

    public float meltTimeByProximity = 1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        int layerIndex = 0; // 默认第 0 层
        int stateHash = Animator.StringToHash("ice_start");

        if (animator.HasState(layerIndex, stateHash))
        {
            animator.Play(stateHash);
        }
    }

    public void TriggerMelt()
    {
        isMelting = true;
        if (isDestroyed) return;
        isDestroyed = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetBool("isMelting", isMelting);
    }

    public void TriggerDisappear()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        animator.SetBool("isDisappearing", isDestroyed);
    }

    // 在动画最后一帧用 Animation Event 调用此函数
    public void DestroySelf()
    {
        Destroy(gameObject, 0.2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fireball"))
        {
            isMelting = true;
            TriggerMelt();
        }
    }
}
