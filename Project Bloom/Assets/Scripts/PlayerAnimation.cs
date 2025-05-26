using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController playerCon;
    private Animator playerAnim;

    // not implemented player objects
    //public GameObject hammer;
    public GameObject bat;
    public GameObject syringe;
    public GameObject pistol;
    public GameObject assaultRifle;
    public GameObject shotgun;
    public GameObject rPG7;
    public GameObject flamethrower;

    private bool isBatActive;
    private bool isPistolActive;
    private bool isAssaultRifleActive;
    private bool isShotgunActive;
    private bool isRPG7Active;
    private bool isFlameThrowerActive;

    public GameObject savedActiveWeapon;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerCon = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementAnim();

        isBatActive = bat.activeSelf;
        if (isBatActive)
        {
            savedActiveWeapon = bat;
            playerAnim.SetInteger("weaponType", 2);
        }

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
    }

    void MovementAnim()
    {
        // Movement
        float playerSpeed = new Vector2(playerCon.velocity.x, playerCon.velocity.z).magnitude;
        playerAnim.SetFloat("playerSpeed", playerSpeed / playerCon.runningSpeed, 0.1f, Time.deltaTime);

        // Attack
        //if (Input.GetMouseButtonDown(0) && !isBatActive)
        //{
        //    playerAnim.SetTrigger("Attack");
        //}
        if (Input.GetMouseButtonDown(1) && isBatActive && !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            playerAnim.SetTrigger("Attack");
        }
    }
}
