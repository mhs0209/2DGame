using UnityEngine;

public class ShopRoom : ItemRoom 
{
    public GameObject[] shopSpots; // 인스펙터에서 설정하거나 스크립트로 지정
    private void Start() {
        if (controller.roomData is ShopMap data) {
            for (int i = 0; i < shopSpots.Length; i++) { ;
                // 아이템 혹은 픽업 소환
                GameObject item = (i < 2) ? data.shopItemPool[Random.Range(0, data.shopItemPool.Length)] 
                    : data.pickupPool[Random.Range(0, data.pickupPool.Length)];
                Instantiate(item, shopSpots[i].transform.position + Vector3.up * 0.5f, Quaternion.identity, shopSpots[i].transform);
                // TODO: 여기에 가격표 UI 띄우는 로직 추가 예정
            }
        }
    }
    public override void OnRoomCleared() { }
}