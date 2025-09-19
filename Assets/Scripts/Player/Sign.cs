using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private Animator animator;
    public GameObject signSprite;
    private bool canPress;
    public PlayerInputControl inputControl;
    private IInteractable interactable;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputControl = new PlayerInputControl();
        inputControl.GamePlay.Confirm.started += OnConfirm;
    }
    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        signSprite.transform.localScale = transform.localScale;
        signSprite.SetActive(canPress);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canPress = true;
            interactable = collision.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canPress = false;
        }
    }

    //½»»¥
    private void OnConfirm(InputAction.CallbackContext obj)
    {
        Debug.Log("interactable: " + interactable);
        if (canPress) {
            interactable.TriggerAction();
        }
    }
}
