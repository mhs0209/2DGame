using UnityEngine;
using System.Collections.Generic;

public enum RoomState { Empty, Battle, Locked, Cleared }

public class RoomController : MonoBehaviour
{
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

                    Debug.Log("열쇠를 사용하여 문을 열었습니다. 이제 자유롭게 드나들 수 있습니다.");
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
        
        // 자식 방 스크립트(NormalRoom, BossRoom 등)를 찾아 즉시 실행
        ItemRoom handler = GetComponent<ItemRoom>();
        if (handler != null) 
        {
            handler.OnRoomCleared(); 
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
    
    // RoomController.cs 일부 수정
    private void SpawnEnemies()
    {
        // 1. 노말 맵일 때
        if (roomData is NormalMap normalData)
        {
            int count = Random.Range(normalData.minEnemies, normalData.maxEnemies + 1);
            for (int i = 0; i < count; i++)
            {
                CreateMonster(normalData.enemyPrefabs[Random.Range(0, normalData.enemyPrefabs.Length)]);
            }
        }
        // 2. 보스 맵일 때 (이 부분이 추가/확인되어야 함)
        else if (roomData is BossMap bossData)
        {
            if (bossData.bossPrefabs.Length > 0)
            {
                // 보스는 보통 방 중앙에 하나 생성
                CreateMonster(bossData.bossPrefabs[Random.Range(0, bossData.bossPrefabs.Length)], transform.position);
            }
        }
    }

    private void CreateMonster(GameObject prefab, Vector3 pos = default)
    {
        if (prefab == null) return;
        Vector3 spawnPos = (pos == default) ? transform.position + (Vector3)Random.insideUnitCircle * 2f : pos;
        GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity);
        monster.GetComponent<NormalMonster>()?.Setup(this); // 몬스터 스크립트에 맞게 수정
        activeEnemies.Add(monster);
    }
    
}