using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Treasure Room Data", menuName = "RoomData/TreasureRoomData")]
public class TreasureMap : BaseRoomData
{
    public int itemPoolID; // 어떤 아이템 풀에서 뽑을지 (카테고리 ID)
    public bool spawnPedestalAtStart = true; // 시작하자마자 제단을 생성할지
    public GameObject specialEffect; // 황금방 전용 빛 연출 등
}
