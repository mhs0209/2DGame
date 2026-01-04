using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float fuseTime = 3.0f;
    public float explosionRadius = 2.0f;
    public float monsterDamage = 10.0f;
    public float playerDamage = 2.0f;

    [Header("Visual Feedback")]
    private SpriteRenderer sr;
    private Color originalColor;

    private Rigidbody2D rb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = sr.color;

        // [중요] 마찰력을 설정하여 미끄러짐 방지 (코드 혹은 인스펙터에서 설정)
        rb.drag = 5f; 
        rb.angularDrag = 5f;
    }

    void Start()
    {
        // 생성 시 플레이어와 잠시 충돌을 무시하여 튕김 현상 방지
        StartCoroutine(IgnorePlayerCollision());
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator IgnorePlayerCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCol = player.GetComponent<Collider2D>();
            Collider2D bombCol = GetComponent<Collider2D>();
            
            Physics2D.IgnoreCollision(bombCol, playerCol, true);
            yield return new WaitForSeconds(0.5f); // 0.5초 후 다시 충돌 허용
            Physics2D.IgnoreCollision(bombCol, playerCol, false);
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        float elapsed = 0f;

        while (elapsed < fuseTime)
        {
            // 시간이 지날수록 깜빡임 속도가 빨라짐 (Lerp 사용)
            // 0초일 때 0.4초 간격 -> 3초에 가까워질수록 0.05초 간격
            float t = elapsed / fuseTime;
            float blinkInterval = Mathf.Lerp(0.4f, 0.05f, t);

            sr.color = Color.red;
            yield return new WaitForSeconds(blinkInterval);
            sr.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval * 2;
        }

        Explode();
    }

    private void Explode()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var obj in hitObjects)
        {
            Stat targetStat = obj.GetComponent<Stat>();
            if (targetStat != null)
            {
                float damage = obj.CompareTag("Player") ? playerDamage : monsterDamage;
                targetStat.TakeDamage(damage);
            }
        }
        
        // 폭발 파티클이 있다면 여기서 생성
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 충돌 지점에서 나를 밀어내는 방향 계산
        Vector2 pushDir = (transform.position - collision.transform.position).normalized;

        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어가 밀 때는 Impulse로 툭툭 밀리게
            rb.AddForce(pushDir * 2.0f, ForceMode2D.Impulse);
        }
        else if (collision.gameObject.CompareTag("Monster"))
        {
            // 몬스터는 천천히 밀어냄
            rb.velocity = pushDir * 0.8f;
        }
    }
}