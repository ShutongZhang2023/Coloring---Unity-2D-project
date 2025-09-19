using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressGate : MonoBehaviour
{
    public bool isOpened = false;
    public GameObject gateObject;
    public List<PressableButton> pressableButtons;

    private void Update()
    {
        if (isOpened) return;

        bool allPressed = true;
        foreach (var button in pressableButtons)
        {
            if (!button.isPressed)
            {
                allPressed = false;
                break;
            }
        }
        if (allPressed && !isOpened)
        {
            OpenGate();
        }
    }

    private void OpenGate()
    {
        isOpened = true;
        gateObject.SetActive(false);
        foreach (var button in pressableButtons)
        {
            button.LockButton();
        }
    }

}
