using UnityEngine;

public class ActiveRadialAttack : Active
{
    [Header("Radial Attack Settings")]
    [SerializeField] private GameObject projectilePrefab; // 여기서 직접 할당

    public override void Use(GameObject player)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("ActiveRadialAttack: Projectile Prefab이 할당되지 않았습니다!");
            return;
        }

        PlayerStat stat = player.GetComponent<PlayerStat>();
        // 대미지는 플레이어 대미지의 2배
        float damage = stat.atk * stat.atkMult * 2f;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            
            // ObjectPoolManager를 사용한 스폰
            GameObject bObj = ObjectPoolManager.Instance.SpawnFromPool(projectilePrefab, player.transform.position, rotation);
            
            if (bObj.TryGetComponent(out Bullet bullet))
            {
                // Bullet.cs의 Setup(float dmg, float rng, bool piercing) 호출
                bullet.Setup(damage, stat.range, stat.isPiercing);
            }
        }
    }
}