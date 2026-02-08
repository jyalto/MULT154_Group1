using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehavior : MonoBehaviour
{
    [Header("Trap Attributes")]
    [SerializeField] int damage;                // Damage dealt
    [SerializeField] float triggerRate;         // Damage interval (If <=0, trap is single use)
    public float uses;                          // Total use time is triggerRate * uses
    public AudioClip activationSound;
    public bool luring = false;

    [Header("Remote Activation")]
    public bool isActivatable;
    [SerializeField] bool activatesOnContact;
    public int activatorChannel = -1;

    // Runtime Values
    private BuildingBehavior buildingBehavior;
    private List<GameObject> targets;

    SoundManager soundManager;

    void Start()
    {
        GameObject soundManagerObject = GameObject.Find("Sound Manager");
        soundManager = soundManagerObject.GetComponent<SoundManager>();

        buildingBehavior = GetComponent<BuildingBehavior>();
        targets = new List<GameObject>();

        // Auto activate lure devices immediately
        if (GetComponent<LureDevice>() != null)
        {
            luring = true;
            PlayPitchedSound(activationSound);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            targets.Add(other.gameObject);

            if (activatesOnContact)
                ActivateTrap();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            targets.Remove(other.gameObject);
    }

    public void ActivateTrap()
    {
        if (uses > 0)
        {
            if (activatesOnContact)
            {
                InvokeRepeating(nameof(InflictContactDamage), 0f, triggerRate);
            }
            else
            {
                if (GetComponent<LureDevice>() != null)
                    luring = true;

                uses--;
            }

            //PlayPitchedSound(activationSound);

            soundManager.BearTrap();
        }
        else
        {
            if (activatesOnContact)
                buildingBehavior.durability = 0;
        }
    }

    public bool ApplyRemoteActivator(int channel)
    {
        if (isActivatable)
        {
            activatorChannel = channel;
            return true;
        }
        return false;
    }

    private void InflictContactDamage()
    {
        if (targets.Count > 0)
        {
            List<GameObject> toRemove = new List<GameObject>();

            foreach (GameObject target in targets)
            {
                if (target == null)
                {
                    toRemove.Add(target);
                    continue;
                }

                Enemy enemy = target.GetComponent<Enemy>();

                if (enemy.health - damage <= 0)
                {
                    enemy.health -= damage;
                    toRemove.Add(target);
                }
                else
                {
                    enemy.health -= damage;
                }
            }

            foreach (GameObject t in toRemove)
                targets.Remove(t);

            uses--;

            if (uses <= 0)
            {
                soundManager.BearTrapBreak();
                CancelInvoke();
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            CancelInvoke();
        }
    }

    public void PlayPitchedSound(AudioClip sound)
    {
        AudioSource src = GetComponent<AudioSource>();
        src.pitch = Random.Range(0.8f, 1.2f);
        src.PlayOneShot(sound);
    }
}
