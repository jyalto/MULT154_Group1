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
    
    private AudioSource[] audioSources;
    private Weapon weapon;
    private GameManager gameManager;
    private CharacterController controller;
    private float xRotation = 0;
    private float yRotation = 0;
    private Vector3 velocity;

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

        EquipWeapon(assaultRifle);
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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pistol PickUp"))
        {
            if (weapon.typeOfWeapon != Weapon.WeaponType.PISTOL)
            {
                audioSources[0].Play();
                EquipWeapon(pistol);
            }
            else
            {
                audioSources[1].Play();
                weapon.ammo[(int)AmmoType.PISTOL] += 8;
            }
        }
        if (other.CompareTag("Assault Rifle PickUp"))
        {
            if (weapon.typeOfWeapon != Weapon.WeaponType.ASSAULTRIFLE)
            {
                audioSources[0].Play();
                EquipWeapon(assaultRifle);
            }
            else
            {
                audioSources[1].Play();
                weapon.ammo[(int)AmmoType.ASSAULTRIFLE] += 25;
            }
        }
        if (other.CompareTag("Ammo PickUp"))
        {
            audioSources[1].Play();
            if (weapon.typeOfWeapon == Weapon.WeaponType.PISTOL)
            {
                weapon.ammo[(int)AmmoType.PISTOL] += 15;
            }
            else if (weapon.typeOfWeapon == Weapon.WeaponType.ASSAULTRIFLE)
            {
                weapon.ammo[(int)AmmoType.ASSAULTRIFLE] += 50;
            }
        }
        Destroy(other.gameObject);
    }

    void EquipWeapon(GameObject newWeapon)
    {
        pistol.SetActive(false);
        assaultRifle.SetActive(false);

        newWeapon.SetActive(true);

        weapon = newWeapon.GetComponent<Weapon>();
    }
}
