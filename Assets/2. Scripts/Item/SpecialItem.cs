using UnityEngine;
using TMPro;

public class SpecialItem : MonoBehaviour
{
    public int requiredKeys = 1;
    private ItemObject item;
    private TextMeshPro priceUI;

    public void Initialize(int k)
    {
        requiredKeys = k;
        item = GetComponent<ItemObject>();
        
        // 가격 표시 UI 생성 (상점 로직과 동일)
        GameObject uiObj = new GameObject("KeyCostUI");
        uiObj.transform.SetParent(this.transform);
        uiObj.transform.localPosition = Vector3.up * -1.2f;
        
        priceUI = uiObj.AddComponent<TextMeshPro>();
        priceUI.alignment = TextAlignmentOptions.Center;
        priceUI.fontSize = 4;
        priceUI.color = Color.yellow; // 열쇠 느낌을 주기 위한 노란색
        priceUI.text = $"{requiredKeys} Key";
        priceUI.sortingOrder = 5;

        // 아이템의 물리 트리거를 꺼서 몸으로 밀어야 구매되도록 설정 (상점과 동일)
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStat pStat = collision.gameObject.GetComponent<PlayerStat>();
            
            // 골드 대신 열쇠 확인
            if (pStat.keys >= requiredKeys)
            {
                pStat.keys -= requiredKeys;
                pStat.OnStatChanged?.Invoke();
                
                item.OnPickup(collision.gameObject); // 아이템 효과 발동
                Destroy(gameObject); // 아이템 제거
                Debug.Log($"열쇠 {requiredKeys}개를 사용하여 특수 아이템을 획득했습니다!");
            }
            else
            {
                Debug.Log("열쇠가 부족합니다!");
            }
        }
    }
}