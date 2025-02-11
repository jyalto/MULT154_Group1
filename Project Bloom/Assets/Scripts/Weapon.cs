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

    public enum WeaponType
    {
        PISTOL,
        ASSAULTRIFLE,
        SHOTGUN
    }

    public enum AmmoType
    {
        PISTOL,
        ASSAULTRIFLE,
        SHOTGUN
    }

    public WeaponType typeOfWeapon;
    public int[] ammo;

    // Start is called before the first frame update
    void Start()
    {
        ammo = new int[3];
        if (typeOfWeapon == WeaponType.PISTOL)
        {
            fireRate = 0.25f;
        }
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE)
        {
            fireRate = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (typeOfWeapon == WeaponType.PISTOL && ammo[(int)AmmoType.PISTOL] > 0)
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
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE && ammo[(int)AmmoType.ASSAULTRIFLE] > 0)
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
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE && ammo[(int)AmmoType.ASSAULTRIFLE] <= 0)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
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
            ammo[(int)AmmoType.PISTOL] -= 1;
        }
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            ammo[(int)AmmoType.ASSAULTRIFLE] -= 1;
        }
        
        StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
