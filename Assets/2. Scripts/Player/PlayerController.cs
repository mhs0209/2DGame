using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStat playerStat; // [추가] 플레이어 스탯 참조
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // [추가] 같은 오브젝트에 있는 PlayerStat 컴포넌트를 가져옴
        playerStat = GetComponent<PlayerStat>();
    }

    void Update()
    {
        // 1. 화살표 키 입력을 직접 받기 (GetKeyDown/Up 보다 GetKey가 이동에 적합)
        // Horizontal(좌우) 계산
        float moveX = 0;
        if (Input.GetKey(KeyCode.RightArrow)) moveX += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) moveX -= 1f;

        // Vertical(상하) 계산
        float moveY = 0;
        if (Input.GetKey(KeyCode.UpArrow)) moveY += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveY -= 1f;

        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // 2. playerStat에 실시간으로 반영된 speed 값을 사용합니다.
        // 이제 아이템을 먹어서 stat.speed가 변하면 이동 속도도 즉시 변합니다.
        if (playerStat != null)
        {
            rb.MovePosition(rb.position + moveInput * (playerStat.speed * Time.fixedDeltaTime));
        }
    }
}