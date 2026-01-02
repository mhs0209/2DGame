using FreeworkGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel; // 게임 오버 UI (기본 비활성화)
    
    private float restartHoldTime = 0f;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject); // 이미 존재한다면 새로 생긴 녀석을 제거!
        }
    }

    private void Update()
    {
        // 2. R키를 2초간 누르면 재시작
        HandleRestartKey();
    }

    private void HandleRestartKey()
    {
        if (Input.GetKey(KeyCode.R))
        {
            restartHoldTime += Time.deltaTime;
            if (restartHoldTime >= 3.0f)
            {
                RestartGame();
            }
        }
        else
        {
            restartHoldTime = 0f;
        }
    }

    public void OnPlayerDeath()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // 게임 일시정지
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        // 중요: 싱글톤 플레이어를 파괴해야 스탯이 초기화된 새 플레이어가 생성됨
        if (AniManager.Instance != null)
        {
            Destroy(AniManager.Instance.gameObject);
        }

        // 현재 씬(1스테이지) 재로드
        SceneManager.LoadScene("Stage01");
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        // 메인 메뉴 씬 이름이 "MainMenu"라고 가정
        SceneManager.LoadScene("MainMenu");
    }
}