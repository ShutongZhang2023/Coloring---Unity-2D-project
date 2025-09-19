using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (CompareTag("Water"))
        {
            if (PlayerBasic.Instance.currentColorId == 2) return;
            other.GetComponent<Character>()?.TakeDamage(this);
        }
        else {
            other.GetComponent<Character>()?.TakeDamage(this);
        }
    }
}
