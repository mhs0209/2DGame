using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class InitializationManager : MonoBehaviour
{
    public static InitializationManager Instance;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += ExecuteInit;
    private void OnDisable() => SceneManager.sceneLoaded -= ExecuteInit;

    private void ExecuteInit(Scene scene, LoadSceneMode mode)
    {
        // 1. 아이템 풀 반납 로직 실행
        //ItemPoolManager.Instance.ReturnUncollectedItems();

        // 2. 씬에 존재하는 모든 IInitializable 인터페이스를 구현한 객체 찾기
        // (DDOL 객체와 일반 씬 객체 모두 포함)
        var initObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IInitializable>();

        foreach (var obj in initObjects)
        {
            obj.OnLevelInit();
        }
        
        Debug.Log($"{scene.name} 시스템 초기화 완료");
    }
}