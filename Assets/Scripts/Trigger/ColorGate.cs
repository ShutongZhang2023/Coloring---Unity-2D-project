using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorGate : MonoBehaviour
{
    [Header("Initial Color")]
    public int currentColorId = -1; //当前颜色ID

    [Header("Have Restriction")]
    public bool hasTrigger;
    public bool hasRequirement = false;
    public int conditionNumber; //有几个要求
    public List<int> conditionIds; //如有要求，储存要求的颜色ID

    private SpriteRenderer squareInsideRenderer;
    private Collider2D squareOutsideCollider;

    private Dictionary<int, int> idColorMap = new Dictionary<int, int>();
    private bool isOpened;
    
    private void Awake()
    {
        squareInsideRenderer = transform.Find("square_inside")?.GetComponent<SpriteRenderer>();
        squareOutsideCollider = transform.Find("square_outside")?.GetComponent<Collider2D>();

        if (squareInsideRenderer == null || squareOutsideCollider == null)
        {
            Debug.LogWarning($"{name}：自动绑定失败，请检查子物体命名和结构");
        }
    }

    private void Start()
    {
        if (!hasTrigger)
        {
            isOpened = true; //如果没有触发器，默认开启
            if (currentColorId == 1) squareInsideRenderer.color = new Color(1f, 0f, 0f, 0.95f);
            else if (currentColorId == 2) squareInsideRenderer.color = new Color(0f, 0.5f, 1f, 0.95f);
            else if (currentColorId == 3) squareInsideRenderer.color = new Color(1f, 1f, 0f, 0.95f);
        }
        else {
            isOpened = false; //如果有触发器，默认关闭
        }
        
    }

    private void Update()
    {
        //每帧检查玩家颜色是否匹配
        UpdateGateCollider();
    }

    public void UpdateColor(int triggerId, int triggerColor)
    {
        idColorMap[triggerId] = triggerColor;
       // Debug.Log("当前上传颜色: " + triggerColor);
        var allColors = idColorMap.Values;
        Debug.Log(allColors);
        //只在所有条件被填充时才进行检查
        if (idColorMap.Count == conditionNumber) {
            //有条件
            if (hasRequirement)
            {
                bool allMatch = !conditionIds.Except(allColors).Any();

                //所有条件都满足
                if (allMatch)
                {
                    //所有条件的颜色是否相同，不同变为黑色，相同则染上对应颜色
                    if (allColors.Distinct().Count() > 1)
                    {
                        isOpened = true;
                        SetGateVisual("black", allColors); //不同
                    }
                    else
                    {
                        isOpened = true;
                        SetGateVisual("changeColor", allColors);
                    }
                }
                //有条件但不满足，白色
                else
                {
                    isOpened = false;
                    if (allColors.Distinct().Count() > 1)
                        SetGateVisual("white", allColors);
                    else
                    {
                        SetGateVisual("changeColor", allColors);
                    }
                }
            }
            //无条件
            else
            {
                Debug.Log("无条件检查");
                //颜色不一致
                if (allColors.Distinct().Count() > 1)
                {
                    isOpened = false;
                    SetGateVisual("white", allColors);
                }
                //一致
                else
                {
                    isOpened = true;
                    SetGateVisual("changeColor", allColors);
                }
            }
        }
           
    }

    private void SetGateVisual(string mode, IEnumerable<int> colors)
    {
        switch (mode)
        {
            case "changeColor":
                squareInsideRenderer.color = ColorFromId(colors.First());
                currentColorId = colors.First();
                break;
            case "black":
                squareInsideRenderer.color = new Color(0f, 0f, 0f, 1f);
                currentColorId = 0;
                break;
            case "white":
                squareInsideRenderer.color = new Color(1f, 1f, 1f, 1f);
                currentColorId = -1;
                break;
        }
    }

    private Color ColorFromId(int id)
    {
        return id switch
        {
            1 => new Color(1f, 0f, 0f, 0.95f),   // 红
            2 => new Color(0f, 0.5f, 1f, 0.95f), // 蓝
            3 => new Color(1f, 1f, 0f, 0.95f),   // 黄
        };
    }

    private void UpdateGateCollider()
    {
        if (isOpened) {
            if (PlayerBasic.Instance.currentColorId == currentColorId || currentColorId == 0)
            {
                squareOutsideCollider.enabled = false;
            }
            else
            {
                squareOutsideCollider.enabled = true;
            }
        }
        else
        {
            squareOutsideCollider.enabled = true;
        }
    }
}
