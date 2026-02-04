using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DataManager : MonoBehaviour {
    public static DataManager Instance;

    public List<ItemRow> itemTable = new List<ItemRow>();
    public List<MapRow> mapTable = new List<MapRow>();
    private HashSet<int> spawnedUniqueItems = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        } else {
            Destroy(gameObject);
        }
        LoadTables();
    }

    void LoadTables() {
        itemTable = ParseTSV<ItemRow>("ItemTable.tsv");
        mapTable = ParseTSV<MapRow>("MapTable.tsv");
    }

    // TSV 파싱 로직 (리플렉션을 활용해 자동 매핑)
    private List<T> ParseTSV<T>(string fileName) where T : new() {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        string[] lines = File.ReadAllLines(path);
        string[] headers = lines[0].Split('\t');
        List<T> list = new List<T>();

        for (int i = 1; i < lines.Length; i++) {
            string[] data = lines[i].Split('\t');
            T obj = new T();
            for (int j = 0; j < headers.Length; j++) {
                var field = typeof(T).GetField(headers[j].ToLower().Replace(" ", ""));
                if (field != null) {
                    if (field.FieldType == typeof(int)) field.SetValue(obj, int.Parse(data[j]));
                    else if (field.FieldType == typeof(bool)) field.SetValue(obj, data[j].ToUpper() == "TRUE");
                    else field.SetValue(obj, data[j]);
                }
            }
            list.Add(obj);
        }
        return list;
    }

    // --- 데이터 요청 메서드들 ---

    public GameObject GetRandomRoom(string roomType, int currentStage) {
        var candidates = mapTable.Where(m => m.roomtype == roomType && 
                                           currentStage >= m.minstage && 
                                           currentStage <= m.maxstage).ToList();
        if (candidates.Count == 0) return null;
        string prefabName = candidates[Random.Range(0, candidates.Count)].prefabname;
        return Resources.Load<GameObject>($"1. Prefabs/Map/Stage{currentStage}/{prefabName}");
    }

    public GameObject GetItem(string roomType, int currentStage = 1) {
        var candidates = itemTable.Where(i => {
            if (spawnedUniqueItems.Contains(i.id)) return false;
            switch(roomType) {
                case "Normal": return i.innormal && i.itemtype == "Pickup";
                case "Treasure": return i.intreasure;
                case "Boss": return i.inboss && i.itemtype == "Passive";
                case "Shop": return i.inshop;
                case "Special": return i.inspecial;
                default: return false;
            }
        }).ToList();

        if (candidates.Count == 0) return null;

        ItemRow selected;
        if (roomType == "Normal") { // 노말룸만 가중치 적용
            int totalWeight = candidates.Sum(c => c.dropweight);
            int rand = Random.Range(0, totalWeight);
            selected = candidates.First(c => (rand -= c.dropweight) < 0);
        } else { // 나머지는 균등 확률
            selected = candidates[Random.Range(0, candidates.Count)];
            if (selected.itemtype != "Pickup") spawnedUniqueItems.Add(selected.id);
        }

        return Resources.Load<GameObject>($"1. Prefabs/Item/{selected.itemtype}/{selected.name}");
    }
}