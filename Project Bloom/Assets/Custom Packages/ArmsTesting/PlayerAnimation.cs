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

    private int currentlyEquippedWeapon;

    public bool isPistolActive;
    public bool isAssaultRifleActive;
    public bool isShotgunActive;
    public bool isRPG7Active;
    public bool isFlameThrowerActive;

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

        isPistolActive = pistol.activeSelf;
        if (isPistolActive)
        {
            playerAnim.SetInteger("weaponType", 3);
            isPistolActive = false;
        }
        isAssaultRifleActive = assaultRifle.activeSelf;
        if(isAssaultRifleActive)
        {
            playerAnim.SetInteger("weaponType", 4);
            isAssaultRifleActive = false;
        }


        if (Input.GetKeyDown(KeyCode.Alpha8) && buildingManager.buildingModeActive == false)
        {
            if (currentlyEquippedWeapon != 0)
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
        if (Input.GetKeyDown(KeyCode.Alpha9) && buildingManager.buildingModeActive == false)
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
    void ActivatePistol()
    {
        pistol.SetActive(true);
    }
    void DectivatePistol()
    {
        pistol.SetActive(false);
    }

    void MovementAnim()
    {
        // Movement
        float playerSpeed = new Vector2(playerCon.velocity.x, playerCon.velocity.z).magnitude;
        playerAnim.SetFloat("playerSpeed", playerSpeed, 0.1f, Time.deltaTime);

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
    public void CheckActiveWeapon()
    {
        //pistolActive = playerCon.pistol.activeInHierarchy;
        //assaultRifleActive = playerCon.assaultRifle.activeInHierarchy;
        //shotgunActive = playerCon.shotgun.activeInHierarchy;
        //rPG7Active = playerCon.rpg.activeInHierarchy;
        //flameThrowerActive = playerCon.flamethrower.activeInHierarchy;

        //bool pistolActive = pistol.activeInHierarchy;
        //bool assaultRifleActive = assaultRifle.activeInHierarchy;
        //bool shotgunActive = shotgun.activeInHierarchy;
        //bool rPG7Active = rPG7.activeInHierarchy;
        //bool flameThrowerActive = flamethrower.activeInHierarchy;

        //if (pistolActive)
        //{
        //    Debug.Log("pistol is active");
        //    playerAnim.SetInteger("weaponType", 3);
        //}
        //if (assaultRifleActive)
        //{
        //    playerAnim.SetInteger("weaponType", 4);
        //}
        //if (shotgunActive)
        //{
        //    playerAnim.SetInteger("weaponType", 5);
        //}
        //if (rPG7Active)
        //{
        //    playerAnim.SetInteger("weaponType", 6);
        //}
        //if (flameThrowerActive)
        //{
        //    playerAnim.SetInteger("weaponType", 7);
        //}
    }
}
