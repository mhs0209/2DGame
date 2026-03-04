using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Map", menuName = "Map/Boss")]
public class BossMap : BaseRoomData 
{
    [Header("Stage Transition")]
    public GameObject stagePortalPrefab; // 다음 스테이지 이동 포탈 프리팹
}
