using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressableButton : MonoBehaviour
{
    [Header("Vision")]
    public Transform buttonTop;
    public Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    public float moveSpeed = 5f;

    [Header("Color")]
    public SpriteRenderer buttonRenderer;
    public Color normalColor = Color.white;
    public Color blue;
    public Color red;
    public Color yellow;

    [Header("Event")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    [Header("Gate")]
    public bool isLocked = false;

    private Vector3 initialPosition;
    public bool isPressed = false;
    private int pressCount = 0;

    void Start()
    {
        if (buttonTop != null)
            initialPosition = buttonTop.localPosition;

        if (buttonRenderer != null)
            buttonRenderer.color = normalColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isLocked) return;
        if (other.CompareTag("Player"))
        {
            pressCount++;
            if (!isPressed)
            {
                isPressed = true;
                onPressed.Invoke();
                if (PlayerBasic.Instance.currentColorId == 1) {
                    buttonRenderer.color = red;
                }
                else if (PlayerBasic.Instance.currentColorId == 2)
                {
                    buttonRenderer.color = blue;
                }
                else if (PlayerBasic.Instance.currentColorId == 3)
                {
                    buttonRenderer.color = yellow;
                }
            }
        }

        else if (other.CompareTag("Ice"))
        {
            pressCount++;
            if (!isPressed)
            {
                isPressed = true;
                onPressed.Invoke();
                buttonRenderer.color = blue;
            }
        }

        else if (other.CompareTag("Fireball"))
        {
            pressCount++;
            if (!isPressed)
            {
                isPressed = true;
                onPressed.Invoke();
                buttonRenderer.color = red;
            }
        }

        else if (other.CompareTag("Drip"))
        {
            pressCount++;
            if (!isPressed)
            {
                isPressed = true;
                onPressed.Invoke();
                buttonRenderer.color = blue;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isLocked) return;
        if (other.CompareTag("Player") || other.CompareTag("Ice") || other.CompareTag("Fireball") || other.CompareTag("Drip"))
        {
            pressCount--;
            if (pressCount <= 0)
            {
                isPressed = false;
                onReleased.Invoke();
                buttonRenderer.color = normalColor;
            }
        }
    }

    void Update()
    {
            Vector3 targetPos = isPressed ? initialPosition + pressedOffset : initialPosition;
            buttonTop.localPosition = Vector3.Lerp(buttonTop.localPosition, targetPos, Time.deltaTime * moveSpeed);
    }

    public void LockButton()
    {
        isLocked = true;
    }
}
