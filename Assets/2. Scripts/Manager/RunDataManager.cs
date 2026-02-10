using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct ObtainmentData
{
    public string itemName;
    public Sprite itemSprite;
    public Color itemColor;
}

public class RunDataManager : MonoBehaviour
{
    public static RunDataManager Instance;

    [Header("Run Data")]
    public float elapsedTime;
    public bool isTimerRunning;
    
    public List<ObtainmentData> collectedPassives = new List<ObtainmentData>();
    public ObtainmentData currentActiveItem;
    
    private const string SAVE_KEY = "BestClearTimes";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        ResetData();
        StartTimer();
    }

    void Update()
    {
        if (isTimerRunning) elapsedTime += Time.deltaTime;
    }

    public void StartTimer() => isTimerRunning = true;
    public void StopTimer() => isTimerRunning = false;

    public void ResetData()
    {
        elapsedTime = 0;
        collectedPassives.Clear();
        currentActiveItem = new ObtainmentData();
        isTimerRunning = false;
    }

    // 아이템 데이터를 생성하고 리스트에 추가하는 핵심 로직
    public void RecordPassive(ItemData data)
    {
        collectedPassives.Add(CreateObtainmentData(data));
    }

    public void RecordActive(ItemData data)
    {
        currentActiveItem = CreateObtainmentData(data);
    }

    private ObtainmentData CreateObtainmentData(ItemData data)
    {
        // 프리팹에서 SpriteRenderer를 찾아 정보 추출
        SpriteRenderer sr = data.itemPrefab.GetComponent<SpriteRenderer>();
        if (sr == null) sr = data.itemPrefab.GetComponentInChildren<SpriteRenderer>();

        return new ObtainmentData
        {
            itemName = data.itemName,
            itemSprite = sr != null ? sr.sprite : null,
            itemColor = sr != null ? sr.color : Color.white
        };
    }
    
    public void SaveCurrentRunTime()
    {
        // 1. 기존 기록 불러오기
        string savedData = PlayerPrefs.GetString(SAVE_KEY, "");
        List<float> times = new List<float>();

        if (!string.IsNullOrEmpty(savedData))
        {
            times = savedData.Split(',').Select(float.Parse).ToList();
        }

        // 2. 현재 기록 추가 및 정렬 (오름차순 - 빠른 순)
        times.Add(elapsedTime);
        times = times.OrderBy(t => t).Take(5).ToList();

        // 3. 다시 문자열로 변환하여 저장
        string dataToSave = string.Join(",", times);
        PlayerPrefs.SetString(SAVE_KEY, dataToSave);
        PlayerPrefs.Save();
    }

    public void ClearBestTimes()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        Debug.Log("클리어 기록이 초기화되었습니다.");
    }
}