using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FallInWater : MonoBehaviour
{
    public float exitBounceForce = 8f;       // 出水冲击力
    public float delayBeforeBuoyancy = 0.5f; // 延迟开始浮力

    private float enterWaterTime;

    private Vignette vignette;
    public Color waterColor = new Color(0.2f, 0.4f, 0.8f, 1f); // 自定义水中颜色
    public Color defaultColor = Color.black;

    void Start()
    {
        Volume volume = FindObjectOfType<Volume>();
        if (volume != null && volume.profile.TryGet(out Vignette v))
        {
            vignette = v;
        }
        else
        {
            Debug.LogWarning("Vignette not found in Volume!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = PlayerBasic.Instance;
        player.isInWater = true;
        vignette.color.value = waterColor;
        vignette.intensity.value = 0.5f;
        player.buoyancyTimer = 0f;

        // 记录进入水的时间
        enterWaterTime = Time.time;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var player = PlayerBasic.Instance;
        player.isInWater = false;
        vignette.color.value = defaultColor;
        vignette.intensity.value = 0.3f;

        // 只有在水中待满一定时间后才给予出水弹力
        if (Time.time - enterWaterTime >= delayBeforeBuoyancy)
        {
            player.rb.AddForce(Vector2.up * exitBounceForce, ForceMode2D.Impulse);
        }
    }
}
