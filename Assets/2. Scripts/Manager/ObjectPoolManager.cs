using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject); // 이미 존재한다면 새로 생긴 녀석을 제거!
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        string key = obj.name.Replace("(Clone)", "");
        poolDict[key].Enqueue(obj);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ClearPoolOnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ClearPoolOnSceneLoaded;
    }

    private void ClearPoolOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 바뀌면 풀에 들어있는 가짜 주소들을 비워줍니다.
        foreach (var queue in poolDict.Values)
        {
            queue.Clear();
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        string key = prefab.name;
        if (!poolDict.ContainsKey(key)) poolDict.Add(key, new Queue<GameObject>());

        // 큐에 데이터가 있어도, 실제 오브젝트가 null(파괴됨)인지 한 번 더 체크
        while (poolDict[key].Count > 0)
        {
            GameObject obj = poolDict[key].Dequeue();
            if (obj != null) // 살아있는 오브젝트일 때만 사용
            {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                obj.SetActive(true);
                return obj;
            }
        }
        
        // 큐가 비었거나 살아있는 오브젝트가 없으면 새로 생성
        GameObject newObj = Instantiate(prefab, pos, rot);
        return newObj;
    }
}