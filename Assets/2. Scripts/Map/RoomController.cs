using System.Collections;
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
    
    public void OnPlayerTryEntry(DoorPhysics door)
    {
        PlayerStat pStat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>();
        
        // GetKey가 아닌 GetKeyDown을 사용하여 프레임 입력 보장
        if (Input.GetKeyDown(KeyCode.Alpha1) || (pStat.keys > 0)) 
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1)) 
            {
                pStat.keys--;
                Debug.Log($"열쇠 사용! 남은 열쇠: {pStat.keys}");
                pStat.OnStatChanged?.Invoke();
            }

            door.isSpecialLock = false;
            door.SetLock(false);
            Debug.Log("잠긴 문을 열었습니다!");
        }
    }
    
    public void LockAllDoorsTemporarily(float duration)
    {
        // 이미 전투 중이면 전투 로직이 문을 관리함
        if (currentState == RoomState.Battle) return;

        StopAllCoroutines();
        StartCoroutine(TempLockRoutine(duration));
    }

    private IEnumerator TempLockRoutine(float duration)
    {
        // 모든 문 잠금 (빨간색 벽)
        foreach (var door in doorMap.Values)
        {
            if (door != null) door.SetLock(true);
        }

        yield return new WaitForSeconds(duration);

        // 다시 모든 문 상태 복구
        foreach (var door in doorMap.Values)
        {
            if (door == null) continue;

            // [체크] 열쇠를 아직 안 쓴 특수방 문이 아니라면 문을 열어줌
            // 이미 열쇠를 써서 isSpecialLock이 false가 된 문은 정상적으로 열립니다.
            if (!door.isSpecialLock)
            {
                door.SetLock(false);
            }
        }
    }

    // --- 전투 로직 ---
    
    public void StartBattle()
    {
        if (currentState == RoomState.Cleared || currentState == RoomState.Battle) return;

        currentState = RoomState.Battle;

        // 모든 문을 잠금 (isTrigger = false)
        foreach (var d in doorMap.Values) 
        {
            d.SetLock(true);
        }

        DetectEnemies();
        if (activeEnemies.Count <= 0) EndBattle();
    }

    private void DetectEnemies()
    {
        activeEnemies.Clear();
        // 자식 오브젝트들 중에서 MonsterStat을 가진 모든 객체를 찾음
        MonsterStat[] enemiesInRoom = GetComponentsInChildren<MonsterStat>(true);

        foreach (var enemyStat in enemiesInRoom)
        {
            GameObject enemy = enemyStat.gameObject;
            enemy.SetActive(true); // 비활성화 상태였다면 활성화
            enemyStat.myRoom = this; // 몬스터에게 현재 방 참조 전달
            activeEnemies.Add(enemy);
            
            // 만약 보스라면 UI 매니저를 통해 체력바 출력
            if (enemyStat.monsterType == MonsterType.Boss)
            {
                // 임시로 Find 사용 (나중엔 Singleton UI Manager 권장)
                FindObjectOfType<BossHealthUI>(true).ShowBossBar(enemyStat);
            }
        }
        
        Debug.Log($"{gameObject.name} 방에서 {activeEnemies.Count}마리의 적 감지!");
    }

    public void OnEnemyDeath(GameObject enemy) 
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            if (activeEnemies.Count <= 0) EndBattle();
        }
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
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<ActiveInventory>()?.AddChargeAll(1);
    }
    
    // ActivateRoomLogic 함수는 에러 방지를 위해 유지
    public void ActivateRoomLogic()
    {
        // 1. 전투가 필요한 방이고 아직 안 싸웠다면 전투 시작
        if (currentState != RoomState.Cleared && (baseRoom.type == RoomType.Normal || baseRoom.type == RoomType.Boss)) 
        {
            StartBattle();
            return; // 전투 로직에서 모든 문을 잠그므로 여기서 종료
        }

        // 2. 이미 클리어했거나 빈 방(Empty)인 경우 -> 역행 방지용 임시 잠금 실행
        // 전투 중(Battle) 상태가 아닐 때만 실행됩니다.
        LockAllDoorsTemporarily(1.0f);
    }
}