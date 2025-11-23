using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public static Character Instance;

    private HitEffect hitEffect;
    [Header("basic properties")]

    public int maxHealth;
    public int currentHealth;
    public bool isDead = false;

    [Header("invincible")]
    public float invincibleTime;
    private float invincibleTimer;
    public bool isInvincible;

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    private void Awake()
    {
        Instance = this;
        hitEffect = GetComponent<HitEffect>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        UIController.Instance.UpdateHealthUI();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(Damage attacker)
    {
        if (isInvincible) return;

        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerIncincible();
            //受伤的其他逻辑，比如音效等
            onTakeDamage?.Invoke();
            AudioManager.Instance.PlayHurtSFX();
        }
        else {
            currentHealth = 0;
            isDead = true;
            onDeath?.Invoke();
            AudioManager.Instance.PlayDeadSFX();
        }
        
        UIController.Instance.UpdateHealthUI();

    }

    private void TriggerIncincible() {
        if (!isInvincible) {
            isInvincible = true;
            invincibleTimer = invincibleTime;
            hitEffect.hitEffect();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UIController.Instance.UpdateHealthUI();
    }
}
