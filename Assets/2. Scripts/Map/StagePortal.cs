using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePortal : MonoBehaviour
{
    private bool isActive = false; // 포탈 활성화 상태
    [SerializeField] private float activationDelay = 1.0f; // 활성화 대기 시간 (1초)

    private void Start()
    {
        // 생성 1초 후에 포탈을 활성화하도록 설정
        Invoke(nameof(ActivatePortal), activationDelay);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void ActivatePortal()
    {
        isActive = true;
        gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 1. 플레이어인지 확인하고 2. 포탈이 활성화 상태일 때만 작동
        if (collision.CompareTag("Player") && isActive)
        {
            MoveToNextStage();
        }
    }

    private void MoveToNextStage()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= totalScenes)
        {
            // 마지막 씬이므로 현재 씬을 다시 로드
            SceneManager.LoadScene(currentSceneIndex);
        }
        else
        {
            // 다음 씬으로 이동
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}