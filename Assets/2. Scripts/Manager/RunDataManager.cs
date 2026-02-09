using System.Collections.Generic;
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
}