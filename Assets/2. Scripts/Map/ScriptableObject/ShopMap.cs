using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Room Data", menuName = "RoomData/ShopRoomData")]
public class ShopMap : BaseRoomData
{
    public int shopItemCount = 3; // 상점에서 팔 아이템 개수
    public int pickupCount = 2; // 소모품(픽업) 개수
    public Vector2 priceRange; // 가격 범위 (x: 최소, y: 최대)
}
