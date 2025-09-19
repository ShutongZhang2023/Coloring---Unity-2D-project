using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeltIceDetector : MonoBehaviour
{
    public IceBlock iceBlock;
    public bool isIce = false;
    public UnityEvent OnMelt;

    private bool lastMeltingState = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ice"))
        {
            iceBlock = collision.GetComponent<IceBlock>();
            isIce = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ice"))
        {
            iceBlock = null;
            isIce = false;
        }
    }

    private void Update()
    {
        if (!isIce) return;

        bool current = iceBlock.isMelting;

        if (!lastMeltingState && current)
        {
            OnMelt?.Invoke();
        }

        lastMeltingState = current;
    }
}
