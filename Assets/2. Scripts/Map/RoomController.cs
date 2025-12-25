using UnityEngine;
using System.Collections.Generic;

public enum RoomState { Empty, Battle, Locked, Cleared }

public class RoomController : MonoBehaviour
{
    // private BaseRoom baseRoom;
    // public BaseRoomData roomData; // 방 타입별 SO (NormalRoomData 등)
    // public RoomState currentState = RoomState.Empty;
    //
    // [Header("Doors to Control")]
    // public List<GameObject> activeDoors = new List<GameObject>(); // 전투 시 끄고 켤 문 오브젝트들
    //
    // private List<GameObject> activeEnemies = new List<GameObject>();
    //
    // void Awake()
    // {
    //     baseRoom = GetComponent<BaseRoom>();
    // }
    //
    // void Update()
    // {
    //     // [테스트용] 1번 키로 잠긴 문 열기
    //     if (currentState == RoomState.Locked && Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         UnlockRoom();
    //     }
    // }
    //
    // // BaseRoom에서 호출하거나, 트리거를 통해 직접 실행
    // public void ActivateRoomLogic()
    // {
    //     if (currentState == RoomState.Cleared) return;
    //
    //     switch (baseRoom.type)
    //     {
    //         case RoomType.Normal:
    //         case RoomType.Boss:
    //             StartBattle();
    //             break;
    //         case RoomType.Shop:
    //         case RoomType.Treasure:
    //         case RoomType.Special:
    //             LockRoom(); // 열쇠가 필요한 방
    //             break;
    //         default:
    //             currentState = RoomState.Cleared;
    //             break;
    //     }
    // }
    //
    // private void StartBattle()
    // {
    //     currentState = RoomState.Battle;
    //     SetDoorsActive(true);
    //     SpawnEnemies();
    // }
    //
    // private void LockRoom()
    // {
    //     currentState = RoomState.Locked;
    //     SetDoorsActive(true);
    //     Debug.Log($"<color=yellow>{baseRoom.type}</color> 방이 잠겼습니다. 1번 키로 열 수 있습니다.");
    // }
    //
    // private void UnlockRoom()
    // {
    //     currentState = RoomState.Cleared;
    //     SetDoorsActive(false);
    //     Debug.Log("열쇠를 사용하여 문을 열었습니다.");
    // }
    //
    // private void SpawnEnemies()
    // {
    //     if (roomData is NormalMap normalData) // 알려주신 NormalMap 클래스 사용
    //     {
    //         int count = Random.Range(normalData.minEnemies, normalData.maxEnemies + 1);
    //         for (int i = 0; i < count; i++)
    //         {
    //             Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
    //             GameObject enemy = Instantiate(normalData.enemyPrefabs[0], spawnPos, Quaternion.identity);
    //             
    //             // 몬스터에게 이 컨트롤러 전달
    //             enemy.GetComponent<NormalMonster>().Setup(this);
    //             activeEnemies.Add(enemy);
    //         }
    //     }
    // }
    //
    // public void OnEnemyDeath(GameObject enemy)
    // {
    //     activeEnemies.Remove(enemy);
    //     if (activeEnemies.Count <= 0)
    //     {
    //         currentState = RoomState.Cleared;
    //         SetDoorsActive(false);
    //         Debug.Log("전투 종료!");
    //     }
    // }
    //
    // private void SetDoorsActive(bool isActive)
    // {
    //     // 기존 BaseRoom의 DoorSet에 있는 door 오브젝트들을 제어
    //     if (baseRoom.top.door != null && baseRoom.top.door.activeSelf) activeDoors.Add(baseRoom.top.door);
    //     if (baseRoom.bottom.door != null && baseRoom.bottom.door.activeSelf) activeDoors.Add(baseRoom.bottom.door);
    //     if (baseRoom.left.door != null && baseRoom.left.door.activeSelf) activeDoors.Add(baseRoom.left.door);
    //     if (baseRoom.right.door != null && baseRoom.right.door.activeSelf) activeDoors.Add(baseRoom.right.door);
    //
    //     // 실제 전투용 '닫힌 문' 그래픽이나 콜라이더를 켜고 끔
    //     foreach (var d in activeDoors)
    //     {
    //         // 여기서 d.SetActive(isActive)를 하거나, 
    //         // 별도의 'Lock' 오브젝트를 제어할 수 있습니다.
    //     }
    // }
    // static 딕셔너리로 모든 방 관리 (MapGenerator에서 등록 필수)
    public static Dictionary<Vector2Int, RoomController> RoomMap = new Dictionary<Vector2Int, RoomController>();

