using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Stat ownerStat;

    void Awake() => ownerStat = GetComponent<Stat>();

    public void Fire(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        switch (ownerStat.fireShape)
        {
            case FireShape.Base:
            case FireShape.Multi:
                SpawnBullets(angle, ownerStat.projectileCount, 15f);
                break;
            case FireShape.Radial:
                SpawnRadial(ownerStat.projectileCount);
                break;
            case FireShape.Shotgun:
                SpawnBullets(angle, 12, 45f); // 샷건은 넓은 범위
                break;
        }
    }

    void SpawnBullets(float baseAngle, int count, float spread)
    {
        float startAngle = baseAngle - (spread * (count - 1) * 0.5f);
        for (int i = 0; i < count; i++)
        {
            float currentAngle = startAngle + (i * spread);
            CreateBullet(currentAngle);
        }
    }

    void SpawnRadial(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateBullet(i * (360f / count));
        }
    }

    void CreateBullet(float angle)
    {
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        // Bullet 스크립트에 관통 정보 전달
        // b.GetComponent<Bullet>().Setup(ownerStat.isPiercing);
    }
}