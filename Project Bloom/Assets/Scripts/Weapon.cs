using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;
    public float fireRate = 0.1f;

    private float nextFireTime = 0f;

    public PlayerController player;

    public enum WeaponType
    {
        PISTOL,
        ASSAULTRIFLE,
        SHOTGUN,
        RPG
    }

    public WeaponType typeOfWeapon;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        if (typeOfWeapon == WeaponType.PISTOL)
        {
            fireRate = 0.25f;
        }
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE)
        {
            fireRate = 0.1f;
        }
        else if (typeOfWeapon == WeaponType.SHOTGUN)
        {
            fireRate = 3f;
        }
        else if (typeOfWeapon == WeaponType.RPG)
        {
            fireRate = 10f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (typeOfWeapon == WeaponType.PISTOL && player.ammo[(int)PlayerController.AmmoType.PISTOL] > 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Time.time >= nextFireTime)
                {
                    FireWeapon();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE && player.ammo[(int)PlayerController.AmmoType.ASSAULTRIFLE] > 0)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= nextFireTime)
                {
                    FireWeapon();
                    nextFireTime = Time.time + fireRate;
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE && player.ammo[(int)PlayerController.AmmoType.ASSAULTRIFLE] <= 0)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else if (typeOfWeapon == WeaponType.SHOTGUN && player.ammo[(int)PlayerController.AmmoType.SHOTGUN] > 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Time.time >= nextFireTime)
                {
                    FireWeapon();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
        else if (typeOfWeapon == WeaponType.RPG && player.ammo[(int)PlayerController.AmmoType.RPG] > 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Time.time >= nextFireTime)
                {
                    FireWeapon();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    private void FireWeapon()
    {
        if (typeOfWeapon == WeaponType.RPG)
        {
            if (player.rocketShell.activeSelf)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

                player.rocketShell.SetActive(false);
                gameObject.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                audioSource.Play();
                player.ammo[(int)PlayerController.AmmoType.RPG] -= 1;
                bulletPrefabLifeTime = 10f;
                bulletVelocity = 10f;
                if (player.ammo[(int)PlayerController.AmmoType.RPG] >= 1)
                {
                    player.StartCoroutine(player.ReloadRocket());
                }

                StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
            }
        }

        else
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

            if (typeOfWeapon == WeaponType.PISTOL)
            {
                gameObject.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                audioSource.Play();
                player.ammo[(int)PlayerController.AmmoType.PISTOL] -= 1;
            }
            else if (typeOfWeapon == WeaponType.ASSAULTRIFLE)
            {
                if (!audioSource.isPlaying)
                {
                    gameObject.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.95f, 1.2f);
                    audioSource.Play();
                }
                player.ammo[(int)PlayerController.AmmoType.ASSAULTRIFLE] -= 1;
            }
            else if (typeOfWeapon == WeaponType.SHOTGUN)
            {
                gameObject.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.2f);
                audioSource.Play();
                player.ammo[(int)PlayerController.AmmoType.SHOTGUN] -= 1;
                bulletPrefabLifeTime = 0.35f;
            }

            StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
        }
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
