using UnityEngine;

public class PlayerWeapon : Weapon
{
    void Update()
    {
        Vector2 fireDir = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) fireDir = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow)) fireDir = Vector2.down;
        else if (Input.GetKey(KeyCode.LeftArrow)) fireDir = Vector2.left;
        else if (Input.GetKey(KeyCode.RightArrow)) fireDir = Vector2.right;

        if (fireDir != Vector2.zero) Fire(fireDir);
    }
}