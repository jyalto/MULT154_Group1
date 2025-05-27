using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Weapon;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float walkingSpeed = 10f;
    public float runningSpeed = 20f;
    public float mouseSensitivity = 2.0f;
    //public float verticalRotationLimit = 90.0f;
    public float lookUp = -90.0f;
    public float lookDown = 45.0f;
    public float gravity = -9.81f;
    public float jumpForce = 5.0f;
    public int maxHealth = 25;          // Maximum possible health
    public int healthLimit = 25;        // Temporary limit on health
    public int health = 25;             // Current health
    public bool flameActive = false;
    public bool lureActive = false;
    public bool batActive = false;
    public GameObject pistol;
    public GameObject assaultRifle;
    public GameObject shotgun;
    public GameObject keyGreen;
    public GameObject keyGold;
    public GameObject keyRed;
    public GameObject rpg;
    public GameObject rocketShell;
    public GameObject flamethrower;
    public GameObject bat;
    public GameObject lureDevice;
    public GameObject hands;
    public ParticleSystem flamethrowerParticles;
    public TreasureChest chestGreen;
    public TreasureChest ChestGold;
    public TreasureChest ChestRed;
    public Animator playerAnim;

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
    private bool rpgInteractable = false;
    private bool flamethrowerInteractable = false;
    private bool canOpenChestGreen = false;
    private bool canOpenChestGold = false;
    private bool canOpenChestRed = false;
    private Coroutine switchWeaponCoroutine = null;
    private Coroutine reloadRocketRoutine = null;
    public Vector3 velocity;

    private List<GameObject> weapons = new List<GameObject>();
    private List<GameObject> keyItems = new List<GameObject>();

    private Dictionary<Resource.ResourceTypes, int> resources = new Dictionary<Resource.ResourceTypes, int>();
    [SerializeField] AudioClip[] playerSounds;
    [SerializeField] AudioClip[] pickupSounds;

    public bool usingSyringe = false;
    private int currentWeaponIndex = 0;

    public enum AmmoType
    {
        PISTOL,
        ASSAULTRIFLE,
        SHOTGUN,
        RPG,
        FLAMETHROWER
    }

    public int[] ammo = new int[4];

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameObject gameManagerObject = GameObject.Find("Game Manager");

        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }

        controller = GetComponent<CharacterController>();
        audioSources = GetComponents<AudioSource>();
        playerAnim = GetComponent<Animator>();

        AddResource(Resource.ResourceTypes.PLANT_PASTE, 0);
        AddResource(Resource.ResourceTypes.TOUGH_CLOTH, 0);
        AddResource(Resource.ResourceTypes.SCRAP_METAL, 0);
        AddResource(Resource.ResourceTypes.TECH_PARTS, 0);
        AddResource(Resource.ResourceTypes.GAS_CAN, 0);
        AddResource(Resource.ResourceTypes.WEED_SPRAY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4) && gameManager.wave != 4)
            {
                gameManager.wave = 4;
            }

            float moveDirectionX = Input.GetAxis("Horizontal");
            float moveDirectionZ = Input.GetAxis("Vertical");

            Vector3 move = new Vector3(moveDirectionX, 0, moveDirectionZ).normalized;
            move = transform.TransformDirection(move);

            velocity.x = move.x * speed;
            velocity.z = move.z * speed;

            if (controller.isGrounded)
            {
                /*if (Input.GetButtonDown("Jump"))
                {
                    velocity.y = jumpForce;
                    speed = walkingSpeed;
                }

                else
                {
                    if (move.magnitude > 0)
                    {
                        if (Input.GetKey(KeyCode.LeftShift) && !flamethrowerParticles.isPlaying)
                        {
                            speed = runningSpeed;
                        }
                        else
                        {
                            speed = walkingSpeed;
                        }
                    }
                    else
                    {
                        speed = walkingSpeed;
                    }
                }*/

                if (move.magnitude > 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift) && !flamethrowerParticles.isPlaying)
                    {
                        speed = runningSpeed;
                    }
                    else
                    {
                        speed = walkingSpeed;
                    }
                }
                else
                {
                    speed = walkingSpeed;
                }
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
                speed = walkingSpeed;
            }

            controller.Move(velocity * Time.deltaTime);

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yRotation += mouseX;
            transform.localEulerAngles = new Vector3(0, yRotation, 0);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, lookUp, lookDown);
            Camera.main.transform.localEulerAngles = new Vector3(xRotation, 0, 0);

            //transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) == 0.1f && weaponSwitchEnable && !Input.GetMouseButton(0) && batActive == false)
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
            print("Shotgun Ammo: " + ammo[(int)AmmoType.SHOTGUN]);
            print("RPG Ammo: " + ammo[(int)AmmoType.RPG]);*/

            /*foreach (var resource in resources)
            {
                Debug.Log($"Resource: {resource.Key}, Count: {resource.Value}");
            }*/


            if (canOpenChestGreen && Input.GetButtonDown("Interact"))
            {
                if (keyItems.Contains(keyGreen))
                {
                    chestGreen.OpenChest();
                    keyItems.Remove(keyGreen);
                }
                else if (!chestGreen.opened)
                {
                    chestGreen.LockedChest();
                }
            }

            if (canOpenChestGold && Input.GetButtonDown("Interact"))
            {
                if (keyItems.Contains(keyGold))
                {
                    ChestGold.OpenChest();
                    keyItems.Remove(keyGold);
                }
                else if (!ChestGold.opened)
                {
                    ChestGold.LockedChest();
                }
            }

            if (canOpenChestRed && Input.GetButtonDown("Interact"))
            {
                if (keyItems.Contains(keyRed))
                {
                    ChestRed.OpenChest();
                    keyItems.Remove(keyRed);
                }
                else if (!ChestRed.opened)
                {
                    ChestRed.LockedChest();
                }
            }

            if (pistolInteractable)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (!weapons.Contains(pistol))
                    {
                        weapons.Add(pistol);
                        ammo[(int)AmmoType.PISTOL] += 8;
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
                        ammo[(int)AmmoType.ASSAULTRIFLE] += 25;
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
                        if (gameManager.shotgunDrop == false)
                        {
                            ammo[(int)AmmoType.SHOTGUN] += 6;
                            gameManager.shotgunDrop = true;
                        }
                        else
                        {
                            ammo[(int)AmmoType.SHOTGUN] += 4;
                        }
                        weapons.Add(shotgun);
                        audioSources[0].Play();
                        EquipWeapon(shotgun);
                        Destroy(currentWeaponPickup);
                        shotgunInteractable = false;
                    }
                }
            }
            if (rpgInteractable)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (!weapons.Contains(rpg))
                    {
                        if (gameManager.rpgDrop == false)
                        {
                            ammo[(int)AmmoType.RPG] += 2;
                            gameManager.rpgDrop = true;
                        }
                        else
                        {
                            ammo[(int)AmmoType.RPG] += 1;
                            if (!rocketShell.activeSelf && reloadRocketRoutine == null)
                            {
                                reloadRocketRoutine = StartCoroutine(ReloadRocket());
                            }
                        }
                        weapons.Add(rpg);
                        audioSources[0].Play();
                        EquipWeapon(rpg);
                        Destroy(currentWeaponPickup);
                        rpgInteractable = false;
                    }
                }
            }
            if (flamethrowerInteractable)
            {
                if (Input.GetButtonDown("Interact"))
                {
                    if (!weapons.Contains(flamethrower))
                    {
                        gameManager.flamethrowerDrop = true;
                        weapons.Add(flamethrower);
                        ammo[(int)AmmoType.FLAMETHROWER] += 50;
                        audioSources[0].Play();
                        EquipWeapon(flamethrower);
                        Destroy(currentWeaponPickup);
                        flamethrowerInteractable = false;
                    }
                }
            }

            if (usingSyringe && Input.GetMouseButtonDown(0))
            {
                HealWithSyringe();
            }

            if (!batActive && Input.GetKeyDown(KeyCode.Z) && !flameActive)
            {
                if (weapon != null)
                {
                    weapon.gameObject.SetActive(false);
                }
                playerAnim.SetTrigger("unequipWeapon");
                playerAnim.ResetTrigger("Attack");
                //playerAnim.SetInteger("weaponType", 2);
                bat.SetActive(true);
            }

            else if (batActive && Input.GetKeyDown(KeyCode.Z) && !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && weapon != null)
            {
                weapon.gameObject.SetActive(true);
                playerAnim.SetTrigger("unequipWeapon");
                playerAnim.ResetTrigger("Attack");
                //playerAnim.SetInteger("weaponType", 0);
                bat.SetActive(false);
            }

            if (playerAnim.GetInteger("weaponType") == 2)
            {
                batActive = true;
            }

            else
            {
                batActive = false;
            }

            //print(playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
        }

        else
        { 
            StartCoroutine(gameManager.ReloadScene());
            hands.SetActive(false);
        }
    }
    public void HealWithSyringe()
    {
        healthLimit -= 1;
        health += 2;
        if (health > healthLimit)
        {
            health = healthLimit;
        }

        playerAnim.SetTrigger("Activate");
    }
    public void playSyringeSound()
    {
        audioSources[3].PlayOneShot(playerSounds[0]);
    }


    void SwitchWeapon()
    {

        if (weapons.Count > 1)
        {
            if (weapon.typeOfWeapon == Weapon.WeaponType.FLAMETHROWER)
            {
                flameActive = false;
            }
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
                }
                ammo[(int)AmmoType.PISTOL] += 8;
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
                }
                ammo[(int)AmmoType.ASSAULTRIFLE] += 25;
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
                }

                if (gameManager.shotgunDrop == false)
                {
                    ammo[(int)AmmoType.SHOTGUN] += 6;
                    gameManager.shotgunDrop = true;
                }
                else
                {
                    ammo[(int)AmmoType.SHOTGUN] += 4;
                }
                Destroy(other.gameObject);
            }
            else
            {
                if (weapons.Contains(shotgun))
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.SHOTGUN] += 4;
                    gameManager.shotgunDrop = true;
                    Destroy(other.gameObject);
                }
                else
                {
                    currentWeaponPickup = other.gameObject;
                    shotgunInteractable = true;
                }
            }
        }
        if (other.CompareTag("RPG PickUp"))
        {
            if (weapons.Count < 2)
            {
                if (weapon == null || !weapons.Contains(rpg))
                {
                    weapons.Add(rpg);
                    audioSources[0].Play();
                    EquipWeapon(rpg);
                }
                else
                {
                    audioSources[1].Play();
                }

                if (gameManager.rpgDrop == false)
                {
                    ammo[(int)AmmoType.RPG] += 2;
                    gameManager.rpgDrop = true;
                }
                else
                {
                    ammo[(int)AmmoType.RPG] += 1;
                    if (!rocketShell.activeSelf && reloadRocketRoutine == null)
                    {
                        reloadRocketRoutine = StartCoroutine(ReloadRocket());
                    }
                }
                Destroy(other.gameObject);
            }
            else
            {
                if (weapons.Contains(rpg))
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.RPG] += 1;
                    if (!rocketShell.activeSelf && reloadRocketRoutine == null)
                    {
                        reloadRocketRoutine = StartCoroutine(ReloadRocket());
                    }
                    Destroy(other.gameObject);
                }
                else
                {
                    currentWeaponPickup = other.gameObject;
                    rpgInteractable = true;
                }
            }
        }
        if (other.CompareTag("Flamethrower PickUp"))
        {
            if (weapons.Count < 2)
            {
                if (weapon == null || !weapons.Contains(flamethrower))
                {
                    weapons.Add(flamethrower);
                    audioSources[0].Play();
                    EquipWeapon(flamethrower);
                }
                else
                {
                    audioSources[1].Play();
                }

                if (gameManager.flamethrowerDrop == false)
                {
                    ammo[(int)AmmoType.FLAMETHROWER] += 50;
                    gameManager.flamethrowerDrop = true;
                }
                else
                {
                    ammo[(int)AmmoType.FLAMETHROWER] += 35;
                }
                Destroy(other.gameObject);
            }
            else
            {
                if (weapons.Contains(flamethrower))
                {
                    audioSources[1].Play();
                    ammo[(int)AmmoType.FLAMETHROWER] += 35;
                    gameManager.flamethrowerDrop = true;
                    Destroy(other.gameObject);
                }
                else
                {
                    currentWeaponPickup = other.gameObject;
                    flamethrowerInteractable = true;
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
                ammo[(int)AmmoType.SHOTGUN] += 4;
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.RPG)
            {
                ammo[(int)AmmoType.RPG] += 1;
                if (!rocketShell.activeSelf)
                {
                    StartCoroutine(ReloadRocket());
                }
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.FLAMETHROWER)
            {
                ammo[(int)AmmoType.FLAMETHROWER] += 50;
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Key Green"))
        {
            keyItems.Add(keyGreen);
            audioSources[2].Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Chest Green"))
        {
            canOpenChestGreen = true;
        }

        if (other.CompareTag("Key Gold"))
        {
            keyItems.Add(keyGold);
            audioSources[2].Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Chest Gold"))
        {
            canOpenChestGold = true;
        }

        if (other.CompareTag("Key Red"))
        {
            keyItems.Add(keyRed);
            audioSources[2].Play();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Chest Red"))
        {
            canOpenChestRed = true;
        }

        if (other.CompareTag("ResourcePickup"))
        {
            Resource tempResource = other.gameObject.GetComponent<Resource>();
            AddResource(tempResource.ResourceType, 1);

            switch (tempResource.ResourceType)
            {
                case Resource.ResourceTypes.PLANT_PASTE: // Index 1
                    audioSources[3].PlayOneShot(pickupSounds[1]);
                    break;
                case Resource.ResourceTypes.TOUGH_CLOTH: // Index 2
                    audioSources[3].PlayOneShot(pickupSounds[2]);
                    break;
                case Resource.ResourceTypes.SCRAP_METAL: // Index 3
                    audioSources[3].PlayOneShot(pickupSounds[3]);
                    break;
                case Resource.ResourceTypes.TECH_PARTS: // Index 4
                    audioSources[3].PlayOneShot(pickupSounds[4]);
                    break;
                case Resource.ResourceTypes.GAS_CAN: // Index 5
                    audioSources[3].PlayOneShot(pickupSounds[5]);
                    break;
                case Resource.ResourceTypes.WEED_SPRAY: // Index 5... too
                    audioSources[3].PlayOneShot(pickupSounds[5]);
                    break;
            }

            Destroy(other.gameObject);
        }
        if (other.CompareTag("Explosion"))
        {
            health = 0;
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
        else if (other.CompareTag("RPG PickUp"))
        {
            rpgInteractable = false;
            currentWeaponPickup = null;
        }
        else if (other.CompareTag("Flamethrower PickUp"))
        {
            flamethrowerInteractable = false;
            currentWeaponPickup = null;
        }
        if (other.CompareTag("Chest Green"))
        {
            canOpenChestGreen = false;
        }
        if (other.CompareTag("Chest Gold"))
        {
            canOpenChestGold = false;
        }
        if (other.CompareTag("Chest Red"))
        {
            canOpenChestRed = false;
        }
    }

    public IEnumerator ReloadRocket()
    {
        playerAnim.SetTrigger("reload");
        yield return new WaitForSeconds(1.5f);
        rocketShell.SetActive(true);
        reloadRocketRoutine = null;
    }

    void EquipWeapon(GameObject newWeapon)
    {

        if (weapon != null)
        {
            weapon.gameObject.SetActive(false);
            bat.SetActive(false);
        }
        else
        {
            gameManager.bulletImage.gameObject.SetActive(true);
            bat.SetActive(false);
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
            else if (weapon.typeOfWeapon == Weapon.WeaponType.RPG)
            {
                weapons.Remove(rpg);
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.FLAMETHROWER)
            {
                weapons.Remove(flamethrower);
            }
        }
        playerAnim.SetTrigger("unequipWeapon");
        playerAnim.ResetTrigger("Attack");
        newWeapon.SetActive(true);
        weapon = newWeapon.GetComponent<Weapon>();
    }

    public void AddResource(Resource.ResourceTypes resourceType, int resourceCount)
    {
        if (resources.ContainsKey(resourceType))
        {
            resources[resourceType] += resourceCount;
        }
        else
        {
            resources[resourceType] = 1;
        }

        Debug.Log($"{resourceType} collected. Total: {resources[resourceType]}");
    }

    public int CheckResource(Resource.ResourceTypes resourceType)
    {
        if (resources.ContainsKey(resourceType))
        {
            return resources[resourceType];
        }
        return -1;
    }

    public void RemoveResource(Resource.ResourceTypes resourceType, int resourceCount)
    {
        if (resources.ContainsKey(resourceType))
        {
            resources[resourceType] -= resourceCount;
            if (resources[resourceType] <= 0)
            {
                resources.Remove(resourceType);
            }
        }
    }
}
