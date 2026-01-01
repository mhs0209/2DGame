using UnityEngine;

namespace FreewrokGame
{
    public enum targetDirectType { forward = 0, backward = 1 }
    public enum AniType { idle = 0, walk = 1, run = 2, win = 3, lose = 4 }

    public class AniManager : MonoBehaviour
    {
        [Header("Components")]
        private Rigidbody2D rb;
        private PlayerStat playerStat;
        public Animator[] targetAnimators; // 0: forward, 1: backward

        [Header("Movement Settings")]
        private float moveXstep = 1f;
        private float moveYstep = 0.7f;
        private Vector2 moveInput;
        
        [Header("State")]
        private targetDirectType targetType;
        private bool isRun = false;
        private bool isPose = false;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerStat = GetComponent<PlayerStat>();
            
            // 초기 설정
            ResetState();
        }

        void Update()
        {
            HandleInput();
            HandleRestart();
            HandlePose();
        }

        void FixedUpdate()
        {
            Move();
        }

        private void HandleInput()
        {
            // 1. 이동 입력 받기
            float x = 0;
            float y = 0;

            if (Input.GetKey(KeyCode.RightArrow)) x += 1;
            if (Input.GetKey(KeyCode.LeftArrow)) x -= 1;
            if (Input.GetKey(KeyCode.UpArrow)) y += 1;
            if (Input.GetKey(KeyCode.DownArrow)) y -= 1;

            // 입력을 기반으로 방향 벡터 설정 (아이소메트릭 비율 적용)
            // 기존 코드의 로직을 따라 방향에 따른 애니메이션 종류 및 스케일 설정
            if (x != 0 || y != 0)
            {
                isPose = false;
                moveInput = new Vector2(x * moveXstep, y * moveYstep).normalized;

                // 방향에 따른 애니메이션 상태 결정
                DetermineDirection(x, y);
                
                // 걷기/달리기 상태 결정
                if (Input.GetKey(KeyCode.Alpha1)) isRun = false;
                if (Input.GetKey(KeyCode.Alpha2)) isRun = true;

                SetAni(isRun ? AniType.run : AniType.walk);
            }
            else
            {
                moveInput = Vector2.zero;
                if (!isPose) SetAni(AniType.idle);
            }
        }

        private void DetermineDirection(float x, float y)
        {
            // 기존 스크립트의 방향 로직 적용
            // y가 양수(위쪽 방향)면 backward, 음수(아래쪽 방향)면 forward
            if (y > 0) targetType = targetDirectType.backward;
            else if (y < 0) targetType = targetDirectType.forward;
            
            // x가 양수면 반전(-1), 음수면 정방향(1)
            float scaleX = (x > 0) ? -1f : 1f;
            transform.localScale = new Vector3(scaleX, 1, 1);
        }

        private void Move()
        {
            if (playerStat != null && moveInput != Vector2.zero)
            {
                // 달리 상태일 때 속도 보정 (필요 시)
                float currentSpeed = isRun ? playerStat.speed * 1.5f : playerStat.speed;
                rb.MovePosition(rb.position + moveInput * (currentSpeed * Time.fixedDeltaTime));
            }
        }

        private void HandlePose()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                isPose = true;
                targetType = targetDirectType.forward;
                SetAni(AniType.win);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                isPose = true;
                targetType = targetDirectType.forward;
                SetAni(AniType.lose);
            }
        }

        private void HandleRestart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ResetState();
                transform.localPosition = new Vector3(11, -8, 0);
            }
        }

        private void ResetState()
        {
            isPose = false;
            isRun = false;
            targetType = targetDirectType.forward;
            transform.localScale = Vector3.one;
            
            // 모든 애니메이터 비활성화 후 기본 설정
            foreach (var anim in targetAnimators) anim.gameObject.SetActive(false);
            SetAni(AniType.idle);
        }

        private void SetAni(AniType aType)
        {
            // 현재 방향의 애니메이터 활성화 및 반대 방향 비활성화
            int currentIndex = (int)targetType;
            int otherIndex = 1 - currentIndex;

            if (!targetAnimators[currentIndex].gameObject.activeSelf)
            {
                targetAnimators[currentIndex].gameObject.SetActive(true);
                targetAnimators[otherIndex].gameObject.SetActive(false);
            }

            targetAnimators[currentIndex].SetInteger("aniInt", (int)aType);
        }
    }
}