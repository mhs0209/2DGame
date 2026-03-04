using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Map", menuName = "Map/Shop")]
public class ShopMap : BaseRoomData 
{
    // 테이블이 채워줄 리스트
    public List<ItemData> shopItemPool = new List<ItemData>();
    public List<ItemData> pickupPool = new List<ItemData>();
}