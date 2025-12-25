using UnityEngine;
using System.Collections.Generic;

public enum RoomState { Empty, Battle, Locked, Cleared }

public class RoomController : MonoBehaviour
{
    private BaseRoom baseRoom;
    public BaseRoomData roomData; // 방 타입별 SO (NormalRoomData 등)
    public RoomState currentState = RoomState.Empty;

    [Header("Doors to Control")]
    public List<GameObject> activeDoors = new List<GameObject>(); // 전투 시 끄고 켤 문 오브젝트들

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Awake()
    {
        baseRoom = GetComponent<BaseRoom>();
    }

    void Update()
    {
        // [테스트용] 1번 키로 잠긴 문 열기
        if (currentState == RoomState.Locked && Input.GetKeyDown(KeyCode.Alpha1))
        {
            UnlockRoom();
        }
    }

    // BaseRoom에서 호출하거나, 트리거를 통해 직접 실행
    public void ActivateRoomLogic()
    {
        if (currentState == RoomState.Cleared) return;

        switch (baseRoom.type)
        {
            case RoomType.Normal:
            case RoomType.Boss:
                StartBattle();
                break;
            case RoomType.Shop:
            case RoomType.Treasure:
            case RoomType.Special:
                LockRoom(); // 열쇠가 필요한 방
                break;
            default:
                currentState = RoomState.Cleared;
                break;
        }
    }

    private void StartBattle()
    {
        currentState = RoomState.Battle;
        SetDoorsActive(true);
        SpawnEnemies();
    }

    private void LockRoom()
    {
        currentState = RoomState.Locked;
        SetDoorsActive(true);
        Debug.Log($"<color=yellow>{baseRoom.type}</color> 방이 잠겼습니다. 1번 키로 열 수 있습니다.");
    }

    private void UnlockRoom()
    {
        currentState = RoomState.Cleared;
        SetDoorsActive(false);
        Debug.Log("열쇠를 사용하여 문을 열었습니다.");
    }

    private void SpawnEnemies()
    {
        if (roomData is NormalMap normalData) // 알려주신 NormalMap 클래스 사용
        {
            int count = Random.Range(normalData.minEnemies, normalData.maxEnemies + 1);
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
                GameObject enemy = Instantiate(normalData.enemyPrefabs[0], spawnPos, Quaternion.identity);
                
                // 몬스터에게 이 컨트롤러 전달
                enemy.GetComponent<NormalMonster>().Setup(this);
                activeEnemies.Add(enemy);
            }
        }
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        if (activeEnemies.Count <= 0)
        {
            currentState = RoomState.Cleared;
            SetDoorsActive(false);
            Debug.Log("전투 종료!");
        }
    }

    private void SetDoorsActive(bool isActive)
    {
        // 기존 BaseRoom의 DoorSet에 있는 door 오브젝트들을 제어
        if (baseRoom.top.door != null && baseRoom.top.door.activeSelf) activeDoors.Add(baseRoom.top.door);
        if (baseRoom.bottom.door != null && baseRoom.bottom.door.activeSelf) activeDoors.Add(baseRoom.bottom.door);
        if (baseRoom.left.door != null && baseRoom.left.door.activeSelf) activeDoors.Add(baseRoom.left.door);
        if (baseRoom.right.door != null && baseRoom.right.door.activeSelf) activeDoors.Add(baseRoom.right.door);

        // 실제 전투용 '닫힌 문' 그래픽이나 콜라이더를 켜고 끔
        foreach (var d in activeDoors)
        {
            // 여기서 d.SetActive(isActive)를 하거나, 
            // 별도의 'Lock' 오브젝트를 제어할 수 있습니다.
        }
    }
}