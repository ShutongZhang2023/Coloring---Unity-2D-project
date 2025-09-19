using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTrigger : MonoBehaviour
{
    public int colorId = 0;
    public int triggerId;
    private Animator animator;
    public ColorGate connectedGate;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (connectedGate == null)
        {
            connectedGate = GetComponent<ColorGate>();
        }
        switch (colorId)
        {
            case 1: // Red
                animator.Play("CT_red");
                break;
            case 2: // Blue
                animator.Play("CT_blue");
                break;
            case 3: // Yellow
                animator.Play("CT_yellow");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fireball")) {
            colorId = 1;
            animator.Play("CT_red");
            connectedGate.UpdateColor(triggerId, colorId);
        }

        if (collision.CompareTag("Ice"))
        {
            colorId = 2;
            animator.Play("CT_blue");
            connectedGate.UpdateColor(triggerId, colorId);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            colorId = PlayerBasic.Instance.currentColorId;
            switch (colorId) {
                case 1: // Red
                    animator.Play("CT_red");
                    connectedGate.UpdateColor(triggerId, colorId);
                    break;
                case 2: // Blue
                    animator.Play("CT_blue");
                    connectedGate.UpdateColor(triggerId, colorId);
                    break;
                case 3: // Yellow
                    animator.Play("CT_yellow");
                    connectedGate.UpdateColor(triggerId, colorId);
                    break;
            }
            
        }
    }


}
