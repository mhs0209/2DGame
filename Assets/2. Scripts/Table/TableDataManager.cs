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

    public void InitializeData() {
    // 1. Resources 로드 및 TSV 파싱 (기존 코드 유지)
    var masterRooms = Resources.LoadAll<BaseRoomData>("Data/Rooms").ToDictionary(r => r.roomID);
    var masterItems = Resources.LoadAll<ItemData>("Data/Items").ToDictionary(i => i.itemID);
    List<MapTableData> mapRows = ParseTSV<MapTableData>("MapTable.tsv");
    List<ItemTableData> itemRows = ParseTSV<ItemTableData>("ItemTable.tsv");

    // 2. 아이템 마스터 딕셔너리 생성 (GetItemTableData용)
    itemTableDict.Clear();
    ClearItemLists();
    foreach (var row in itemRows) {
        itemTableDict[row.ID] = row;
        if (masterItems.TryGetValue(row.ID, out ItemData item)) {
            if (row.InTreasure) treasureItems.Add(item);
            if (row.InBoss) bossItems.Add(item);
            if (row.InShop) shopItems.Add(item);
            if (row.InSpecial) specialItems.Add(item);
            if (row.InNormal) normalItems.Add(item);
        }
    }

    // 3. 맵 데이터 매칭 및 아이템 풀 주입
    allRooms.Clear();
    foreach (var row in mapRows) {
        if (masterRooms.TryGetValue(row.ID, out BaseRoomData room)) {
            allRooms.Add(room);
            mapTableDict[row.ID] = row;
            room.itemDropPool.Clear();

            // Enum으로 직접 비교 (오타 방지)
            switch (row.RoomType) {
                case RoomType.Treasure: room.itemDropPool.AddRange(treasureItems); break;
                case RoomType.Boss: room.itemDropPool.AddRange(bossItems); break;
                case RoomType.Special: room.itemDropPool.AddRange(specialItems); break;
                case RoomType.Normal: room.itemDropPool.AddRange(normalItems); break;
                case RoomType.Shop:
                    if (room is ShopMap shopMap) {
                        shopMap.shopItemPool = new List<ItemData>(shopItems);
                        shopMap.pickupPool = new List<ItemData>(normalItems);
                    }
                    break;
            }
        }
    }
    Debug.Log($"로드 완료: 맵 {allRooms.Count}개. 데이터 주입 성공.");
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