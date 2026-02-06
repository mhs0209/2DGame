using System.Collections;
using UnityEngine;

public enum StatType { Health, MaxHealth, Atk, AtkMult, Delay, Range, Speed, Gold, Key, Bomb }
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
    
    protected SpriteRenderer[] childrenRenderers;

    protected virtual void Awake()
    {
        // 자식들의 모든 SpriteRenderer를 한 번에 가져옵니다.
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    
    public virtual void TakeDamage(float damage)
    {
        if (isInvincible) return;

        health -= damage;

        if (health <= 0) Die();
        else
        {
            // 피격 시 실행될 시각적 효과 (기본은 내용 없음)
            OnHitVisual(); 

            if (useHitInvincibility)
            {
                StartCoroutine(HitInvincibleRoutine(damage * invincibilityMult)); // 예시 시간
            }
        }
    }

    // 자식들이 각자 입맛에 맞게 구현할 "입구"
    protected virtual void OnHitVisual() { }
    
    // Stat.cs 수정본 일부
    protected IEnumerator HitInvincibleRoutine(float duration)
    {
        isInvincible = true;
    
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // 모든 자식 스프라이트의 알파값 조절 (투명도 0.5)
            foreach (var sr in childrenRenderers)
            {
                if (sr == null) continue;
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
            }
            yield return new WaitForSeconds(0.1f);

            // 모든 자식 스프라이트 복구 (투명도 1.0)
            foreach (var sr in childrenRenderers)
            {
                if (sr == null) continue;
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        // 최종 복구 (루프가 끝난 후 확실히 원래대로 돌려놓음)
        foreach (var sr in childrenRenderers)
        {
            if (sr == null) continue;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        }

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