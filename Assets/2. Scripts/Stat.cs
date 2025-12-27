using UnityEngine;

public enum AttackType { Base, Double, Triple, Cross, Radial, Shotgun }

public class Stat : MonoBehaviour
{
    [Header("Basic Stats")]
    public float health;            // 체력
    public float maxHealth;         // 최대 체력
    public float atk;               // 기초 공격력
    public float atkMult = 1.0f;    // 공격력 배율 (10% 증가 등)
    public float delay;             // 공격 속도
    public float range;             // 사거리
    public float speed;             // 이동 속도

    [Header("Projectile Info")]
    public AttackType attackType = AttackType.Base;
    public int projectileCount = 1; // 산탄이나 방사 시 개수

    // 실시간 공격력 계산 공식
    public float GetFinalDamage() => atk * atkMult;

    // 아이템 획득 시 호출될 함수 예시
    public void AddModifier(float a, float m, float s) {
        atk += a;
        atkMult += m;
        speed += s;
    }
}