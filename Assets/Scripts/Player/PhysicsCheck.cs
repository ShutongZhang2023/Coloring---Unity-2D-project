using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    public bool isGround;
    public bool isSafe;

    public float checkRaduis;
    public LayerMask groundLayer;

    public Vector2 lastContactNormal;
    private PlayerBasic playerBasic;
    public bool isPulsed;
    private bool isLeftGround;
    private bool isRightGround;

    [Header("µ×²¿¼ì²âµãÆ«ÒÆ")]
    public Vector2 bottomOffset;
    public Vector2 leftBottomOffset;
    public Vector2 rightBottomOffset;
    // Start is called before the first frame update
    void Start()
    {
        playerBasic = GetComponent<PlayerBasic>();
    }

    // Update is called once per frame
    void Update()
    {
        Check();
    }

    public void Check() {
        //check ground
        //isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);

        //check safe
        isLeftGround = Physics2D.OverlapCircle(
            (Vector2)transform.position + leftBottomOffset,
            checkRaduis,
            groundLayer
        );

        isRightGround = Physics2D.OverlapCircle(
            (Vector2)transform.position + rightBottomOffset,
            checkRaduis,
            groundLayer
        );

        isSafe = isLeftGround && isRightGround;
        isGround = isLeftGround || isRightGround;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftBottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightBottomOffset, checkRaduis);
    }

    //collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            lastContactNormal = collision.contacts[0].normal;
        }
        isPulsed = false; // ÖØÖÃÂö³å×´Ì¬
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            lastContactNormal = collision.contacts[0].normal;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        playerBasic.currentInkColor = 0;

    }
}
