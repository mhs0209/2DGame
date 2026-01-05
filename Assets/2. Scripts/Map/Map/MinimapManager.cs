using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour, IInitializable
{
    public static MinimapManager Instance;

    [Header("UI References")]
    public RectTransform minimapFrame;     // 최상위 프레임
    public RectTransform minimapWindow;    // 마스크 창 (배경 Image 포함)
    public RectTransform minimapContainer; // 아이콘 부모
    public GameObject iconPrefab;
    private Image windowImage;             // 배경 투명도 조절용

    [Header("Small Map Settings")]
    public Vector2 smallWindowSize = new Vector2(300, 300);
    public Vector2 smallFramePos = new Vector2(750, 400); 
    public float smallScale = 1.0f;

    [Header("Large Map Settings")]
    public Vector2 largeWindowSize = new Vector2(1920, 1080); // 화면 전체 크기 (해상도에 맞게)
    public Vector2 largeFramePos = Vector2.zero;             // 화면 중앙
    public float largeScale = 1.5f;                          // 전체보기 시 적절한 배율

    [Header("Movement Settings")]
    public float minimapSpacing = 50f;
    public float smoothSpeed = 10f;

    private Dictionary<Vector2Int, Image> minimapIcons = new Dictionary<Vector2Int, Image>();
    private HashSet<Vector2Int> visitedRooms = new HashSet<Vector2Int>();
    private Vector2Int currentRoomPos = new Vector2Int(-999, -999);
    private bool isLargeMap = false;
    
    void Awake()
    {
        if (Instance == null) {
            Instance = this;    
        } else {
            Destroy(gameObject); // 이미 존재한다면 새로 생긴 녀석을 제거!
        }
        if (minimapWindow != null) windowImage = minimapWindow.GetComponent<Image>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) isLargeMap = !isLargeMap;
        UpdateMapTransform();
    }

    private void UpdateMapTransform()
    {
        // 1. 목표 설정
        Vector2 targetWinSize = isLargeMap ? largeWindowSize : smallWindowSize;
        Vector2 targetFramePos = isLargeMap ? largeFramePos : smallFramePos;
        float targetScale = isLargeMap ? largeScale : smallScale;
        float targetAlpha = isLargeMap ? 0f : 0.6f; // 전체보기일 때 배경 투명하게 (평소엔 0.6)

        // 2. 부드러운 전환
        minimapFrame.anchoredPosition = Vector2.Lerp(minimapFrame.anchoredPosition, targetFramePos, Time.deltaTime * smoothSpeed);
        minimapWindow.sizeDelta = Vector2.Lerp(minimapWindow.sizeDelta, targetWinSize, Time.deltaTime * smoothSpeed);
        minimapContainer.localScale = Vector3.Lerp(minimapContainer.localScale, Vector3.one * targetScale, Time.deltaTime * smoothSpeed);

        // 배경 알파값 조절
        if (windowImage != null)
        {
            Color color = windowImage.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * smoothSpeed);
            windowImage.color = color;
        }

        // 3. 중심점 결정 (중요!)
        // 소형일 때는 현재 방(currentRoomPos), 대형일 때는 시작 방(0, 0) 기준
        Vector2 focusPos = isLargeMap ? Vector2.zero : (Vector2)currentRoomPos;

        if (currentRoomPos.x != -999)
        {
            Vector2 targetContainerPos = -focusPos * minimapSpacing;
            minimapContainer.anchoredPosition = Vector2.Lerp(minimapContainer.anchoredPosition, targetContainerPos, Time.deltaTime * smoothSpeed);
        }
    }

    // --- 이하 UpdateRoomIcon 및 RefreshAllIcons 코드는 기존과 동일 ---
    public void UpdateRoomIcon(Vector2Int pos, bool isCurrent)
    {
        if (!MapGenerator.Instance.DungeonMap.ContainsKey(pos)) return;

        if (!minimapIcons.ContainsKey(pos))
        {
            GameObject icon = Instantiate(iconPrefab, minimapContainer);
            icon.GetComponent<RectTransform>().anchoredPosition = (Vector2)pos * minimapSpacing;
            minimapIcons.Add(pos, icon.GetComponent<Image>());
            
            RoomType type = MapGenerator.Instance.DungeonMap[pos];
            var data = MapGenerator.Instance.allRoomSO.Find(so => so.roomType == type);
            if (data != null && data.minimapIcon != null) icon.GetComponent<Image>().sprite = data.minimapIcon;
        }

        if (isCurrent)
        {
            currentRoomPos = pos;
            visitedRooms.Add(pos);
        }
        RefreshAllIcons();
    }
    
    public void OnLevelInit()
    {
        ClearMinimap(); // 기존에 만들었던 초기화 로직 호출
    }
    
    public void ClearMinimap()
    {
        // 1. 하이러키에 생성된 모든 아이콘 오브젝트 파괴
        foreach (var img in minimapIcons.Values)
        {
            if (img != null && img.gameObject != null)
            {
                Destroy(img.gameObject);
            }
        }

        // 2. 내부 데이터 구조 초기화
        minimapIcons.Clear();
        visitedRooms.Clear();
        currentRoomPos = new Vector2Int(-999, -999);

        // 3. 컨테이너 위치 초기화 (0,0으로 복귀)
        if (minimapContainer != null)
        {
            minimapContainer.anchoredPosition = Vector2.zero;
        }

        Debug.Log("미니맵 초기화 완료 (아이콘 및 데이터 제거)");
    }

    // [방어 코드 추가] RefreshAllIcons에서 Null 체크
    private void RefreshAllIcons()
    {
        // 딕셔너리를 순회할 때 이미 파괴된 객체가 있을 수 있으므로 방어 로직 추가
        foreach (var pair in minimapIcons)
        {
            Vector2Int pos = pair.Key;
            Image img = pair.Value;

            // 이미 파괴된 이미지라면 건너뜀
            if (img == null) continue;

            // MapGenerator의 데이터가 현재 미니맵 데이터와 일치하는지 확인
            if (!MapGenerator.Instance.DungeonMap.ContainsKey(pos)) continue;

            RoomType type = MapGenerator.Instance.DungeonMap[pos];

            if (pos == currentRoomPos) img.color = Color.white;
            else if (visitedRooms.Contains(pos)) img.color = Color.gray;
            else img.color = (type == RoomType.Normal || type == RoomType.Base) ? Color.black : new Color(0.3f, 0.3f, 0.3f, 1f);
        }
    }
    
}