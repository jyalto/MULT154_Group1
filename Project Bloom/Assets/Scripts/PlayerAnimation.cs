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
    public GameObject pistol;
    public GameObject assaultRifle;
    public GameObject shotgun;
    public GameObject rPG7;
    public GameObject flamethrower;
    public OffhandUtilities offhandUtilities;

    private bool isBatActive;
    private bool isPistolActive;
    private bool isAssaultRifleActive;
    private bool isShotgunActive;
    private bool isRPG7Active;
    private bool isFlameThrowerActive;

    [SerializeField]
    private AudioSource batSwing;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerCon = GetComponent<PlayerController>();
        offhandUtilities = GetComponent<OffhandUtilities>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementAnim();
        if(offhandUtilities.equipmentModeEnabled == false)
        {
            CheckEquipment();
        }
        else
        {
            isBatActive = false;
            isPistolActive = false;
            isAssaultRifleActive = false;
            isShotgunActive = false;
            isRPG7Active = false;
            isFlameThrowerActive = false    ;
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
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            if (isBatActive && !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                batSwing.Play();
                playerAnim.SetTrigger("Attack");
            }
        }
    }

    private void CheckEquipment()
    {

        isBatActive = bat.activeSelf;
        if (isBatActive)
        {
            playerAnim.SetInteger("weaponType", 2);
        }

        isPistolActive = pistol.activeSelf;
        if (isPistolActive)
        {
            playerAnim.SetInteger("weaponType", 3);
        }

        isAssaultRifleActive = assaultRifle.activeSelf;
        if (isAssaultRifleActive)
        {
            playerAnim.SetInteger("weaponType", 4);
        }

        isShotgunActive = shotgun.activeSelf;
        if (isShotgunActive)
        {
            playerAnim.SetInteger("weaponType", 5);
        }

        isRPG7Active = rPG7.activeSelf;
        if (isRPG7Active)
        {
            playerAnim.SetInteger("weaponType", 6);
        }

        isFlameThrowerActive = flamethrower.activeSelf;
        if (isFlameThrowerActive)
        {
            playerAnim.SetInteger("weaponType", 7);
        }
    }
}
