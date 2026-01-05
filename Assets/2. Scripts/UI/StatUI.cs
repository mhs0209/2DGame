using FreeworkGame;
using UnityEngine;
using TMPro;

public class StatUI : MonoBehaviour, IInitializable
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
    
    public void OnLevelInit()
    {
        // 씬 로드 시점에 새롭게 생성된(혹은 초기화된) 플레이어를 찾아 이벤트 재구독
        if (AniManager.Instance != null)
        {
            playerStat = AniManager.Instance.GetComponent<PlayerStat>();
            
            // 기존 구독 해제 후 재구독 (중복 방지)
            playerStat.OnStatChanged -= RefreshUI;
            playerStat.OnHealthChanged -= RefreshUI;
            
            playerStat.OnStatChanged += RefreshUI;
            playerStat.OnHealthChanged += RefreshUI;
            
            RefreshUI();
        }
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