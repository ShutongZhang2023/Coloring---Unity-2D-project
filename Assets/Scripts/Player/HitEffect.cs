using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    public int repeatCount = 8;
    [SerializeField] private float _duration = 0.25f;

    private static readonly int HitEffectID = Shader.PropertyToID("_HitEffectAmount");

    private SpriteRenderer _spriteRenderer;
    private Material _material;

    private float _lerpAmount;


    // Start is called before the first frame update
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
    }

    // Update is called once per frame
    public void hitEffect()
    {
        StartCoroutine(PlayHitEffectRepeatedly());
    }

    private IEnumerator PlayHitEffectRepeatedly()
    {
        for (int i = 0; i < repeatCount; i++)
        {
            _lerpAmount = 0f;

            bool isComplete = false;
            DOTween.To(GetLerpValue, SetLerpVlue, 1f, _duration)
                .SetEase(Ease.OutExpo)
                .OnUpdate(OnLerpUpdate)
                .OnComplete(() => {
                    OnLertComplete();
                    isComplete = true;
                });

            // 等待这一轮动画完成
            yield return new WaitUntil(() => isComplete);
        }
    }


    private void OnLerpUpdate() { 
        _material.SetFloat(HitEffectID, GetLerpValue());
    }

    private void OnLertComplete()
    {
        DOTween.To(GetLerpValue, SetLerpVlue, 0f, _duration).OnUpdate(OnLerpUpdate);
    }

    private float GetLerpValue() {
        return _lerpAmount;
    }

    private void SetLerpVlue(float newValue) { 
        _lerpAmount = newValue;
    }
}
