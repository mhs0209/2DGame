using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FreeworkGame
{
    public enum targetDirectType { forward = 0, backward = 1 }
    public enum AniType { idle = 0, walk = 1, run = 2, win = 3, lose = 4 }

    public class AniManager : MonoBehaviour
    {
        public static AniManager Instance { get; private set; }
        
        [Header("Components")]
        private Rigidbody2D rb;
        private PlayerStat playerStat;
        public Animator[] targetAnimators; // 0: forward, 1: backward

        [Header("Movement Settings")]
        private float moveXstep = 1f;
        private float moveYstep = 0.7f;
        private Vector2 moveInput;
        
        [Header("Bomb Setting")]
        public GameObject bombPrefab; // 인스펙터에서 ActiveBomb 프리팹 할당
        
        [Header("State")]
        private targetDirectType targetType;
        private bool isRun = false;
        private bool isPose = false;
        private bool isCollidingWithMonster = false; // 몬스터와 접촉 중인지 확인용

        private void Awake()
        {
            // [추가] 싱글톤 로직: 이미 존재하면 나를 파괴, 없으면 나를 유지
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 씬이 넘어가도 파괴되지 않음
            }
            else
            {
                Destroy(gameObject); // 이미 존재하므로 새로 생성된 객체 삭제
                return;
            }

            rb = GetComponent<Rigidbody2D>();
            playerStat = GetComponent<PlayerStat>();
        }

        private void Start()
        {
            transform.localPosition = Vector3.zero;
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
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        // 씬이 바뀔 때마다 실행되는 함수
        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            // 포탈을 탔을 때 (씬이 새로 로드됐을 때) 위치를 0,0으로 강제 이동
            transform.position = Vector3.zero;
    
            // Rigidbody가 있다면 물리 위치도 초기화해주는 것이 안전합니다.
            if(rb != null) rb.position = Vector2.zero;
    
            Debug.Log("새로운 씬 로드: 플레이어 위치 초기화 완료");
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
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPlaceBomb();
            }
        }
        
        private void TryPlaceBomb()
        {
            if (playerStat.UseBomb())
            {
                // 플레이어 발밑 혹은 약간 앞에 폭탄 생성
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
                Debug.Log("폭탄 설치!");
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
            if (playerStat != null)
            {
                if (moveInput != Vector2.zero)
                {
                    float currentSpeed = isRun ? playerStat.speed * 1.5f : playerStat.speed;
                    rb.MovePosition(rb.position + moveInput * (currentSpeed * Time.fixedDeltaTime));
                }
                else
                {
                    // [조건 수정] 입력이 없고, 몬스터와 충돌 중이 아닐 때만 물리 속도를 0으로!
                    if (!isCollidingWithMonster)
                    {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
        }

        // 충돌 체크 로직 추가
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 충돌한 대상의 태그가 Monster인 경우
            if (collision.gameObject.CompareTag("Monster"))
            {
                isCollidingWithMonster = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            // 몬스터와 떨어지는 순간
            if (collision.gameObject.CompareTag("Monster"))
            {
                isCollidingWithMonster = false;
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