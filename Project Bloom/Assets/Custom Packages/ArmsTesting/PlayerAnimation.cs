using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerCon;
    private Animator playerAnim;
    private BuildingManager buildingManager;
    public GameObject hammer;
    public GameObject bat;
    private int currentlyEquippedWeapon;
    //private Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerCon = GetComponent<PlayerController>();
        buildingManager = GetComponent<BuildingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //CheckEquipment();
        if (Input.GetKeyDown(KeyCode.Alpha6) && buildingManager.buildingModeActive == false)
        {
            if(currentlyEquippedWeapon != 0)
            {
                playerAnim.SetTrigger("unequipWeapon");
            }
            currentlyEquippedWeapon = 0;
            playerAnim.SetInteger("weaponType", 0);

        }
        if (/*Input.GetKeyDown(KeyCode.B))*/buildingManager.buildingModeActive == true)
        {
            if (currentlyEquippedWeapon != 1)
            {
                playerAnim.SetTrigger("unequipWeapon");
            }
            playerAnim.SetInteger("weaponType", 1);
            currentlyEquippedWeapon = 1;
            //bat.SetActive(false);
            //hammer.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) && buildingManager.buildingModeActive == false)
        {
            if (currentlyEquippedWeapon != 2)
            {
                playerAnim.SetTrigger("unequipWeapon");
            }
            currentlyEquippedWeapon = 2;
            playerAnim.SetInteger("weaponType", 2);
            //hammer.SetActive(false);
            //bat.SetActive(true);

        }
        MovementAnim();
    }
    void ActivateBat()
    {
        bat.SetActive(true);
    }
    void DectivateBat()
    {
        bat.SetActive(false);
    }
    void ActivateHammer()
    {
        hammer.SetActive(true);
    }
    void DectivateHammer()
    {
        hammer.SetActive(false);
    }
    void MovementAnim()
    {
        // Movement
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            playerAnim.SetFloat("playerSpeed", 0f, 0.1f, Time.deltaTime);
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && !Input.GetKey(KeyCode.LeftShift))
        {
            playerAnim.SetFloat("playerSpeed", 0.5f, 0.1f, Time.deltaTime);
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.LeftShift))
        {
            playerAnim.SetFloat("playerSpeed", 1.0f, 0.1f, Time.deltaTime);
        }
        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            playerAnim.SetTrigger("Attack");
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerAnim.SetTrigger("Bonk");
        }
        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerAnim.SetTrigger("Jump");
        }
    }

    //void CheckEquipment()
    //{

    //    if (weapon.typeOfWeapon == Weapon.WeaponType.PISTOL)
    //    {
    //        playerAnim.SetInteger("weaponType", 0);
    //    }
    //    if (weapon.typeOfWeapon == Weapon.WeaponType.ASSAULTRIFLE)
    //    {
    //        playerAnim.SetInteger("weaponType", 1);
    //    }
    //    if (weapon.typeOfWeapon == Weapon.WeaponType.SHOTGUN)
    //    {
    //        playerAnim.SetInteger("weaponType", 2);
    //    }
    //    if (weapon.typeOfWeapon == Weapon.WeaponType.RPG)
    //    {
    //        playerAnim.SetInteger("weaponType", 3);
    //    }
    //    if (weapon.typeOfWeapon == Weapon.WeaponType.FLAMETHROWER)
    //    {
    //        playerAnim.SetInteger("weaponType", 4);
    //    }
    //}
}
