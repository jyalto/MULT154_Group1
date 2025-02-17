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
        SHOTGUN
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
    }

    private void FireWeapon()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        if (typeOfWeapon == WeaponType.PISTOL)
        {
            audioSource.Play();
            player.ammo[(int)PlayerController.AmmoType.PISTOL] -= 1;
        }
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            player.ammo[(int)PlayerController.AmmoType.ASSAULTRIFLE] -= 1;
        }
        else if (typeOfWeapon == WeaponType.SHOTGUN)
        {
            audioSource.Play();
            player.ammo[(int)PlayerController.AmmoType.SHOTGUN] -= 1;
            bulletPrefabLifeTime = 0.35f;
        }

        StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
