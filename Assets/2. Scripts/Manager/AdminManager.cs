using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminManager : MonoBehaviour
{
    public GameObject stagePortalPrefab; // 스테이지 이동용 포탈 프리팹
    public static AdminManager Instance;

    public void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) ToggleInvincibility();
        if (Input.GetKeyDown(KeyCode.F2)) BoostPlayerStats();
        if (Input.GetKeyDown(KeyCode.F3)) TeleportToRoom(RoomType.Shop);
        if (Input.GetKeyDown(KeyCode.F4)) TeleportToRoom(RoomType.Treasure);
        if (Input.GetKeyDown(KeyCode.F5)) TeleportToRoom(RoomType.Boss);
        if (Input.GetKeyDown(KeyCode.F6)) TeleportToRoom(RoomType.Special);
        if (Input.GetKeyDown(KeyCode.F7)) SpawnStagePortal();
        if (Input.GetKeyDown(KeyCode.F8)) ResetLeaderboard();
    }

    // 2-1. 무적 활성화 (PlayerStat에 isInvincible 변수가 있다고 가정)
    public void ToggleInvincibility()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var stat = player.GetComponent<PlayerStat>();
            stat.isInvincible = !stat.isInvincible;
            stat.isPiercing = !stat.isPiercing;
            Debug.Log($"무적 모드: {stat.isInvincible}");
            Debug.Log($"관통 모드: {stat.isPiercing}");
        }
    }

    // 2-2. 다음 스테이지 이동 (포탈 소환)
    public void SpawnStagePortal()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Instantiate(stagePortalPrefab, player.transform.position + Vector3.down, Quaternion.identity);
        }
    }

    // 2-3. 특정 방으로 이동 (상점, 보물, 보스 등)
    public void TeleportToRoom(RoomType type)
    {
        foreach (var room in MapGenerator.Instance.SpawnedRooms.Values)
        {
            if (room.type == type)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                // 방의 중앙 위치로 이동
                player.transform.position = room.transform.position;
                
                // 메인 카메라가 플레이어를 추적하는 스크립트를 가지고 있다면 그 위치를 강제 고정
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    // 카메라의 Z값은 유지하면서 X, Y만 플레이어에게 맞춤
                    Vector3 targetCamPos = room.transform.position;
                    targetCamPos.z = mainCam.transform.position.z;
                    mainCam.transform.position = targetCamPos;
                
                    // 시네머신(Cinemachine) 용
                    // var virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
                    // virtualCam.OnTargetObjectWarped(player.transform, room.transform.position - player.transform.position);
                }
                
                room.OnPlayerEnter(); // 방 입장 로직 강제 실행
                return;
            }
        }
        Debug.LogWarning($"{type} 방을 찾을 수 없습니다.");
    }

    // 2-4. 기록 초기화
    public void ResetLeaderboard() => RunDataManager.Instance.ClearBestTimes();

    // 2-5. 플레이어 스탯 대폭 강화
    public void BoostPlayerStats()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var stat = player.GetComponent<PlayerStat>();
            stat.atk += 100;
            stat.speed += 1;
            stat.keys += 10;
            stat.gold += 50;
            stat.OnStatChanged?.Invoke();
            Debug.Log("스탯 강화 완료");
        }
    }
}