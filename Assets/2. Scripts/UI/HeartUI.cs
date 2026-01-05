using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartUI : MonoBehaviour, IInitializable
{
    public PlayerStat playerStat;
    public GameObject heartPrefab;

    private List<Image> heartImages = new List<Image>();

    private Color fullColor = Color.green;   // 2 HP
    private Color halfColor = Color.red;     // 1 HP
    private Color emptyColor = Color.black;  // 0 HP

    // 1. 최대 체력에 맞춰 하트 오브젝트 생성/삭제
    private void RefreshHeartCount()
    {
        int targetHeartCount = Mathf.CeilToInt(playerStat.maxHealth / 2f);

        // 현재 하트가 부족하면 추가 생성
        while (heartImages.Count < targetHeartCount)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);
            heartImages.Add(newHeart.GetComponent<Image>());
        }

        // 현재 하트가 너무 많으면 삭제
        while (heartImages.Count > targetHeartCount)
        {
            int lastIndex = heartImages.Count - 1;
            Destroy(heartImages[lastIndex].gameObject);
            heartImages.RemoveAt(lastIndex);
        }
    }

    // 2. 피격/회복 시에만 호출되어 색상 변경
    private void UpdateHeartColors()
    {
        float currentHP = playerStat.health;

        for (int i = 0; i < heartImages.Count; i++)
        {
            float heartThreshold = currentHP - (i * 2);

            if (heartThreshold >= 2)
                heartImages[i].color = fullColor;
            else if (heartThreshold == 1)
                heartImages[i].color = halfColor;
            else
                heartImages[i].color = emptyColor;
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 이벤트 구독 해제
        if (playerStat != null)
        {
            playerStat.OnMaxHealthChanged -= RefreshHeartCount;
            playerStat.OnHealthChanged -= UpdateHeartColors;
        }
    }

    public void OnLevelInit()
    {
        playerStat = FindObjectOfType<PlayerStat>();
        if (playerStat != null)
        {
            // 이벤트 구독
            playerStat.OnMaxHealthChanged += RefreshHeartCount;
            playerStat.OnHealthChanged += UpdateHeartColors;

            // 초기 설정
            RefreshHeartCount();
            UpdateHeartColors();
        }
    }
}