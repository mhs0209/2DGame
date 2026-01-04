using FreeworkGame;
using UnityEngine;
using TMPro;

public class StatUI : MonoBehaviour
{
    public TextMeshProUGUI atkText, speedText, rangeText, delayText;
    public TextMeshProUGUI goldText, keyText, bombText;
    
    private PlayerStat playerStat;

    void Start()
    {
        if (AniManager.Instance != null)
        {
            playerStat = AniManager.Instance.GetComponent<PlayerStat>();
            
            // 이벤트 구독
            playerStat.OnStatChanged += RefreshUI;
            
            // 초기 UI 셋팅
            RefreshUI();
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 구독 해제
        if (playerStat != null)
            playerStat.OnStatChanged -= RefreshUI;
    }

    // 이벤트가 발생했을 때만 실행됨
    private void RefreshUI()
    {
        if (playerStat == null) return;

        atkText.text = $"ATK: {playerStat.atk * playerStat.atkMult:F2}";
        delayText.text = $"DELAY: {playerStat.delay:F2}";
        speedText.text = $"SPD: {playerStat.speed:F2}";
        rangeText.text = $"RNG: {playerStat.range:F2}";
        
        goldText.text = $"Gold: {playerStat.gold}";
        keyText.text = $"Key: {playerStat.keys}";
        bombText.text = $"Bomb: {playerStat.bombs}";
    }
}