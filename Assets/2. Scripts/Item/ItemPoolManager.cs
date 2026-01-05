using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPoolManager : MonoBehaviour
{
    public static ItemPoolManager Instance;

    [Header("Table Data")]
    public List<ItemData> masterItemTable; // 모든 아이템 데이터 SO 리스트

    // 내부 상태 관리
    private List<ItemData> availablePool;    // 아직 등장하지 않은 아이템
    private HashSet<ItemData> itemsInWorld;  // 현재 맵 어딘가에 스폰되어 있는 아이템
    private HashSet<ItemData> playerItems;   // 플레이어가 획득한 아이템

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); Init(); }
        else Destroy(gameObject);
    }

    private void Init()
    {
        availablePool = new List<ItemData>(masterItemTable);
        itemsInWorld = new HashSet<ItemData>();
        playerItems = new HashSet<ItemData>();
    }

    // 각 방(Shop, Treasure 등)에서 호출할 함수
    public ItemData RequestItem(RoomType roomType)
    {
        // 1. 해당 방 타입이 가질 수 있는 아이템 필터링 (테이블 내 RoomType 조건 확인)
        // 실제 데이터 구조에 따라 i.appearableRooms.Contains(roomType) 형태가 될 것임
        var candidates = availablePool.Where(i => i.type != ItemType.Pickup).ToList();

        if (candidates.Count == 0) return null;

        ItemData selected = candidates[Random.Range(0, candidates.Count)];
        
        // 2. 예약 상태로 변경 (풀에서 빼고 '필드 존재' 목록으로)
        availablePool.Remove(selected);
        itemsInWorld.Add(selected);
        
        return selected;
    }

    // 플레이어가 아이템을 먹었을 때 호출
    public void OnItemPickedUp(ItemData item)
    {
        itemsInWorld.Remove(item);
        playerItems.Add(item);
    }

    // [중요] 씬 이동 시 먹지 않은 아이템 반납
    public void ReturnUncollectedItems()
    {
        foreach (var item in itemsInWorld)
        {
            availablePool.Add(item);
        }
        itemsInWorld.Clear();
    }
}