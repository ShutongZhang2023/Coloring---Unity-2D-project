using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour, IInteractable
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void TriggerAction()
    {
        animator.SetBool("isEat", true);
        AudioManager.Instance.PlayEatSFX();
        Character.Instance.Heal(1);
        UIController.Instance.UpdateHealthUI();
        Destroy(gameObject);
    }

}
