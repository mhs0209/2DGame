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
        //if (currentActive != null) currentActive.gameObject.SetActive(true); // 기존꺼 버림(예시)
        currentActive = newItem;
    }

    public void AddChargeAll(int amount) => currentActive?.AddCharge(amount);
}