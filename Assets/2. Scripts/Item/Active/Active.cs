using UnityEngine;

public abstract class Active : ItemObject
{
    public ActiveItemData activeData; // SO 데이터
    public int currentCharge;
    public bool isFirstPickup = true; // 최초 획득 확인용

    public override void OnPickup(GameObject player)
    {
        // 1. 인벤토리에 등록 (여기서 DropCurrentItem과 위치 변환 발생)
        player.GetComponent<ActiveInventory>().EquipActive(this);
    
        // 2. 부모 설정 및 위치 초기화 (인벤토리에 장착된 상태이므로 Local 0)
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;

        // 3. 비활성화
        if (TryGetComponent(out SpriteRenderer sr)) sr.enabled = false;
        if (TryGetComponent(out Collider2D col)) col.enabled = false;
    
        Debug.Log($"{activeData.itemName} 장착 완료");
    }

    public void AddCharge(int amount)
    {
        if (activeData == null) return;
        currentCharge = Mathf.Min(currentCharge + amount, activeData.maxCharges);
    }

    public abstract void Use(GameObject player);
}