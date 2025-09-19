using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDetect : MonoBehaviour
{
    private IceBlock parentIceBlock;
    private float redPlayerTimer = 0f;
    public float meltTimeByProximity = 1f;

    void Awake()
    {
        parentIceBlock = GetComponentInParent<IceBlock>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = PlayerBasic.Instance;
            if (player.currentColorId == 1)
            {
                redPlayerTimer += Time.deltaTime;
                if (redPlayerTimer >= meltTimeByProximity)
                {
                    parentIceBlock.TriggerMelt();
                }
            }
            else
            {
                redPlayerTimer = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            redPlayerTimer = 0;
        }
    }
}
