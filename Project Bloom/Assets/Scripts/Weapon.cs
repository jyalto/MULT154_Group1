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

    public WeaponType typeOfWeapon;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.CompareTag("Pistol"))
        {
            typeOfWeapon = WeaponType.PISTOL;
            fireRate = 0.25f;
        }
        else if (gameObject.CompareTag("Assault Rifle"))
        {
            typeOfWeapon = WeaponType.ASSAULTRIFLE;
            fireRate = 0.1f;
        }
        else
        {
            typeOfWeapon = WeaponType.SHOTGUN;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (typeOfWeapon == WeaponType.PISTOL)
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
        else if (typeOfWeapon == WeaponType.ASSAULTRIFLE)
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
    }

    private void FireWeapon()
    {
        if (typeOfWeapon == WeaponType.ASSAULTRIFLE)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (typeOfWeapon == WeaponType.PISTOL)
        {
            audioSource.Play();
        }
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
