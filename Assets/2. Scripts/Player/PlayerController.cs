using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f; // 이동 속도

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // 시작할 때 Rigidbody2D 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. 키보드 입력 받기 (W, A, S, D 또는 화살표)
        // GetAxisRaw는 즉각적인 반응(0 아니면 1)을 주어 아이작 같은 느낌에 적합합니다.
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 대각선 이동 시 속도가 빨라지지 않게 정규화(Normalize)
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        // 2. 물리 엔진을 이용해 캐릭터 이동 처리 (프레임 독립적)
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}