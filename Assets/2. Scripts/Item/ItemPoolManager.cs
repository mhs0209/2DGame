using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
    public static ItemPoolManager Instance;
    
    // SO로 만든 전체 아이템 리스트
    public List<BaseItemData> allItems = new List<BaseItemData>();
    
    // 현재 게임에서 사용 가능한 아이템 풀 (중복 제거용)
    private List<BaseItemData> currentItemPool;

    void Awake() => Instance = this;

    void Start() => currentItemPool = new List<BaseItemData>(allItems);

    // 카테고리에 맞는 랜덤 아이템 반환 후 풀에서 제거
    public BaseItemData GetRandomItem(ItemCategory category)
    {
        var filteredPool = currentItemPool.Where(i => i.category == category).ToList();
        
        if (filteredPool.Count == 0) return null;

        BaseItemData selected = filteredPool[Random.Range(0, filteredPool.Count)];
        currentItemPool.Remove(selected); // 중복 방지 핵심
        return selected;
    }
}