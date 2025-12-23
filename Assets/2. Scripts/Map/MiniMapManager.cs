using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance;
    public GameObject iconPrefab; // 미니맵에 표시될 UI 이미지 프리팹
    public Transform minimapContainer;
    public float minimapSpacing = 50f;
    
    private Dictionary<Vector2Int, Image> minimapIcons = new Dictionary<Vector2Int, Image>();

    void Awake() => Instance = this;

    // MinimapManager.cs (핵심 로직)
    public void UpdateRoomIcon(Vector2Int pos, bool isVisited)
    {
        // 맵 데이터에 없는 좌표면 무시
        if (!MapGenerator.Instance.DungeonMap.ContainsKey(pos)) return;

        if (!minimapIcons.ContainsKey(pos))
        {
            // 처음 발견 시 아이콘 생성
            GameObject icon = Instantiate(iconPrefab, minimapContainer);
            icon.GetComponent<RectTransform>().anchoredPosition = (Vector2)pos * minimapSpacing;
            minimapIcons.Add(pos, icon.GetComponent<Image>());
        }

        Image targetImage = minimapIcons[pos];
        RoomType type = MapGenerator.Instance.DungeonMap[pos];

        if (isVisited)
        {
            // 방문한 방: 밝은 색 + 실제 아이콘 표시 (보스, 상점 등)
            targetImage.color = Color.white;
            // targetImage.sprite = GetSpriteByType(type); // SO에서 아이콘 가져오기
        }
        else
        {
            // 인접한 방(발견만 됨): 어두운 색 또는 물음표
            targetImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); 
        }
    }
}