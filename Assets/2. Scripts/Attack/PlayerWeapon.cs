using UnityEngine;

public class PlayerWeapon : Weapon
{
    void Update()
    {
        Vector2 fireDir = Vector2.zero;
        if (Input.GetKey(GameManager.Instance.attackUp)) fireDir = Vector2.up;
        else if (Input.GetKey(GameManager.Instance.attackDown)) fireDir = Vector2.down;
        else if (Input.GetKey(GameManager.Instance.attackLeft)) fireDir = Vector2.left;
        else if (Input.GetKey(GameManager.Instance.attackRight)) fireDir = Vector2.right;

        if (fireDir != Vector2.zero) Fire(fireDir);
    }
}