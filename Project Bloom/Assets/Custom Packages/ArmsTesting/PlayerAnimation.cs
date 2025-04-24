using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerCon;
    private Animator playerAnim;
    private BuildingManager buildingManager;

    // not implemented player objects
    public GameObject hammer;
    public GameObject bat;
    public GameObject syringe;
    public GameObject pistol;
    public GameObject assaultRifle;
    public GameObject shotgun;
    public GameObject rPG7;
    public GameObject flamethrower;

    public bool isBatActive;
    public bool isPistolActive;
    public bool isAssaultRifleActive;
    public bool isShotgunActive;
    public bool isRPG7Active;
    public bool isFlameThrowerActive;

    public GameObject savedActiveWeapon;

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
        MovementAnim();

        //isBatActive = bat.activeSelf;

        isPistolActive = pistol.activeSelf;
        if (isPistolActive)
        {
            savedActiveWeapon = pistol;
            playerAnim.SetInteger("weaponType", 3);
        }

        isAssaultRifleActive = assaultRifle.activeSelf;
        if (isAssaultRifleActive)
        {
            savedActiveWeapon = assaultRifle;
            playerAnim.SetInteger("weaponType", 4);
        }

        isShotgunActive = shotgun.activeSelf;
        if (isShotgunActive)
        {
            savedActiveWeapon = shotgun;
            playerAnim.SetInteger("weaponType", 5);
        }

        isRPG7Active = rPG7.activeSelf;
        if (isRPG7Active)
        {
            savedActiveWeapon = rPG7;
            playerAnim.SetInteger("weaponType", 6);
        }

        isFlameThrowerActive = flamethrower.activeSelf;
        if (isFlameThrowerActive)
        {
            savedActiveWeapon = flamethrower;
            playerAnim.SetInteger("weaponType", 7);
        }

        //if (Input.GetKeyDown(KeyCode.Alpha8) && buildingManager.buildingModeActive == false)
        //{
        //    if (currentlyEquippedWeapon != 0)
        //    {
        //        playerAnim.SetTrigger("unequipWeapon");
        //    }
        //    currentlyEquippedWeapon = 0;
        //    playerAnim.SetInteger("weaponType", 0);

        //}
        //if (buildingManager.buildingModeActive == true)
        //{
        //    if (currentlyEquippedWeapon != 1)
        //    {
        //        playerAnim.SetTrigger("unequipWeapon");
        //    }
        //    playerAnim.SetInteger("weaponType", 1);
        //    currentlyEquippedWeapon = 1;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    playerAnim.SetTrigger("unequipWeapon");
        //    if (!isBatActive)
        //    {
        //        if (savedActiveWeapon != null)
        //        {
        //            savedActiveWeapon.SetActive(false);
        //        }
        //        bat.SetActive(true);
        //        playerAnim.SetInteger("weaponType", 2);
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    if (savedActiveWeapon != null)
        //    {
        //        playerAnim.SetTrigger("unequipWeapon");
        //        bat.SetActive(false);
        //        savedActiveWeapon.SetActive(true);
        //    }
        //    else
        //    {
        //        return;
        //    }
        //}
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
        float playerSpeed = new Vector2(playerCon.velocity.x, playerCon.velocity.z).magnitude;
        playerAnim.SetFloat("playerSpeed", playerSpeed / playerCon.runningSpeed, 0.1f, Time.deltaTime);

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            playerAnim.SetTrigger("Attack");
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerAnim.SetTrigger("Bonk");
        }
        //// Jump
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    playerAnim.SetTrigger("Jump");
        //}
    }
}
