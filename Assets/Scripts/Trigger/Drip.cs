using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drip : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(DestroyAfterDelay(0.5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
