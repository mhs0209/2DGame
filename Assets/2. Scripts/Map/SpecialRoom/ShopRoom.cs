using UnityEngine;

public class ShopRoom : ItemRoom 
{
    public Transform[] shopSpots; // 인스펙터에서 설정하거나 스크립트로 지정
    private void Start() {
        if (controller.roomData is ShopMap data) {
            foreach(var spot in shopSpots) {
                // 상점 제단 및 가격 설정 로직
            }
        }
    }
    public override void OnRoomCleared() { }
}