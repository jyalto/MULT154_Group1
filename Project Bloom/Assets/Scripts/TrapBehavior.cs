using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TrapBehavior : MonoBehaviour
{
    [Header("Trap Attributes")]
    [SerializeField] int damage;                            // Damage dealt
    [SerializeField] float triggerRate;                     // Damage interval (If <=0, trap is single use)
    public float uses;                            // Total use time is triggerRate * uses
    public AudioClip activationSound;
    public bool luring = false;

    [Header("Remote Activation")]
    public bool isActivatable;                              // Whether a remote activator can be applied
    [SerializeField] bool activatesOnContact;
    public int activatorChannel = -1;                       // The channel that triggers the building when fired

    // Runtime Values
    private BuildingBehavior buildingBehavior;
    private List<GameObject> targets;

    // Start is called before the first frame update
    void Start()
    {
        buildingBehavior = GetComponent<BuildingBehavior>();
        targets = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            targets.Add(other.gameObject);
            if (activatesOnContact)
            {
                ActivateTrap();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            targets.Remove(other.gameObject);
        }
    }

    public void ActivateTrap() // Activate trap functions
    {
        if (uses > 0)
        {
            if (activatesOnContact)
            {
                InvokeRepeating("InflictContactDamage", 0.0f, triggerRate);
            }
            else
            {
                if (gameObject.GetComponent<LureDevice>() != null)
                {
                    luring = true;
                }
                uses--;
            }

            PlayPitchedSound(activationSound);
        }
        else // Depleted functions
        {
            if (activatesOnContact) // Contact traps break when depleted
            {
                gameObject.GetComponent<BuildingBehavior>().durability = 0;
            }
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
            foreach (GameObject target in targets)
            {
                // Remove dead enemies
                if (target == null || target.GetComponent<Enemy>().health - damage <= 0)
                {
                    targets.Remove(target);
                }
                // Damage targets
                target.GetComponent<Enemy>().health -= damage;
            }
            uses--;
        }
        else
        {
            CancelInvoke();
        }
    }

    public void PlayPitchedSound(AudioClip sound)
    {
        gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
        gameObject.GetComponent<AudioSource>().PlayOneShot(sound);
    }
}