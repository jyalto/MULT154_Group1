using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class PlayerController : MonoBehaviour
{
    public float speed = 15.0f;
    public float mouseSensitivity = 2.0f;
    public float verticalRotationLimit = 90.0f;
    public float gravity = -40.0f;
    public float jumpForce = 20.0f;
    public int health = 25;
    public GameObject pistol;
    public GameObject assaultRifle;
    public GameObject shotgun;
    public GameObject keyGreen;
    public GameObject keyRed;
    public GameObject gasCan;
    public TreasureChest chestGreen;
    public TreasureChest chestRed;

    private AudioSource[] audioSources;
    private Weapon weapon;
    private GameManager gameManager;
    private CharacterController controller;
    private GameObject currentWeaponPickup;
    private float xRotation = 0;
    private float yRotation = 0;
    private bool weaponSwitchEnable = true;
    private bool pistolInteractable = false;
    private bool assaultRifleInteractable = false;
    private bool shotgunInteractable = false;
    private bool canOpenChestGreen = false;
    private bool canOpenChestRed = false;
    private bool canCollectGasCan = false;
    private Coroutine switchWeaponCoroutine = null;
    private Vector3 velocity;

    private List<GameObject> weapons = new List<GameObject>();
    private List<GameObject> inventory = new List<GameObject>();

    private int currentWeaponIndex = 0;

    public enum AmmoType
    {
        PISTOL,
        ASSAULTRIFLE,
        SHOTGUN
    }

    public int[] ammo = new int[3];

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManagerObject = GameObject.Find("Game Manager");

        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }

        controller = GetComponent<CharacterController>();
        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveDirectionX = Input.GetAxis("Horizontal");
        float moveDirectionZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveDirectionX, 0, moveDirectionZ);
        move = transform.TransformDirection(move);

        velocity.x = move.x * speed;
        velocity.z = move.z * speed;

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                speed = 15;
            }

            else
            {
                if (move.magnitude > 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        speed = 30;
                    }
                    else
                    {
                        speed = 15;
                    }
                }
                else
                {
                    speed = 15;
                }
            }

        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
            speed = 15;
        }

        controller.Move(velocity * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalRotationLimit, verticalRotationLimit);
        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) == 0.1f && weaponSwitchEnable)
        {
            SwitchWeapon();
            weaponSwitchEnable = false;
            if (switchWeaponCoroutine == null)
            {
                switchWeaponCoroutine = StartCoroutine(SwitchWeaponTime(0.15f));
            }
        }

        /*print("Pistol Ammo: " + ammo[(int)AmmoType.PISTOL]);
        print("AR Ammo: " + ammo[(int)AmmoType.ASSAULTRIFLE]);
        print("Shotgun Ammo: " + ammo[(int)AmmoType.SHOTGUN]);*/


        if (canOpenChestGreen && Input.GetButtonDown("Interact"))
        {
            if (inventory.Contains(keyGreen))
            {
                chestGreen.OpenChest();
                inventory.Remove(keyGreen);
            }
            else if (!chestGreen.opened)
            {
                chestGreen.LockedChest();
            }
        }

        if (canOpenChestRed && Input.GetButtonDown("Interact"))
        {
            if (inventory.Contains(keyRed))
            {
                chestRed.OpenChest();
                inventory.Remove(keyRed);
            }
            else if (!chestRed.opened)
            {
                chestRed.LockedChest();
            }
        }
        if (pistolInteractable)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!weapons.Contains(pistol))
                {
                    weapons.Add(pistol);
                    audioSources[0].Play();
                    EquipWeapon(pistol);
                    Destroy(currentWeaponPickup);
                    pistolInteractable = false;
                }
            }
        }
        if (assaultRifleInteractable)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!weapons.Contains(assaultRifle))
                {
                    weapons.Add(assaultRifle);
                    audioSources[0].Play();
                    EquipWeapon(assaultRifle);
                    Destroy(currentWeaponPickup);
                    assaultRifleInteractable = false;
                }
            }
        }
        if (shotgunInteractable)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!weapons.Contains(shotgun))
                {
                    ammo[(int)AmmoType.SHOTGUN] += 4;
                    weapons.Add(shotgun);
                    audioSources[0].Play();
                    EquipWeapon(shotgun);
                    Destroy(currentWeaponPickup);
                    shotgunInteractable = false;
                }
            }
        }

        if (canCollectGasCan && Input.GetButtonDown("Interact"))
        {
            if (!inventory.Contains(gasCan))
            {
                inventory.Add(gasCan);
                audioSources[3].Play();
                Destroy(gasCan);
            }
        }
    }

    void SwitchWeapon()
    {
        if (weapons.Count > 1)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
            EquipWeapon(weapons[currentWeaponIndex]);
        }
    }

    private IEnumerator SwitchWeaponTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        weaponSwitchEnable = true;
        switchWeaponCoroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pistol PickUp"))
        {
            if (weapons.Count < 2)
            {
                if (weapon == null || !weapons.Contains(pistol))
                {
                    weapons.Add(pistol);
                    audioSources[0].Play();
                    EquipWeapon(pistol);
                }
                else
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.PISTOL] += 8;
                }
                Destroy(other.gameObject);
            }
            else
            {
                if (weapons.Contains(pistol))
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.PISTOL] += 8;
                    Destroy(other.gameObject);
                }
                else
                {
                    currentWeaponPickup = other.gameObject;
                    pistolInteractable = true;
                }
            }
        }
        if (other.CompareTag("Assault Rifle PickUp"))
        {
            if (weapons.Count < 2)
            {
                if (weapon == null || !weapons.Contains(assaultRifle))
                {
                    weapons.Add(assaultRifle);
                    audioSources[0].Play();
                    EquipWeapon(assaultRifle);
                }
                else
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.ASSAULTRIFLE] += 25;
                }
                Destroy(other.gameObject);
            }
            else
            {
                if (weapons.Contains(assaultRifle))
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.ASSAULTRIFLE] += 25;
                    Destroy(other.gameObject);
                }
                else
                {
                    currentWeaponPickup = other.gameObject;
                    assaultRifleInteractable = true;
                }
            }
        }
        if (other.CompareTag("Shotgun PickUp"))
        {
            if (weapons.Count < 2)
            {
                if (weapon == null || !weapons.Contains(shotgun))
                {
                    weapons.Add(shotgun);
                    audioSources[0].Play();
                    EquipWeapon(shotgun);
                }
                else
                {
                    audioSources[1].Play();
                    //ammo[(int)AmmoType.SHOTGUN] += 4;
                }
                ammo[(int)AmmoType.SHOTGUN] += 4;
                Destroy(other.gameObject);
            }
            else
            {
                if (weapons.Contains(shotgun))
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.SHOTGUN] += 4;
                    Destroy(other.gameObject);
                }
                else
                {
                    currentWeaponPickup = other.gameObject;
                    shotgunInteractable = true;
                }
            }
        }
        if (other.CompareTag("Ammo PickUp") && weapon != null)
        {
            audioSources[1].Play();
            if (weapon.typeOfWeapon == Weapon.WeaponType.PISTOL)
            {
                ammo[(int)AmmoType.PISTOL] += 15;
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.ASSAULTRIFLE)
            {
                ammo[(int)AmmoType.ASSAULTRIFLE] += 50;
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.SHOTGUN)
            {
                ammo[(int)AmmoType.SHOTGUN] += 8;
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Key Green"))
        {
            inventory.Add(keyGreen);
            audioSources[2].Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Chest Green"))
        {
            canOpenChestGreen = true;
        }

        if (other.CompareTag("Key Red"))
        {
            inventory.Add(keyRed);
            audioSources[2].Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Chest Red"))
        {
            canOpenChestRed = true;
        }

        if (other.CompareTag("Gas Can PickUp"))
        {
            canCollectGasCan = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pistol PickUp"))
        {
            pistolInteractable = false;
            currentWeaponPickup = null;
        }
        else if (other.CompareTag("Assault Rifle PickUp"))
        {
            assaultRifleInteractable = false;
            currentWeaponPickup = null;
        }
        else if (other.CompareTag("Shotgun PickUp"))
        {
            shotgunInteractable = false;
            currentWeaponPickup = null;
        }
        if (other.CompareTag("Chest Green"))
        {
            canOpenChestGreen = false;
        }
        if (other.CompareTag("Chest Red"))
        {
            canOpenChestRed = false;
        }
        if (other.CompareTag("Gas Can PickUp"))
        {
            canCollectGasCan = false;
        }
    }

    void EquipWeapon(GameObject newWeapon)
    {
        if (weapon != null)
        {
            weapon.gameObject.SetActive(false);
        }

        if (weapons.Count > 2)
        {
            if (weapon.typeOfWeapon == Weapon.WeaponType.PISTOL)
            {
                weapons.Remove(pistol);
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.ASSAULTRIFLE)
            {
                weapons.Remove(assaultRifle);
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.SHOTGUN)
            {
                weapons.Remove(shotgun);
            }
        }

        newWeapon.SetActive(true);

        weapon = newWeapon.GetComponent<Weapon>();
    }
}
