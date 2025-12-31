using UnityEngine;

public class PlayerWeapon : Weapon
{
    void Update()
    {
        Vector2 fireDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) fireDir = Vector2.up;
        else if (Input.GetKey(KeyCode.S)) fireDir = Vector2.down;
        else if (Input.GetKey(KeyCode.A)) fireDir = Vector2.left;
        else if (Input.GetKey(KeyCode.D)) fireDir = Vector2.right;

        if (fireDir != Vector2.zero) Fire(fireDir);
    }
}