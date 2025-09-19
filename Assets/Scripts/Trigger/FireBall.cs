using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private bool hasCollided = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!hasCollided)
        {
            Vector2 velocity = rb.velocity;
            if (velocity.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided) return;
        hasCollided = true;
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 0.5f);
    }

}