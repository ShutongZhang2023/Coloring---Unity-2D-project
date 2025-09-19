using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TubeButton : MonoBehaviour
{
    public int ColorId;
    public SpriteRenderer buttonRenerer;
    public Transform buttonTransform;
    public UnityEvent onPressed;

    [Header("Color")]
    public Color normalColor = Color.white;
    public Color blue;
    public Color red;
    public Color yellow;
    private Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    private float moveSpeed = 5f;

    public bool isActivate = false;

    private Vector3 initialPosition;
    public bool isPressed = false;
    private int pressCount = 0;

    void Start()
    {
        if (buttonTransform != null)
            initialPosition = buttonTransform.localPosition;
    }

    private void Update()
    {
        Vector3 targetPosition = isPressed ? initialPosition + pressedOffset : initialPosition;
        buttonTransform.localPosition = Vector3.Lerp(buttonTransform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            pressCount++;
            if (!isPressed)
            {
                isPressed = true;
                if (PlayerBasic.Instance.currentColorId == 1)
                {
                    buttonRenerer.color = red;
                }
                else if (PlayerBasic.Instance.currentColorId == 2)
                {
                    buttonRenerer.color = blue;
                }
                else if (PlayerBasic.Instance.currentColorId == 3)
                {
                    buttonRenerer.color = yellow;
                }
                if (PlayerBasic.Instance.currentColorId == ColorId)
                {
                    isActivate = true;
                    onPressed.Invoke();
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            pressCount--;
            if (pressCount <= 0)
            {
                isPressed = false;
                buttonRenerer.color = normalColor;
                isActivate = false;
            }
        }
    }
}
