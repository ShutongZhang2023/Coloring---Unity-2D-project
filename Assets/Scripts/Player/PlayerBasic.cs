using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBasic : MonoBehaviour
{
    public static PlayerBasic Instance;
    public PlayerInputControl inputControl;
    private PhysicsCheck physicsCheck;
    public Vector2 inputDirection;
    private InkPainter inkPainter;
    private Character character;
    public Rigidbody2D rb;

    //logic about yellow and gravity flip
    private bool isInGravityZone = false;

    //animator
    private Animator animator;
    public RuntimeAnimatorController redCtrl;
    public RuntimeAnimatorController blueCtrl;
    public RuntimeAnimatorController yellowCtrl;

    //fall into water / get hurt
    private Vector3 safePosition;
    public bool inDanger = false;

    [Header("Basic parameter")]
    public float speed;
    public float jumpForce;
    public int currentColorId = 1;
    public int currentInkColor = 0;
    public float collisionPushForce = 30f;

    //for particles
    [SerializeField] private ParticleSystem dustParticles;

    //about ladder
    public bool isLadder = false;

    //Skill
    //火球技能
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private float chargeRate = 10f; // 每秒增长多少力度

    private float currentChargeTime = 0f;
    private bool isCharging = false;

    //collection system
    public List<int> collectionList;

    //movement in water
    public bool isInWater = false;
    [SerializeField] private float buoyancyLiftSpeed = 0.8f; // 浮力最低上升速度
    [SerializeField] private float maxBuoyancySpeed = 2f;    // 防止飞出水面太快
    [SerializeField] private float buoyancyRampUpTime = 1f; // 浮力从 0 到 100% 所需时间
    public float buoyancyTimer = 0f;

    //ice block
    [Header("冰块相关")]
    public GameObject iceBlockPrefab;
    public float maxIceSpawnDistance = 5f;
    private GameObject currentIceBlock;
    public GameObject iceRangeCircle;

    private void Awake()
    {
        Instance = this;
        //animator
        animator = GetComponent<Animator>();

        //health
        character = GetComponent<Character>();

        //control
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        
        //input
        inputControl = new PlayerInputControl();
        inputControl.GamePlay.Jump.started += Jump;
        inputControl.GamePlay.Red.started += SetColorRed;
        inputControl.GamePlay.Blue.started += SetColorBlue;
        inputControl.GamePlay.Yellow.started += SetColorYellow;
        inputControl.GamePlay.Skill.started += SkillStart;
        inputControl.GamePlay.Skill.canceled += SkillEnd;

        //collection
        collectionList = new List<int>(new int[16]);
    }

    private void Start()
    {
        inkPainter = GetComponent<InkPainter>();
        SetColorRed(new InputAction.CallbackContext()); // 初始化颜色为红色
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
        //Debug.Log("IsGround: " + physicsCheck.isGround);
        if (physicsCheck.isSafe && !inDanger)
        {
            safePosition = transform.position;
        }
        //记录安全位置

        //读取脚下颜色
        currentInkColor = inkPainter.currentInkColor;
        RedYellowCollision();

        //animator
        SetAnimation();

        //skill
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    //移动与跳跃
    public void Move()
    {
        if (isInWater && currentColorId == 2)
        {
            float xVel = inputDirection.x * speed * Time.deltaTime;
            float yInput = inputDirection.y * speed * Time.deltaTime;

            // 计算浮力强度因子（0 ~ 1），随时间逐渐增强
            buoyancyTimer += Time.fixedDeltaTime;
            float buoyancyFactor = Mathf.Clamp01(buoyancyTimer / buoyancyRampUpTime);

            // 浮力部分
            float buoyantY = buoyancyLiftSpeed * buoyancyFactor;

            // Y 方向 = 玩家输入 + 浮力（手感考虑）
            float yVel = yInput + buoyantY+rb.velocity.y;

            rb.velocity = new Vector2(xVel, Mathf.Clamp(yVel, -maxBuoyancySpeed, maxBuoyancySpeed));
        }
        else if (isLadder)
        {
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, inputDirection.y * speed * Time.deltaTime);
        }
        else
        {
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        }

        //face direction
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
        {
            faceDir = -1;
            dustParticles.transform.rotation = Quaternion.Euler(0, 180, 0); 
        }
        if (inputDirection.x < 0) 
        {
            faceDir = 1;
            dustParticles.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        transform.localScale = new Vector3(faceDir, 1, 1);

        //animator
        bool isMove = Mathf.Abs(inputDirection.x) > 0.01f;
        animator.SetBool("isMove", isMove);

        //fx control
        if (isMove && physicsCheck.isGround)
        {
            if (!dustParticles.isPlaying)
            {
                dustParticles.Play();
            }
        }
        else { 
            dustParticles.Stop();
            dustParticles.Clear();
        }
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround && !isInWater) {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            AudioManager.Instance.PlayJumpSFX();
        }
            
        YellowGravity();
    }

    //受伤与死亡
    public void Die()
    {
        inputControl.GamePlay.Disable();
        animator.SetBool("isDead", true);
    }

    private IEnumerator DelayedReturn()
    {
        inputControl.GamePlay.Disable();
        yield return new WaitForSeconds(1f); //等待1秒后执行后续操作
        rb.velocity = Vector2.zero;
        transform.position = safePosition;
        yield return new WaitForSeconds(1f);
        inputControl.GamePlay.Enable();
        // vision
    }

    public void ReturnToSafePosition()
    {
        StartCoroutine(DelayedReturn());
    }

    //技能
    private void SkillStart(InputAction.CallbackContext obj)
    {
        if (currentColorId == 1) //红色技能
        {
            StartCharging();
        }
        else if (currentColorId == 2) //蓝色技能
        {
            StartBlue();
        }
    }

    private void SkillEnd(InputAction.CallbackContext obj) 
    {
        if (currentColorId == 1) //红色技能
        {
            ShootFireball();
        }
        else if (currentColorId == 2) //蓝色技能
        {
            BlueSkill();
        }
        else if (currentColorId == 3) //黄色技能
        {

        }
    }

    private void StartCharging()
    {
        isCharging = true;
        currentChargeTime = 0f;
    }

    private void ShootFireball() 
    {
        isCharging = false;
        float force = Mathf.Min(minForce + currentChargeTime * chargeRate, maxForce);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mouseWorldPos - firePoint.position);

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        // 赋予初速度
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * force;
    }

    //blue skill
    private void BlueSkill()
    {
        iceRangeCircle.SetActive(false);
        // 获取鼠标位置
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        Vector3 playerPos = transform.position;
        Vector3 offset = mouseWorldPos - playerPos;

        // 限制最大生成距离
        if (offset.magnitude > maxIceSpawnDistance)
        {
            offset = offset.normalized * maxIceSpawnDistance;
        }

        Vector3 spawnPos = playerPos + offset;

        // 如果已有冰块，触发其消失
        if (currentIceBlock != null)
        {
            IceBlock oldIce = currentIceBlock.GetComponent<IceBlock>();
            if (oldIce != null)
            {
                oldIce.TriggerDisappear();
            }
            else
            {
                Destroy(currentIceBlock);
            }
        }

        // 生成新的冰块
        currentIceBlock = Instantiate(iceBlockPrefab, spawnPos, Quaternion.identity);

    }

    private void StartBlue() {
        iceRangeCircle.SetActive(true);
    }

    //动画
    private void SetAnimation() {
        animator.SetFloat("velocityY", rb.velocity.y);
        animator.SetBool("isGround", physicsCheck.isGround);
    }

    //颜色变换
    private void SetColorRed(InputAction.CallbackContext obj) =>
        SetColor(1, redCtrl, new Color(1f, 0.4f, 0.4f, 1f));

    private void SetColorBlue(InputAction.CallbackContext obj) =>
        SetColor(2, blueCtrl, new Color(0.4f, 0.6f, 1f, 1f));

    private void SetColorYellow(InputAction.CallbackContext obj) =>
        SetColor(3, yellowCtrl, Color.yellow);

    private void SetColor(int colorId, RuntimeAnimatorController controller, Color particleColor)
    {
        currentColorId = colorId;
        animator.runtimeAnimatorController = controller;

        var main = dustParticles.main;
        main.startColor = particleColor;
    }

    //红黄碰撞逻辑
    private void RedYellowCollision() {

        if (!physicsCheck.isGround || currentInkColor == 0) {
            return;
        }

        if (!physicsCheck.isPulsed) {
            if ((currentColorId == 1 && currentInkColor == 3) || (currentColorId == 3 && currentInkColor == 1))
            {
                Vector2 pushDirection = physicsCheck.lastContactNormal;
                rb.AddForce(pushDirection * collisionPushForce, ForceMode2D.Impulse);
                AudioManager.Instance.PlayJumpSFX();
                physicsCheck.isPulsed = true; // 设置脉冲状态，避免重复触发
                //Debug.Log("conflict");
            }
        }
    }

    //黄色与重力反转逻辑
    private void YellowGravity() {
        if (currentColorId == 3 && inkPainter.checkPreviousColor() == 3 && isInGravityZone) {
            rb.gravityScale *= -1;
            Debug.Log("Yellow gravity activated!");
        }
    }

    //collection
    public void UnlockCollection(int collectionId)
    {
        collectionList[collectionId] = 1;
    }

    // 示例：判断某个物品是否已获得
    public int HasCollection(int collectionId)
    {
        return collectionList[collectionId];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GravityZone"))
        {
            isInGravityZone = true;
        }

        if (collision.CompareTag("Spike") || collision.CompareTag("Water"))
        {
            inDanger = true;
        }

        if (collision.CompareTag("Ladder"))
        { 
            isLadder = true;
            rb.gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("GravityZone"))
        {
            isInGravityZone = false;
        }

        if(collision.CompareTag("Spike") || collision.CompareTag("Water")) //之后这里要加water
        {
            inDanger = false;
        }

        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            rb.gravityScale = 8f;
        }
    }
}
