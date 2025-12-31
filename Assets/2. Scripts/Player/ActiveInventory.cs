using UnityEngine;

public class ActiveInventory : MonoBehaviour
{
    public Active currentActive;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentActive != null)
        {
            if (currentActive.currentCharge >= currentActive.activeData.maxCharges)
            {
                currentActive.Use(gameObject);
                currentActive.currentCharge = 0;
            }
        }
    }
    
    public void EquipActive(Active newItem)
    {
        // 1. 기존 아이템이 있다면 바닥에 버리기
        if (currentActive != null)
        {
            DropCurrentItem();
        }

        // 2. 새 아이템 장착
        currentActive = newItem;

        // 3. 최초 획득 시에만 풀 충전
        if (currentActive.isFirstPickup)
        {
            currentActive.currentCharge = currentActive.activeData.maxCharges;
            currentActive.isFirstPickup = false; // 다시는 풀충전 안됨
            Debug.Log($"{currentActive.activeData.itemName} 최초 획득! 풀 충전.");
        }
    }

    private void DropCurrentItem()
    {
        currentActive.transform.SetParent(null); // 부모 해제
        // 플레이어 근처 랜덤 위치에 드랍
        currentActive.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 1.5f;

        // 시각적/물리적 재활성화
        if (currentActive.TryGetComponent(out SpriteRenderer sr)) sr.enabled = true;
        if (currentActive.TryGetComponent(out Collider2D col)) col.enabled = true;

        currentActive = null;
    }

    public void AddChargeAll(int amount) => currentActive?.AddCharge(amount);
}