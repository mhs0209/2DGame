using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClearSceneUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public Image activeItemImage;
    public Transform passiveContainer;
    public GameObject itemIconPrefab; // Image 컴포넌트가 포함된 프리팹

    void Start()
    {
        // 결과창 진입 시 타이머는 확실히 정지
        RunDataManager.Instance.StopTimer();
        DisplayRunResults();
    }

    void DisplayRunResults()
    {
        var data = RunDataManager.Instance;

        // 1. 시간 포맷팅 (00:00:00)
        int hours = (int)(data.elapsedTime / 3600);
        int minutes = (int)((data.elapsedTime % 3600) / 60);
        int seconds = (int)(data.elapsedTime % 60);
        timeText.text = $"CLEAR TIME - {hours:D2}:{minutes:D2}:{seconds:D2}";

        // 2. 액티브 아이템 표시
        if (data.currentActiveItem.itemSprite != null)
        {
            activeItemImage.sprite = data.currentActiveItem.itemSprite;
            activeItemImage.color = data.currentActiveItem.itemColor;
            activeItemImage.gameObject.SetActive(true);
        }

        // 3. 패시브 아이템 리스트 생성
        foreach (var item in data.collectedPassives)
        {
            GameObject iconObj = Instantiate(itemIconPrefab, passiveContainer);
            Image img = iconObj.GetComponent<Image>();
            img.sprite = item.itemSprite;
            img.color = item.itemColor;
            img.preserveAspect = true; // 비율 유지
        }
    }
}