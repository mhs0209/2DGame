using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    // =================================================================================
    #region Singleton
    public static MapGenerator Instance;
    #endregion
    // =================================================================================


    // =================================================================================
    #region Public Variables
    [Header("Map Settings")]
    public int maxRooms = 10;
    public float roomSpacing = 20f; // 방 사이의 실제 간격
    
    [Header("Room Prefabs")]
    public List<BaseRoomData> allRoomSO;

    public IReadOnlyDictionary<Vector2Int, RoomType> DungeonMap => dungeonMap;
    #endregion
    // =================================================================================


    // =================================================================================
    #region Private Variables
    private Dictionary<Vector2Int, RoomType> dungeonMap = new Dictionary<Vector2Int, RoomType>();
    private List<Vector2Int> roomPositions = new List<Vector2Int>();
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private Dictionary<Vector2Int, BaseRoom> spawnedRooms = new Dictionary<Vector2Int, BaseRoom>();
    #endregion
    // =================================================================================


    // =================================================================================
    #region Unity Lifecycle
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        allRoomSO = TableDataManager.Instance.GetRoomsForStage(SceneManager.GetActiveScene().buildIndex);
        GenerateMap();
        // You can now use the 'data' variable which contains the parsed TSV data.
    }
    #endregion
    // =================================================================================
    
    
    // =================================================================================
    #region Map Generation
    private void GenerateMap()
    {
        dungeonMap.Clear();
        roomPositions.Clear();
        spawnedRooms.Clear();

        // 1. 시작 방 배치
        Vector2Int startPos = Vector2Int.zero;
        dungeonMap.Add(startPos, RoomType.Base);
        roomPositions.Add(startPos);

        // 2. 일반 방 생성 (밀집도 체크 강화)
        CreateNormalRooms();

        // 3. 특수 방 배치 (연결점이 1개인 '막다른 길' 위주로 배치)
        PlaceSpecialRooms();
        
        // 4. 프리팹 생성
        InstantiateRooms();

        // 5. 문/벽 상태 업데이트 (모든 생성이 끝난 직후 호출)
        SetupAllRoomDoors();

        // [중요] 시작 방(0,0)의 미니맵과 인접 방을 강제로 활성화
        if (spawnedRooms.ContainsKey(Vector2Int.zero))
        {
            spawnedRooms[Vector2Int.zero].OnPlayerEnter();
        }
        
        SetupAllDoorLocks();
        
        AstarPath.active.Scan();
    }

    private void CreateNormalRooms()
    {
        int attempts = 0;
        while (roomPositions.Count < maxRooms && attempts < 500)
        {
            attempts++;
            // 기존 방 중 하나를 골라 주변에 새 방 시도
            Vector2Int origin = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2Int nextPos = origin + directions[Random.Range(0, 4)];

            if (CanPlaceRoom(nextPos))
            {
                dungeonMap.Add(nextPos, RoomType.Normal);
                roomPositions.Add(nextPos);
            }
        }
    }

    private void PlaceSpecialRooms()
    {
        // 보스방과 특수방은 가장 먼 곳으로 (true)
        PlaceSpecialRoomStrict(RoomType.Boss, true);
        if (Random.Range(0, 100) < 90) PlaceSpecialRoomStrict(RoomType.Special, true);

        // 상점과 보물방은 랜덤한 위치에 (false)
        PlaceSpecialRoomStrict(RoomType.Shop, false);
        PlaceSpecialRoomStrict(RoomType.Treasure, false);
    }

    /// <summary>
    /// 특수 방 배치 로직
    /// findFarthest가 true면 가장 먼 곳, false면 랜덤한 후보지 선택
    /// </summary>
    private void PlaceSpecialRoomStrict(RoomType type, bool findFarthest)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        foreach (var pos in roomPositions)
        {
            foreach (var dir in directions)
            {
                Vector2Int potentialPos = pos + dir;

                if (dungeonMap.ContainsKey(potentialPos)) continue;

                int connectionCount = 0;
                bool isAdjacentToSpecial = false;

                foreach (var d in directions)
                {
                    if (dungeonMap.TryGetValue(potentialPos + d, out RoomType neighborType))
                    {
                        connectionCount++;
                        if (neighborType != RoomType.Normal && neighborType != RoomType.Base)
                        {
                            isAdjacentToSpecial = true;
                        }
                    }
                }

                // 조건: 막다른 길(연결 1개) + 주변에 특수방 없음
                if (connectionCount == 1 && !isAdjacentToSpecial)
                    candidates.Add(potentialPos);
            }
        }

        if (candidates.Count > 0)
        {
            Vector2Int chosenPos;

            if (findFarthest)
            {
                // 가장 먼 곳 찾기 (보스, 특수이벤트용)
                chosenPos = candidates.OrderByDescending(p => Vector2Int.Distance(p, Vector2Int.zero)).First();
            }
            else
            {
                // 리스트 중 완전 랜덤 선택 (상점, 보물방용)
                // 시작점 바로 옆 후보지가 포함되어 있다면 거기에도 생성될 수 있음
                chosenPos = candidates[Random.Range(0, candidates.Count)];
            }

            dungeonMap.Add(chosenPos, type);
            roomPositions.Add(chosenPos);
        }
    }

    // MapGenerator.cs 의 SetupAllDoorLocks 내부 수정
    private void SetupAllDoorLocks()
    {
        foreach (var roomPos in spawnedRooms.Keys)
        {
            // 1. 현재 방의 RoomController 가져오기
            RoomController currentRoom = spawnedRooms[roomPos].GetComponent<RoomController>();
            if (currentRoom == null) continue;

            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (Vector2Int d in dirs)
            {
                // 2. 이웃 방 오브젝트를 먼저 GameObject로 찾기
                if (spawnedRooms.TryGetValue(roomPos + d, out BaseRoom neighborObj))
                {
                    // 3. 해당 오브젝트에서 RoomController 가져오기
                    RoomController neighbor = neighborObj.GetComponent<RoomController>();
                    if (neighbor == null) continue;

                    // 옆방이 특수방이라면 양쪽 문 마킹
                    if (IsSpecialRoom(neighbor.baseRoom.type))
                    {
                        var myDoor = currentRoom.GetDoorPhysics(d);
                        if (myDoor != null) {
                            myDoor.isSpecialLock = true;
                            myDoor.SetLock(true);
                        }
                    }
                }
            }
        }
    }

    private bool IsSpecialRoom(RoomType type) 
    {
        return type == RoomType.Shop || type == RoomType.Treasure || type == RoomType.Special;
    }
    
    /// <summary>
    /// [수정] 방 밀집도 체크: 상하좌우뿐만 아니라 대각선에 방이 있는지 확인
    /// </summary>
    private bool CanPlaceRoom(Vector2Int pos)
    {
        // 1. 이미 방이 있는 위치면 안 됨
        if (dungeonMap.ContainsKey(pos)) return false;

        int neighborCount = 0;
        foreach (var dir in directions)
        {
            // 상하좌우에 방이 몇 개 있는지 체크
            if (dungeonMap.ContainsKey(pos + dir))
            {
                neighborCount++;
            }
        }

        // 인접한 방이 딱 1개일 때만 생성 허용 (뭉침 방지 핵심)
        // 인접한 방이 2개 이상이라는 뜻은 이미 그 자리가 다른 방들 사이에 끼어있다는 뜻입니다.
        return neighborCount == 1;
    }

    private void InstantiateRooms()
    {
        foreach (var pair in dungeonMap)
        {
            BaseRoomData roomData = GetRoomDataByType(pair.Value);
            if (roomData == null) continue;

            GameObject roomObj = Instantiate(roomData.roomPrefab, new Vector3(pair.Key.x * roomSpacing, pair.Key.y * roomSpacing, 0), Quaternion.identity);
            BaseRoom roomScript = roomObj.GetComponent<BaseRoom>();
            roomScript.gridPos = pair.Key;
            roomScript.type = pair.Value;
            spawnedRooms.Add(pair.Key, roomScript);
        }
    }

    private void SetupAllRoomDoors()
    {
        foreach (var room in spawnedRooms.Values)
        {
            room.SetupDoors(pos => dungeonMap.ContainsKey(pos));
        }
    }
    
    // MapGenerator.cs 에 추가
    public BaseRoom GetRoomAt(Vector2Int pos)
    {
        if (spawnedRooms.ContainsKey(pos)) return spawnedRooms[pos];
        return null;
    }
    #endregion
    // =================================================================================

    
    // =================================================================================
    #region Utility
    // MapGenerator.cs 내부 수정
    private BaseRoomData GetRoomDataByType(RoomType type)
    {
        // 1. 해당 타입에 맞는 모든 SO를 리스트로 추출
        var matchingData = allRoomSO.FindAll(so => so.roomType == type);

        if (matchingData.Count == 0)
        {
            Debug.LogError($"{type} 타입의 RoomData가 allRoomSO에 없습니다!");
            return null;
        }

        // 2. 그중 하나를 랜덤하게 반환
        return matchingData[Random.Range(0, matchingData.Count)];
    }
    
    #endregion
    // =================================================================================
}