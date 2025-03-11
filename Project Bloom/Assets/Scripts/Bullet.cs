using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Weapon weapon;

    public float damage = 0;

    void Start()
    {
        weapon = FindObjectOfType<Weapon>();

        if (weapon != null)
        {
            if (weapon.typeOfWeapon == Weapon.WeaponType.PISTOL)
            {
                damage = 3.5f;
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.ASSAULTRIFLE)
            {
                damage = 1;
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.SHOTGUN)
            {
                damage = 10;
            }
        }
    }
}
