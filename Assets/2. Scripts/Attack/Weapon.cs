using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    protected Stat ownerStat;
    protected float lastFireTime;

    protected virtual void Awake() => ownerStat = GetComponent<Stat>();

    protected bool CanFire() => Time.time >= lastFireTime + ownerStat.delay;

    public virtual void Fire(Vector2 direction)
    {
        if (!CanFire()) return;
        lastFireTime = Time.time;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ExecuteFirePattern(angle);
    }

    private void ExecuteFirePattern(float baseAngle)
    {
        switch (ownerStat.fireShape)
        {
            case FireShape.Base:
            case FireShape.Multi:
                SpawnBullets(baseAngle, ownerStat.projectileCount, 15f, ownerStat.range);
                break;
            case FireShape.Radial:
                for (int i = 0; i < ownerStat.projectileCount; i++) 
                    CreateBullet(i * (360f / ownerStat.projectileCount), ownerStat.range);
                break;
            case FireShape.Shotgun:
                // 샷건: 공격 방향 중심 20도 이내에 5발 밀집, 사거리는 원래의 60%
                SpawnBullets(baseAngle, 5, 20f, ownerStat.range * 0.6f); 
                break;
        }
    }

    // 사거리를 매개변수로 받도록 수정
    protected void SpawnBullets(float baseAngle, int count, float spread, float range)
    {
        // 탄환이 1개일 때는 각도 계산 없이 baseAngle로 발사
        if (count <= 1)
        {
            CreateBullet(baseAngle, range);
            return;
        }

        // 여러 발일 때만 간격 계산 (count - 1로 나누어 양 끝에 배치)
        float startAngle = baseAngle - (spread * 0.5f);
        float angleStep = spread / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            CreateBullet(currentAngle, range);
        }
    }

    protected void CreateBullet(float angle, float range)
    {
        // 1. 인스턴스 존재 확인
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("ObjectPooler가 씬에 없습니다! 빈 오브젝트를 만들고 스크립트를 붙이세요.");
            return;
        }
        // 2. 프리팹 할당 확인
        if (bulletPrefab == null)
        {
            Debug.LogError($"{gameObject.name}의 Weapon에 Bullet Prefab이 할당되지 않았습니다.");
            return;
        }
        
        GameObject b = ObjectPoolManager.Instance.SpawnFromPool(bulletPrefab, transform.position + Vector3.up, Quaternion.Euler(0, 0, angle));
        if (b.TryGetComponent(out Bullet bullet))
        {
            bullet.Setup(ownerStat.atk * ownerStat.atkMult, range, ownerStat.isPiercing);
        }
    }
}