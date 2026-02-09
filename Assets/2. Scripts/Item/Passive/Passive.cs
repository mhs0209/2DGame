using UnityEngine;

public abstract class Passive : ItemObject
{
    public override void OnPickup(GameObject player)
    {
        PlayerStat pStat = player.GetComponent<PlayerStat>();
        if (pStat != null)
        {
            ApplyEffect(pStat);
        }
    }
    public abstract void ApplyEffect(PlayerStat stat);
}