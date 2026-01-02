using System.Collections.Generic;
using UnityEngine;

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

    public GameObject SpawnFromPool(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        string key = prefab.name;
        if (!poolDict.ContainsKey(key)) poolDict.Add(key, new Queue<GameObject>());

        if (poolDict[key].Count > 0)
        {
            GameObject obj = poolDict[key].Dequeue();
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab, pos, rot);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        string key = obj.name.Replace("(Clone)", "");
        poolDict[key].Enqueue(obj);
    }
}