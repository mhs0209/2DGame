using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    public GameObject uiPanel; // 평소엔 꺼둘 패널
    public Slider healthSlider;
    //public TextMeshProUGUI bossNameText;

    private MonsterStat targetBoss;

    // 보스가 스폰될 때 호출
    public void ShowBossBar(MonsterStat boss)
    {
        targetBoss = boss;
        uiPanel.SetActive(true);
        //bossNameText.text = boss.gameObject.name.Replace("(Clone)", "");
        
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.health;
    }

    void Update()
    {
        if (targetBoss == null || targetBoss.health <= 0)
        {
            if (uiPanel.activeSelf) uiPanel.SetActive(false);
            return;
        }

        // 부드러운 체력 감소 연출 (Lerp)
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetBoss.health, Time.deltaTime * 5f);
    }
}