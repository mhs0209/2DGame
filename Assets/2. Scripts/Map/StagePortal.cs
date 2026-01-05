using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필수

public class StagePortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. 현재 활성화된 씬의 인덱스를 가져옵니다.
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            
            // 2. 빌드 설정에 등록된 전체 씬 개수를 가져옵니다.
            int totalScenes = SceneManager.sceneCountInBuildSettings;

            // 3. 다음 인덱스 계산
            int nextSceneIndex = currentSceneIndex + 1;

            // 4. 만약 다음 인덱스가 전체 개수보다 크거나 같다면(마지막이라면) 
            // 현재 인덱스를 유지하고, 아니면 다음 인덱스로 이동합니다.
            if (nextSceneIndex >= totalScenes)
            {
                // 마지막 씬이므로 현재 씬을 다시 로드 (질문하신 '되풀이' 로직)
                SceneManager.LoadScene(currentSceneIndex);
            }
            else
            {
                // 다음 씬으로 이동
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
}