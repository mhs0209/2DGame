using UnityEngine;
using System.Collections.Generic;

public abstract class ItemRoom : MonoBehaviour
{
    [Header("Item Spawning Config")]
    public Transform[] spawnPoints; // 아이템이 생성될 위치들
    public GameObject pedestalPrefab; // 아이템을 띄울 제단 프리팹

    protected RoomController roomController;

    protected virtual void Awake()
    {
        roomController = GetComponent<RoomController>();
    }

    // 아이템이나 보상을 생성하는 공통 메소드
    public abstract void OnRoomCleared();

    // 자식들이 각자 다르게 구현할 추상 함수
    public abstract void SpawnItems();

    // 공통 기능: 아이템 매니저로부터 아이템을 받아와 제단에 배치
    protected void CreatePedestal(Vector3 position, RoomType category)
    {
        // ItemManager(추후 제작)에서 이미 먹은 아이템을 제외하고 가져옴
        // 지금은 테스트용이므로 간단히 생성 로직만 기재
        GameObject pedestal = Instantiate(pedestalPrefab, position, Quaternion.identity, transform);
        
        Debug.Log($"{category} 카테고리 아이템이 생성되었습니다.");
        
        // 여기에 추후 아이템 데이터를 주입하는 로직이 들어갑니다.
        // pedestal.GetComponent<Pedestal>().Setup(ItemManager.Instance.GetItem(category));
    }
}