using System.Collections;
using UnityEngine;

public enum StatType { Health, MaxHealth, Atk, AtkMult, Delay, Range, Speed }
public enum FireShape { Base, Multi, Radial, Shotgun }

[System.Serializable]
public struct StatModifier
{
    public StatType statType;
    public float value;
}

public abstract class Stat : MonoBehaviour
{
    [Header("Basic Stats")]
    public float health;
    public float maxHealth;
    public float atk;
    public float atkMult = 1.0f;
    public float delay;
    public float range;
    public float speed;

    [Header("Projectile Info")]
    public FireShape fireShape = FireShape.Base;
    public int projectileCount = 1; 
    public bool isPiercing = false; // 관통 여부
    
    [Header("Invincibility Settings")]
    public bool isInvincible = false;       // 현재 무적 상태인가? (아이템/스킬용)
    public bool useHitInvincibility = false; // 피격 시 자동으로 무적 시간을 가질 것인가?
    public float invincibilityMult = 1.0f;  // 무적 시간 계수
    
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (isInvincible) return;

        health -= damage;

        if (health <= 0) Die();
        else
        {
            // 스위치가 켜져 있는 경우에만 피격 무적 루틴 실행
            if (useHitInvincibility)
            {
                StartCoroutine(HitInvincibleRoutine(damage * invincibilityMult));
            }
        }
    }

    protected IEnumerator HitInvincibleRoutine(float duration)
    {
        isInvincible = true;
        // 깜빡임 연출...
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }
        spriteRenderer.color = Color.white;
        isInvincible = false;
    }

    protected abstract void Die();

    public virtual void ApplyStatChange(StatType type, float value)
    {
        switch (type)
        {
            case StatType.Health: health += value; break;
            case StatType.MaxHealth: maxHealth += value; break;
            case StatType.Atk: atk += value; break;
            case StatType.AtkMult: atkMult += value; break;
            case StatType.Delay: delay += value; break;
            case StatType.Range: range += value; break;
            case StatType.Speed: speed += value; break;
        }
    }
}