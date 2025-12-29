using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed = 10f;
    private float range;
    private Vector3 startPos;
    private bool isPiercing;

    public void Setup(float dmg, float rng, bool piercing)
    {
        damage = dmg;
        range = rng;
        isPiercing = piercing;
        startPos = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * (speed * Time.deltaTime));
        if (Vector3.Distance(startPos, transform.position) >= range) ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tag에 따른 처리는 유니티 에디터에서 PlayerBullet/MonsterBullet 프리팹을 나눠 설정
        if (collision.CompareTag("Wall"))
        {
            if (isPiercing == false) ReturnToPool();
            return;
        }

        // 2. 적/플레이어 타격 처리
        if ((CompareTag("PlayerBullet") && collision.CompareTag("Monster")) ||
            (CompareTag("MonsterBullet") && collision.CompareTag("Player")))
        {
            // 컴포넌트가 있는지 안전하게 체크 (TryGetComponent 권장)
            if (collision.TryGetComponent(out Stat targetStat))
            {
                targetStat.TakeDamage(damage);
                if (isPiercing == false) ReturnToPool();
            }
            else
            {
                // Stat이 없는데 몬스터/플레이어 태그인 경우 예외 처리
                if (isPiercing == false) ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }
}