using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileColorVariant
{
    public TileBase originalTile;
    public TileBase redVersion;
    public TileBase blueVersion;
    public TileBase yellowVersion;
}

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private Rigidbody2D rb;
    public Vector2 inputDirection;
    private PhysicsCheck physicsCheck;
    private Animator animator;

    //inverse mapping
    private Dictionary<TileBase, TileBase> tileToOriginalDict;

    //coloring attribute
    public Color currentColor { get; private set; }
    private SpriteRenderer spriteRenderer;

    [Header("Color Mapping for Tiles")]
    public List<TileColorVariant> tileVariants;
    private Dictionary<TileBase, Dictionary<int, TileBase>> tileColorDict;

    [Header("Basic parameter")]
    public float speed;
    public float jumpForce;
    public int currentColorId = 1;
    public float colorConflictPushForce = 5f; 

    [Header("Coloring parameter")]
    public Tilemap tilemap;
    private TileBase targetTile;


    private void Awake()
    {
        //animator
        animator = GetComponent<Animator>();
        //control
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        inputControl = new PlayerInputControl();
        inputControl.GamePlay.Jump.started += Jump; //注册，将Jump方法添加到按键按下的那一刻执行
        inputControl.GamePlay.Red.started += SetColorRed;
        inputControl.GamePlay.Blue.started += SetColorBlue;
        inputControl.GamePlay.Yellow.started += SetColorYellow;

        //color dic
        tileColorDict = new Dictionary<TileBase, Dictionary<int, TileBase>>();
        tileToOriginalDict = new Dictionary<TileBase, TileBase>();
        foreach (var variant in tileVariants)
        {
            tileColorDict[variant.originalTile] = new Dictionary<int, TileBase>
            {
                { 1, variant.redVersion },
                { 2, variant.blueVersion },
                { 3, variant.yellowVersion }
            };
            tileToOriginalDict[variant.originalTile] = variant.originalTile;
            if (variant.redVersion != null) tileToOriginalDict[variant.redVersion] = variant.originalTile;
            if (variant.blueVersion != null) tileToOriginalDict[variant.blueVersion] = variant.originalTile;
            if (variant.yellowVersion != null) tileToOriginalDict[variant.yellowVersion] = variant.originalTile;
        }
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl?.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

        Debug.Log("IsGround: " + physicsCheck.isGround);

    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        //face direction
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;
        transform.localScale = new Vector3(faceDir, 1, 1);
    }

    //jump
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void SetColorRed(InputAction.CallbackContext obj)
    {
        currentColorId = 1;
        animator.SetInteger("ColorId", currentColorId);
        TryRepaintCurrentTile();
    }

    private void SetColorBlue(InputAction.CallbackContext obj)
    {
        currentColorId = 2;
        animator.SetInteger("ColorId", currentColorId);
        TryRepaintCurrentTile();
    }

    private void SetColorYellow(InputAction.CallbackContext obj)
    {
        currentColorId = 3;
        animator.SetInteger("ColorId", currentColorId);
        TryRepaintCurrentTile();
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector3 contactPoint = contact.point + contact.normal * -0.05f;
            Vector3Int cellPos = tilemap.WorldToCell(contactPoint);

            Debug.DrawRay(contact.point, contact.normal, Color.red, 1f);

            TileBase currentTile = tilemap.GetTile(cellPos);

            //change tile color
            if (currentTile != null && tileToOriginalDict.ContainsKey(currentTile))
            {
                TileBase originalTile = tileToOriginalDict[currentTile];
                TileBase newTile = tileColorDict[originalTile][currentColorId];

                //deal with the logic before color changing
                HandlePrePaintLogic(currentTile, originalTile, contact.normal);

                //change color
                if (newTile != null && newTile != currentTile)
                {
                    tilemap.SetTile(cellPos, newTile);
                }
            }
        }
    }

    private void TryRepaintCurrentTile()
    {
        Vector3 origin = transform.position + Vector3.down * 0.1f;
        Vector3Int cellPos = tilemap.WorldToCell(origin);
        TileBase currentTile = tilemap.GetTile(cellPos);

        if (currentTile != null && tileToOriginalDict.ContainsKey(currentTile))
        {
            TileBase originalTile = tileToOriginalDict[currentTile];
            TileBase newTile = tileColorDict[originalTile][currentColorId];

            if (newTile != null && newTile != currentTile)
            {
                tilemap.SetTile(cellPos, newTile);
            }
        }
    }

    private void HandlePrePaintLogic(TileBase currentTile, TileBase originalTile, Vector2 contactNormal)
    {
        // 获取 tile 当前的颜色 id
        int tileColorId = -1;

        if (tileColorDict[originalTile][1] == currentTile) tileColorId = 1; // red
        if (tileColorDict[originalTile][2] == currentTile) tileColorId = 2; // blue
        if (tileColorDict[originalTile][3] == currentTile) tileColorId = 3; // yellow

        //红-黄逻辑
        bool conflict =
            (currentColorId == 1 && tileColorId == 3) || 
            (currentColorId == 3 && tileColorId == 1); 

        if (conflict)
        {
            float pushForce = colorConflictPushForce;
            rb.AddForce(contactNormal * pushForce, ForceMode2D.Impulse);
            Debug.Log("Color conflict detected! Pushing player away.");
        }

    }


}
