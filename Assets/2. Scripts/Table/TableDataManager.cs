using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using System.Linq;

public class TableDataManager : MonoBehaviour {
    public static TableDataManager Instance;

    [Header("Mapped SO Lists")]
    public List<BaseRoomData> allRooms = new List<BaseRoomData>();
    public List<ItemData> treasureItems = new List<ItemData>();
    public List<ItemData> bossItems = new List<ItemData>();
    public List<ItemData> shopItems = new List<ItemData>();
    public List<ItemData> specialItems = new List<ItemData>();
    public List<ItemData> normalItems = new List<ItemData>();

    // 스테이지 필터링용 데이터 저장 (ID, MapTableData)
    private Dictionary<int, MapTableData> mapTableDict = new Dictionary<int, MapTableData>();
    // ID로 테이블 로우 데이터를 빠르게 찾기 위한 딕셔너리
    private Dictionary<int, ItemTableData> itemTableDict = new Dictionary<int, ItemTableData>();

    void Awake() {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        InitializeData();
    }

    public void InitializeData()
    {
        // 1. Resources 로드 및 TSV 파싱
        var masterRooms = Resources.LoadAll<BaseRoomData>("Data/Rooms").ToDictionary(r => r.roomID);
        var masterItems = Resources.LoadAll<ItemData>("Data/Items").ToDictionary(i => i.itemID);
        List<MapTableData> mapRows = ParseTSV<MapTableData>("MapTable.tsv");
        List<ItemTableData> itemRows = ParseTSV<ItemTableData>("ItemTable.tsv");

        // 2. 아이템 마스터 딕셔너리 및 기본 리스트 초기화
        itemTableDict.Clear();
        ClearItemLists();

        foreach (var row in itemRows)
        {
            itemTableDict[row.ID] = row;
            if (masterItems.TryGetValue(row.ID, out ItemData item))
            {
                // 각 카테고리별 리스트 채우기
                if (row.InTreasure) treasureItems.Add(item);
                if (row.InBoss) bossItems.Add(item);
                if (row.InSpecial) specialItems.Add(item);

                // [분류 핵심] 상점용 아이템 리스트 (15G 패시브/액티브)
                // InShop이 T이면서 InNormal이 F인 경우만 순수 상점템으로 분류
                if (row.InShop && !row.InNormal) shopItems.Add(item);

                // [분류 핵심] 노말 드랍 및 상점 픽업용 (InNormal이 T인 모든 아이템)
                if (row.InNormal) normalItems.Add(item);
            }
        }

        // 3. 맵 데이터 매칭 및 아이템 풀 주입
        allRooms.Clear();
        foreach (var row in mapRows)
        {
            if (masterRooms.TryGetValue(row.ID, out BaseRoomData room))
            {
                allRooms.Add(room);
                mapTableDict[row.ID] = row;
                room.itemDropPool.Clear();

                switch (row.RoomType)
                {
                    case RoomType.Treasure: room.itemDropPool.AddRange(treasureItems); break;
                    case RoomType.Boss: room.itemDropPool.AddRange(bossItems); break;
                    case RoomType.Special: room.itemDropPool.AddRange(specialItems); break;
                    case RoomType.Normal: room.itemDropPool.AddRange(normalItems); break;

                    case RoomType.Shop:
                        if (room is ShopMap shopMap)
                        {
                            // 1. 패시브 아이템 풀 (InShop: T, InNormal: F)
                            shopMap.shopItemPool = new List<ItemData>(shopItems);

                            // 2. 상점 전용 픽업 풀 (InShop: T, InNormal: T)
                            // normalItems 중에서 InShop까지 TRUE인 것들만 골라냅니다.
                            shopMap.pickupPool = itemRows
                                .Where(r => r.InShop && r.InNormal)
                                .Select(r => masterItems.ContainsKey(r.ID) ? masterItems[r.ID] : null)
                                .Where(i => i != null)
                                .ToList();

                            // 3. 기본 풀 (혹시 모를 리롤/참조 대비)
                            room.itemDropPool.AddRange(shopMap.shopItemPool);
                            room.itemDropPool.AddRange(shopMap.pickupPool);
                        }

                        break;
                }
            }
        }

        Debug.Log(
            $"[TableDataManager] 로드 완료: 맵 {allRooms.Count}개. 상점 분류(장비:{shopItems.Count}, 픽업:{normalItems.Count})");
    }

    private void ClearItemLists() {
        treasureItems.Clear(); bossItems.Clear(); shopItems.Clear();
        specialItems.Clear(); normalItems.Clear();
    }

    // --- 리플렉션 기반 TSV 파서 (전달해주신 로직 적용) ---
    private List<T> ParseTSV<T>(string fileName) where T : new() {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path)) {
            Debug.LogError($"파일을 찾을 수 없습니다: {path}");
            return new List<T>();
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length <= 1) return new List<T>();

        string[] headers = lines[0].Trim().Split('\t');
        List<T> list = new List<T>();

        for (int i = 1; i < lines.Length; i++) {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] data = lines[i].Split('\t');
            T obj = new T();

            for (int j = 0; j < headers.Length; j++) {
                if (j >= data.Length) break;

                // 헤더명 공백 제거 및 대소문자 무시 매칭
                string headerName = headers[j].Trim().Replace(" ", "");
                FieldInfo field = typeof(T).GetField(headerName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (field != null) {
                    string value = data[j].Trim();
                    // ParseTSV 함수 내부 수정
                    try {
                        if (field.FieldType == typeof(int)) field.SetValue(obj, int.Parse(value));
                        else if (field.FieldType == typeof(bool)) field.SetValue(obj, value.ToUpper() == "TRUE" || value == "1");
                        else if (field.FieldType == typeof(float)) field.SetValue(obj, float.Parse(value));
                        // --- 추가된 Enum 처리 로직 ---
                        else if (field.FieldType.IsEnum) field.SetValue(obj, Enum.Parse(field.FieldType, value, true));
                        // -------------------------
                        else field.SetValue(obj, value);
                    } catch { /* 스킵 */ }
                }
            }
            list.Add(obj);
        }
        return list;
    }

    // --- 스테이지 필터링 (MapGenerator에서 사용) ---
    public List<BaseRoomData> GetRoomsForStage(int stage) {
        return allRooms.Where(room => {
            if (mapTableDict.TryGetValue(room.roomID, out MapTableData data)) {
                return stage >= data.MinStage && stage <= data.MaxStage;
            }
            return false;
        }).ToList();
    }
    
    public ItemTableData GetItemTableData(int id) {
        if (itemTableDict.TryGetValue(id, out ItemTableData data)) {
            return data;
        }
        return null;
    }
}