using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    [SerializeField] private float dissolveTime = 0.75f;

    private SpriteRenderer spriteRenderer;
    private Material material;

    private int dissolveAmount = Shader.PropertyToID("_DissolveAmount");

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fireball"))
        {
            StartCoroutine(onFire());
        }
    }

    private IEnumerator onFire() {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0f, 1.1f, elapsedTime / dissolveTime);
            material.SetFloat(dissolveAmount, dissolveValue);
            yield return null;
        }
        Destroy(gameObject);
    }
}
