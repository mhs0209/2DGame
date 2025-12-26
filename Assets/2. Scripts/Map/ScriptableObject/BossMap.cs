using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Room Data", menuName = "RoomData/BossRoomData")]
public class BossMap : BaseRoomData
{
    public GameObject[] bossPrefabs; // 보스 몬스터들
    public int bossRewardPoolID; // 보스 처치 후 나올 아이템 풀
    public string bossEntryMessage; // 보스 등장 문구
}