    public BaseRoom baseRoom;
    public BaseRoomData roomData;
    public RoomState currentState = RoomState.Empty;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Dictionary<Vector2Int, DoorPhysics> doorMap = new Dictionary<Vector2Int, DoorPhysics>();

    void Awake()
    {
        baseRoom = GetComponent<BaseRoom>();
        CacheDoors();
        
        // 좌표 등록 (이미 있다면 갱신)
        RoomMap[baseRoom.gridPos] = this;
    }

    private void CacheDoors()
    {
        // 부모 오브젝트에 DoorPhysics를 붙이고 자식 2개를 제어하게 함
        AddOrGetDoorPhysics(Vector2Int.up, baseRoom.top.door);
        AddOrGetDoorPhysics(Vector2Int.down, baseRoom.bottom.door);
        AddOrGetDoorPhysics(Vector2Int.left, baseRoom.left.door);
        AddOrGetDoorPhysics(Vector2Int.right, baseRoom.right.door);
    }

    private void AddOrGetDoorPhysics(Vector2Int dir, GameObject doorObj)
    {
        if (doorObj != null)
            doorMap[dir] = doorObj.GetComponent<DoorPhysics>() ?? doorObj.AddComponent<DoorPhysics>();
    }

    public DoorPhysics GetDoorPhysics(Vector2Int dir) => doorMap.ContainsKey(dir) ? doorMap[dir] : null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryUnlock();
    }

    private void TryUnlock()
    {
        foreach (var pair in doorMap)
        {
            Vector2Int dir = pair.Key;
            DoorPhysics myDoor = pair.Value;

            if (myDoor.isSpecialLock)
            {
                float dist = Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, myDoor.transform.position);
                if (dist < 2.5f)
                {
                    // 1. 내 방의 문 해제
                    UnlockSpecificDoor(dir);

                    // 2. 이웃 방의 반대편 문도 직접 찾아 해제 (양방향 동시 오픈)
                    Vector2Int neighborPos = baseRoom.gridPos + dir;
                    if (RoomMap.TryGetValue(neighborPos, out RoomController neighbor))
                    {
                        neighbor.UnlockSpecificDoor(-dir);
                    }

                    Debug.Log($"[{baseRoom.type}]방에서 [{dir}] 방향 문을 양방향으로 영구 개방했습니다.");
                    return;
                }
            }
        }
    }

    // 특정 문을 완전히 여는 함수
    public void UnlockSpecificDoor(Vector2Int dir)
    {
        if (doorMap.TryGetValue(dir, out DoorPhysics door))
        {
            door.isSpecialLock = false; // 특수 잠금 해제 (전투 종료 후에도 영향 안 받음)
            door.SetLock(false);       // 물리 해제 및 흰색 변경
        }
    }

    // --- 전투 로직 ---
    public void StartBattle()
    {
        currentState = RoomState.Battle;
        foreach (var d in doorMap.Values) d.SetLock(true);
        SpawnEnemies();
    }

    public void EndBattle()
    {
        currentState = RoomState.Cleared;
        foreach (var pair in doorMap)
        {
            // 아직 열쇠가 필요한 문이면 열지 않음
            if (pair.Value.isSpecialLock) continue; 
            pair.Value.SetLock(false);
        }
    }

    public void OnEnemyDeath(GameObject enemy) {
        activeEnemies.Remove(enemy);
        if (activeEnemies.Count <= 0) EndBattle();
    }
    
    // ActivateRoomLogic 함수는 에러 방지를 위해 유지
    public void ActivateRoomLogic()
    {
        if (currentState == RoomState.Cleared || currentState == RoomState.Battle) return;
        if (baseRoom.type == RoomType.Normal || baseRoom.type == RoomType.Boss) StartBattle();
    }

    private void SpawnEnemies()
    {
        if (roomData is NormalMap normalData && normalData.enemyPrefabs.Length > 0)
        {
            int count = Random.Range(normalData.minEnemies, normalData.maxEnemies + 1);
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = transform.position + (Vector3)Random.insideUnitCircle * 3f;
                GameObject enemy = Instantiate(normalData.enemyPrefabs[0], spawnPos, Quaternion.identity);
                enemy.GetComponent<NormalMonster>().Setup(this);
                activeEnemies.Add(enemy);
            }
        }
    }
}